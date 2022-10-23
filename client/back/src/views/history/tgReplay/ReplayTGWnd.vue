<template>
  <!--:model="UpDateUserInfoForm"-->
  <!--:rules="UpDateUserInfoRules"-->
  <div
    class="dialog-box__"
    ref="UpDateUserInfoForm"
    style="width: 100%; height: 80%"
    label-width="120px"
  >
    <div class="dialog-lt"></div>
    <div class="dialog-lb"></div>
    <div class="dialog-rt"></div>
    <div class="dialog-rb"></div>
    <div class="__dialog-box-header">
      <span class="bg_01"></span>
      <span class="bg_02"></span>
      <span class="bg_03"></span>
      <span class="bg_04"></span>
      <span class="bg_05"></span>
      <span class="title">目标航迹</span>
    </div>
    <div class="map-wrapper">
      <div class="map-box">
        <!-- 地图 -->
        <div id="TheMap"></div>

        <!-- 左上角目标信息 -->
        <div class="Legend">
          <div style="margin-left: 15px; margin-top: 10px">
            <span>目标ID：{{ tgId }}</span>
          </div>
          <div style="margin-left: 15px; margin-top: 10px">
            <span>开始时间：{{ tgSatrtTime }}</span>
          </div>
          <div style="margin-left: 15px; margin-top: 10px">
            <span>结束时间：{{ tgEndTime }}</span>
          </div>
          <!--<div style="margin-left: 15px;margin-top: 10px">-->
          <!--<span >平均飞行速度：{{flySpeed}}</span>-->
          <!--</div>-->
          <!--<div style="margin-left: 15px;margin-top: 10px">-->
          <!--<span >总飞行距离：{{flyDis}}</span>-->
          <!--</div>-->
          <div style="margin-left: 15px; margin-top: 10px">
            <span>总航迹点：{{ flyPoints }}</span>
          </div>
        </div>

         <div class="scale-box">
            <div id="scale-line" />
            <!--下面div为尺子效果-->
            <div
              style="height: 8px; border: 1px solid #40cef9; border-top: none"
            />
          </div>
      </div>

      <!-- 右侧表格 -->
      <div style="background: #0f152f; flex: 28; height: 100%">
        <div class="list-container">
          <div class="list-header">
            <span class="list-header-item">序号</span>
            <span class="list-header-item">时间</span>
            <span class="list-header-item">海拔</span>
            <span class="list-header-item">位置</span>
          </div>
          <div class="list-scroll-box">
            <div class="scroll-box">
              <vue-scroll :ops="options" style="height: 700px">
                <div
                  class="list-item"
                  @click.stop="submitHandle(item.lat, item.lng)"
                  v-for="(item, key) in InfoList"
                  :key="key"
                >
                  <span class="list-item-cell ellipsis">{{ item.number }}</span>
                  <span class="list-item-cell ellipsis">{{ item.time }}</span>
                  <span class="list-item-cell ellipsis">{{
                    item.alt | formateNumber
                  }} 米</span>
                  <span class="list-item-cell ellipsis"
                    >({{ item.lng | formateNumber }},{{
                      item.lat | formateNumber
                    }})</span
                  >
                </div>
              </vue-scroll>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import "@/assets/dialog/dialog.css";
import {
  http_request,
  request_his_targets_by_tgid,
  update_current,
} from "../../../modes/api";
import "ol/ol.css";
import TileLayer from "ol/layer/Tile";
import XYZ from "ol/source/XYZ";
import ScaleLine from "ol/control/ScaleLine";
import OlFeature from "ol/Feature";
import LineString from "ol/geom/LineString";
import Point from "ol/geom/Point";
import OSM from "ol/source/OSM";
import Map from "ol/Map";
import View from "ol/View";
import { fromLonLat, transform } from "ol/proj";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { defaults as defaultControls, OverviewMap } from "ol/control.js";
import { Style, Stroke, Icon } from "ol/style";
import { mapState, mapActions } from "vuex";
import bus from "../../../modes/tool/bus";

