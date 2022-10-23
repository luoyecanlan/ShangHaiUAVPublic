import {
  COMMIT_LOGIN_INFO,
  COMMIT_DEVICE_CATEGORIES,

  REQUEST_ALL_DEVICES,
  COMMIT_DEVICE_STATUS,
  COMMIT_DEVICE_RELATION_SHIPS,
  UPDATE_TGID,
  QUERY_DEVICES,
  QUERY_DEVICE_INFO,
  QUERY_DEVICE_STATUS,
  QUERY_TARGETS,
  QUERY_TARGET_INFO,
  QUERY_WARNING_ZONES,
  ADD_TARGET_STATUS_INFO,
  SET_TARGET_0LEVEL_NUM,
  SET_TARGET_2LEVEL_NUM,
  SET_TARGET_1LEVEL_NUM,
  SET_TARGET_3LEVEL_NUM,
  SET_TARGET_OUT_LEVEL_NUM,
  SELECT_TARGET_ID,DEL_TARGET_INFO,
  SET_DEV_STATUS,SELECT_WAS_INFO,
  SET_ZERO_LEVEL_CHECK,DO_SELECT_TARGET_INFO,
  SET_ONE_LEVEL_CHECK, SET_DEV_COVER_LINE_STATUS,
  SET_TWO_LEVEL_CHECK, SET_DEV_ALARM_STATUS,
  SET_THREE_LEVEL_CHECK, CHANGE_INFO_USER_CONFIG,
  SET_OUT_LEVEL_CHECK,
  CLEAR_TARGETS, SET_MAP_INFO,
  CLEAR_TARGETS_FILTER, SET_USER_CONFIG_INFO,
  SET_ALT_SLIDER, SET_TARGET_TURN, SET_MAP_TYPE,
  SET_SPEED_SLIDER, DEL_TARGET_FILTER_INFO,SET_BAND_ID
} from "./mutation-types";
import {event_type} from "../modes/tool";
import Bus from '../modes/tool/bus'
import store from "./index";
import {cosh} from "ol/math";
import {Draw_DevIcon_Marker, Draw_DevListIcon_Marker, Draw_PositionDev_Icon_Marker} from '../map/mapHandle/mapDrawDev'
import {HiddenDevWarnLine, HiddenDevWarnRound,ChangeDevFeatureImg} from "../map/mapHandle/mapDraw"
const _lodash=require('lodash')


