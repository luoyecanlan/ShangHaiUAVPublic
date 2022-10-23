<template>
  <div class="MapToolsViewer" style="child-align: middle">
    <button
      title="地图鹰眼"
      class="spanButtonHawkEye"
      @click="VisHawkEyeCheck()"
      style="margin-left: 10px"
      @focus="buttonBlur($event)"
      :class="{ spanButtonHawkEyeSelect: HawkEyeCheck === true }"
    />

    <button
      title="图示"
      class="spanButtonLegend"
      @click="VisLegend()"
      @focus="buttonBlur($event)"
      :class="{ spanButtonLegendSelect: LegendCheck === true }"
    />

    <span class="v-line__" style="margin-left: 78px" />
    <span class="v-line__" style="margin-left: 108px" />

    <button
      title="测距"
      class="spanButtonRod"
      @click="VisRod()"
      @focus="buttonBlur($event)"
      :class="{ spanButtonRodSelect: RodCheck === true }"
    />

    <button
      title="测距"
      class="spanButtonSign"
      @click="VisSign()"
      @focus="buttonBlur($event)"
      :class="{ spanButtonSignSelect: SignCheck === true }"
    />

    <button
      title="在线/离线地图切换"
      class="spanButtonOnlineMap"
      @click="ChangeOnLineMap()"
      @focus="buttonBlur($event)"
      :class="{ spanButtonOfflineMap: map_type == false }"
    />

    <button
      title="临时标记"
      class="spanButtonClearDraw"
      @click="ClearDraw()"
      @focus="buttonBlur($event)"
    />

    <img
      src="../assets/sign/coord.png"
      style="position: absolute; right: 210px; margin-top: -1px"
      alt=""
    />

    <span
      class="FontFamily"
      id="latitudeSpan"
      style="
        cursor: default;
        font-size: 15px;
        float: right;
        margin-left: 0px;
        margin-right: 10px;
      "
      >40.94201</span
    >
    <span
      class="FontFamily"
      style="
        cursor: default;
        font-size: 15px;
        float: right;
        margin-left: 0px;
        margin-right: 10px;
      "
      >,</span
    >
    <span
      class="FontFamily"
      id="longitudeSpan"
      style="
        cursor: default;
        font-size: 15px;
        float: right;
        margin-left: 0px;
        margin-right: 10px;
      "
      >113.20812</span
    >

    <span
      class="v-line__"
      style="margin-left: 16px; margin-right: 45px; float: right"
    />

    <div
      style="
        text-align: center;
        color: #43dcff;
        display: inline-block;
        font-family: 'Microsoft YaHei';
        float: right;
        margin-right: 16px;
        vertical-align: 26%;
      "
    >
      <div id="scale-line" style="height: 10px" />
      <!--下面div为尺子效果-->
      <div
        style="
          height: 5px;
          border-left: 1px solid #40cef9;
          border-right: 1px solid #40cef9;
          border-bottom: 1px solid #40cef9;
        "
      />
    </div>
    <div id="overLengthVal" class="overLine" style="display: none">
      {{ lineLength }}
    </div>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
import { show_message, msg_enum } from "./../modes/elementUI";
import {
  Get_Main_Map,
  DelMainMapClickEvent,
  AddMainMapClickEvent,
  Get_Ranging_Layer,
  Remove_Marque_Layer,
  Remove_RangingLayer_Layer,
} from "./js";
import "ol/ol.css";
import { unByKey } from "ol/Observable";
import { Draw } from "ol/interaction";
import { Circle as CircleStyle, Fill, Stroke, Style } from "ol/style";
import Overlay from "ol/Overlay";
import { getLength } from "ol/sphere";
import { Draw_Temp_Icon_Marker } from "./mapHandle/mapDraw";
import { MapTile_Init } from "./js/init";
import { update_person_info, http_request_await } from "../modes/api";
import "ol/ol.css";

