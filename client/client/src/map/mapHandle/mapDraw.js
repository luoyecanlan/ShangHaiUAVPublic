import OlGeomPoint from 'ol/geom/Point'
import OlFeature from 'ol/Feature'
import { fromLonLat, toLonLat } from "ol/proj"
import OlStyleIcon from 'ol/style/Icon'
import OlStyleStroke from 'ol/style/Stroke'
import OlStyleFill from 'ol/style/Fill'
import {
  Get_Target_Layer,
  Get_Dev_Layer,
  Get_Zone_Layer,
  GetEquipLineTextStyle,
  GetFeatureStyle__Dev,
  Get_Main_Map,
  Get_Marque_Layer,
  GetTGUsuallyStyle,
  GetTGSectorTextStyle,
  GetTGSelectStyle,
  GetTGIMGText,
  GetWarnningZoneStyle,
  GetTGSectorStyle,
  GetTGSectorSelectStyle, GetFlyerStyle,
} from "../js";
import { mapColor, EquipmentLineStyle } from "../js/styles";
import Overlay from "ol/Overlay";
import { circular } from "ol/geom/Polygon";
import { offset as sphereOffset } from "ol/sphere";
import {
  Style,
  Icon,
  Circle,
  Fill,
  Stroke,
  Text,
  RegularShape
} from "ol/style";
import { Polygon } from "ol/geom";
import OLGeomCircle from "ol/geom/Circle";
import GeometryLayout from 'ol/geom/GeometryLayout'
import LineString from 'ol/geom/LineString'
import { extend } from "ol/array";
import {
  ATTACK_IMG, ATTACK_SELECT_IMG, DECOY_IMG, DECOY_SELECT_IMG,
  GUANGDIAN_IMG, GUANGDIAN_SELECT_IMG, PINPU_ICON,
  PINPU_IMG, PINPU_SELECT_IMG, RADAR_ICON,
  RADAR_IMG, RADAR_SELECT_IMG, TARGET_TEMP_IMG, YUNSHAO_ICON
} from "../mapHandle/mapImgSource";


import store from "../../store";
import bus from "../../modes/tool/bus";
import { GetTargetAlarmColor } from "../../../static/css/GeneralStyle";
import "../../../static/fonts/style.css"

// 画出图标
export function Draw_Icon_Marker(IconText, { lat, lng }, BindingInfo, IconTypeName) {
  let iconFeature = new OlFeature({
    geometry: new OlGeomPoint(fromLonLat([lng, lat]), "XY"),
    name: IconTypeName,
  })

  iconFeature.setProperties({ info: BindingInfo });
  iconFeature.setId(BindingInfo.id);

  // 将图标特性添加进矢量中
  let iconText = '';
  console.log("sn", BindingInfo.uavSn);
  switch (IconTypeName) {
    case featureType_enum.target:
      iconText = BindingInfo.uavSn
      break;
    case featureType_enum.dev:
      iconText = BindingInfo.name;
      break;
    case featureType_enum.targetSector:
      break;
    default:
      iconText = '';
      break
  }

  // let tgStyle = GetFeatureStyle__Al(iconText,false,BindingInfo.alarmLevel,BindingInfo.tgStates);
  let tgStyle = GetFeatureStyle__Al(iconText, false, BindingInfo.alarmLevel, BindingInfo.tgStates, BindingInfo.uavSn);
  iconFeature.setStyle(tgStyle);

  switch (IconTypeName) {
    case featureType_enum.target:
      Get_Target_Layer().getSource().addFeature(iconFeature);
      break;
    case featureType_enum.dev:
      Get_Dev_Layer().getSource().addFeature(iconFeature);
      break;
    case featureType_enum.targetSector:
      break;
    default:
      return;
  }
}

/**
 * 地图上画出飞手
 * @param IconText
 * @param lat
 * @param lng
 * @param id
 * @param IconTypeName
 * @constructor
 */
export function Draw_Flyer_Icon_Marker(IconText, { lat, lng }, id, IconTypeName) {
  let iconFeature = new OlFeature({
    geometry: new OlGeomPoint(fromLonLat([lng, lat]), "XY"),
    name: IconTypeName,
  })

  iconFeature.setProperties({ id: id });
  iconFeature.setId(id);

  let tgStyle = GetFlyerStyle(IconText);

  iconFeature.setStyle(tgStyle);

  Get_Target_Layer().getSource().addFeature(iconFeature);
}

