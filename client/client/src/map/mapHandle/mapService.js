import { Get_Target_Layer, Get_Dev_Layer } from "../js";
import {
  Draw_Icon_Marker,
  Remove_TargetFeature,
  RemoveTargetById,
  GetTgStatusSVGIMG,
  Draw_Target_SectorMarker,
  featureType_enum, Draw_Flyer_Icon_Marker, Draw_FlyAddr_Icon_Marker, DrawUpdate_FlyAddr_Icon_Marker
} from "./mapDraw";
import { Feature_Move, Feature_Sector_Move } from "./mapMove";
import store from '../../store'
import Vue from "vue";
import { GetTargetAlarmColor } from "../../../static/css/GeneralStyle";
import bus from "../../modes/tool/bus";

export function Clear_Aspnet_Signalr() {
  if (connection.state == 1) {
    console.log("在运行中---准备注销！");
    connection.stop().then(() => {
      console.log("注销成功！");
      connection = undefined;
    });
  }
}

// 是否符合筛选条件
export function Is_Meet_Condition(vt, alt, threat) {
  let flag = false;
  if (vt >= store.state.speed_slider[0] && vt <= store.state.speed_slider[1] &&
    alt >= store.state.alt_slider[0] && alt <= store.state.alt_slider[1]) {
    flag = true;
    // switch (threat) {
    //   case 0:
    //     if (store.state.zero_level_check) flag = true;
    //     break;
    //   case 1:
    //     if (store.state.one_level_check) flag = true;
    //     break;
    //   case 2:
    //     if (store.state.two_level_check) flag = true;
    //     break;
    //   case 3:
    //     if (store.state.three_level_check) flag = true;
    //     break;
    //   case 4:
    //     if (store.state.out_level_check) flag = true;
    //     break;
    //   default:
    //     if (store.state.out_level_check) flag = true;
    //     break;
    // }
  }
  return flag;
}

export function GetAlarmLevel(alarmPercentage) {
  let alarmLevel = 0;
  if (alarmPercentage <= 10) {
    alarmLevel = 0;
  } else if (alarmPercentage > 10 && alarmPercentage <= 60) {
    alarmLevel = 1;
  } else if (alarmPercentage > 60) {
    alarmLevel = 3;
  }
  return alarmLevel;
}

export function DrawTG(coordinateType, info, isHide) {
  let showText = info.uavSn;
  switch (coordinateType) {
    case 0:
      //Draw_Target_SectorMarker({lat:116.2770,lng:39.79100}, 1200, 55, 5,info);
      break;
    case 1:
      Draw_Icon_Marker(showText,
        { lat: info.lat, lng: info.lng },
        {
          id: info.id, status: info.alarmLevel, deviceId: info.deviceId, uavSn: info.uavSn,
          alarmLevel: info.alarmLevel, tgStates: info.tgStates
        },
        featureType_enum.target);

      if (null != info.appLat || info.appLat != 0) {
        if (!isHide) {
          Draw_Flyer_Icon_Marker(showText, { lat: info.appLat, lng: info.appLng }, info.id + 'Flyer', featureType_enum.flyer);
        }
      }
      break;
    default:
      Draw_Icon_Marker(showText,
        { lat: info.lat, lng: info.lng },
        {
          id: showText, status: info.alarmLevel, deviceId: info.deviceId, uavSn: info.uavSn,
          alarmLevel: info.alarmLevel, tgStates: info.tgStates
        },
        featureType_enum.target);
      break;
  }
}

// export function Feature_Swtich_Move(coordinateType,feature_,info,isHideTrack) {

