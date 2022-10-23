<template>
  <div v-if="info && changeHeight(info.deviceErrorMsg)"  >
    <div
      class="__group-box__"
      style="
        box-shadow: 0px 0px 6px 0px rgba(64, 204, 249, 0.4) inset;
        padding-top: 10px;
      "
    >
      <div
        class="device-icon-box__"
        :style="{
          border: targetThreatBorderColor(
            null == info || null == info.status ? 0 : info.status
          ),
        }"
        style="margin-top: 5px; margin-left: 0px"
      >
        <img :src="GetDevImg(info.category)" alt="" />
        <!--<img src="../assets/device/device-icon.png" alt="">-->
      </div>
      <div class="device-info-box__" style="margin-top: 5px">
        <div
          class="device-title__"
          :style="{
            color: targetThreatTextColor(null == info.status ? 0 : info.status),
          }"
        >
          <!--  // 0灰色离线 1蓝色待机 2绿色工作中 3红色异常-->
          <img
            :src="deviceStatus(null == info.status ? 0 : info.status)"
            style="vertical-align: -12%"
            alt=""
          />
          {{ info.name }}
        </div>
        <div class="device-info-item__">ID-{{ info.id }}</div>
        <div class="device-info-item__">
          <img src="../assets/device/position.png" alt="" />
          <span>[{{ info.lng.toFixed(5) }},{{ info.lat.toFixed(5) }}]</span>
        </div>
        <div class="device-info-item__">
          <img src="../assets/device/alt.png" alt="" />
          <span>{{ info.alt }}m</span>
        </div>
      </div>
    </div>

    <div
      :style="{
        marginBottom:
          null == info.deviceErrorMsg || '' == info.deviceErrorMsg
            ? '0px'
            : '20px',
        paddingBottom:
          null == info.deviceErrorMsg || '' == info.deviceErrorMsg
            ? '0px'
            : '22px',
        paddingLeft:
          null == info.deviceErrorMsg || '' == info.deviceErrorMsg
            ? '0px'
            : '10px',
      }"
      style="
        flex-direction: column;
        display: flex;
        -webkit-box-shadow: 3px 3px 3px rgba(64, 204, 249, 0.2);
      "
    >
      <div style="font-size: 17px; letter-spacing: 1px" id="ErrorInfoDiv">
        <span
          style="
            color: #e50e03;
            font-weight: bold;
            overflow-wrap: break-word;
            font-size: 15px;
            font-family: 'Microsoft YaHei';
          "
        >
          {{
            null == info.deviceErrorMsg || info.deviceErrorMsg == ""
              ? ""
              : $t("m.devErrorInfo") + ":" + info.deviceErrorMsg
          }}
        </span>
      </div>
    </div>

    <div class="__group-box__" style="flex-direction: column">
      <div class="device-info-item__">
        <span class="bold-font__">{{ $t("m.device") }}：</span>
        <span class="itemSpanFont">{{ info.ip }}:{{ info.port }}</span>
      </div>
      <div class="device-info-item__">
        <span class="bold-font__">{{ $t("m.sevice") }}：</span>
        <!--          info.lip == "" ? "空值":info.lip-->
        <span class="itemSpanFont">{{ info.lip }}:{{ info.lport }}</span>
      </div>
    </div>

    <div class="__group-box__">
      <div class="group-item-box__">
        <div class="device-info-item__">
          <span class="bold-font__">{{ $t("m.timeout") }}：</span>
          <span>{{ info.targetTimeOut }}"</span>
        </div>
        <div class="device-info-item__">
          <span class="bold-font__">{{ $t("m.statusReport") }}：</span>
          <span class="itemSpanFont">{{ info.statusReportingInterval }}"</span>
        </div>
      </div>
      <div class="group-item-box__">
        <div class="device-info-item__">
          <span class="bold-font__">{{ $t("m.targetReport") }}：</span>
          <span class="itemSpanFont">{{ info.probeReportingInterval }}"</span>
        </div>
        <div class="device-info-item__">
          <span class="bold-font__">{{ $t("m.threatDetermin") }}：</span>
          <span class="itemSpanFont">3</span>
        </div>
      </div>
    </div>

    <div class="__group-box__">
      <div
        class="group-item-box__"
        style="height: 80px; display: flex; flex-direction: row"
      >
        <div class="bold-font__" style="line-height: 80px">
          {{ $t("m.rectify") }}
        </div>
        <div class="group-middle__"></div>
        <div>
          <div class="device-info-item__" style="line-height: 40px">
            <img
              src="../assets/device/el.png"
              style="vertical-align: -10%"
              alt=""
            />
            <span class="itemSpanFont">{{ info.rectifyAz }}°</span>
          </div>
          <div class="device-info-item__" style="line-height: 40px">
            <img
              src="../assets/device/az.png"
              style="vertical-align: -12%"
              alt=""
            />
            <span class="itemSpanFont">{{ info.rectifyEl }}°</span>
          </div>
        </div>
      </div>
      <div class="group-item-box__ cover-bg__" style="position: relative">
        <div style="position: absolute; top: 10px; right: 0px">
          <div class="device-info-item__ bold-font__">
            {{ info.coverS }}°-{{ info.coverE }}°
          </div>
          <div class="device-info-item__">R:{{ info.coverR }}m</div>
        </div>
      </div>
    </div>

    <div class="__group-box__" style="flex-direction: column">
      <div class="device-info-item__">
        <span class="bold-font__">{{ $t("m.creatTime") }}：</span>
        <span class="itemSpanFont">{{ info.createTime }}</span>
      </div>
      <div class="device-info-item__">
        <span class="bold-font__">{{ $t("m.modifyTime") }}：</span>
        <span class="itemSpanFont">{{ info.updateTime }}</span>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