/**
 * 更新航迹位置
 * @param id
 * @param lat
 * @param lng
 * @param IconTypeName
 * @constructor
 */
export function DrawUpdate_FlyAddr_Icon_Marker(id, { lat, lng }, IconTypeName, isHideTrack) {
  let feature_ = Get_Target_Layer().getSource().getFeatureById(id);
  if (null == feature_) {
    Draw_FlyAddr_Icon_Marker(id, { lat, lng }, IconTypeName)
  } else {
    let Point = fromLonLat([lng, lat]);
    feature_.getGeometry().appendCoordinate(Point);
    if (isHideTrack) {


      Get_Target_Layer().setStyle(null);
      feature_.setStyle(null);

    }
    else {
      let styles = [
        new Style({
          stroke: new Stroke({
            color: '#5394EC',
            width: 3
          })
        })
      ];
      feature_.setStyle(styles);
    }

    //console.log("ssss",s);
  }
}

export function Draw_FlyAddr_Icon_Marker(id, { lat, lng }, IconTypeName) {
  let Point = fromLonLat([lng, lat]);
  //实例一个线(标记点)的全局变量
  let geometryLineString = new LineString([Point]); //线,Point 点,Polygon 面

  let LineStringFeature = new OlFeature({
    geometry: geometryLineString,
    name: IconTypeName,
  })

  // let styles = [
  //   new Style({
  //     stroke: new Stroke({
  //       color: '#5394EC',
  //       width: 3
  //     })
  //   })
  // ];

  // LineStringFeature.setStyle(styles);

  LineStringFeature.setProperties({ id: id });
  LineStringFeature.setId(id);

  Get_Target_Layer().getSource().addFeature(LineStringFeature);
}

export function Draw_Dev_Icon_Marker(IconText, { lat, lng }, ImageSrc, BindingInfo, IconTypeName) {
  let iconFeature = new OlFeature({
    geometry: new OlGeomPoint(fromLonLat([lng, lat]), "XY"),
    name: IconTypeName,
  })

  iconFeature.setProperties({ info: BindingInfo });
  iconFeature.setId(BindingInfo.id);

  //将图标特性添加进矢量中
  let iconText = BindingInfo.name;
  let iconStyle = GetFeatureStyle__Dev(iconText, ImageSrc, BindingInfo.offestX);

  iconFeature.setStyle(iconStyle[0], ImageSrc);
  // 去掉设备标签   临时注释掉
  //iconFeature.setStyle(iconStyle,ImageSrc);
  Get_Dev_Layer().getSource().addFeature(iconFeature);
}

/**
 * 临时标记
 * @param IconText
 * @param lat
 * @param lng
 * @constructor
 */
export function Draw_Temp_Icon_Marker(IconText, { lat, lng }) {
  let iconFeature = new OlFeature({
    geometry: new OlGeomPoint([lng, lat], "XY"),
    name: IconText,
  })

  let Styles = [];
  Styles.push(
    new Style({
      image: new OlStyleIcon({
        src: TARGET_TEMP_IMG,
      })
    })
  )

  // 设置style
  Styles.push(
    new Style({
      text: new Text({
        textAlign: 'left',
        text: IconText,
        font: '16px Microsoft YaHei',
        offsetX: -35,
        offsetY: 30,
        backgroundStroke: new OlStyleStroke({
          color: 'rgba(0,0,0,1)',
          width: 1
        }),
        //标签的背景填充
        backgroundFill: new OlStyleFill({
          color: '#42DCFD'
        }),
        fill: new OlStyleFill({
          color: 'rgba(255,255,255,1)',
        })
      })
    })
  )

  iconFeature.setStyle(Styles);

  Get_Marque_Layer().getSource().addFeature(iconFeature);
}

export function Draw_Target_SectorMarker_Select(iconFeature, colorFill, colorTans) {
  let iconStyle = GetTGSectorSelectStyle(colorFill, colorTans);
  iconFeature.setStyle(iconStyle);
}

