<template>
  <div style="width: 1316px">
    <div
      id=""
      class="list-box"
      v-if="devices.length"
      style="padding-left: 45px"
    >
      <!--<span class="list-title" style="margin-left: 22px">设备列表：</span>-->

      <span
        class="list-item"
        v-for="(dev, key) in devices"
        :key="key"
        @click.prevent="doSelect(dev)"
        @mouseenter.prevent="onHover(dev)"
        @mouseleave.prevent="onLeave()"
        :ref="dev.id"
      >
        <!--&lt;!&ndash; 0:离线;1:待机;2:工作;3：异常&ndash;&gt;-->
        <img
          :src="deviceStatus(null == dev.status ? 0 : dev.status, dev.category)"
          class="iconPanel"
          :style="{ verticalAlign: getImgAlign(dev.category) }"
          style="margin-right: 2px"
        />
        {{ deviceName(dev.name) }}
        <img
          src="../../static/img/dev/devLine.png"
          style="vertical-align: -18%; margin-left: 10px"
        />
      </span>
    </div>

    <!-- 设备详情弹框 -->
    <transition name="fade">
      <hover-box
        id="TranHoverBoxDeviceInfo"
        :position-style="infoPosition"
        title=""
        v-show="hover_device"
        style="height: 570px"
      >
        <device-info slot="content" :info="hover_device" />
      </hover-box>
    </transition>

    <!-- 设备显示配置弹框 -->
    <transition name="fade">
      <hover-box
        id="SettingPanelDiv"
        v-show="openSetting"
        class="setting-panel"
      >
        <setting-panel slot="content" style="height: 100%" />
      </hover-box>
    </transition>
  </div>
</template>

<script>
import { ChangeMapCenterFromLngLat } from "./js";
import DeviceInfo from "../components/DeviceInfo";
import HoverBox from "../components/OverBox";
import SettingPanel from "../components/SettingPanel";
import { mapState, mapActions } from "vuex";
import { deviceType } from "../modes/tool";
import { Select_TargetDraw } from "../map/mapHandle/mapDraw";
import { Get_Dev_Layer } from "../map/js/init";
import { featureType_enum } from "./mapHandle/mapDraw";

export default {
  name: "DeviceList",
  data() {
    return {
      deviceType,
      hover_device: undefined,
      infoOffsetLeft: 0,
      openSetting: false,
    };
  },
  computed: {
    ...mapState(["devices"]),
    infoPosition() {
      return {
        left: this.infoOffsetLeft + "px",
        transition: "all 0.2s",
      };
    },
  },
  components: {
    DeviceInfo,
    HoverBox,
    SettingPanel,
  },
  methods: {
    ...mapActions(["set_select_target_id"]),
    onHover(info) {
      this.hover_device = info;
      const { id } = info;
      let _item = this.$refs[id][0];
      this.infoOffsetLeft = _item.offsetLeft + _item.offsetWidth / 2;
    },
    onLeave() {
      this.hover_device = undefined;
    },
    deviceName(name) {
      if (name.length > 4) {
        name = name.substring(0, 4) + "...";
      }
      return name;
    },
    getImgAlign(category) {
      // 0:离线;1:待机;2:工作;3：异常
      let align = "";
      switch (category) {
        case 10100: // 雷达
          align = "-16%";
          break;
        case 10200:
          align = "-16%";
          break;
        case 20100:
          align = "-10%";
          break;
        case 20111:
          align = "-10%";
          break;
        case 30100:
          align = "-16%";
          break;
        case 30500:
          align = "-16%";
          break;
        default:
          align = "-16%";
          break;
      }
      return align;
    },
    deviceStatus(state, category) {
      let src = "";
      let categoryName = "";
      // 0:离线;1:待机;2:工作;3：异常
      if (category < 20000) {
        if (category > 10200) {
          categoryName = "pinpu_state";
          src = require("../../static/img/devCategoryState/" +
            categoryName +
            state +
            ".png");
        } else {
          categoryName = "leida_state";
          src = require("../../static/img/devCategoryState/" +
            categoryName +
            state +
            ".png");
        }
      } else if (category > 20000 && category < 30000) {
        categoryName = "guangdian_state";
        src = require("../../static/img/devCategoryState/" +
          categoryName +
          state +
          ".png");
      } else if (category > 30000 && category < 30500) {
        categoryName = "ganraopao_state";
        src = require("../../static/img/devCategoryState/" +
          categoryName +
          state +
          ".png");
      } else if (category > 30500) {
        categoryName = "youpian_state";
        src = require("../../static/img/devCategoryState/" +
          categoryName +
          state +
          ".png");
      } else {
        categoryName = "leida_state";
        src = require("../../static/img/devCategoryState/" +
          categoryName +
          state +
          ".png");
      }

      return src;
    },
    doSelect(_device) {
      //修改地图中心点
      ChangeMapCenterFromLngLat({ lng: _device.lng, lat: _device.lat });
      //選中設備信息
      let feature_ = Get_Dev_Layer()
        .getSource()
        .getFeatureById(_device.id.toString());
      Select_TargetDraw(feature_, featureType_enum.dev);
      this.set_select_target_id({ id: "", did: "" });
    },
  },
  mounted() {},
};
</script>

<style scoped>
.list-box {
  height: 34px;
  line-height: 34px;
  color: #40cef9;
  width: 1260px;
  overflow: hidden;
}

.list-box:before {
  content: "";
  position: absolute;
  width: 100%;
  height: 100%;
  left: 0px;
  top: 0px;
  border: 1px solid #40cef9;
  transform: skew(-48deg);
  box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2) inset;
}

.device-status {
  margin: 0 40px 0 10px;
  display: inline-block;
  height: 16px;
  width: 16px;
  border-radius: 50%;
}

.status_run {
  vertical-align: -6%;
  background-color: #40cef9;
  box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.3);
}

.list-item {
  font-family: "Microsoft YaHei";
  position: relative;
  cursor: pointer;
  padding-right: 35px;
}

.list-item:hover {
  color: yellow;
}

.list-title,
.list-item {
  margin-left: 0px;
  color: #40cef9;
  font-size: 16px;
}

.list-title {
  font-weight: bold;
}

.btn-setting {
  display: inline-block;
  width: 23px;
  height: 23px;
  position: absolute;
  right: 48px;
  top: 5px;
  cursor: pointer;
}

.setting-panel {
  right: 53px;
  width: 300px !important;
  height: 100%;
  margin-right: -144px;
}

.iconPanel {
  margin-left: -8px;
}
</style>

