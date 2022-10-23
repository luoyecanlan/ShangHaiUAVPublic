import Map from "ol/Map";
import View from "ol/View";
import {fromLonLat, transform} from "ol/proj";
import Feature from "ol/Feature";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import {Draw, Modify, Snap, Translate} from "ol/interaction";
import {Circle, Polygon} from "ol/geom";
import {Warnning_Zone_Style} from "./styles";
import store from '../../store'
import bus from "../../modes/tool/bus";
import {http_request_await, request_current, request_map_info_list, request_map_info,
  create_person_info, request_person_info_list} from "../../modes/api";
import ScaleLine from "ol/control/ScaleLine";
import FullScreen from "ol/control/FullScreen";
import ZoomSlider from "ol/control/ZoomSlider";
import ZoomControl from "ol/control/Zoom";
import {Fly_To} from "../mapHandle/mapMove";
import {Select_TargetDraw,featureType_enum} from "../mapHandle/mapDraw";
import OSM from 'ol/source/OSM'
import XYZ from 'ol/source/XYZ'
import {defaults as defaultControls, OverviewMap} from 'ol/control.js';
import DeviceInfoClickShow from "../../components/DeviceInfoClickShow"
Vue.component(DeviceInfoClickShow.name,DeviceInfoClickShow)

// 用来添加相关文字描述的
import Text from 'ol/style/Text'
import Fill from 'ol/style/Fill'
import Style from "ol/src/style/Style";

import {forEach} from "ol/geom/flat/segments";
import Vue from "vue";

/**
 * 获取地图中心位置
 * @constructor
 */
function Get_Map_Center() {
  //console.log(Vue.prototype.$MAP_CENTER)
  return [121.658733, 31.1487694]; //Vue.prototype.$MAP_CENTER;116.570607, 39.777473
}

/**
 * 初始化地图
 * @param source
 * @returns {TileLayer}
 * @constructor
 */
function Init_TileLayer(source) {
  return new TileLayer({
    source
  });
}

/**
 * 初始化图层
 * @param _opacity
 * @returns {VectorLayer}
 * @constructor
 */
function Init_VectorLayer(_opacity = 1) {
  let source = new VectorSource();
  return new VectorLayer({
    source,
    opacity: _opacity
  });
}

/**
 * 地图 区域层 绘制层
 */
let map, initLayer, zoneLayer, drawLayer, drawEvent, translateEvent,
  targetLayer, devLayer, marqueLayer, rangingLayer,
  mapInfo = {
    name: '', address: '', mapId: 0,
    zoomMax: 0, zoomMin: 0, zoomDefault: 0,
  }; // 地图初始化用到的参数;

/**
 * 按钮切换时使用
 * @param flag
 * @constructor
 */
export function MapTile_Init(flag) {
  if (flag == 0)//离线地图
  {
    map.removeLayer(initLayer);

    let offLineUrl = '';
    if ('' != store.state.map_offline_address) {
      offLineUrl = store.state.map_offline_address
    } else {
      offLineUrl = 'http://47.92.98.138:11001/{z}/{x}/{y}.png'
    }

    initLayer = new TileLayer(
      {
        source: new XYZ
        ({
          url: offLineUrl
        })
      })
    let layersArray = map.getLayers();
    layersArray.insertAt(1, initLayer);
  } else if (flag == 1) {
    map.removeLayer(initLayer);

    let onLineUrl = '';
    if ('' != store.state.map_online_address) {
      onLineUrl = store.state.map_online_address
    } else {
      onLineUrl = 'http://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png'
    }

    initLayer = new TileLayer(
      {
        source: new XYZ
        ({
          url: onLineUrl
        })
      })
    let layersArray = map.getLayers();
    layersArray.insertAt(1, initLayer);
  }
}

export async function Get_Init_Map_Info() {
}

/**
 * 根据用户设置加载在线或离线地图
 * @param mapType
 * @constructor
 */
