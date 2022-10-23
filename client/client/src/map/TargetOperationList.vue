<template>
  <div class="option-box__" style="display: inline-block">
    <!--isTracking引导,isTranspond转发,isHiting打击,isTicking诱骗,isMonitoring监视-->

    <div :style="{ display: XTentacionTg }">
      <monitor-swtich title="监视目标"
                      :value="null == select_target_info ||
                      null ==  select_target_info.isMonitoring ? false:select_target_info.isMonitoring"
                      :disabled="select_target_id == '' ? false:true"
                      fid="realTimeMonitoringButton"
                      :index="1" class="item-switch" :handle="false"
                      @changSwtichOn="monitorTargetOn" @changSwtichOff="monitorTargetOff"/>

<!--      <monitor-swtich v-if="false"-->
<!--        :title="$t('m.monitorTarget')"-->
<!--        :value="-->
<!--          null == select_target_info || null == select_target_info.isMonitoring-->
<!--            ? false-->
<!--            : select_target_info.isMonitoring-->
<!--        "-->
<!--        :disabled="select_target_id == '' ? false : true"-->
<!--        fid="[[realTimeMonitoringButton"-->
<!--        :index="1"-->
<!--        class="item-switch"-->
<!--        :handle="false"-->
<!--        style="margin-left: 18px"-->
<!--        @changSwtichOn="monitorTargetOn"-->
<!--        @changSwtichOff="monitorTargetOff"-->
<!--      />-->

<!--            <attack-swtich :title="getHitDevName(0)"-->
<!--                           :value="isHitting_1"-->
<!--                           :disabled="select_target_id == '' ? false:true"-->
<!--                           fid="attackTargetButton"-->
<!--                           :index="1" class="item-switch" :handle="false"-->
<!--                           @changSwtichOn="attackTargetOn(1)" @changSwtichOff="attackTargetOff(1)"/>-->

<!--            <attack-swtich :title="getHitDevName(1)"-->
<!--                           :value="isHitting_2"-->
<!--                           :disabled="select_target_id == '' ? false:true"-->
<!--                           fid="attackTargetButton"-->
<!--                           :index="1" class="item-switch" :handle="false"-->
<!--                           @changSwtichOn="attackTargetOn(2)" @changSwtichOff="attackTargetOff(2)"/>-->
      <!-- <attack-swtich
        :title="isHitting_1 ? '关闭打击' : '开启打击'"
        :value="isHitting_1"
        :disabled="select_target_id == '' ? false : true"
        class="item-switch"
        :index="1"
        @changSwtichOn="attackTargetOn(0)"
        @changSwtichOff="attackTargetOff(0)"
      /> -->
      <attack-swtich
        :title="isHitting_1 ? '关闭打击' : '开启打击'"
        :value="isHitting_1"
        :disabled="select_target_id == '' ? false : true"
        class="item-switch"
        :index="1" style="margin-left:20px"
        @changSwtichOn="attackTargetOn(0)"
        @changSwtichOff="attackTargetOff(0)"
      />
      <b-select
        :SeDisabled="false"
        style="
          display: inline-block;
          position: absolute;
          margin-left: -6px;
          margin-top: 12px;
          cursor: default;
        "
      />

      <forward-swtich
        :title="$t('m.forwardTarget')"
        :value="
          null == select_target_info || null == select_target_info.isTranspond
            ? false
            : select_target_info.isTranspond
        "
        :disabled="select_target_id == '' ? false : true"
        fid="forwardButton"
        :index="1"
        class="item-switch"
        :handle="false"
        style="margin-left: 222px"
        @changSwtichOn="forwardTargetOn"
        @changSwtichOff="forwardTargetOff"
      />

      <m-select
        :SeDisabled="
          null == select_target_info || null == select_target_info.isTranspond
            ? false
            : select_target_info.isTranspond
        "
        style="
          display: none;
          position: absolute;
          margin-left: -6px;
          margin-top: 12px;
          cursor: default;
        "
      />
    </div>

    <div :style="{ display: XTentacionDev }">
      <dev-alarm-line-swtich v-if="true"
        :title="$t('m.lsometricLineSwitch')"
        :value="VSwtichDevLine"
        fid="VSwtichDevLine"
        :index="1"
        class="item-switch"
        :handle="false"
        style="margin-left: 18px"
        @changSwtichOn="DevLineChangeOn"
        @changSwtichOff="DevLineChangeOff"
      />

      <dev-alarm-range-swtich v-if="true"
        :title="$t('m.coverageAreaSwitch')"
        :value="VSwtichDevRange"
        fid="VSwtichDevRange"
        :index="1"
        class="item-switch"
        :handle="false"
        @changSwtichOn="DevRangeChangeOn"
        @changSwtichOff="DevRangeChangeOff"
      />

      <span v-if="false"
        class="v-line__"
        style="
          position: absolute;
          margin-left: 5px;
          margin-top: 12px;
          cursor: default;
        "
      ></span>

      <dev-attack-swtich
        :style="{ display: XTentacionAttackDev }"
        style="margin-left: 34px"
        title="干扰设备开关"
        :value="devChangeHitting"
        :disabled="true"
        fid="attackTargetButton"
        :index="1"
        class="item-switch"
        :handle="false"
        @changSwtichOn="devAttackTargetOn()"
        @changSwtichOff="devAttackTargetOff()"
      />

      <b-select
        :SeDisabled="false"
        :style="{ display: XTentacionAttackDev }"
        style="
          display: inline-block;
          position: absolute;
          margin-left: 6px;
          margin-top: 12px;
          cursor: default;
        "
      />
    </div>

    <!--无操作时 显示该div-->
    <div :style="{ display: XTentacionNone }">
      <img
        src="../assets/sign/noSwtich.png"
        class="GeneralIMG"
        style="margin-left: 8px"
      />
      <img src="../assets/sign/noSwtich.png" class="GeneralIMG" />
      <img src="../assets/sign/noSwtich.png" class="GeneralIMG" />
      <img src="../assets/sign/noSwtich.png" class="GeneralIMG" />
    </div>
  </div>