export function Draw_SelectIcon_Marker(iconFeature, isSelect) {
  let info = iconFeature.getProperties().info;

  let tgstatus = info.tgStates;

  let iconStyle = GetFeatureStyle__Al(info.id, isSelect, info.alarmLevel, tgstatus, info.uavSn);

  iconFeature.setStyle(iconStyle);
}

export function Draw_SelectDevIcon_Marker(iconFeature, ImageSrc) {
  let info = iconFeature.getProperties().info;

  let iconStyle = GetFeatureStyle__Dev(info.name, ImageSrc, info.offestX);

  iconFeature.setStyle(iconStyle[0]);
}

/**
 * 绘制扇形区域
 * @param center
 * @param radius
 * @param start
 * @param end
 * @param opt_sphereRadius
 */
export function createFan(center, radius, start, end, opt_sphereRadius = 0) {
  let flatCoordinates = [];
  flatCoordinates.push(center[0])
  flatCoordinates.push(center[1])
  for (let i = start; i < end; ++i) {
    extend(flatCoordinates, sphereOffset(center, radius, 2 * Math.PI * i / 360, opt_sphereRadius));
    //extend(flatCoordinates,gisTool({lon:center[0],lat:center[1]},i,radius));
  }
  flatCoordinates.push(flatCoordinates[0], flatCoordinates[1]);
  return new Polygon(flatCoordinates, GeometryLayout.XY, [flatCoordinates.length]);
}

//展示弹框信息
export function Show_Overlay(feature, element) {
  let map = Get_Main_Map();
  element.hidden = false;

  let marker = new Overlay({
    position: [feature.getGeometry().extent_[0], feature.getGeometry().extent_[1]],
    positioning: 'top-left',//选填参数，控制marker的相对位置
    element: element,
    stopEvent: true//选填参数，阻止默认事件行为
  });

  marker.on("change:position", function () {
    console.log("位置改变！");
  })

  map.addOverlay(marker);
}

//告警区域
export function Draw_Warning_RoundMarker({ lat, lng }, radius, devId, startR, endR) {

  //console.log('cccccccc',radius,radius);
  // let point = circular([lng, lat], radius,90 ,360)
  //   .clone().transform("EPSG:4326", "EPSG:3857");

  let point = createFan([lng, lat], radius, startR, endR)
    .clone().transform("EPSG:4326", "EPSG:3857");


  let iconFeature = new OlFeature({
    geometry: point,
    name: featureType_enum.devRange,
  })

  let iconFeatureStyle = new Style({
    fill: new Fill({
      color: mapColor.rang_red
    }),
    stroke: new Stroke({
      color: mapColor.rang_red_stroke
    })
  });

  //iconFeature.setId(`eq_rang_`);
  iconFeature.setStyle(iconFeatureStyle);
  iconFeature.setId(devId + featureType_enum.devRange);//WarningArea

  //console.log('WARNING_MAKER',iconFeature);
  Get_Dev_Layer().getSource().addFeature(iconFeature);
}

//告警区域等距线
export function Draw_Warning_Marker_Line({ lat, lng }, coverR, devId) {
  let point;
  let count = coverR / 1000;

  for (let i = 1; i <= count; i++) {
    point = circular([lng, lat], i * 1000, 360)
      .clone().transform("EPSG:4326", "EPSG:3857");

    let feature = new OlFeature(
      {
        geometry: point,
        name: featureType_enum.devLine,
      }
    );

    feature.setProperties({ info: count });
    feature.setId(devId + featureType_enum.devLine + i) //'WarningLine' +i);

    feature.setStyle([
      GetEquipmentLineStyle(1), GetEquipLineTextStyle(`${i}km`, 14, i * 1000)
    ]);

    //console.log(feature.getProperties().name);
    Get_Dev_Layer().getSource().addFeature(feature);
  }
}