function Init_Map_Source_Layer() {
  switch (mapInfo.name) {
    case 'offlineconfig':
      let offLineUrl = '';
      if (null != mapInfo.address && '' != mapInfo.address) {
        offLineUrl = mapInfo.address
      } else {
        offLineUrl = 'http://47.92.98.138:11001/{z}/{x}/{y}.png'
      }
      initLayer = new TileLayer(
        {
          source: new XYZ
          ({
            url: offLineUrl
          })
        });
      store.dispatch('set_map_type', false);
      break;
    case 'onlinemapconfig':
      let onLineUrl = '';
      // 优先使用获取到的地址
      if (null != mapInfo.address && '' != mapInfo.address) {
        onLineUrl = mapInfo.address
      } else {
        onLineUrl = 'http://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png'
      }

      initLayer = new TileLayer(
        {
          source: new XYZ
          ({
            url: onLineUrl
          })
        });
      break;
    default:
      break;
  }
}

/**
 * 初始化地图
 * @param _id
 * @constructor
 */
export async function Init_Map(_id)
{
  let userInfo = await http_request_await(request_current, '');

  if (null != userInfo) {
    //console.log('获取到了用户信息---', userInfo);
    store.dispatch('commit_login_info', userInfo);

    let person_info_list = await http_request_await(request_person_info_list, '');

    let flag = true;

    if (null != person_info_list)
    {
      //console.log('获取到了个性化配置信息---', person_info_list);

      person_info_list.forEach(info =>
      {
        if (store.state.login_info.id == info.userId)
        {
          flag = false;
          store.dispatch('set_user_config_info', info);

          let map_info = http_request_await(request_map_info, info.id);

          if (null != map_info)
          {
            mapInfo.mapId = info.mapId;
            if (info.name == 'offlineconfig') {
              store.dispatch('set_map_type', false);
            }
          }
        }
      })
    }

    if(flag){
      // 如果未配置个性化配置信息 提交初始化配置
      let create_map_info = {
        userId: store.state.login_info.id,
        mapId: 2,
        deviceCover: "",
        deviceLine: "",
        filterVMin: -100,
        filterVMax: 200,
        filterDiscMin: 0,
        filterDiscMax: 200,
        filterAltMin: 0,
        filterAltMax: 3000,
        filterThreat: '{"zeroLevelCheck":true,"oneLevelCheck":true,"twoLevelCheck":true,"threeLevelCheck":true,"outLevelCheck":true}',
        filterCategory: "",
        other: "",
        updatetime: new Date()
      }

      mapInfo.mapId = create_map_info.mapId;
      let map_info_ = await http_request_await(create_person_info, create_map_info);
      if (null != map_info_) {
        console.log("未找到个性化配置。已添加默认配置");
      }
    }

    let my_map_info = await http_request_await(request_map_info_list, '');
    //console.log('开始获取地图信息',my_map_info);
    if (null != my_map_info)
    {
      my_map_info.forEach(info =>
        {
          if (mapInfo.mapId == info.id)
          {
            mapInfo.name = info.name;
            mapInfo.address = info.url;
            mapInfo.zoomMax = info.zoomMax;
            mapInfo.zoomMin = info.zoomMin;
            mapInfo.zoomDefault = info.zoomDefault;
          }
          store.dispatch('set_map_info', info);
        }
      )
    }
  }

  zoneLayer = Init_VectorLayer();
  drawLayer = Init_VectorLayer();
  targetLayer = Init_VectorLayer();
  devLayer = Init_VectorLayer();
  marqueLayer = Init_VectorLayer();
  rangingLayer = Init_VectorLayer();

  Init_Map_Source_Layer();

  // 向地图添加比例尺
  let scaleLineControl = new ScaleLine({
    Units: 'metric',//单位有5种：degrees imperial us nautical metric
    className: 'scale',//必须加上该参数
    target: document.getElementById('scale-line') //显示比例尺的目标容器
  });
  let zoomControl = new ZoomControl({
    className: 'zoom',//必须加上该参数
    index:999,
    target:map
  });
  let fullScreen = new FullScreen({
    className: 'full',//必须加上该参数
    target:map
  });
  let zoomSlider = new ZoomSlider({
    className: 'slider',//必须加上该参数
    target:map
  });
  //初始化地图
  map = new Map({
    controls:defaultControls({zoom:true,ratate:true}).extend([
      scaleLineControl
    ]),
    target: _id,
    layers: [initLayer,drawLayer,devLayer,zoneLayer,marqueLayer,rangingLayer,targetLayer],
    view: new View({
      center: fromLonLat(Get_Map_Center()),
      zoom: mapInfo.zoomDefault,
      minZoom: mapInfo.zoomMin,
      maxZoom: mapInfo.zoomMax,
      // controls:defaultControls({zoom:true}).extend([]),
      projection: "EPSG:3857"
    })
  });

  let overviewMapControl = new OverviewMap({
    className: 'ol-overviewmap ol-custom-overviewmap',    //鹰眼控制样式
    //在鹰眼中加载不同的数据源图层
    layers: [Init_Over_TileLayer('8d32a0ab14d14f75b4df7ef5a6f020b2')],
    target:document.getElementById('HawkeyeViewer')
  });
  map.addControl(overviewMapControl);
  map.addControl(zoomControl);
  map.addControl(fullScreen);
  map.addControl(zoomSlider);
  document.getElementById('HawkeyeViewer').style.display = 'none';

  //初始化Snap (鼠标吸附效果)
  let snap = new Snap({
    source: drawLayer.getSource()
  });

  map.addInteraction(snap);

  //地图初始化完成 通知事件
  bus.$emit('MapInitSuccess');

  let targetListfilter = {
    minSpeed:store.state.user_config_info.filterVMin,
    maxSpeed:store.state.user_config_info.filterVMax,
    minAlt:store.state.user_config_info.filterAltMin,
    maxAlt:store.state.user_config_info.filterAltMax,
    zeroLevelCheck:true,
    oneLevelCheck:true,
    twoLevelCheck:true,
    threeLevelCheck:true,
    outLevelCheck:true,
  }

  if(store.state.user_config_info.filterThreat != '')
  {
    try
    {
      let ThreatLevelJson = JSON.parse(store.state.user_config_info.filterThreat);
      //console.log('成功解析Threat-->',ThreatLevelJson);
      targetListfilter.zeroLevelCheck=ThreatLevelJson.zeroLevelCheck;
      targetListfilter.oneLevelCheck=ThreatLevelJson.oneLevelCheck;
      targetListfilter.twoLevelCheck=ThreatLevelJson.twoLevelCheck;
      targetListfilter.threeLevelCheck=ThreatLevelJson.threeLevelCheck;
      targetListfilter.outLevelCheck=ThreatLevelJson.outLevelCheck;
    }
    catch (e) {}
  }

  if(store.state.user_config_info.deviceCover != '')
  {
    try
    {
      let CoverList = JSON.parse( store.state.user_config_info.deviceCover)
      //console.log('成功解析CoverList-->',CoverList);
      store.dispatch('set_dev_cover_line_status', {type:'cover',data:CoverList});
    }
    catch (e)
    {
      console.log('捕获异常---未能成功解析CoverList Json');
    }
  }else{
    console.log('CoverList值为空');
    store.dispatch('set_dev_cover_line_status', {type:'cover',data:[]});
  }

  if(store.state.user_config_info.deviceLine != '')
  {
    try
    {
      let LineList = JSON.parse(store.state.user_config_info.deviceLine)
      //console.log('成功解析LineList-->',LineList);
      store.dispatch('set_dev_cover_line_status', {type:'line',data:LineList});
    }
    catch (e)
    {
      console.log('捕获异常---未能成功解析LineList Json');
    }
  }else{
    console.log('LineList值为空');
    store.dispatch('set_dev_cover_line_status', {type:'line',data:[]});
  }

  // 目标列表的筛选配置条件
  //console.log('targetListfilter',targetListfilter)
  bus.$emit('ResetTargetListFilter',targetListfilter);

  //注册移动事件 比例尺
  map.on('pointermove', function (e)
  {
    //鼠标移动事件
    let lonlat = transform(e.coordinate, 'EPSG:3857', 'EPSG:4326');
    document.getElementById("longitudeSpan").innerText = lonlat[0].toFixed(6) + "°";
    document.getElementById("latitudeSpan").innerText = lonlat[1].toFixed(6) + "°";
  });

  map.on('click',mapMainClick);
}