export default {
  [COMMIT_LOGIN_INFO](state, {data}) {
    state.login_info = data
  },
  [COMMIT_DEVICE_CATEGORIES](state, {data}) {
    state.device_categories = data
  },
  [QUERY_DEVICES](state, {data}) {
    //Bus.$emit(event_type.test_event,data);
    console.log('====成功加载设备列表信息====');

    data.forEach(info =>
    {
      info.line = true;
      info.range = true;
      info.status = 0;

      if(info.category>30000 && info.category<40000){
        state.hitDevList.push({id: info.id,name: info.name});
      }
    });

    let positionDev=[];
    data.forEach(temp=>{
      if(temp.category==10401){
        positionDev.push(temp);
      }
    })
    state.positionDevices=positionDev;
    state.devices = data;

    Draw_DevListIcon_Marker(data);

    //console.log('store.state.deviceCover',store.state.deviceCover);
    //console.log('store.state.deviceLine',store.state.deviceLine);

    // 修改个性化配置信息 检查是否需要隐藏告警区域
    if (store.state.deviceCover != null && store.state.deviceCover.length > 0) {
      store.state.deviceCover.forEach(info =>
      {
        state.devices.forEach(devicesItem =>
        {
          if (info == devicesItem.id)
          {
            devicesItem.range = false;
          }
        });

        HiddenDevWarnRound(info, false);
      })
    }

    if (store.state.deviceLine != null && store.state.deviceLine.length > 0) {
      store.state.deviceLine.forEach(info => {
        state.devices.forEach(devicesItem => {
          if (info == devicesItem.id) {
            devicesItem.line = false;
          }
        });
        HiddenDevWarnLine(info, false);
      })
    }

  },
  [QUERY_DEVICE_INFO](state, {data}) {
    state.device_info = data;
  },
  [QUERY_DEVICE_STATUS](state, {data}) {
    state.device_status = data;
  },
  /**
   * 获取设备目标列表
   * @param state
   * @param did 设备id
   * @param data  返回的目标列表
   */
  [QUERY_TARGETS](state, {did, data}) {
    //目标数据组合
    //过期清除计划
    //目标数据增加更新时间标签  每次更新与当前时间进行比较  过期则直接删除
    //地图更新同
    state.device_targets = data;
  },
  [QUERY_TARGET_INFO](state, {data}) {
    if (null != data) {
      state.device_target_info = data;
    }
  },
  [QUERY_WARNING_ZONES](state, {data}) {
    state.device_warning_zones = data;
  },
  [SET_TARGET_0LEVEL_NUM](state, {data}) {
    state.targets_0level_num = data;
  },
  [SET_TARGET_1LEVEL_NUM](state, {data}) {
    state.targets_1level_num = data;
  },
  [SET_TARGET_2LEVEL_NUM](state, {data}) {
    state.targets_2level_num = data;
  },
  [SET_TARGET_3LEVEL_NUM](state, {data}) {
    state.targets_3level_num = data;
  },
  [SET_TARGET_OUT_LEVEL_NUM](state, {data}) {
    state.targets_out_level_num = data;
  },
  [SELECT_TARGET_ID](state, {data, did}) {
    state.select_target_id = data;
    state.select_dev_id = did;
  },
  [SELECT_WAS_INFO](state, data) {
    state.select_was_info.id = data.id;
    state.select_was_info.type = data.type;
  },
  [DO_SELECT_TARGET_INFO](state, data) {
    state.select_target_info = data;
  },
  [SET_DEV_STATUS](state, data) {
    let tempDevs=state.devices;
    tempDevs.forEach(info =>
    {
      if (info.id == data.did)
      {
        // 更改设备状态
        if(null == info.status)
        {
          info.status = data.status;
        }else
          {
            if(data.status != info.status)
            {
              info.status = data.status;
              // 修改图标信息
              // --- ChangeDevFeatureImg (id,devStatus,category)
              //console.error("mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm");
              ChangeDevFeatureImg(data.did,data.status,data.deviceCategory);
            }
        }

        if(null != data.deviceErrorMsg && data.deviceErrorMsg != '')
        {
          info.deviceErrorMsg = data.deviceErrorMsg;
        }
         info.runInfo = data.runInfo;
      }
    });
    state.devices=_lodash.cloneDeep(tempDevs)
  },
  [SET_ZERO_LEVEL_CHECK](state, {data}) {
    state.zero_level_check = data;
  },
  [SET_ONE_LEVEL_CHECK](state, {data}) {
    state.one_level_check = data;
  },
  [SET_TWO_LEVEL_CHECK](state, {data}) {
    state.two_level_check = data;
  },
  [SET_THREE_LEVEL_CHECK](state, {data}) {
    state.three_level_check = data;
  },
  [SET_OUT_LEVEL_CHECK](state, {data}) {
    state.out_level_check = data;
  },
  [CLEAR_TARGETS](state, {data}) {
    state.targets = [];
  },
  [CLEAR_TARGETS_FILTER](state, {data}) {
    state.targets_filter = [];
  },
  [SET_ALT_SLIDER](state, {data}) {
    state.alt_slider = data;
  },
  [SET_SPEED_SLIDER](state, {data}) {
    state.speed_slider = data;
  },
  [DEL_TARGET_INFO](state,{data})
  {
    // 筛选出ID不一致的
    state.targets = state.targets.filter(t => t.id != data)
  },
  [DEL_TARGET_FILTER_INFO](state, {data}) {
    // 筛选出ID不一致的
    state.targets_filter = state.targets_filter.filter(t => t.id != data);
  },
  [ADD_TARGET_STATUS_INFO](state, {data}) {

  },
  [SET_DEV_ALARM_STATUS](state, {did, type, data}) {
    console.log('个性化配置修改信息====', did, type, data);
    state.devices.forEach(info => {
      if (did == info.id) {
        switch (type) {
          case 'LineList':
            info.line = data;
            break;
          case 'CoverList':
            info.range = data;
            break;
        }
      }
    });
  },
  [SET_TARGET_TURN](state, {ip, port}) {
    state.target_turn_ip = ip;
    state.target_turn_port = port;
  },
  [SET_DEV_COVER_LINE_STATUS](state, {type, data}) {
    if (type == 'cover')
    {
      state.deviceCover = data;
    } else {
      state.deviceLine = data;
    }
  },
  [SET_MAP_TYPE](state, data) {
    state.map_type = data;
  },
  [SET_MAP_INFO](state, data) {
    if (data.name == 'onlinemapconfig') //onlinemapconfig/offlineconfig
    {
      state.map_online_address = data.url;
      state.map_online_id = data.id;
    }
    else if (data.name == 'offlineconfig') {
      state.map_offline_address = data.url;
      state.map_offline_id = data.id;
    }
  },
  [CHANGE_INFO_USER_CONFIG](state, {type, data}) {
    switch (type) {
      case 'mapId':
        state.user_config_info.mapId = data;
        break;
      case 'deviceCover':
        state.user_config_info.deviceCover = data;
        break;
      case 'deviceLine':
        state.user_config_info.deviceLine = data;
        break;
      case 'filterThreat':
        state.user_config_info.filterThreat = data;
        break;
      case 'filterCategory':
        state.user_config_info.filterCategory = data;
        break;
      case 'filterVMin':
        state.user_config_info.filterVMin = data;
        break;
      case 'filterVMax':
        state.user_config_info.filterVMax = data;
        break;
      case 'filterDiscMin':
        state.user_config_info.filterDiscMin = data;
        break;
      case 'filterDiscMax':
        state.user_config_info.filterDiscMax = data;
        break;
      case 'filterAltMin':
        state.user_config_info.filterAltMin = data;
        break;
      case 'filterAltMax':
        state.user_config_info.filterAltMax = data;
        break;
    }
  },
  [SET_USER_CONFIG_INFO](state, data) {
    state.user_config_info.mapId = data.mapId;
    state.user_config_info.deviceCover = data.deviceCover;
    state.user_config_info.deviceLine = data.deviceLine;
    state.user_config_info.filterVMin = data.filterVMin;
    state.user_config_info.filterVMax = data.filterVMax;
    state.user_config_info.filterDiscMin = data.filterDiscMin;
    state.user_config_info.filterDiscMax = data.filterDiscMax;
    state.user_config_info.filterAltMin = data.filterAltMin;
    state.user_config_info.filterAltMax = data.filterAltMax

    if (data.filterThreat != '') {
      state.user_config_info.filterThreat = data.filterThreat;
    }

    state.user_config_info.filterCategory = data.filterCategory;
    state.user_config_info.updatetime = data.updatetime;
    state.user_config_info.id = data.id;
  },
  [SET_BAND_ID](state, data) {
    state.bandId = data;
  },
  [REQUEST_ALL_DEVICES](state,{data}){
    state.device_infos=data;
  },
  [COMMIT_DEVICE_STATUS](state,{data}) {
    //console.log(data,Object.keys(data));
    if (data && data.length > 0) {
      data.map(f => {
        if(f){
          state.device_status[f.deviceId] = f;
        }

      });
    }
  },
  [COMMIT_DEVICE_RELATION_SHIPS](state,{data}) {
    state.device_relation_ships=[];
    if(data){
      state.device_relation_ships=data;
    }
  },
  [UPDATE_TGID](state,{data}) {
    state.tgId=data;

  }

}
