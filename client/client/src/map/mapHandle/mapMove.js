import {Get_Main_Map, Get_SelectTarGetId, Get_Dev_Layer, Get_Target_Layer} from "../js";
import {fromLonLat, transform} from "ol/proj"
import {featureType_enum} from "./mapDraw";

//图标移动到某一点
export function Feature_Move(feature, {lat, lng})
{
  const ex = feature.getGeometry().getExtent();
  let _point = fromLonLat([lng,lat]);
  feature.getGeometry().translate(_point[0] - ex[0], _point[1] - ex[1]);

  // 如果移动的id 是选中的id 的话 始终跟随
  // if(feature.getProperties().info.id == Get_SelectTarGetId())
  // {
  //   Fly_To([feature.getGeometry().extent_[0], feature.getGeometry().extent_[1]], function () {});
  // }
  // console.log("点移动成功！");
}

//positionAngle 探测角度 probeAngle +-偏差角度
export function Feature_Sector_Move(feature, positionAngle,probeAngle)
{
  let startAngle = positionAngle-probeAngle;
  let endAngle = positionAngle+probeAngle;
  // 重新设置样式

  feature.setStyle();

}

// 根据ID找目标
export function Fly_ToByTaergetID(id,type) {

  let feature_ = null;

  switch (type) {
    case featureType_enum.target:
      feature_= Get_Target_Layer().getSource().getFeatureById(id);
      break;
    case featureType_enum.dev:
      //feature_= Get_Dev_Layer().getSource().getFeatureById(id);
      break;
    default:
      break;
  }

  if (null != feature_) {
    Fly_To([feature_.getGeometry().extent_[0], feature_.getGeometry().extent_[1]]);
  }
}

export function Fly_To(location) {
  let map = Get_Main_Map();
  let duration = 250;
  // let zoom = map.getZoom;
  // let parts = 1; // 判断下列两个动画效果是否都执行完毕
  // let called = false;
  // let viewAnimate = map.getView();
  //
  // function callback(complete) {
  //   --parts;
  //   if (called) {
  //     return;
  //   }
  //   if (parts === 0 || !complete) {
  //     //动画效果完成 或 动画效果中断 complete是内部传入参数，判断动画执行还是中断
  //     called = true;
  //     done(complete); //动画效果完后执行的函数
  //   }
  // }

  // viewAnimate.animate({
  //   zoom: zoom + 5,
  //   duration: duration / 2
  // }, {
  //   zoom: zoom - 1,
  //   duration: duration / 2
  // }, {
  //   zoom: zoom,
  //   duration: duration / 2
  // }, callback);
  //第一个动画效果 到达目的点
  //第二个动画效果 执行放大缩小
  //两个动画换位，则两个先放大缩小，在转到目的点Strategy.Refresh
  //
  map.getView().animate({
    center: location,
    duration: duration
  });
}