/**
 * 注册修改完成事件
 * @param callback
 * @constructor
 */
export function Register_Modify_Event(callback) {
  let modify = new Modify({source: drawLayer.getSource()});
  if (callback) {
    modify.on("modifyend", e => callback(e));
  }
  map.addInteraction(modify);
}

/**
 * 注册绘制完成事件
 * @param callback
 * @constructor
 */
export function Register_Draw_Event(_drawType, callback) {
  let source = drawLayer.getSource();
  //初始化绘制功能
  drawEvent = new Draw({
    source,
    type: _drawType
  });
  drawEvent.on("drawend", e => {
    callback(e);
  });
  map.addInteraction(drawEvent);
}

/**
 * 移除绘制结束事件
 * @constructor
 */
export function Unregister_Draw_Event() {
  map.removeInteraction(drawEvent);
}

/**
 * 鹰眼
 * @param key
 * @returns {TileLayer}
 * @constructor
 */
export function Init_Over_TileLayer(key) {
  return new TileLayer({
    source: new OSM({
      'url': `https://{a-c}.tile.thunderforest.com/cycle/{z}/{x}/{y}.png?apikey=${key}`
    })
  })
}

/**
 * 获取图层
 * @param key
 * @constructor
 */
export function Get_Layer(draw) {
  return draw ? drawLayer : zoneLayer;
}

