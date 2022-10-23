import Map from "ol/Map";
import View from "ol/View";
import { fromLonLat } from "ol/proj";
import { OSM_Source } from "./resource";
import Feature from "ol/Feature";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { Draw, Modify, Snap, Translate } from "ol/interaction";
import { Circle, Polygon } from "ol/geom";
import { Warnning_Zone_Style } from "./styles";
import XYZ from 'ol/source/XYZ'

/**
 * 获取地图中心位置
 * @constructor
 */
function Get_Map_Center() {
  //console.log(Vue.prototype.$MAP_CENTER)
  return [116.570607, 39.777473]; //Vue.prototype.$MAP_CENTER;
}

/**
 * 初始化地图
 * @param source
 * @returns {TileLayer}
 * @constructor
 */
function Init_TileLayer(source) {
  let initLayer = new TileLayer(
    {
      source: new XYZ
      ({
       // url:'http://mt2.google.cn/vt/lyrs=y&hl=zh-CN&gl=CN&src=app&x={x}&y={y}&z={z}&s=G'
	   url:'http://wprd0{1-4}.is.autonavi.com/appmaptile?lang=zh_cn&size=1&style=7&x={x}&y={y}&z={z}'
      })
    });
  return initLayer;
  // return new TileLayer({
  //   source
  // });
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
let map, zoneLayer, drawLayer, drawEvent, translateEvent;
/**
 * 初始化地图
 * @param _id
 * @constructor
 */
export function Init_Map(_id) {
  zoneLayer = Init_VectorLayer(0.5);
  drawLayer = Init_VectorLayer();
  //初始化地图
  map = new Map({
    target: _id,
    layers: [Init_TileLayer(OSM_Source()), zoneLayer, drawLayer],
    view: new View({
      center: fromLonLat(Get_Map_Center()),
      zoom: 10,
      minZoom: 4,
      maxZoom: 22,
      projection: "EPSG:3857"
    })
  });
  //初始化Snap (鼠标吸附效果)
  let snap = new Snap({
    source: drawLayer.getSource()
  });
  map.addInteraction(snap);
}

/**
 * 注册修改完成事件
 * @param callback
 * @constructor
 */
export function Register_Modify_Event(callback) {
  let modify = new Modify({ source: drawLayer.getSource() });
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
 * 获取图层
 * @param key
 * @constructor
 */
export function Get_Layer(draw) {
  return draw ? drawLayer : zoneLayer;
}

function featureId(info) {
  const { id, name } = info;
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
    duration: 500
  });
}