// 移除目标
export function Remove_TargetFeature(id) {
  //删除目标
  let _feature = Get_Target_Layer().getSource().getFeatureById(id);
  if (null === _feature) {
  } else {
    Get_Target_Layer().getSource().removeFeature(_feature);
  }
  //删除航迹
  let _featureFlyAddr = Get_Target_Layer().getSource().getFeatureById(id + 'FlyAddr');
  if (null === _featureFlyAddr) {
  } else {
    Get_Target_Layer().getSource().removeFeature(_featureFlyAddr);
  }
  //删除飞手
  let _featureFlyer = Get_Target_Layer().getSource().getFeatureById(id + 'Flyer');
  if (null === _featureFlyer) {
  } else {
    Get_Target_Layer().getSource().removeFeature(_featureFlyer);
  }
}

// 移除设备
export function Remove_DevFeature(id) {
  let _feature = Get_Dev_Layer().getSource().getFeatureById(id);
  if (null === _feature) {
  } else {
    Get_Dev_Layer().getSource().removeFeature(_feature);
    // 还需一起移除等距线和告警区域
  }
}

// 画扇形目标文字的样式
export function Draw_Target_SectorTextMarker(centerPoint, id, colorFill) {
  let textFeature = new OlFeature({
    geometry: new OlGeomPoint(fromLonLat([centerPoint[0], centerPoint[1]]), "XY"),
    name: 'XY',
  })

  let textStyle = GetTGSectorTextStyle(id, colorFill);

  textFeature.setStyle(textStyle);
  textFeature.setId(id + featureType_enum.targetSectorText);

  Get_Target_Layer().getSource().addFeature(textFeature);
}
// 画扇形目标
export function Draw_Target_SectorMarker(
  { lat, lng }, covertR, positionAngle, probeAngle, BindingInfo) {
  let covertStar = positionAngle - probeAngle;
  let covertEnd = positionAngle + probeAngle;

  let point = createFan([lng, lat], covertR, covertStar, covertEnd)
    .clone().transform("EPSG:4326", "EPSG:3857");

  let centerPointX = point.getInteriorPoint().flatCoordinates[0];
  let centerPointY = point.getInteriorPoint().flatCoordinates[1];

  let centerPoint = toLonLat([centerPointX, centerPointY]);

  let colorTans = GetTargetAlarmColor(BindingInfo.alarmLevel, 0.2);
  let colorFill = GetTargetAlarmColor(BindingInfo.alarmLevel, 1);

  // 文字ID 和 图像 是分开的
  Draw_Target_SectorTextMarker(centerPoint, BindingInfo.id, colorFill);

  let iconFeature = new OlFeature({
    geometry: point,
    name: featureType_enum.targetSector,
  })

  let iconFeatureStyle = GetTGSectorStyle(colorFill, colorTans);

  iconFeature.setProperties({ info: BindingInfo });
  iconFeature.setId(BindingInfo.id);
  iconFeature.setStyle(iconFeatureStyle);

  Get_Target_Layer().getSource().addFeature(iconFeature);
}

export function DrawBindLine(id, lng, lat) {

}

// 显示隐藏设备的告警线
export function HiddenDevWarnLine(id, VisBoolean) {
  let feature_ = Get_Dev_Layer().getSource().getFeatureById(id + featureType_enum.devLine + 1);

  if (null == feature_) return;

  if (VisBoolean) {
    feature_.setStyle([
      GetEquipmentLineStyle(1),
      GetEquipLineTextStyle(`${1}km`, 14, 1 * 1000)
    ]);
  } else {
    let hiddenStyle = new Style({
      stroke: new Stroke({
        lineDash: [3, 3],
        color: [0, 0, 0, 0]
      })
    });
    feature_.setStyle(hiddenStyle);
  }

  // 检测其余的告警线
  for (let k = 1; k <= feature_.getProperties().info; k++) {
    let feature__ = Get_Dev_Layer().getSource().getFeatureById(id + featureType_enum.devLine + k);

    if (null == feature__) {
      continue;
    }

    if (VisBoolean) {
      feature__.setStyle([
        GetEquipmentLineStyle(1),
        GetEquipLineTextStyle(`${k}km`, 14, 1 * 1000)
      ]);
    }
    else {
      let hiddenStyle = new Style({
        stroke: new Stroke({
          lineDash: [3, 3],
          color: [0, 0, 0, 0]
        })
      });
      feature__.setStyle(hiddenStyle);
    }
  }
}