//   switch (coordinateType) {
//     case 0:
//       Feature_Sector_Move(feature_,20,20);
//       break;
//     case 1:
//       Feature_Move(feature_, {lat: info.lat, lng: info.lng});
//       DrawUpdate_FlyAddr_Icon_Marker(info.id+'FlyAddr',{lat: info.lat, lng: info.lng},featureType_enum.flyAddr,isHideTrack)
//       break;
//     default:
//       break;
//   }
// }
export function Feature_Swtich_Move(coordinateType, feature_, info, isHideTrack) {

  switch (coordinateType) {
    case 0:
      Feature_Sector_Move(feature_, 20, 20);
      break;
    case 1:
      Feature_Move(feature_, { lat: info.lat, lng: info.lng });
      if (!isHideTrack) {
        Draw_Flyer_Icon_Marker(info.id, { lat: info.appLat, lng: info.appLng }, info.id + 'Flyer', featureType_enum.flyer);
      } else {
        //删除飞手
        let _featureFlyer = Get_Target_Layer().getSource().getFeatureById(info.id + 'Flyer');
        _featureFlyer && Get_Target_Layer().getSource().removeFeature(_featureFlyer);
      }
      DrawUpdate_FlyAddr_Icon_Marker(info.id + 'FlyAddr', { lat: info.lat, lng: info.lng }, featureType_enum.flyAddr, isHideTrack)
      break;
    default:
      break;
  }
}

const signalR = require("@aspnet/signalr");
let connection = undefined;

async function start() {
  try {
    await connection.start();
  }
  catch (err) {
    setTimeout(() => start(), 5000);
  }
}