function featureId(info) {
  const {id, name} = info;
  return `${name}_${id}`;
}

/**
 * 注册图形移动事件
 * @param _feature
 * @param callback
 * @constructor
 */
function Register_Translate_Event(_layer, _feature, callback) {
  translateEvent = new Translate({
    layers: [_layer],
    features: [_feature]
  });
  translateEvent.on("translateend", e => callback(e));
  map.addInteraction(translateEvent);
}

/**
 * 移除图形移动事件
 * @constructor
 */
function Unregister_Translate_Event() {
  map.removeInteraction(translateEvent);
}

/**
 * 指定图层绘制图形
 * @param _layer
 * @param zinfo
 * @param isDraw
 * @constructor
 */
export function Draw_Zone(_layer, _info, _isLocation) {
  if (!_layer) return;
  if (_info.zonePoints !== "") {
    let zone = JSON.parse(_info.zonePoints);
    if (zone["type"] === "Circle") {
      let _feature = new Feature({
        geometry: new Circle(zone.center, zone.radius, zone.layout)
      });
      _feature.setStyle(Warnning_Zone_Style(_info));
      _feature.drawType = "Circle";
      _feature.setId(featureId(_info));
      // Unregister_Translate_Event();
      if (_isLocation) {
        //   //定位地图中心
        ChangeMapCenter(zone.center);
        //   //添加移动多边形事件
        //   if(callback){
        //     Register_Translate_Event(_layer,_feature,callback);
      }
      // }
      _layer.getSource().addFeature(_feature);
    } else if (
      zone.type === "Polygon" &&
      zone.coordinates &&
      zone.coordinates.length > 0
    ) {
      let _feature = new Feature({
        geometry: new Polygon(zone.coordinates, zone.layout)
      });
      _feature.setStyle(Warnning_Zone_Style(_info));
      _feature.drawType = "Polygon";
      _feature.setId(featureId(_info));
      // Unregister_Translate_Event();
      if (_isLocation) {
        //定位地图中心
        ChangeMapCenter(_feature.getGeometry().getFirstCoordinate());
        //   //添加移动多边形事件
        //   if(callback){
        //     Register_Translate_Event(_layer,_feature,callback);
      }
      // }
      _layer.getSource().addFeature(_feature);
    }
  }
}