// 显示隐藏设备的告警区域
export function HiddenDevWarnRound(id, VisBoolean) {
  let feature_ = Get_Dev_Layer().getSource().getFeatureById(id + featureType_enum.devRange);

  if (null == feature_) return;

  if (VisBoolean) {
    let iconFeatureStyle = new Style({
      fill: new Fill({
        color: mapColor.rang_red
      }),
      stroke: new Stroke({
        color: mapColor.rang_red_stroke
      })
    });

    feature_.setStyle(iconFeatureStyle);
  } else {
    let hiddenStyle =
      new Style({
        fill: new Fill({
          color: [0, 0, 0, 0]
        }),
        stroke: new Stroke({
          color: [0, 0, 0, 0]
        })
      });
    feature_.setStyle(hiddenStyle);
  }
}

/**
 * 封装设备绑定Feature
 * @param _points
 * @param _id
 * @returns {Feature}
 * @private
 */
function __packageBindFeature(_points, _id) {
  let _feature = new OlFeature({
    geometry: new Polygon(_points, GeometryLayout.XY, [_points.length])
      .clone()
      .transform("EPSG:4326", "EPSG:3857")
  });
  _feature.setId(_id);
  _feature.setStyle(
    new Style({
      stroke: new Stroke({
        lineDash: [1, 5],
        color: [10, 226, 227, 0.6],
        width: 2
      })
    })
  );
  return _feature;
}