</template>

<script>
import { show_message, msg_enum } from "./../modes/elementUI";
import TargetInputSearch from "../components/TargetInputSearch";
import { mapActions, mapState } from "vuex";
import MSelect from "../components/Select.vue";
import BSelect from "../components/BandSelect.vue";
import bus from "../modes/tool/bus";
import {
  http_request,
  http_request_await,
  update_person_info,
  request_dev_target_transmit,
  request_target_monitor,
  request_target_hit,
  request_target_monitor_del,
  request_target_hit_del,
  request_dev_target_del_transmit,
} from "../modes/api";
import VSwtich from "../components/Swtich/VSwtich.vue";
import TrackSwtich from "../components/Swtich/TrackSwtich.vue";
import MonitorSwtich from "../components/Swtich/MonitorSwtich.vue";
import AttackSwtich from "../components/Swtich/AttackSwtich.vue";
import DecoySwtich from "../components/Swtich/DecoySwtich.vue";
import ForwardSwtich from "../components/Swtich/ForwardSwtich.vue";
import DevAlarmLineSwtich from "../components/Swtich/DevAlarmLineSwtich.vue";
import DevAlarmRangeSwtich from "../components/Swtich/DevAlarmRangeSwtich.vue";
import DevAttackSwtich from "../components/Swtich/DevAttackSwtich.vue";
import {
  HiddenDevWarnLine,
  HiddenDevWarnRound,
} from "../map/mapHandle/mapDraw";