export default {
  name: "MapToolsViewer",
  data() {
    return {
      map__: "",
      HawkEyeCheck: false,
      LegendCheck: false,
      RodCheck: false,
      SignCheck: false,
      measureElement: undefined,
      measureLayer: undefined,
      sketch: undefined,
      rangListener: undefined,
      clickListener: undefined,
      lineLength: "0.00m",
      mapSource: undefined,
    };
  },
  methods: {
    ...mapActions(["set_select_target_id"]),
    ...mapActions(["set_map_type"]),
    VisHawkEyeCheck() {
      this.HawkEyeCheck = !this.HawkEyeCheck;
      if (this.HawkEyeCheck) {
        document.getElementById("HawkeyeViewer").style.display = "";
      } else {
        document.getElementById("HawkeyeViewer").style.display = "none";
      }
    },
    VisRod() { // 测距
      this.RodCheck = !this.RodCheck;
      if (this.RodCheck) {
        //开启测距
        if (this.SignCheck) {
          //如果当前在标记 则关闭标记事件
          // 取消标记时点击事件
          unByKey(this.clickListener);
        }

        // 首先删除默认地图点击事件
        DelMainMapClickEvent();
        // 执行测距方法
        this._addRangInteractions();
        // 修改按钮状态
        this.SignCheck = false;
      } else {
        //关闭测距
        this.removeRodEvent();
        this.RodCheck = false;
      }
    },
    VisSign() {
      this.SignCheck = !this.SignCheck;
      if (this.SignCheck) {
        // 开启标记
        // 不能和测距同时使用 如果开启 则关闭
        if (this.RodCheck) {
          this.removeRodEvent();
          this.RodCheck = false;
        }
        //开启标记
        this.openSignEvent();
      } else {
        // 关闭标记事件
        unByKey(this.clickListener);
        // 重新绑定事件
        AddMainMapClickEvent();
      }
    },
    ClearDraw() {
      show_message(msg_enum.success, this.$t("m.clearSuccess"));
      // 刷新Layer
      Remove_Marque_Layer();
      Remove_RangingLayer_Layer();

      if (this.RodCheck) {
        // 移除再重新添加
        this.removeRodEvent();
        this._addRangInteractions();
      }
    },
    VisLegend() {//图例
      this.LegendCheck = !this.LegendCheck;
      if (this.LegendCheck) {
        document.getElementById("LegendViewer").style.display = "";
      } else {
        document.getElementById("LegendViewer").style.display = "none";
      }
    },
    ...mapActions(["change_info_user_config"]),
    ChangeOnLineMap() {
      this.set_map_type(!this.map_type);

      if (this.map_type) {
        MapTile_Init(1);
        show_message(msg_enum.success, this.$t("m.switchOnlineMapSource"));

        this.change_info_user_config({
          type: "mapId",
          data: this.map_online_id,
        });

        http_request_await(update_person_info, this.user_config_info);
      } else {
        MapTile_Init(0);
        show_message(msg_enum.success, this.$t("m.switchOfflineMapSource"));

        this.change_info_user_config({
          type: "mapId",
          data: this.map_offline_id,
        });

        http_request_await(update_person_info, this.user_config_info);
      }
    },
    createMeasureTooltip() {
      if (this.measureElement) {
        this.measureElement.parentNode.removeChild(this.measureElement);
      }
      this.measureElement = document.createElement("div");
      this.measureElement.className = "label-rang";

      this.measureLayer = new Overlay({
        element: this.measureElement,
        offset: [0, -5],
        positioning: "bottom-center",
      });
      let map__ = Get_Main_Map();

      map__.addOverlay(this.measureLayer);
    },
    _addRangInteractions() {
      let map__ = Get_Main_Map();
      this.mapSource = Get_Ranging_Layer().getSource();
      let source = this.mapSource;

      //初始化绘制功能
      this.rangDraw = new Draw({
        source,
        type: "LineString",
        style: new Style({
          fill: new Fill({
            color: "rgba(255, 0, 0, 1)",
          }),
          stroke: new Stroke({
            color: "rgba(255, 0, 0, 1)",
            lineDash: [5, 5],
            width: 1,
          }),
          image: new CircleStyle({
            radius: 4,
            stroke: new Stroke({
              color: "rgba(255, 0, 255, 1)",
            }),
            fill: new Fill({
              color: "rgba(255, 255, 255, 0.5)",
            }),
          }),
        }),
      });

      map__.addInteraction(this.rangDraw);

      this.createMeasureTooltip();

      let that = this;
      this.rangDraw.on("drawstart", (evt) => {
        that.sketch = evt.feature;
        let tooltipCoord = evt.coordinate;
        that.rangListener = that.sketch.getGeometry().on("change", (evt) => {
          let geom = evt.target;
          let outLine = getLength(geom);
          that.lineLength =
            outLine > 1000
              ? (outLine / 1000).toFixed(2) + " km"
              : outLine.toFixed(2) + " m";
          let labelCoord = geom.getLastCoordinate();
          that.measureElement.innerHTML = that.lineLength;
          that.measureLayer.setPosition(labelCoord);
        });
      });

      this.rangDraw.on("drawend", () => {});
    },
    removeRodEvent() {
      AddMainMapClickEvent();
      // 移除测距动作事件
      let map__ = Get_Main_Map();
      this.measureElement.className = "label-rang";
      this.measureLayer.setOffset([0, -5]);
      this.sketch = null;
      unByKey(this.rangListener);
      map__.removeInteraction(this.rangDraw);
    },
    openSignEvent() {
      // 开启标记
      // 取消原有的监听事件
      DelMainMapClickEvent();
      // 添加绑定新的事件
      let map__ = Get_Main_Map();
      this.clickListener = map__.on("click", function (e) {
        let longitude = e.coordinate[0].toFixed(5);
        let latitude = e.coordinate[1].toFixed(5);
        Draw_Temp_Icon_Marker("临时标记", { lat: latitude, lng: longitude });
      });
    },
    buttonBlur(e) {
      e.currentTarget.blur();
    },
  },
  mounted() {},
  computed: {
    ...mapState(["map_type"]),
    ...mapState(["login_info"]),
    ...mapState(["user_config_info"]),
    ...mapState(["map_online_address"]),
    ...mapState(["user_config_info"]),
    ...mapState(["map_online_id"]),
    ...mapState(["map_offline_id"]),
  },
};
</script>