export default {
  name: "Index",
  data() {
    return {
      index: 1,
      size: 10,
      total: 50,
      map: {},
      tgSatrtTime: "",
      tgEndTime: "",
      flyDis: "",
      flySpeed: "",
      flyPoints: 0,
      tileLayer: undefined, //this.Init_XYZ_TileLayer()
      targetLayer: undefined, //this.Init_VectorLayer(),
      mapCenter: [116.403694, 39.915599],
      InfoList: [],
      LineList: [],
      PointList: [],
      options: {
        keepShow: false,
        bar: {
          keepShow: true,
          opacity: 0.5,
          onlyShowBarOnScroll: false, //是否只有滚动的时候才显示滚动条
          background: "#43DCFF",
        },
        rail: {
          opacity: 0,
          size: "4px",
        },
      },
    };
  },
  methods: {
    InitMap() {
      this.tileLayer = this.Init_XYZ_TileLayer();
      this.targetLayer = this.Init_VectorLayer();

      let scaleLineControl = new ScaleLine({
        Units: "metric", //单位有5种：degrees imperial us nautical metric
        className: "", //必须加上该参数
        target: document.getElementById("scale-line"), //显示比例尺的目标容器
      });

      //初始化地图
      this.map = new Map({
        controls: defaultControls().extend([scaleLineControl]),
        target: "TheMap",
        layers: [this.tileLayer, this.targetLayer],
        view: new View({
          center: fromLonLat(this.mapCenter),
          zoom: 16,
          minZoom: 10,
          maxZoom: 23,
          projection: "EPSG:3857",
        }),
      });
    },
    ReMoveFeature() {
      let _feature = this.targetLayer.getSource().getFeatureById("AddressLine");
      if (null === _feature) {
      } else {
        this.targetLayer.getSource().removeFeature(_feature);
      }
    },
    InitFlyAddressData(tgId) {
      let center = [];
      http_request(request_his_targets_by_tgid, tgId, (_data) => {
        if (null == _data || _data.length == 0) {
          return;
        }

        this.InfoList = [];
        this.LineList = [];

        center = [_data[0].lng, _data[0].lat];

        for (let i = 0; i < _data.length; i++) {
          let item = _data[i];

          if (i == 0) {
            this.tgSatrtTime = item.trackTime;
            this.flyPoints = _data.length;
          }
          if (i == _data.length - 1) {
            this.tgEndTime = item.trackTime;
          }

          this.InfoList.push({
            number: i + 1,
            alt: item.alt,
            time: item.trackTime.slice(-8),
            lat: item.lat,
            lng: item.lng,
          });
          this.LineList.push(fromLonLat([item.lng, item.lat]));
        }
        // _data.forEach(item =>
        // {
        //   //this.PointList.push({time:item.trackTime,lat:item.lat,lng:item.lng});
        // })
        this.mapCenterMove(fromLonLat(center));
        this.DrawAddressLine(this.LineList);
      });
    },
    DrawAddressLine(LineList) {
      console.log("DrawAddressLine", LineList);
      //实例一个线(标记点)的全局变量
      let geometryLineString = new LineString(LineList); //线,Point 点,Polygon 面

      let LineStringFeature = new OlFeature({
        geometry: geometryLineString,
        name: "AddressLine",
      });

      let styles = [
        new Style({
          stroke: new Stroke({
            color: "#43DCFF", //'#867AE4',
            width: 2,
          }),
        }),
      ];

      let lineStringsArray = LineStringFeature.getGeometry();

      lineStringsArray.forEachSegment(function (start, end) {
        // 计算箭头偏差
        let startP = transform([start[0], start[1]], "EPSG:3857", "EPSG:4326");
        let endP = transform([end[0], end[1]], "EPSG:3857", "EPSG:4326");

        let lng_a = startP[0];
        let lat_a = startP[1];
        let lng_b = endP[0];
        let lat_b = endP[1];

        let a = ((90 - lat_b) * Math.PI) / 180;
        let b = ((90 - lat_a) * Math.PI) / 180;
        let AOC_BOC = ((lng_b - lng_a) * Math.PI) / 180;
        let cosc =
          Math.cos(a) * Math.cos(b) +
          Math.sin(a) * Math.sin(b) * Math.cos(AOC_BOC);
        let sinc = Math.sqrt(1 - cosc * cosc);
        let sinA = (Math.sin(a) * Math.sin(AOC_BOC)) / sinc;
        let A = (Math.asin(sinA) * 180) / Math.PI;
        let res = 0;

        if (lng_b > lng_a && lat_b > lat_a) res = A;
        else if (lng_b > lng_a && lat_b < lat_a) res = 180 - A;
        else if (lng_b < lng_a && lat_b < lat_a) res = 180 - A;
        else if (lng_b < lng_a && lat_b > lat_a) res = 360 + A;
        else if (lng_b > lng_a && lat_b == lat_a) res = 90;
        else if (lng_b < lng_a && lat_b == lat_a) res = 270;
        else if (lng_b == lng_a && lat_b > lat_a) res = 0;
        else if (lng_b == lng_a && lat_b < lat_a) res = 180;

        res = res.toFixed(0);

        styles.push(
          new Style({
            geometry: new Point(end),
            image: new Icon({
              src: "static/res/icon/arrow.png",
              anchor: [0.5, 0.5],
              rotateWithView: false,
              rotation: (6.3 / 360) * res,
            }),
          })
        );
      });

      LineStringFeature.setStyle(styles);
      LineStringFeature.setProperties({ id: "AddressLine" });
      LineStringFeature.setId("AddressLine");

      this.targetLayer.getSource().addFeature(LineStringFeature);
    },
    mapCenterMove(location) {
      let duration = 250;
      this.map.getView().animate({
        center: location,
        duration: duration,
      });
    },
    submitHandle(lat, lng) {
      this.mapCenterMove(fromLonLat([lng, lat]));
    },
    Init_VectorLayer(_opacity = 1) {
      let source = new VectorSource();
      return new VectorLayer({
        source,
        opacity: _opacity,
      });
    },
    Init_XYZ_TileLayer() {
      return new TileLayer({
        source: new OSM(),
        // source: new XYZ({
        //   url:
        //     "http://mt2.google.cn/vt/lyrs=y&hl=zh-CN&gl=CN&src=app&x={x}&y={y}&z={z}&s=G",
        // }),
      });
    },
  },
  computed: {
    ...mapState(["tgId"]),
  },
  mounted() {
    this.InitMap();

    bus.$on("TGPlay__", () => {
      this.ReMoveFeature();
      this.InitFlyAddressData(this.tgId);
    });
    this.InitFlyAddressData(this.tgId);
  },
};
</script>

<style scoped>
.list-item-cell{
  font-size: 12px;
}
.scale-box{
  position: absolute;
  right: 30%;
  bottom: 20px;
}
#scale-line{
  height: 10px;
  text-align: center;

}
.Legend {
  top: 0;
  font-size: 12px;
  width: auto;
  height: auto;
  padding-right: 15px;
  padding-bottom: 15px;
}
.map-wrapper {
  flex: 1;
  position: relative;
  display: flex;
  flex-direction: row;
}
.map-box {
  flex: 72;
  height: 100%;
}
.list-container {
  margin-top: 0px;
  padding: 10px;
  padding-left: 0px;
  padding-right: 20px;
  margin-bottom: 0px !important;
}
#TheMap {
  position: absolute;
  height: 100%;
  width: 72%;
}
/deep/.el-dialog__headerbtn{
  right: 50px !important;
}
</style>