/**
 * 更改地图中心位置
 * @param center
 * @constructor
 */
export function ChangeMapCenter(center) {
  map.getView().animate({
    center: center,
    duration: 1000
  });
}

/**
 * 更改地图中心位置
 * @param center
 * @constructor
 */
export function ChangeMapCenterFromLngLat({lat, lng}) {
  let _center = fromLonLat([lng, lat]);
  ChangeMapCenter(_center);
}

export function Get_Main_Map() {
  return map;
}

export function Get_Target_Layer() {
  return targetLayer;
}

export function Get_Dev_Layer() {
  return devLayer;
}

export function Get_Zone_Layer() {
  return zoneLayer;
}

export function Get_Ranging_Layer() {
  return rangingLayer;
}

export function Get_Marque_Layer() {
  return marqueLayer;
}

export function Remove_Marque_Layer() {
  map.removeLayer(marqueLayer);
  marqueLayer = Init_VectorLayer();
  map.addLayer(marqueLayer);
}

export function Remove_RangingLayer_Layer() {
  map.removeLayer(rangingLayer);
  rangingLayer = Init_VectorLayer();
  map.addLayer(rangingLayer);
}

let SelectTarGetId = "";

export function Get_SelectTarGetId() {
  return SelectTarGetId;
}

export function Set_SelectTarGetId(id) {
  SelectTarGetId = id;
}

let mapMainClick = function(e)
{
  let pixel = map.getEventPixel(e.originalEvent);
  let features = map.getFeaturesAtPixel(pixel);
  console.log('获取到的featrue数量：'+features.length);
  //console.error(DeviceInfoClickShow.deviceInfoId.deviceInfoShow+"99999999999999999996666666666666666666666666666666");
  if (features.length <= 0) return;
  let feature_ = undefined ;

  for(let k = 0;k<features.length;k++)
  {
    if(null != features[k].getProperties() &&
       null != features[k].getProperties().name &&
      (features[k].getProperties().name == featureType_enum.target ||
       features[k].getProperties().name == featureType_enum.dev ||
       features[k].getProperties().name == featureType_enum.targetSector))
    {
      feature_ = features[k];
      break;
    }
  }

  if(null == feature_) return;
  let tg_ids;
  switch (feature_.getProperties().name) {
    case featureType_enum.target:
      tg_ids= Select_TargetDraw(feature_,featureType_enum.target);
      // 做选中操作
      store.dispatch('set_select_target_id', {id:tg_ids[0],did:tg_ids[1]});
      let info = store.state.targets.filter(t => t.id == tg_ids[0]);
      store.dispatch('do_select_target_info', info[0]);
      Fly_To([feature_.getGeometry().extent_[0], feature_.getGeometry().extent_[1]]);
      break;
    case featureType_enum.dev:
      Select_TargetDraw(feature_,featureType_enum.dev);
      store.dispatch('set_select_target_id', {id:"",did:""});
      break;
    case featureType_enum.targetSector:
      tg_ids =  Select_TargetDraw(feature_,featureType_enum.targetSector);
      store.dispatch('set_select_target_id', {id:tg_ids[0],did:tg_ids[1]});
      break;
    default:
      console.log('点击的不是设备和目标');
      return;
  }
}

export function DelMainMapClickEvent() {
  map.removeEventListener('click', mapMainClick);
}

export function AddMainMapClickEvent() {
  map.on('click',mapMainClick);
}

// export function AddMouseEnterEvent() {
//   map.on('mouseenter',enter);
// }
// export function AddMouseLeaveEvent() {
//   map.on('mouseleave',leave);
// }export function AddMouseMoveEvent() {
//   map.on('mousemove',move);
// }
export function toggleListShow(){
  deviceInfoId.deviceInfoShow=!deviceInfoId.deviceInfoShow
}