<style scoped>
.clearDraw {
  background: url("../assets/button/qingchu.png");
}

.spanButtonLegend {
  position: absolute;
  left: 35px;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/legend.png");
  cursor: pointer;
}

.spanButtonLegendSelect {
  position: absolute;
  left: 35px;
  width: 22px;
  height: 14px;
  background: url("../assets/sign/legendSelect.png");
  cursor: pointer;
}

.spanButtonHawkEye {
  position: absolute;
  width: 22px;
  height: 14px;
  background: url("../assets/sign/hawkEye.png");
  cursor: pointer;
}

.spanButtonHawkEyeSelect {
  position: absolute;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/hawkEyeSelect.png");
  cursor: pointer;
}

.spanButtonRod {
  position: absolute;
  left: 88px;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/rod.png");
  cursor: pointer;
}

.spanButtonRodSelect {
  position: absolute;
  left: 88px;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/rodSelect.png");
  cursor: pointer;
}

.spanButtonSign {
  left: 118px;
  position: absolute;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/sign.png");
  cursor: pointer;
}

.spanButtonSignSelect {
  left: 118px;
  position: absolute;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/signSelect.png");
  cursor: pointer;
}

.spanButtonOnlineMap {
  position: absolute;
  left: 200px;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/onlineMap.png");
  cursor: pointer;
}

.spanButtonOfflineMap {
  left: 200px;
  position: absolute;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/offlineMap.png");
  cursor: pointer;
}

.spanButtonClearDraw {
  position: absolute;
  left: 148px;
  width: 22px;
  height: 14px;
  margin-left: 6px;
  background: url("../assets/sign/clearDraw.png");
  cursor: pointer;
}

.FontFamily {
  color: #43dcff;
  display: inline-block;
  vertical-align: 26%;
  font-family: "Microsoft YaHei";
  font-size: 11px;
}

.v-line__ {
  cursor: default;
  display: inline-block;
  width: 1px;
  height: 14px;
  background-color: #43dcff;
}

.MapToolsViewer {
  background-color: #0f152f;
  /*!*box-shadow: 0px 0px 35px #43DCFF inset;*!*/
  /*text-align: left;*/
  border: 1px solid #43dcff;
  color: #0f152f;
  width: 100%;
  height: 26px;
}
.overLine {
  position: absolute;
  line-height: 32px;
  height: 32px;
  width: 200px;
  background-color: #42b983;
  color: #000000;
  bottom: 0px;
  z-index: 9999;
  left: 0;
}
</style>