export default {
  name: "TargetOperationList",
  data() {
    return {
      XTentacionAttackDev: "inline-block",
      devChangeHitting: false,
      isHitting_1: false,
      isHitting_2: false,
      IsHitRelationButton: true,
      IsMonitorRelationButton: true,
      IsTickRelationButton: true,
      IsTrackRelationButton: true,
      IsTranspondRelationButton: true,
      VSwtichDevLine: true,
      VSwtichDevRange: true,
      XTentacionDevCategory: "",
      XTentacionTg: "none", //目标操作列是否隐藏
      XTentacionDev: "none", //设备操作列是否隐藏
      XTentacionNone: "", //未选中时显示
      TransmitForm: {
        deviceId: 0,
        targetId: "",
        ip: "",
        port: 0,
      },
    };
  },
  watch: {
    devices: {
      deep: true,
      handler(value, oldValue) {
        if (this.devices && this.select_was_info) {
          let currentDev = this.devices.find(
            (f) => f.id === this.select_was_info.id
          );
          if (currentDev) {
            this.devChangeHitting = currentDev.status && currentDev.status == 2;
          } else {
            this.devChangeHitting = false;
          }
        }
      },
    },
    select_target_info: {
      deep: true,
      handler(value, oldValue) {
        this.isHitting_1 =
          this.select_target_info && this.select_target_info.isHitting;
      },
    },
  },
  props: ["ButtonDis"],
  methods: {
    ...mapActions(["change_info_user_config"]),
    ...mapActions(["set_dev_alarm_status"]),
    // 防止重复点击按钮
    setHitRelationButton() {
      this.IsHitRelationButton = true;
    },
    getHitDevName(i) {
      if (null == this.hitDevList[i]) return "缺少干扰设备";

      return this.hitDevList[i].name;
    },
    setMonitorRelationButton() {
      this.IsMonitorRelationButton = true;
    },
    setTickRelationButton() {
      this.IsTickRelationButton = true;
    },
    setTrackRelationButton() {
      this.IsTrackRelationButton = true;
    },
    setTranspondRelationButton() {
      this.IsTranspondRelationButton = true;
    },
    monitorTargetOn() {
      if (this.IsMonitorRelationButton) {
        this.IsMonitorRelationButton = false;
        let timer = setTimeout(this.setMonitorRelationButton, 2000);

        http_request(request_target_monitor, this.select_target_id, () => {
          //console.log('=============监视返回');
        });
        show_message(msg_enum.success, "ID: " + this.select_target_id);
      } else {
        show_message(msg_enum.success, this.$t("m.pleaseTryAgain2Seconds"));
      }
    },
    monitorTargetOff() {
      if (this.IsMonitorRelationButton) {
        this.IsMonitorRelationButton = false;
        let timer = setTimeout(this.setMonitorRelationButton, 2000);

        http_request(
          request_target_monitor_del,
          this.select_target_info.monitorRelationShip,
          () => {
            show_message(msg_enum.success, this.$t("m.monitorTargetCancel"));
          }
        );
      } else {
        show_message(msg_enum.success, this.$t("m.pleaseTryAgain2Seconds"));
      }
    },
    trackTargetOn() {
      show_message(msg_enum.success, "ID: " + this.select_target_id);
    },
    trackTargetOff() {
      show_message(msg_enum.success, this.$t("m.guideTargetCancel"));
    },
    getDevDisPlay() {
      let flag = "none";
      if (null == this.select_was_info.id || this.select_was_info.id == "") {
        flag = "none";
      } else if (
        this.hitDevList.find((f) => f.id === this.select_was_info.id)
      ) {
        flag = "inline-block";
      }
      // else if(this.select_was_info.id == this.hitDevList[1].id)
      // {
      //   flag = 'inline-block';
      // }
      return flag;
    },
    getDevAttackBind() {
      let flag = true;
      if (null == this.select_was_info.id || this.select_was_info.id == "") {
        flag = false;
      } else if (
        this.hitDevList.find((f) => f.id === this.select_was_info.id)
      ) {
        flag = this.isHitting_1;
      }
      return flag;
    },
    //选中设备->干扰设备的开关
    devAttackTargetOn() {
      //console.log(this.targets_filter);
      if (!this.targets_filter || this.targets_filter.length == 0) {
        return false;
      }
      let tid = this.targets_filter[0].id;
      this.ReqForm = {
        id: tid,
        devId: this.select_was_info.id,
        hitreq: this.bandId,
      };
      http_request(request_target_hit, this.ReqForm, () => {
        // 开启成功后 设置绑定状态

        if (this.hitDevList.find((f) => f.id === this.select_was_info.id)) {
          // this.isHitting_1 = true;
        }

        this.devChangeHitting = true;
        show_message(msg_enum.success, "开启设备成功");
      });
    },
    devAttackTargetOff() {
      if (!this.targets_filter || this.targets_filter.length == 0) {
        return false;
      }

      let rrid = this.targets_filter[0].hitRelationShip;
      this.ReqForm = {
        rid: rrid,
        devId: this.select_was_info.id,
      };

      http_request(request_target_hit_del, this.ReqForm, () => {
        show_message(msg_enum.success, this.$t("m.hitTargetCancel"));
        if (this.hitDevList.find((f) => f.id === this.select_was_info.id)) {
          // this.isHitting_1 = false;
        }

        this.devChangeHitting = false;
        show_message(msg_enum.success, "关闭设备成功");
      });
    },
    attackTargetOn(num = 0) {
      this.ReqForm = {
        id: this.select_target_id,
        devId: this.hitDevList[num].id,
        hitreq: this.bandId,
      };

      http_request(request_target_hit, this.ReqForm, () => {
        // 开启成功后 设置绑定状态
        this.isHitting_1 = true;
        // if(num == 1)
        // {
        //   this.isHitting_1=true;
        // }else{
        //   this.isHitting_2=true;
        // }
        show_message(msg_enum.success, "ID: " + this.select_target_id);
      });
    },
    attackTargetOff(num = 0) {
      this.ReqForm = {
        rid: this.select_target_info.hitRelationShip,
        devId: this.hitDevList[num].id,
      };

      http_request(request_target_hit_del, this.ReqForm, () => {
        show_message(msg_enum.success, this.$t("m.hitTargetCancel"));
        this.isHitting_1 = false;
        // if(num == 1)
        // {
        //   this.isHitting_1=false;
        // }else{
        //   this.isHitting_2=false;
        // }
      });
      // if(this.IsHitRelationButton)
      // {
      //   this.IsHitRelationButton = false;
      //   let timer = setTimeout(this.setHitRelationButton, 2000);
      // }
      // else
      // {
      //   show_message(msg_enum.success,this.$t('m.pleaseTryAgain2Seconds'));
      // }
    },
    decoyTargetOn() {
      show_message(msg_enum.success, "ID: " + this.select_target_id);
    },
    decoyTargetOff() {
      show_message(msg_enum.success, this.$t("m.decoyTargetCancel"));
    },
    forwardTargetOn() {
      if (this.IsTranspondRelationButton) {
        this.IsTranspondRelationButton = false;
        let timer = setTimeout(this.setTranspondRelationButton, 2000);

        let ip = this.target_turn_ip;
        let port = this.target_turn_port;

        console.log(ip, port);
        if (ip != "" && port != "") {
          this.TransmitForm.deviceId = Number(this.select_dev_id);
          this.TransmitForm.targetId = this.select_target_id;
          this.TransmitForm.ip = ip;
          this.TransmitForm.port = Number(port);

          http_request(request_dev_target_transmit, this.TransmitForm, () => {
            show_message(msg_enum.success, this.$t("m.tgForwardingSuceessful"));
            this.$emit("success");
          });
        } else {
          this.noForwarAddressError();
        }
      } else {
        show_message(msg_enum.success, this.$t("m.pleaseTryAgain2Seconds"));
      }
    },
    forwardTargetOff() {
      if (this.IsTranspondRelationButton) {
        this.IsTranspondRelationButton = false;
        let timer = setTimeout(this.setTranspondRelationButton, 2000);

        http_request(
          request_dev_target_del_transmit,
          this.select_target_info.transpondRelationShip,
          () => {
            show_message(
              msg_enum.success,
              this.$t("m.tgForwardingDelSuceessful")
            );
            this.$emit("success");
          }
        );
      } else {
        show_message(msg_enum.success, this.$t("m.pleaseTryAgain2Seconds"));
      }
    },
    realTimeMonitoring() {
      this.$emit('ClickVideoWnd');

      if ('none' == document.getElementById('VideoViewerDisplay').style.display)
      {
        bus.$emit('CloseVideo');
      } else {
        bus.$emit('OpenVideo');
      }
    },
    noForwarAddressError() {
      show_message(msg_enum.success, this.$t("m.selectForwardingAddress"));
    },
    DevLineChangeOn() {
      this.VSwtichDevLine = !this.VSwtichDevLine;

      HiddenDevWarnLine(this.select_was_info.id, true); // 显示告警等距线

      // 从列表移除该ID 只显示隐藏的
      let LineList = JSON.parse(this.user_config_info.deviceLine);

      // 添加设备id到list
      LineList = LineList.filter(
        (t) => t != this.select_was_info.id.toString()
      );
      LineList = JSON.stringify(LineList);

      this.change_info_user_config({ type: "deviceLine", data: LineList });
      this.set_dev_alarm_status({
        did: this.select_was_info.id,
        type: "LineList",
        data: true,
      });

      http_request_await(update_person_info, this.user_config_info);
      console.log("LineList", LineList);

      show_message(msg_enum.success, this.$t("m.open"));
    },
    DevLineChangeOff() {
      this.VSwtichDevLine = !this.VSwtichDevLine;

      HiddenDevWarnLine(this.select_was_info.id, false); // 隐藏告警等距线

      // 获取list 转换成json
      let LineList = [];
      if ("" != this.user_config_info.deviceLine) {
        LineList = JSON.parse(this.user_config_info.deviceLine);
      }
      LineList.push(this.select_was_info.id);
      LineList = JSON.stringify(LineList);

      this.change_info_user_config({ type: "deviceLine", data: LineList });
      this.set_dev_alarm_status({
        did: this.select_was_info.id,
        type: "LineList",
        data: false,
      });
      http_request_await(update_person_info, this.user_config_info);
      console.log("LineList", LineList);

      show_message(msg_enum.success, this.$t("m.close"));
    },
    DevRangeChangeOn() {
      this.VSwtichDevRange = !this.VSwtichDevRange;
      HiddenDevWarnRound(this.select_was_info.id, true); // 显示告警区域

      //移除个性化配置 只显示隐藏的
      let CoverList = JSON.parse(this.user_config_info.deviceCover);

      // 从列表移除该ID
      CoverList = CoverList.filter(
        (t) => t != this.select_was_info.id.toString()
      );
      CoverList = JSON.stringify(CoverList);

      this.change_info_user_config({ type: "deviceCover", data: CoverList });

      this.set_dev_alarm_status({
        did: this.select_was_info.id,
        type: "CoverList",
        data: true,
      });

      http_request_await(update_person_info, this.user_config_info);
      console.log("CoverList", CoverList);

      show_message(msg_enum.success, this.$t("m.open"));
    },
    DevRangeChangeOff() {
      this.VSwtichDevRange = !this.VSwtichDevRange;

      HiddenDevWarnRound(this.select_was_info.id, false);

      // 获取list 转换成json
      let CoverList = [];
      if ("" != this.user_config_info.deviceCover) {
        CoverList = JSON.parse(this.user_config_info.deviceCover);
      }
      CoverList.push(this.select_was_info.id);
      CoverList = JSON.stringify(CoverList);

      this.change_info_user_config({ type: "deviceCover", data: CoverList });
      this.set_dev_alarm_status({
        did: this.select_was_info.id,
        type: "CoverList",
        data: false,
      });

      http_request_await(update_person_info, this.user_config_info);
      console.log("CoverList", CoverList);

      show_message(msg_enum.success, this.$t("m.close"));
    },
    findDevLineAndRangeVisData() {
      this.devices.forEach((info) => {
        if (info.id == this.select_was_info.id) {
          this.VSwtichDevLine = info.line;
          this.VSwtichDevRange = info.range;
        }
      });
    },
  },
  components: {
    TargetInputSearch: TargetInputSearch,
    VSwtich: VSwtich,
    TrackSwtich: TrackSwtich,
    MSelect: MSelect,
    BSelect: BSelect,
    MonitorSwtich: MonitorSwtich,
    AttackSwtich: AttackSwtich,
    ForwardSwtich: ForwardSwtich,
    DecoySwtich: DecoySwtich,
    DevAlarmRangeSwtich: DevAlarmRangeSwtich,
    DevAlarmLineSwtich: DevAlarmLineSwtich,
    DevAttackSwtich: DevAttackSwtich,
  },
  computed: {
    ...mapState(["devices"]),
    ...mapState(["target_turn_ip", "targets_filter"]),
    ...mapState(["target_turn_port"]),
    ...mapState(["select_target_id"]),
    ...mapState(["select_dev_id"]),
    ...mapState(["device_target_info"]),
    ...mapState(["target_status_list"]),
    ...mapState(["select_target_info"]),
    ...mapState(["user_config_info"]),
    ...mapState(["select_was_info"]),
    ...mapState(["bandId"]),
    ...mapState(["hitDevList"]),
  },
  mounted() {
    bus.$on("OnChangeSelectType", (data) => {
      this.XTentacionTg = data.XTentacionTg; //目标操作列是否隐藏
      this.XTentacionDev = data.XTentacionDev; //设备操作列是否隐藏
      this.XTentacionNone = data.XTentacionNone; //未选中时显示
      this.XTentacionDevCategory = data.XTentacionDevCategory; //设备类型
      this.findDevLineAndRangeVisData();
      this.devChangeHitting = this.getDevAttackBind();
      this.XTentacionAttackDev = this.getDevDisPlay();
    });
  },
};
</script>