export function Init_Aspnet_Signalr() {
  connection= new signalR.HubConnectionBuilder();
  console.log("收到的目标===>", !connection);
  //if (!connection)
  {
    connection = connection.withUrl(Vue.prototype.$baseUrl + "/lads_channel").build();

    connection.on("target_channel", data => {
      // 收到目标消息
      console.log("收到的目标===>", data);
      //console.log("target_channel", "收到目标信息：" , data, '条 删除信息：');

      //跟踪isTracking//转发isTranspond//打击isHitting//诱骗isTicking//监视isMonitoring
      if (null != data.targets && data.targets.length > 0) {
        // 清空数据源 重新计算值
        store.dispatch('set_target_0level_num', 0);
        store.dispatch('set_target_1level_num', 0);
        store.dispatch('set_target_2level_num', 0);
        store.dispatch('set_target_3level_num', 0);
        store.dispatch('set_target_out_level_num', 0);

        data.targets.forEach(info => {
          const { id, deviceId, lat, lng, alt, category,
            mode, vt, vr, coordinateType, threat, trackTime, appAddr,
            appLng, appLat, homeLng, homeLat, beginAt, uavType, uavSn,
            hitRelationShip,
            monitorRelationShip,
            tickRelationShip,
            trackRelationShip,
            transpondRelationShip } = info;

          // 威胁度 转换百分比
          let alarmPercentage = (threat / 100).toFixed(2);
          let alarmLevel = GetAlarmLevel(threat);
          // 判断是否存在某数据
          let index = store.state.targets.findIndex(item => item.id === id);

          // 无人机类型 1无人机 2汽车 3鸟 等
          if (category == 1) {
          }

          // 1真实 0外推
          if (mode == 1) {
          }

          // 不存在
          if (index == -1) {
            let color = GetTargetAlarmColor(alarmLevel, 1);

            let data__ = {
              id: id,
              deviceId: deviceId,
              alt: alt,
              speed: vt,
              alarmLevel: alarmLevel,
              color: color,
              lat: lat,
              lng: lng,
              appAddr: appAddr,
              trackTime: trackTime,
              appLng: appLng,
              appLat: appLat,
              homeLng: homeLng,
              homeLat: homeLat,
              beginAt: beginAt,
              uavType: uavType,
              uavSn: uavSn,
              mode: mode,
              mat: alarmPercentage,
              hitRelationShip: hitRelationShip,
              monitorRelationShip: monitorRelationShip,
              tickRelationShip: tickRelationShip,
              trackRelationShip: trackRelationShip,
              transpondRelationShip: transpondRelationShip,
              isTracking: false,
              isTranspond: false,
              isHitting: false,
              isTicking: false,
              isMonitoring: false
            }
            store.state.targets.push(data__);
          } else //存在该数据
          {
            // 更新 总列表 的 目标信息
            store.state.targets[index].alt = alt;
            store.state.targets[index].speed = vt;
            store.state.targets[index].trackTime = trackTime;
            store.state.targets[index].mat = alarmPercentage;

            if (null != hitRelationShip) {
              store.state.targets[index].hitRelationShip = hitRelationShip;
              store.state.targets[index].isHitting = true;
            } else {
              store.state.targets[index].hitRelationShip = hitRelationShip;
              store.state.targets[index].isHitting = false;
            }

            if (null != monitorRelationShip) {
              store.state.targets[index].monitorRelationShip = monitorRelationShip;
              store.state.targets[index].isMonitoring = true;
            } else {
              store.state.targets[index].monitorRelationShip = monitorRelationShip;
              store.state.targets[index].isMonitoring = false;
            }

            if (null != tickRelationShip) {
              store.state.targets[index].tickRelationShip = tickRelationShip;
              store.state.targets[index].isTicking = true;
            } else {
              store.state.targets[index].tickRelationShip = tickRelationShip;
              store.state.targets[index].isTicking = false;
            }

            if (null != trackRelationShip) {
              store.state.targets[index].trackRelationShip = trackRelationShip;
              store.state.targets[index].isTracking = true;
            } else {
              store.state.targets[index].trackRelationShip = trackRelationShip;
              store.state.targets[index].isTracking = false;
            }

            if (null != transpondRelationShip) {
              store.state.targets[index].transpondRelationShip = transpondRelationShip;
              store.state.targets[index].isTranspond = true;
            } else {
              store.state.targets[index].transpondRelationShip = transpondRelationShip;
              store.state.targets[index].isTranspond = false;
            }

            if (store.state.targets[index].alarmLevel != alarmLevel) {
              store.state.targets[index].alarmLevel = alarmLevel;
              let color = GetTargetAlarmColor(alarmLevel);
              store.state.targets[index].color = color;
            }
          }

          // 符合条件
          if (Is_Meet_Condition(vt, alt, alarmLevel)) {
            let index_filter = store.state.targets_filter.findIndex(item => item.id === id);

            // 判断是否存在
            if (index_filter == -1) {
              // 如果没有添加 进行添加
              let color = GetTargetAlarmColor(alarmLevel);

              let info__ = {
                id: id, deviceId: deviceId,
                alt: alt,
                speed: vt,
                alarmLevel: alarmLevel,
                color: color,
                lat: lat,
                lng: lng,
                appLng: appLng,
                appLat: appLat,
                trackTime: trackTime,
                appAddr: appAddr,
                mode: mode,
                mat: alarmPercentage,
                hitRelationShip: hitRelationShip,
                monitorRelationShip: monitorRelationShip,
                tickRelationShip: tickRelationShip,
                trackRelationShip: trackRelationShip,
                transpondRelationShip: transpondRelationShip,
                isTracking: false,
                isTranspond: false,
                isHitting: false,
                isTicking: false,
                isMonitoring: false
              };

              store.state.targets_filter.push(info__);

              let tgStates = {
                isTracking: false, isTranspond: false, isHitting: false,
                isTicking: false, isMonitoring: false
              }

              // 地图添加图标
              let info = {
                id: id, lat: lat, lng: lng, status: alarmLevel, deviceId: deviceId, uavSn: uavSn,
                alarmLevel: alarmLevel, tgStates: tgStates, appLat: appLat, appLng: appLng
              };

              //画出点
              if (transpondRelationShip == null) {
                DrawTG(coordinateType, info, true);
              }
              else {
                DrawTG(coordinateType, info, false);
              }


              //画出航迹
              Draw_FlyAddr_Icon_Marker(id + 'FlyAddr', { lat: lat, lng: lng }, featureType_enum.flyAddr);
            } else {
              // 如果已经添加 修改目标信息
              store.state.targets_filter[index_filter].alt = alt;
              store.state.targets_filter[index_filter].speed = vt;
              store.state.targets_filter[index_filter].trackTime = trackTime;
              store.state.targets_filter[index_filter].mat = alarmPercentage;

              // 更新信息
              if (null != hitRelationShip) {
                store.state.targets_filter[index_filter].hitRelationShip = hitRelationShip;
                store.state.targets_filter[index_filter].isHitting = true;
              } else {
                store.state.targets_filter[index_filter].hitRelationShip = hitRelationShip;
                store.state.targets_filter[index_filter].isHitting = false;
              }

              if (null != monitorRelationShip) {
                store.state.targets_filter[index_filter].monitorRelationShip = monitorRelationShip;
                store.state.targets_filter[index_filter].isMonitoring = true;
              } else {
                store.state.targets_filter[index_filter].monitorRelationShip = monitorRelationShip;
                store.state.targets_filter[index_filter].isMonitoring = false;
              }

              if (null != tickRelationShip) {
                store.state.targets_filter[index_filter].tickRelationShip = tickRelationShip;
                store.state.targets_filter[index_filter].isTicking = true;
              } else {
                store.state.targets_filter[index_filter].tickRelationShip = tickRelationShip;
                store.state.targets_filter[index_filter].isTicking = false;
              }

              if (null != trackRelationShip) {
                store.state.targets_filter[index_filter].trackRelationShip = trackRelationShip;
                store.state.targets_filter[index_filter].isTracking = true;
              } else {
                store.state.targets_filter[index_filter].trackRelationShip = trackRelationShip;
                store.state.targets_filter[index_filter].isTracking = false;
              }

              if (null != transpondRelationShip) {
                store.state.targets_filter[index_filter].transpondRelationShip = transpondRelationShip;
                store.state.targets_filter[index_filter].isTranspond = true;
              } else {
                store.state.targets_filter[index_filter].transpondRelationShip = transpondRelationShip;
                store.state.targets_filter[index_filter].isTranspond = false;
              }

              if (store.state.targets_filter[index_filter].alarmLevel != alarmLevel) {
                let color = GetTargetAlarmColor(alarmLevel);
                store.state.targets_filter[index_filter].alarmLevel = alarmLevel;
                store.state.targets_filter[index_filter].color = color;
              }

              // 目标创建 或 移动移动
              let feature_ = Get_Target_Layer().getSource().getFeatureById(id);

              let tgStates = {
                isTracking: store.state.targets_filter[index_filter].isTracking,
                isTranspond: store.state.targets_filter[index_filter].isTranspond,
                isHitting: store.state.targets_filter[index_filter].isHitting,
                isTicking: store.state.targets_filter[index_filter].isTicking,
                isMonitoring: store.state.targets_filter[index_filter].isMonitoring
              }

              if (null == feature_) // 如果不存在 则新增目标
              {
                let info = {
                  id: id, lat: lat, lng: lng, status: alarmLevel, deviceId: deviceId, uavSn: uavSn,
                  alarmLevel: alarmLevel, tgStates: tgStates, appLat: appLat, appLng: appLng
                };

                if (transpondRelationShip == null) {
                  DrawTG(coordinateType, info, true);
                }
                else {
                  DrawTG(coordinateType, info, false);
                }
                Draw_FlyAddr_Icon_Marker(id + 'FlyAddr', { lat: lat, lng: lng }, featureType_enum.flyAddr);
              } else {
                // 赋值 改变样式
                let info = feature_.getProperties().info;

                if (tgStates.isHitting != info.tgStates.isHitting
                  || tgStates.isTranspond != info.tgStates.isTranspond
                  || tgStates.isTracking != info.tgStates.isTracking
                  || tgStates.isTicking != info.tgStates.isTicking
                  || tgStates.isMonitoring != info.tgStates.isMonitoring
                  || info.alarmLevel != alarmLevel) {
                  info.tgStates = tgStates;
                  info.alarmLevel = alarmLevel;
                  feature_.setProperties({ info: info });
                  // 如果有变更的状态 则修改样式
                  GetTgStatusSVGIMG(feature_, id, alarmLevel, tgStates);
                }

                let info_ = { id: id, lat: lat, lng: lng };
                if (null == transpondRelationShip) {
                  Feature_Swtich_Move(coordinateType, feature_, info_, true);
                }
                else {
                  Feature_Swtich_Move(coordinateType, feature_, info_, false);
                }


              }
            }
          } else {
            // 不符合条件 删除筛选列表的数据
            let index_filter = store.state.targets_filter.findIndex(item => item.id === id);
            if (index_filter != -1) // 如果在 筛选的目标列表 中存在 则删除目标及图标
            {
              store.dispatch('del_target_filter_info', id);
              Remove_TargetFeature(id);  // 删除图标
            }
          }
        })

        store.state.targets_filter.forEach(info => {
          switch (info.alarmLevel) {
            case 0:
              store.dispatch('set_target_0level_num', store.state.targets_0level_num + 1);
              break;
            case 1:
              store.dispatch('set_target_1level_num', store.state.targets_1level_num + 1);
              break;
            case 2:
              store.dispatch('set_target_2level_num', store.state.targets_2level_num + 1);
              break;
            case 3:
              store.dispatch('set_target_3level_num', store.state.targets_3level_num + 1);
              break;
            case 4:
              store.dispatch('set_target_out_level_num', store.state.targets_out_level_num + 1);
              break;
            default:
              break;
          }
        })

      }

      if (null != data.deleteTargets && data.deleteTargets.length > 0) {
        // 删除目标
        data.deleteTargets.forEach(info => {
          //console.log("connection_message_delete_targets",info);
          //删除地图上目标
          RemoveTargetById(info.targetId);
          //移除列表中目标信息
          store.dispatch('del_target_info', info.targetId);
          store.dispatch('del_target_filter_info', info.targetId);
        })

        store.dispatch('set_target_0level_num', 0);
        store.dispatch('set_target_1level_num', 0);
        store.dispatch('set_target_2level_num', 0);
        store.dispatch('set_target_3level_num', 0);
        store.dispatch('set_target_out_level_num', 0);

        store.state.targets_filter.forEach(info => {
          switch (info.alarmLevel) {
            case 0:
              store.dispatch('set_target_0level_num', store.state.targets_0level_num + 1);
              break;
            case 1:
              store.dispatch('set_target_1level_num', store.state.targets_1level_num + 1);
              break;
            case 2:
              store.dispatch('set_target_2level_num', store.state.targets_2level_num + 1);
              break;
            case 3:
              store.dispatch('set_target_3level_num', store.state.targets_3level_num + 1);
              break;
            case 4:
              store.dispatch('set_target_out_level_num', store.state.targets_out_level_num + 1);
              break;
            default:
              break;
          }
        })
      }

      //检查目标列表 播放告警声音
      data.targets && data.targets.length && data.targets.forEach(item => {
        if (item.threat == 100) {
          bus.$emit('TargetAlarmPlayMp3');
          return;
        }
      })

    });

    connection.on("connection_message", data => {
      //console.log("connection_message", data);
    });

    connection.on("device_status_channel", data => {
      //console.log("收到设备状态信息",data);
      console.log('device statue  :>> ', data);
      data.forEach(info => {
        const {
          code,// 状态
          deviceCategory,//类型
          deviceId, // 设备id
          errorMsg, // 错误信息
          isBeGuidance, // 是否在被引导中
          isError, // 是否出现故障
          isGuidance,// 是否在引导中
          isTurnTarget,// 是否在转发中
          runInfo
        } = info;

        let devStatus = code;

        if (isError) {
          // 0离线 1待机 2工作中 3异常
          devStatus = 3;
        }

        //runInfo.currentAz
        //runInfo.currentEl
        //console.log("connection_DevState_message", '设备ID：'+info.deviceId+', 设备状态' + devStatus);
        // 0:离线;1:待机;2:工作;3：异常 serviceErrorMsg  deviceBitMsg
        //console.error("kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk");
        store.dispatch('set_dev_status',
          {
            did: info.deviceId, status: devStatus,
            deviceErrorMsg: '', runInfo: runInfo, deviceCategory: deviceCategory
          });
      });
    });

    connection.onclose(async () => {
      await start();
    })

    start();
  }
};