import {
  DEVSTATE_OFFLINE,
  DEVSTATE_WORK,
  DEVSTATE_ONLINE,
  DEVSTATE_ALARM,
  DEVSTATE_OFFLINE_BorderColor,
  DEVSTATE_ONLINE_BorderColor,
  DEVSTATE_WORK_BorderColor,
  DEVSTATE_ALARM_BorderColor,
} from "../../static/css/GeneralStyle";

export default {
  deviceInfoShow:false,
  name: "DeviceInfo",
  props: {
    info: {
      require: true,
    },
  },
  methods: {
    changeHeight(deviceErrorMsg) {
      //console.log('动态改变高度');

      let addHight = 0;

      //console.log('serviceErrorMsg,deviceBitMsg', deviceErrorMsg)

      if (null == deviceErrorMsg) {
        addHight = 0;
      } else {
        addHight = deviceErrorMsg.length;

        if (addHight == 0) return true;

        // 一行15字 60px高 二行 75px  每行加15 基础60
        let m = Math.floor((addHight + 9) / 15); //Math.ceil(addHight/15);

        addHight = 50 + 15 * m;
        console.log(addHight);
      }

      document.getElementById("TranHoverBoxDeviceInfo").style.height =
        570 + addHight + "px";
      return true;
    },
    targetThreatBorderColor(threat) {
      //0灰色离线 1蓝色待机 2绿色工作中 3红色异常
      let color = "";
      switch (threat) {
        case 0:
          color = DEVSTATE_OFFLINE_BorderColor;
          break;
        case 1:
          color = DEVSTATE_ONLINE_BorderColor;
          break;
        case 2:
          color = DEVSTATE_WORK_BorderColor;
          break;
        case 3:
          color = DEVSTATE_ALARM_BorderColor;
          break;
        default:
          color = DEVSTATE_OFFLINE_BorderColor;
          break;
      }
      return color;
    },
    GetDevImg(category) {
      let src = "";

      if (category <= 20000) {
        if (category > 10200 && category < 10400) {
          src = require("../assets/device/10200.png");
        } else if (category > 10400) {
          src = require("../assets/device/10400.png");
        } else {
          src = require("../assets/device/10100.png");
        }
      } else if (category > 20000 && category <= 30000) {
        src = require("../assets/device/20100.png");
      } else if (category > 30000 && category <= 30500) {
        src = require("../assets/device/30100.png");
      } else if (category > 30500) {
        src = require("../assets/device/30500.png");
      }

      return src;
    },
    targetThreatTextColor(threat) {
      //0灰色离线 1蓝色待机 2绿色工作中 3红色异常
      let color = "";
      switch (threat) {
        case 0:
          color = DEVSTATE_OFFLINE;
          break;
        case 1:
          color = DEVSTATE_ONLINE;
          break;
        case 2:
          color = DEVSTATE_WORK;
          break;
        case 3:
          color = DEVSTATE_ALARM;
          break;
        default:
          color = DEVSTATE_OFFLINE;
      }
      return color;
    },
    deviceStatus(state) {
      let src = "";
      // //0:离线;1:待机;2:工作;3：异常
      switch (state) {
        case 0:
          src = require("../assets/sign/offline.png");
          break;
        case 1:
          src = require("../assets/sign/online.png");
          break;
        case 2:
          src = require("../assets/sign/working.png");
          break;
        case 3:
          src = require("../assets/sign/devError.png");
          break;
        default:
          src = require("../assets/sign/offline.png");
          break;
      }
      return src;
    },
  },
  computed: {
    ...mapState(["device_categories"]),
    category_name() {
      if (this.device_categories && this.info) {
        let __c = this.device_categories.find(
          (f) => f.id === this.info.category
        );
        if (__c) return __c.name;
      }
      return "";
    },
  },
};
</script>

<style scoped>
.__group-box__ {
  box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2);
  -webkit-box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2);
  padding: 10px 20px 6px;
  margin-bottom: 20px;
  display: flex;
  flex-direction: row;
  padding-bottom: 14px !important;
}

.device-icon-box__ {
  width: 100px;
  height: 100px;
  margin: 0 14px;
  border: 1px solid #40cef9;
  border-radius: 50%;
  background-color: #0f152f;
  position: relative;
}

.device-icon-box__ img {
  position: absolute;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  margin: auto;
}

.device-info-box__ {
  flex: 1;
}

.device-title__ {
  font-size: 18px;
  font-weight: bold;
  line-height: 36px;
}

.device-info-box__ .device-info-item__ {
  font-weight: 700;
  font-size: 14px;
  line-height: 22px;
  letter-spacing: 0.5px;
}

.bold-font__ {
  font-family: "Microsoft YaHei";
  font-size: 15px;
  color: rgba(64, 206, 249, 0.8) !important;
  font-weight: bold;
}

.device-info-item__ {
  font-size: 16px;
  line-height: 24px;
  letter-spacing: 1px;
  color: rgba(64, 204, 249, 0.5);
}

.itemSpanFont {
  font-size: 15px;
  font-weight: bold;
}

.group-item-box__ {
  flex: 1;
}

.cover-bg__ {
  min-height: 53px;
  background-image: url("../assets/device/cover.png");
  background-repeat: no-repeat;
  background-position: left center;
}

.group-middle__ {
  height: 40px;
  width: 8px;
  margin: 20px 8px;
  border: 1px solid #40cef9;
  border-right: 1px solid transparent;
}

.info-item:first-child {
  margin-top: 10px;
}

.info-item {
  padding-left: 36px;
}

.info-title__,
.info-content__ {
  line-height: 40px;
  color: #40cef9;
  font-size: 16px;
}

.info-title__ {
  width: 100px;
  text-align: right;
}

</style>