// 修改设备状态图片
export function ChangeDevFeatureImg(id, devStatus, category) {
  let feature_ = Get_Dev_Layer().getSource().getFeatureById(id);

  if (null == feature_) { return; }

  let imgSrc = '';
  let imgSelectSrc = '';

  if (category <= 20000) {
    if (category > 10200&&category<10400) {
      imgSrc = PINPU_ICON ;
      imgSelectSrc = PINPU_ICON ;
    } else if(category>10400){
      imgSrc = YUNSHAO_ICON ;
      imgSelectSrc = YUNSHAO_ICON ;
    }
    else {
      imgSrc = RADAR_ICON ;
      imgSelectSrc = RADAR_ICON ;
    }
  }
  else if (category > 20000 && category <= 30000) {
    imgSrc = GUANGDIAN_ICON ;
    imgSelectSrc = GUANGDIAN_ICON ;
  }
  else if (category > 30000 && category <= 30500) {
    imgSrc = ATTACK_ICON ;
    imgSelectSrc = ATTACK_ICON ;
  }
  // else if (category > 30500) {
  //   imgSrc = DECOY_ICON ;
  //   imgSelectSrc = DECOY_SELECT_ICON ;
  // }

  let info = feature_.getProperties().info;

  info.devImg = imgSrc + devStatus + '.png'
  info.devSelectImg = imgSelectSrc + devStatus + '.png'

  // 重新绑定信息
  feature_.setProperties({ info: info });
  //console.error(id+"-------------"+devStatus+"--------"+info.devImg+"yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy");
  // 设置样式
  if (store.state.select_was_info.id == id) {

    Draw_SelectDevIcon_Marker(feature_, info.devSelectImg); // 重置目标图标
  } else {

    Draw_SelectDevIcon_Marker(feature_, info.devImg); // 重置目标图标
  }
}
//选中目标
export function Select_TargetDraw(feature_, selectType) {
  let ids = ['', ''];
  // 上一组选中的信息是否跟现选中的一致
  let justClear = (store.state.select_was_info.id == feature_.getProperties().info.id) ? true : false;
  //console.error(deviceInfoId.deviceInfoShow+"99999999999999999996666666666666666666666666666666");
  // 清空之前选中的feature
  if ('' != store.state.select_was_info.id) {
    switch (store.state.select_was_info.type) {
      case featureType_enum.target:
        document.getElementById('TargetInfoViewerDisplay').style.display = "none";
        ids[0], ids[1] = '';
        let SelectTgFeature = Get_Target_Layer().getSource().getFeatureById(store.state.select_was_info.id);
        Draw_SelectIcon_Marker(SelectTgFeature, false); // 重置目标图标

        if (justClear) {
          let data = {
            XTentacionTg: 'none',
            XTentacionDev: 'none',
            XTentacionNone: '',
            XTentacionDevCategory: '',
          };
          bus.$emit('OnChangeSelectType', data);
          store.dispatch('set_select_was_info', { id: '', type: featureType_enum.target });
        }
        break;
      case featureType_enum.dev:
        ids[0], ids[1] = '';
        let SelectDevFeature = Get_Dev_Layer().getSource().getFeatureById(store.state.select_was_info.id);

        //20220913 原为点击地图上的设备图表后变一下设备图标为选中状态，需改为点一下图标后弹出DeviceInfo，再次点击则关闭
        Draw_SelectDevIcon_Marker(SelectDevFeature, SelectDevFeature.getProperties().info.devImg); // 重置目标图标

        //deviceInfoId.deviceInfoShow=!deviceInfoId.deviceInfoShow;
        if (justClear) {
          let data = {
            XTentacionTg: 'none',
            XTentacionDev: 'none',
            XTentacionNone: '',
            XTentacionDevCategory: '',
          };
          bus.$emit('OnChangeSelectType', data);
          store.dispatch('set_select_was_info', { id: '', type: featureType_enum.dev });
        }
        break;
      case featureType_enum.targetSector:
        document.getElementById('TargetInfoViewerDisplay').style.display = "none";
        ids[0], ids[1] = '';

        let iconFeature = Get_Target_Layer().getSource().getFeatureById(store.state.select_was_info.id);

        let colorTans = GetTargetAlarmColor(iconFeature.getProperties().info.alarmLevel, 0.2);
        let colorFill = GetTargetAlarmColor(iconFeature.getProperties().info.alarmLevel, 1);

        let iconFeatureStyle = GetTGSectorStyle(colorFill, colorTans);
        iconFeature.setStyle(iconFeatureStyle);

        if (justClear) {
          let data = {
            XTentacionTg: 'none',
            XTentacionDev: 'none',
            XTentacionNone: '',
            XTentacionDevCategory: '',
          };
          bus.$emit('OnChangeSelectType', data);
          store.dispatch('set_select_was_info', { id: '', type: featureType_enum.targetSector });
        }
        break;
      default:
        break;
    }
  }

  if (justClear) { return ids }

  let data;
  switch (selectType) {
    case featureType_enum.target:
      // 显示 无人机信息窗口
      document.getElementById('TargetInfoViewerDisplay').style.display = "";
      // 设置新的选中目标
      ids[0] = feature_.getProperties().info.id;
      ids[1] = feature_.getProperties().info.deviceId;

      store.dispatch('set_select_was_info', { id: ids[0], type: featureType_enum.target });
      Draw_SelectIcon_Marker(feature_, true);
      data = {
        XTentacionTg: '',
        XTentacionDev: 'none',
        XTentacionNone: 'none',
        XTentacionDevCategory: '',
      };
      bus.$emit('OnChangeSelectType', data);
      break;
    case featureType_enum.dev:
      store.dispatch('set_select_was_info', { id: feature_.getProperties().info.id, type: featureType_enum.dev });
      data = {
        XTentacionTg: 'none',
        XTentacionDev: '',
        XTentacionNone: 'none',
        XTentacionDevCategory: feature_.getProperties().info.category,
      };
      bus.$emit('OnChangeSelectType', data);
      Draw_SelectDevIcon_Marker(feature_, feature_.getProperties().info.devSelectImg);
      console.error(deviceInfoId.deviceInfoShow+"99999999999999999996666666666666666666666666666666");
      deviceInfoId.deviceInfoShow=!deviceInfoId.deviceInfoShow;
      break;
    case featureType_enum.targetSector:
      // 显示 无人机信息窗口
      document.getElementById('TargetInfoViewerDisplay').style.display = "";
      // 设置新的选中目标
      ids[0] = feature_.getProperties().info.id;
      ids[1] = feature_.getProperties().info.deviceId;

      store.dispatch('set_select_was_info', { id: ids[0], type: featureType_enum.targetSector });

      data = {
        XTentacionTg: '',
        XTentacionDev: 'none',
        XTentacionNone: 'none',
        XTentacionDevCategory: '',
      };
      bus.$emit('OnChangeSelectType', data);

      let colorTans = GetTargetAlarmColor(feature_.getProperties().info.alarmLevel, 0.2);
      let colorFill = GetTargetAlarmColor(feature_.getProperties().info.alarmLevel, 1);

      Draw_Target_SectorMarker_Select(feature_, colorFill, colorTans);
      break;
    default:
      break;
  }

  return ids;
}

