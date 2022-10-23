<template>
  <div class="TargetInfoViewer">
    <div
      class="__group-box__"
      style="-webkit-box-shadow: unset; padding-top: 10px; margin-bottom: 1px"
    >
      <div
        class="device-icon-box__"
        :style="{
          border: targetThreatBorderColor(
            null == device_target_info || null == device_target_info.threat
              ? 0
              : device_target_info.threat
          ),
        }"
        style="margin-top: 5px; margin-left: 0px"
      >
        <img src="../assets/device/target_icon.png" alt="" />
      </div>
      <div class="device-info-box__" style="margin-top: 5px">
        <div
          class="device-title__"
          :style="{
            color: targetThreatColor(
              null == device_target_info || null == device_target_info.threat
                ? 0
                : device_target_info.threat
            ),
          }"
        >
          {{
            null == device_target_info || null == device_target_info.uavType
              ? "无人机"
              : device_target_info.uavType
          }}
        </div>
        <div class="device-info-item__">
          SN:{{
            null == device_target_info || device_target_info.uavSn == undefined
              ? ""
              : device_target_info.uavSn
          }}
        </div>
        <div class="device-info-item__">
          <img
            src="../assets/device/position.png"
            alt=""
            style="vertical-align: -10%"
          />
          <span>{{
            null == device_target_info || device_target_info.lng == undefined
              ? ""
              : device_target_info.lng.toFixed(5) +
                "," +
                device_target_info.lat.toFixed(5)
          }}</span>
        </div>
        <div class="device-info-item__">
          <img
            src="../assets/device/position.png"
            alt=""
            style="vertical-align: -10%"
          />
          <span
            :title="
              null == device_target_info ? '' : device_target_info.appAddr
            "
          >
            {{
              null == device_target_info
                ? ""
                : addrSubstring(device_target_info.appAddr)
            }}
          </span>
        </div>
      </div>
    </div>

    <div class="__group-box__">
      <div
        class="group-item-box__"
        style="height: 80px; display: flex; flex-direction: row"
      >
        <div class="bold-font__" style="margin-top: 28px">
          {{ $t("m.probe") }}
          <div
            style="
              font-weight: 700;
              margin-left: -5px;
              color: rgba(64, 204, 249, 0.5);
              margin-top: 2px;
              font-size: 14px;
            "
          >
            [
            {{
              null == device_target_info ||
              device_target_info.deviceId == undefined
                ? ""
                : "ID:" + device_target_info.deviceId
            }}
            ]
          </div>
        </div>
        <!--{{device_target_info.deviceId}}-->
        <div class="group-middle__"></div>
        <div>
          <div class="device-info-item__" style="line-height: 40px">
            <img
              src="../assets/device/el.png"
              style="vertical-align: -10%"
              alt=""
            />
            <span
              class="itemSpanFont"
              style="position: absolute; margin-left: 4px"
            >
              {{
                null == device_target_info ||
                device_target_info.probeAz == undefined
                  ? ""
                  : device_target_info.probeAz.toFixed(2)
              }}°
            </span>
          </div>
          <div class="device-info-item__" style="line-height: 40px">
            <img
              src="../assets/device/az.png"
              style="vertical-align: -12%"
              alt=""
            />
            <span
              class="itemSpanFont"
              style="position: absolute; margin-left: 5px"
            >
              {{
                null == device_target_info ||
                device_target_info.peobeEl == undefined
                  ? ""
                  : device_target_info.peobeEl.toFixed(2)
              }}°
            </span>
          </div>
        </div>
      </div>

      <div
        class="group-item-box__ cover-bg__"
        style="position: relative; margin-left: 40px"
      >
        <div style="position: absolute; top: 10px">
          <div
            class="device-info-item__"
            style="position: absolute; left: 20px; top: 4px"
          >
            <!--<div style="font-weight: 700;color: yellow;font-size: 14px;-->
            <!--position: absolute;left: 20px;">-->
            <!--</div>-->
            {{
              null == device_target_info ||
              device_target_info.probeHigh == undefined
                ? ""
                : device_target_info.probeHigh.toFixed(2)
            }}m
          </div>
          <div
            class="device-info-item__"
            style="position: absolute; left: 60px; top: 36px"
          >
            <!--<div style="font-weight: 700;color: yellow;font-size: 14px;-->
            <!--position: absolute;left: -16px;top: 0px">-->
            <!--</div>-->
            {{
              null == device_target_info ||
              device_target_info.probeDis == undefined
                ? ""
                : device_target_info.probeDis.toFixed(2)
            }}m
          </div>
        </div>
      </div>
    </div>

    <div class="__group-box__">
      <div
        class="group-item-box__"
        style="height: 175px; display: flex; flex-direction: row"
      >
        <img
          src="../assets/device/targetInfo.png"
          style="height: 108px; margin-top: 40px; margin-left: -5px"
          alt=""
        />

        <div
          v-if="device_target_info"
          :style="{
            color: targetThreatColor(
              null == device_target_info.threat ? 0 : device_target_info.threat
            ),
          }"
          class="device-title__"
          style="
            position: absolute;
            line-height: 40px;
            margin-top: 10px;
            margin-left: 18px;
            font-size: 16px;
          "
        >
          <img
            :src="
              deviceStatus(
                null == device_target_info.threat
                  ? 0
                  : device_target_info.threat
              )
            "
            style="vertical-align: -8%"
            alt=""
          />
          {{
            deviceText(
              null == device_target_info.threat ? 0 : device_target_info.threat
            )
          }}
        </div>

        <div
          class="device-title__"
          style="
            position: absolute;
            line-height: 40px;
            font-size: 16px;
            margin-top: 65px;
            margin-left: -5px;
            font-weight: 700;
          "
        >
          <img
            src="../assets/device/alt_.png"
            style="vertical-align: -8%"
            alt=""
          />
          {{
            null == device_target_info ||
            device_target_info.maxHeight == undefined
              ? ""
              : device_target_info.maxHeight.toFixed(2)
          }}m
        </div>

        <div
          class="device-title__"
          style="
            position: absolute;
            line-height: 40px;
            font-size: 16px;
            margin-top: 112px;
            margin-left: 34px;
            font-weight: 700;
          "
        >
          <img
            src="../assets/device/plant.png"
            style="vertical-align: -8%"
            alt=""
          />
          飞行中
          <!--{{null == device_target_info || device_target_info.mode == '0' ? $t('m.real'):$t('m.extrapolation')}}-->
        </div>

        <div
          class="device-title__"
          style="
            position: absolute;
            line-height: 40px;
            font-size: 16px;
            color: #e8b110;
            margin-top: 8px;
            margin-left: 218px;
            font-weight: 700;
          "
        >
          <img
            src="../assets/device/v.png"
            style="vertical-align: -8%"
            alt=""
          />
          {{
            null == device_target_info || device_target_info.vr == undefined
              ? ""
              : device_target_info.vr.toFixed(2)
          }}m/s
        </div>

        <!--<div class="device-title__" style="position: absolute;
        line-height: 40px;font-size: 16px;color: #E8B110;
             margin-top: 32px;margin-left: 218px;font-weight: 700">
          <img src="../assets/device/l.png" style="vertical-align: -8%" alt="">
          {{null == device_target_info|| device_target_info.vt == undefined ?
          '':device_target_info.vt.toFixed(2)}}m/s
        </div>-->

        <div
          class="device-title__"
          style="
            position: absolute;
            line-height: 40px;
            font-size: 16px;
            margin-top: 110px;
            margin-left: 232px;
            font-weight: 700;
          "
        >
          <img
            src="../assets/device/signal.png"
            style="vertical-align: 0%"
            alt=""
          />
          标准频段
          <!--{{ (null == device_target_info || device_target_info.freq == undefined)-->
          <!--|| (device_target_info.freq <= 0) ? $t('m.null'):device_target_info.freq}}-->
        </div>

        <div
          class="device-title__"
          style="
            position: absolute;
            line-height: 40px;
            font-size: 16px;
            margin-top: 188px;
            margin-left: 12px;
            font-weight: 700;
          "
        >
          起飞时间：{{

            null == device_target_info || null == device_target_info.beginAt
              ? ""
              : device_target_info.beginAt
          }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import bus from "../modes/tool/bus";
import {
  TG_LEVEL0,
  TG_LEVEL1,
  TG_LEVEL2,
  TG_LEVEL3,
  TG_LEVEL4,
  TG_LEVEL0_BorderColor,
  TG_LEVEL1_BorderColor,
  TG_LEVEL2_BorderColor,
  TG_LEVEL3_BorderColor,
  TG_LEVEL4_BorderColor,
} from "../../static/css/GeneralStyle";
import { GetAlarmLevel } from "./mapHandle/mapService";

export default {
  name: "TargetInfoViewer",
  data() {
    return {
      timer: "",
    };
  },
  methods: {
    ...mapActions(["select_target"]),
    ...mapActions(["set_select_target_id"]),
    timerDo() {
      //console.log('this.select_target_id',this.select_target_id);
      if (this.select_target_id != "") {
        if (this.login_info != null) {
          this.select_target({ tgid: this.select_target_id });
        } else {
          this.set_select_target_id({ id: "", did: "" });
        }
      }
    },
    targetThreatBorderColor(threat) {
      let color = "";
      threat = GetAlarmLevel(threat);

      switch (threat) {
        case 0:
          color = TG_LEVEL0_BorderColor;
          break;
        case 1:
          color = TG_LEVEL1_BorderColor;
          break;
        case 2:
          color = TG_LEVEL2_BorderColor;
          break;
        case 3:
          color = TG_LEVEL3_BorderColor;
          break;
        case 4:
          color = TG_LEVEL4_BorderColor;
          break;
        default:
          color = TG_LEVEL0_BorderColor;
          break;
      }
      return color;
    },
    targetThreatColor(threat) {
      let color = "";

      threat = GetAlarmLevel(threat);

      switch (threat) {
        case 0:
          color = TG_LEVEL0;
          break;
        case 1:
          color = TG_LEVEL1;
          break;
        case 2:
          color = TG_LEVEL2;
          break;
        case 3:
          color = TG_LEVEL3;
          break;
        case 4:
          color = TG_LEVEL4;
          break;
        default:
          color = TG_LEVEL0;
          break;
      }
      return color;
    },
    addrSubstring(name) {
      if (null == name) {
        return "";
      }

      // if (name.length > 10) {
      //   name = name.substring(0, 10) + "...";
      // }
      return name;
    },
    deviceText(threat) {
      let text = "";
      //let per = threat;
      threat = GetAlarmLevel(threat);

      switch (threat) {
        case 0:
          text = this.$t("m.zeroLevelAlarm_");
          break;
        case 1:
          text = this.$t("m.threeLevelAlarm_");
          break;
        case 2:
          text = this.$t("m.twoLevelAlarm_");
          break;
        case 3:
          text = this.$t("m.oneLevelAlarm_");
          break;
        case 4:
          text = this.$t("m.extLevelAlarm_");
          break;
        default:
          text = this.$t("m.zeroLevelAlarm_");
          break;
      }
      return text; // + per;
    },
    deviceStatus(threat) {
      let src = "";
      threat = GetAlarmLevel(threat);

      switch (threat) {
        case 0:
          src = require("../assets/device/alarm1.png");
          break;
        case 1:
          src = require("../assets/device/alarm3.png");
          break;
        case 2:
          src = require("../assets/device/alarm4.png");
          break;
        case 3:
          src = require("../assets/device/alarm2.png");
          break;
        case 4:
          src = require("../assets/device/alarm5.png");
          break;
        default:
          src = require("../assets/device/alarm1.png");
          break;
      }

      return src;
    },
  },
  computed: {
    ...mapState(["device_target_info"]),
    ...mapState(["select_target_id"]),
    ...mapState(["select_dev_id"]),
    ...mapState(["login_info"]),
  },
  mounted() {
    bus.$on("ClearTimer", () => {
      console.log("清除定时器成功");
      window.clearInterval(this.timer);
    });

    // 定时器开启 查询设备 添加设备
    this.timer = setInterval(this.timerDo, 1500);
  },
};
</script>

<style scoped>
.TargetInfoViewer {
  background-color: rgba(0, 0, 0, 0.4);
  border: 1px solid #43dcff;
  width: 355px;
  height: 520px;
}

.__group-box__ {
  box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2);
  -webkit-box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2);
  padding: 10px 20px 6px;
  margin-bottom: 20px;
  display: flex;
  flex-direction: row;
  padding-bottom: 14px !important;
}

.device-info-box__ .device-info-item__ {
  font-weight: 700;
  font-size: 14px;
  line-height: 22px;
  letter-spacing: 0.5px;
}

.__group-box__ {
  box-shadow: unset;
  padding: 10px 20px 6px;
  -webkit-box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2);
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
  color: #40cef9;
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
  color: #40cef9 !important;
  font-weight: bold;
}

.device-info-item__ {
  font-size: 16px;
  line-height: 24px;
  letter-spacing: 1px;
  color: rgba(64, 204, 249, 1);
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
  background-image: url("../assets/device/hight.png");
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

/* .device-info-item__ {
  font-size: 16px;
  line-height: 24px;
  letter-spacing: 1px;
  color: rgba(64, 204, 249, 0.5);
} */
</style>