<style scoped>
.t_selects {
  width: 300px;
  height: 35px;
  z-index: 1000;
}

.option-box__ {
  width: 1240px;
  line-height: 45px;
  margin-left: 18px;
  /*height: 45px;*/
  /*!*background-image: url("../assets/main/TargetOperationImg.png");*!*/
  /*overflow-x: hidden;*/
  /*position: relative;*/
}

.operationButton {
  margin: 0 12px;
  box-shadow: 0px 0px 10px #43dcff inset;
  border: none;
  font-weight: 800;
  background-color: #0f152f;
  color: #43dcff;
  width: 100px;
  height: 25px;
  cursor: pointer;
}

input::-webkit-input-placeholder {
  color: #43dcff;
}

.item-name {
  font-family: "Microsoft YaHei";
  display: inline-block;
  cursor: pointer;
  color: #40cef9;
  line-height: 35px;
  font-size: 16px;
  font-weight: 800;
  vertical-align: -5%;
  margin-left: 15px;
}

.item-switch {
  vertical-align: -5%;
  margin-right: 12px;
  display: inline-block;
}

.v-line__ {
  display: inline-block;
  width: 1px;
  height: 24px;
  background-color: rgba(64, 204, 249, 0.5);
  vertical-align: -0%;
}

.GeneralIMG {
  vertical-align: -23%;
  margin-left: 22px;
}
</style>