/**
 * 根据 选中状态 威胁等级 获取通常目标的样式
 * @param id
 * @param isSelect
 * @param threat
 * @param tgstatus
 * @returns {Array}
 */
export function GetFeatureStyle__Al(id, isSelect, threat, tgstatus, sn) {
  let color = GetTargetAlarmColor(threat, 1);

  // 获取样式的类型 选中true 常规false
  let Styles = [];
  switch (isSelect) {
    case true:
      Styles = GetTGUsuallyStyle(sn, color, threat);
      // push进选中的样式
      let style_select = GetTGSelectStyle(threat, color);

      style_select.forEach(style => {
        Styles.push(style);
      });

      break;
    case false:
      Styles = GetTGUsuallyStyle(sn, color, threat);
      break;
  }

  if (tgstatus) {
    let tgImgText = "";

    if (tgstatus.isTracking) {
      tgImgText += GetTargetStatusImgText(tgState_enum.track);
    }
    if (tgstatus.isTranspond) {
      tgImgText += GetTargetStatusImgText(tgState_enum.transpond);
    }
    if (tgstatus.isHitting) {
      tgImgText += GetTargetStatusImgText(tgState_enum.hit);
    }
    if (tgstatus.isTicking) {
      tgImgText += GetTargetStatusImgText(tgState_enum.tick);
    }
    if (tgstatus.isMonitoring) {
      tgImgText += GetTargetStatusImgText(tgState_enum.monitor);
    }

    if (tgImgText != "") { Styles.push(GetTGIMGText(tgImgText, color)) }
  }

  return Styles;
}

// 获取SVG文字图片资源
export function GetTgStatusSVGIMG(feature_, id, threat, tgstatus) {
  let isSelect = (id == store.state.select_target_id) ? true : false;
  let sn = feature_.getProperties().info.uavSn;
  let Styles = GetFeatureStyle__Al(id, isSelect, threat, tgstatus, sn);
  // let Styles = GetFeatureStyle__Al(id, isSelect, threat, tgstatus);
  feature_.setStyle(Styles);
}

// 告警區域
export function Draw_Zone(_info) {
  let _layer = Get_Zone_Layer();

  if (_info.zonePoints !== "") {
    let zone = JSON.parse(_info.zonePoints);

    if (zone["type"] === "Circle") {
      let _feature = new OlFeature({
        geometry: new OLGeomCircle(zone.center, zone.radius, zone.layout)
      });

      let style = GetWarnningZoneStyle(_info);

      // 去掉告警区域名称
      _feature.setStyle(style);

      _feature.drawType = "Circle";
      _feature.setId(featureId(_info));

      _layer.getSource().addFeature(_feature);
    } else if (
      zone.type === "Polygon" &&
      zone.coordinates &&
      zone.coordinates.length > 0
    ) {
      let _feature = new OlFeature({
        geometry: new Polygon(zone.coordinates, zone.layout)
      });

      let style = GetWarnningZoneStyle(_info);
      _feature.setStyle(style);

      _feature.drawType = "Polygon";
      _feature.setId(featureId(_info));
      _layer.getSource().addFeature(_feature);
    }
  }
}

function featureId(info) {
  const { id, name } = info;
  return `${name}_${id}`;
}

/**
 *获取设备等距线样式
 * @param _type
 */
export const GetEquipmentLineStyle = (_type = 0) => {
  if (_type === 1) {
    return EquipmentLineStyle.line_black;
  } else if (_type === 2) {
    return EquipmentLineStyle.line_black;
  } else { return EquipmentLineStyle.line_red; }
};

// 获取转换成SVG文字的图片信息
export function GetTargetStatusImgText(tgStatus) {
  let TgStatusImgText;
  switch (tgStatus) {
    case tgState_enum.hit:
      //TgStatusImgText = `${String.fromCharCode("0xe90c")} `
      TgStatusImgText = '打击 '
      break;
    case tgState_enum.monitor:
      //TgStatusImgText = `${String.fromCharCode("0xe919")} `
      TgStatusImgText = '监视 '
      break;
    case tgState_enum.tick:
      //TgStatusImgText = `${String.fromCharCode("0xe921")} `
      TgStatusImgText = '诱骗 '
      break;
    case tgState_enum.track:
      //TgStatusImgText = `${String.fromCharCode("0xe912")} `
      TgStatusImgText = '跟踪 '
      break;
    case tgState_enum.transpond:
      //TgStatusImgText = `${String.fromCharCode("0xe922")} `
      TgStatusImgText = ''
      break;
    default:
      break;
  }
  return TgStatusImgText;
}

// 根据ID移除TG
export function RemoveTargetById(id) {
  //移除feature
  let feature_ = Get_Target_Layer().getSource().getFeatureById(id);
  if (null != feature_) { Remove_TargetFeature(id) }
  //如果是选中的目标ID
  if (store.state.select_target_id != '' && id == store.state.select_target_id) {
    store.dispatch('set_select_target_id', { id: '', did: '' });// 重置选中目标ID
    //document.getElementById('VideoViewerDisplay').style.display = 'none';
    document.getElementById('TargetInfoViewerDisplay').style.display = "none";

    let data = {
      XTentacionTg: 'none',
      XTentacionDev: 'none',
      XTentacionNone: '',
      XTentacionDevCategory: '',
    };

    bus.$emit('OnChangeSelectType', data);
    store.dispatch('set_select_was_info', { id: '', type: featureType_enum.target });
  }
}

export const tgState_enum = {
  hit: 'hitRelationShip',
  monitor: 'monitorRelationShip',
  tick: 'tickRelationShip',
  track: 'trackRelationShip',
  transpond: 'transpondRelationShip',
}

export const featureType_enum = {
  target: 'target',// 目标
  targetSector: 'targetSector',// 扇形目标
  dev: 'dev', // 设备
  devLine: 'devLine',// 设备等距线
  devRange: 'devRange', // 设备覆盖范围
  alarmWarnRange: 'alarmWarnRange',// 告警区域
  targetSectorText: 'targetSectorText',
  flyer: 'flyer',//飞手
  flyAddr: 'flyAddr'//航迹
}


// 画出阵地弹出的设备图标
export function Draw_Position_Device_Icon_Marker(IconText, { lat, lng }, ImageSrc, BindingInfo, IconTypeName) {
  let iconFeature = new OlFeature({
    geometry: new OlGeomPoint(fromLonLat([lng, lat]), "XY"),
    name: IconTypeName,
  })

  iconFeature.setProperties({ info: BindingInfo });
  iconFeature.setId(BindingInfo.id);

  //将图标特性添加进矢量中
  let iconText = BindingInfo.name;
  let iconStyle = GetFeatureStyle__Dev(iconText, ImageSrc, BindingInfo.offestX);

  iconFeature.setStyle(iconStyle[0], ImageSrc);
  // 去掉设备标签   临时注释掉
  //iconFeature.setStyle(iconStyle,ImageSrc);
  Get_Dev_Layer().getSource().addFeature(iconFeature);
}

// 画出阵地主图标即云哨位置的图标
export function Draw_Position_Main_Icon_Marker(IconText, { lat, lng }, ImageSrc, BindingInfo, IconTypeName) {
  let iconFeature = new OlFeature({
    geometry: new OlGeomPoint(fromLonLat([lng, lat]), "XY"),
    name: IconTypeName,
  })

  iconFeature.setProperties({ info: BindingInfo });
  iconFeature.setId(BindingInfo.id+"Main");

  //将图标特性添加进矢量中
  let iconText = BindingInfo.name;
  let iconStyle = GetFeatureStyle__Dev(iconText, ImageSrc, BindingInfo.offestX);

  iconFeature.setStyle(iconStyle[0], ImageSrc);

  // 去掉设备标签   临时注释掉
  //iconFeature.setStyle(iconStyle,ImageSrc);
  Get_Dev_Layer().getSource().addFeature(iconFeature);
}
