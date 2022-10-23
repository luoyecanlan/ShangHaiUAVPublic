<template>
  <div class="wapper">
    <div class="tb-border-lt"></div>
    <div class="tb-border-rt"></div>

<!--    <div class="tb-border-rb" style="bottom: 46px" />-->
<!--    <div class="tb-border-lb" style="bottom: 46px" />-->
    <div class="bg-left" />
    <div class="bg-right" />

    <div class="target_list_title__" @click="toggleListShow" style="padding-top: 20px">
      <div
        class="border_top"
        style="height: 25px; border-top: 1px solid #40cef9"
      >
        <span
          class="bbox list-title bbox-bold"
          style="width: 130px; color: #43dcff; margin-top: -28px"
          >{{ $t("m.targetList") }}</span
        >
      </div>
    </div>

    <div
      class="border_"
      v-show="listShow"
      style="
        border-left: 1px solid #40cef9;
        border-right: 1px solid #40cef9;
        border-bottom: 1px solid #40cef9;
      "
    >
      <!--background: #0F152F;-->
      <div
        v-bind:style="{ height: heightDataSwtich }"
        id="SwtichDataDiv"
        style="box-shadow: 0px -6px 3px -3px rgba(17, 235, 255, 0.2) inset"
      >
        <div>
          <img
            src="../assets/sign/screen.png"
            style="
              position: absolute;
              cursor: pointer;
              left: 25px;
              margin-top: 5px;
            "
            @click="submitHandle()"
            alt=""
          />

          <img
            src="../assets/sign/up.png"
            :style="{ display: SwtichMenuDis ? '' : 'none' }"
            style="
              position: absolute;
              cursor: pointer;
              right: 25px;
              float: right;
              margin-top: 5px;
            "
            @click="submitHandle()"
            alt=""
          />

          <img
            src="../assets/sign/down.png"
            :style="{ display: SwtichMenuDis ? 'none' : '' }"
            style="
              position: absolute;
              cursor: pointer;
              right: 25px;
              float: right;
              margin-top: 5px;
            "
            @click="submitHandle()"
            alt=""
          />
        </div>

        <div :style="{ display: divDisplay }" style="padding-top: 30px">
          <div
            style="
              font-size: 16px;
              display: flex;
              flex-direction: row;
              line-height: 30px;
              width: 370px;
            "
          >
            <span class="YellowSpan">{{ $t("m.speed") }} :</span>
            <span
              class="YellowSpan"
              style="width: 30px; font-size: 14px; margin-left: 0px"
              >{{ speedSlider[0] }}</span
            >

            <el-slider
              @change="changeSpeedNum"
              v-model="speedSlider"
              style="width: 210px; margin-top: -2px; margin-left: 15px"
              range
              input-size="mini"
              :min="-100"
              :max="200"
            ></el-slider>

            <span class="YellowSpan" style="margin-left: 20px; width: auto">{{
              speedSlider[1]
            }}</span>
            <span class="YellowSpan" style="margin-left: 0px; width: auto"
              >m/s</span
            >
          </div>
        </div>

        <div :style="{ display: divDisplay }">
          <div
            style="
              margin-top: 5px;
              font-size: 16px;
              display: flex;
              flex-direction: row;
              line-height: 30px;
              width: 370px;
            "
          >
            <span class="YellowSpan">{{ $t("m.alt") }} :</span>
            <span
              class="YellowSpan"
              style="width: 30px; font-size: 14px; margin-left: 0px"
              >{{ altSlider[0] }}</span
            >

            <el-slider
              @change="changeAltNum"
              v-model="altSlider"
              style="width: 205px; margin-top: -2px; margin-left: 13px"
              range
              input-size="mini"
              :min="0"
              :max="300000"
            ></el-slider>

            <span class="YellowSpan" style="margin-left: 20px; width: auto">{{
              altSlider[1]
            }}</span>
            <span class="YellowSpan" style="margin-left: 0px; width: auto"
              >m</span
            >
          </div>
        </div>
      </div>

      <div style="padding-top: 10px"></div>
      <div style="padding-top: 10px">
        <vue-scroll
          :ops="ops"
          style="width: 374px"
          id="TargetListDiv"
          :style="{ height: heightDataTargetList }"
        >
          <div v-if="targets.length == 0">
            <div class="NoTGClass" style="height: 40px">
              <img
                src="../assets/target/noTGsilm.png"
                style="margin-top: 10px; margin-left: 25px"
                alt=""
              />
              <div
                style="
                  display: inline-block;
                  vertical-align: 40%;
                  margin-left: 10px;
                "
              >
                <span
                  style="
                    font-size: 15px;
                    color: rgba(64, 204, 249, 0.5);
                    font-weight: 700;
                  "
                  >{{ $t("m.noTargetFound") }}</span
                >
              </div>
            </div>
            <div style="margin-left: 66px; padding-top: 50px">
              <img src="../assets/target/noTG.png" alt="" />
            </div>
          </div>

          <!--          <div v-for="(item,index) in targets_filter" :key="index"-->
          <!--               v-if="targets_filter.length>0">-->
          <div
            v-for="(item, index) in targets"
            :key="index"
            v-if="targets.length > 0"
          >
            <div
              class="TargetListClass"
              style="height: 56px; flex-direction: row; display: flex"
              @click.stop="doChecked(item)"
              :class="{ TargetListClass_Checked: select_target_id === item.id }"
            >
              <div style="margin-bottom: 10px">
                <!--<img :src="getTgStatusImg(null == item.alarmLevel ? 0:item.alarmLevel)"-->
                <!--style="position: absolute;margin-left: 10px;margin-top: 11px" alt="">-->
                <svg width="50" height="50" viewbox="0 0 48 48">
                  <circle
                    cx="26"
                    cy="26"
                    r="21"
                    stroke-width="3"
                    stroke="#215570"
                    fill="none"
                  ></circle>
                  <circle
                    cx="26"
                    cy="26"
                    r="21"
                    stroke-width="3"
                    stroke="#43DAFE"
                    fill="none"
                    :stroke="getTgStatusColor(item.mode, item.alarmLevel)"
                    transform="matrix(0,-1,1,0,0,52)"
                    :stroke-dasharray="getTheart(item.mat)"
                  ></circle>
                  <!--:stroke-dasharray="item.mat == null ? item.mat = getTheart():item.mat"></circle>-->
                </svg>
                <div
                  style="
                    width: 34px;
                    height: 34px;
                    border-radius: 50%;
                    margin-top: -43px;
                    margin-left: 9px;
                  "
                  :style="{
                    backgroundColor: getTgStatusColor(
                      item.mode,
                      item.alarmLevel
                    ),
                  }"
                >
                  <img
                    :src="getTgStatusImg(item.mode)"
                    style="
                      position: absolute;
                      margin-left: -5px;
                      margin-top: -4px;
                    "
                    alt=""
                  />
                </div>
              </div>

              <div
                style="display: inline-block; margin-top: 12px; width: 225px"
              >
                <div style="display: inline-block; color: #40ccf9">
                  <p
                    class="TargerTable"
                    :class="{ TargerTable_Check: select_target_id === item.id }"
                    style="
                      padding-left: 2px;
                      font-family: 'Microsoft YaHei';
                      font-size: 16px;
                      font-weight: 900;
                    "
                  >
                    ID: {{ item.uavSn }}
                  </p>
                  <p
                    class="TargerTable"
                    :class="{ TargerTable_Check: select_target_id === item.id }"
                    style="
                      padding-left: 2px;
                      font-family: 'Microsoft YaHei';
                      font-size: 14px;
                      text-align: center;
                      padding-top: 5px;
                    "
                  ></p>

                  <!-- 目标状态标识 -->
                  <div
                    class="targetIconSign"
                    style="
                      display: inline-block;
                      vertical-align: top;
                      margin-top: 4px;
                      width: 225px;
                      margin-left: 2px;
                    "
                  >
                    <img
                      src="../assets/sign/track_s.png"
                      :style="{
                        display:
                          item.isTracking && select_target_id == item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px; vertical-align: -5%"
                      alt=""
                    />

                    <img
                      src="../assets/sign/track.png"
                      :style="{
                        display:
                          item.isTracking && select_target_id != item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px; vertical-align: -5%"
                      alt=""
                    />

                    <img
                      src="../assets/sign/monitorIcon_s.png"
                      :style="{
                        display:
                          item.isMonitoring && select_target_id == item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px"
                      alt=""
                    />

                    <img
                      src="../assets/sign/monitorIcon.png"
                      :style="{
                        display:
                          item.isMonitoring && select_target_id != item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px"
                      alt=""
                    />

                    <img
                      src="../assets/sign/attackIcon_s.png"
                      :style="{
                        display:
                          item.isHitting && select_target_id == item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px; vertical-align: -5%"
                      alt=""
                    />

                    <img
                      src="../assets/sign/attackIcon.png"
                      :style="{
                        display:
                          item.isHitting && select_target_id != item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px; vertical-align: -5%"
                      alt=""
                    />

                    <img
                      src="../assets/sign/turn_s.png"
                      :style="{
                        display:
                          item.isTranspond && select_target_id == item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px"
                      alt=""
                    />

                    <img
                      src="../assets/sign/turn.png"
                      :style="{
                        display:
                          item.isTranspond && select_target_id != item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px"
                      alt=""
                    />

                    <img
                      src="../assets/sign/decoyIcon_s.png"
                      :style="{
                        display:
                          item.isTicking && select_target_id == item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px; vertical-align: 5%"
                      alt=""
                    />

                    <img
                      src="../assets/sign/decoyIcon.png"
                      :style="{
                        display:
                          item.isTicking && select_target_id != item.id
                            ? ''
                            : 'none',
                      }"
                      style="margin-right: 6px; vertical-align: 5%"
                      alt=""
                    />
                  </div>

                  <!-- 查看飞手位置链接 -->
                  <a
                    v-show="item.appLng"
                    style="margin-left: 50px"
                    :href="`${flyerAddress}?lng=${item.appLng}&lat=${
                      item.appLat
                    }&appAddr=${encodeURIComponent(item.appAddr)}`"
                    target="_blank"
                    >查看飞手位置</a
                  >
                </div>
              </div>

              <span
                class="v-line__"
                :class="{ 'v-line__Yellow': select_target_id === item.id }"
                style="margin-left: 8px; margin-top: 8px"
              />

              <div style="display: inline-block; margin-top: 10px">
                <div>
                  <p
                    class="TargerTable"
                    :class="{ TargerTable_Check: select_target_id === item.id }"
                    style="
                      padding-left: 12px;
                      color: rgba(64, 204, 249, 1);
                      font-weight: 700;
                    "
                  >
                    A : {{ item.alt.toFixed(1) }} m
                  </p>
                </div>
                <div>
                  <p
                    class="TargerTable"
                    :class="{ TargerTable_Check: select_target_id === item.id }"
                    style="
                      padding-left: 12px;
                      margin-top: 4px;
                      color: rgba(64, 204, 249, 1);
                      font-weight: 700;
                    "
                  >
                    V : {{ item.speed.toFixed(1) }} m/s
                  </p>
                </div>
              </div>
            </div>
          </div>
        </vue-scroll>
      </div>

      <!-- box-shadow: 0 0 3px 3px rgba(64,204,249,0.2) inset;;-->
      <div class="footer" style="border-top: 1px solid rgba(64, 204, 249, 0.4)">
        <div style="width: 60px">
          <div
            style="
              margin-top: 8px;
              font-size: 16px;
              display: flex;
              flex-direction: row;
              line-height: 30px;
              margin-left: 10px;
            "
          >
            <!--<span class="TotalNumLable">总</span>-->
            <span
              class="TotalNumLable"
              style="margin-left: 14px; cursor: default"
              >{{
                targets_0level_num +
                targets_1level_num +
                targets_2level_num +
                targets_3level_num +
                targets_out_level_num
              }}</span
            >
          </div>
        </div>

        <span class="v-line__" style="margin-top: 7px"></span>

        <div>
          <span class="target-icon-box target-green">
            {{ targets_0level_num }}
          </span>
          <span class="target-icon-box target-blue">
            {{ targets_1level_num }}
          </span>
          <span class="target-icon-box target-red">
            {{ targets_3level_num }}
          </span>
        </div>

        <!-- <div
          style="
            width: 250px;
            margin-left: 26px;
            margin-top: 2px;
            cursor: default;
          "
        >
          <div
            style="
              font-size: 16px;
              display: flex;
              flex-direction: row;
              line-height: 30px;
              width: 370px;
            "
          >
            <span style="width: 18px; color: #fe0201">⬤</span>
            <span class="FontFamily" style="width: 75px; color: #42dcff">{{
              targets_1level_num
            }}</span>
          </div>
          <div style="width: 250px; margin-top: -8px">
            <div
              style="
                font-size: 16px;
                display: flex;
                flex-direction: row;
                line-height: 30px;
                width: 370px;
              "
            >
              <span style="width: 18px; color: #18BD3A">⬤</span>
              <span class="FontFamily" style="width: 75px; color: #42dcff">{{
                targets_0level_num
              }}</span>
            </div>
          </div>

          <div style="width: 250px; margin-top: -8px">
            <div
              style="
                font-size: 16px;
                display: flex;
                flex-direction: row;
                line-height: 30px;
                width: 370px;
              "
            >
              <span style="width: 18px; color: #42dcff">⬤</span>
              <span class="FontFamily" style="width: 75px; color: #42dcff">{{
                targets_0level_num
              }}</span>
            </div>
          </div>
        </div> -->
      </div>
    </div>
  </div>
</template>

<script>
import { show_message, msg_enum } from "./../modes/elementUI";
import { mapActions, mapState } from "vuex";
import bus from "../modes/tool/bus";
import { Fly_ToByTaergetID } from "../map/mapHandle/mapMove";
import {
  Select_TargetDraw,
  Draw_Icon_Marker,
  Remove_TargetFeature,
} from "../map/mapHandle/mapDraw";
import { Get_Target_Layer } from "../map/js";
import {
  update_person_info,
  http_request_await,
  request_current,
  request_map_info_list,
  request_map_info,
  create_person_info,
  request_person_info_list,
  distory_access,
  get_app_config,
} from "../modes/api";
import { GetTargetAlarmColor } from "../../static/css/GeneralStyle";
import { featureType_enum } from "./mapHandle/mapDraw";

export default {
  data() {
    return {
      listShow:false,
      ops: {
        keepShow: false,
        bar: {
          keepShow: true,
          opacity: 0.5,
          onlyShowBarOnScroll: false, //是否只有滚动的时候才显示滚动条
          background: "#43DCFF",
        },
        rail: {
          opacity: 0,
          size: "2px",
        },
      },
      zeroLevelCheck: new Boolean(true),
      oneLevelCheck: new Boolean(true),
      twoLevelCheck: new Boolean(true),
      threeLevelCheck: new Boolean(true),
      outLevelCheck: new Boolean(true),

      LevelZeroNum: "0",
      LevelOneNum: "0",
      LevelTwoNum: "0",
      LevelThreeNum: "0",
      LevelExtNum: "0",

      SwtichMenuDis: true,
      altSlider: [0, 3000],
      speedSlider: [-100, 200],
      heightDataSwtich: "30px",
      heightDataTargetList: "",
      InitialHeight: 0,
      divDisplay: "",
      flyerAddress: "",
    };
  },
  computed: {
    ...mapState(["zero_level_check"]),
    ...mapState(["one_level_check"]),
    ...mapState(["two_level_check"]),
    ...mapState(["three_level_check"]),
    ...mapState(["out_level_check"]),

    ...mapState(["target_status_list"]),

    ...mapState(["speed_slider"]),
    ...mapState(["alt_slider"]),

    ...mapState(["targets_filter"]),
    ...mapState(["targets"]),
    ...mapState(["select_target_id"]),
    ...mapState(["targets_out_level_num"]),
    ...mapState(["targets_0level_num"]),
    ...mapState(["targets_1level_num"]),
    ...mapState(["targets_2level_num"]),
    ...mapState(["targets_3level_num"]),

    ...mapState(["user_config_info"]),
  },
  methods: {
    ...mapActions(["do_select_target_info"]),
    ...mapActions(["clear_targets_filter"]),
    ...mapActions(["clear_targets"]),

    ...mapActions(["change_info_user_config"]),
    ...mapActions(["set_speed_slider"]),
    ...mapActions(["set_alt_slider"]),

    ...mapActions(["set_zero_level_check"]),
    ...mapActions(["set_one_level_check"]),
    ...mapActions(["set_two_level_check"]),
    ...mapActions(["set_three_level_check"]),
    ...mapActions(["set_out_level_check"]),

    ...mapActions(["set_select_target_id"]),
    ...mapActions(["set_target_0level_num"]),
    ...mapActions(["set_target_1level_num"]),
    ...mapActions(["set_target_2level_num"]),
    ...mapActions(["set_target_3level_num"]),
    ...mapActions(["set_target_out_level_num"]),
    // openFlyerPosition(item){
    //   console.log("%%%%%",this.flyerAddress);
    //   window.open(`${this.flyerAddress}?lng=116.280190&lat=40.049191`,"_blank");
    // },
    toggleListShow(){
      this.listShow=!this.listShow
    },
    getTheart(mat) {
      mat = Math.floor(mat * 131) + ",131";
      return mat;
    },
    getTgStatusColor(mode, alarmLevel, isBackGround = false) {
      let transparency = 1;

      // 外推样式会赋值透明度
      if (mode != 0) {
        transparency = 0.5;
        if (isBackGround) {
          return "";
        }
      }

      let src = GetTargetAlarmColor(alarmLevel, transparency);

      return src;
    },
    getTgStatusImg(mode) {
      let src = "";

      if (mode == 0) {
        src = require("../assets/sign/tgDefault.png");
      } else if (mode == 1) {
        src = require("../assets/sign/tgDefaultGray.png");
      }

      return src;
    },
    selecteChange(value, alarmLevel) {
      let threat = this.user_config_info.filterThreat;

      switch (alarmLevel) {
        case 0:
          this.set_zero_level_check(this.zeroLevelCheck);

          threat = JSON.parse(threat);
          threat.zeroLevelCheck = this.zeroLevelCheck;
          threat = JSON.stringify(threat);

          this.change_info_user_config({ type: "filterThreat", data: threat });

          http_request_await(update_person_info, this.user_config_info);
          break;
        case 1:
          this.set_one_level_check(this.oneLevelCheck);

          threat = JSON.parse(threat);
          threat.oneLevelCheck = this.oneLevelCheck;
          threat = JSON.stringify(threat);
          this.change_info_user_config({ type: "filterThreat", data: threat });

          http_request_await(update_person_info, this.user_config_info);
          break;
        case 2:
          this.set_two_level_check(this.twoLevelCheck);

          threat = JSON.parse(threat);
          threat.twoLevelCheck = this.twoLevelCheck;
          threat = JSON.stringify(threat);
          this.change_info_user_config({ type: "filterThreat", data: threat });

          http_request_await(update_person_info, this.user_config_info);
          break;
        case 3:
          this.set_three_level_check(this.threeLevelCheck);

          threat = JSON.parse(threat);
          threat.threeLevelCheck = this.threeLevelCheck;
          threat = JSON.stringify(threat);
          this.change_info_user_config({ type: "filterThreat", data: threat });

          http_request_await(update_person_info, this.user_config_info);

          break;
        case 4:
          this.set_out_level_check(this.outLevelCheck);

          threat = JSON.parse(threat);
          threat.outLevelCheck = this.outLevelCheck;
          threat = JSON.stringify(threat);
          this.change_info_user_config({ type: "filterThreat", data: threat });

          http_request_await(update_person_info, this.user_config_info);
          break;
      }
      this.checkTargetsInfoList();
    },
    submitHandle() {
      if (this.heightDataSwtich == "110px") {
        this.SwtichMenuDis = true;
        this.divDisplay = "none";
        this.heightDataSwtich = "30px";
        this.heightDataTargetList =
          document.documentElement.clientHeight - 303 + 27 + "px";
      } else {
        this.SwtichMenuDis = false;
        this.divDisplay = "";
        this.heightDataSwtich = "110px";
        this.heightDataTargetList =
          document.documentElement.clientHeight - 443 - 10 + 97 + "px";
      }
    },
    changeSpeedNum() {
      this.set_speed_slider(this.speedSlider);
      this.checkTargetsInfoList();
      this.change_info_user_config({
        type: "filterVMin",
        data: this.speedSlider[0],
      });
      this.change_info_user_config({
        type: "filterVMax",
        data: this.speedSlider[1],
      });
      http_request_await(update_person_info, this.user_config_info);
    },
    changeAltNum() {
      this.set_alt_slider(this.altSlider);
      this.checkTargetsInfoList();
      this.change_info_user_config({
        type: "filterAltMin",
        data: this.altSlider[0],
      });
      this.change_info_user_config({
        type: "filterAltMax",
        data: this.altSlider[1],
      });
      http_request_await(update_person_info, this.user_config_info);
    },
    checkAlarmLevel(alarmLevel) {
      // 判断告警等级是否选中
      // let flag = false;
      // switch (alarmLevel) {
      //   // 判断告警等级限制是否显示
      //   case 0:
      //     if (this.zeroLevelCheck) flag = true;
      //     break;
      //   case 1:
      //     if (this.oneLevelCheck) flag = true;
      //     break;
      //   case 2:
      //     if (this.twoLevelCheck) flag = true;
      //     break;
      //   case 3:
      //     if (this.threeLevelCheck) flag = true;
      //     break;
      //   case 4:
      //     if (this.outLevelCheck) flag = true;
      //     break;
      //   default:
      //     break;
      // }
      //return flag;
    },
    checkTargetsInfoList() {
      // 筛选速度和海拔
      for (let k = this.targets_filter.length - 1; k >= 0; k--) {
        let speed =
          this.targets_filter[k].speed == undefined
            ? 0
            : this.targets_filter[k].speed;
        let alt =
          this.targets_filter[k].alt == undefined
            ? 0
            : this.targets_filter[k].alt;

        if (
          speed <= this.speedSlider[0] ||
          speed >= this.speedSlider[1] ||
          alt <= this.altSlider[0] ||
          alt >= this.altSlider[1]
        ) {
          //| !this.checkAlarmLevel(this.targets_filter[k].alarmLevel)
          // 不符合条件
          Remove_TargetFeature(this.targets_filter[k].id); // 删除图标
          // 选中的ID清除
          if (this.select_target_id == this.targets_filter[k].id) {
            this.set_select_target_id({ id: "", did: "" });
          }
          this.targets_filter.splice(k, 1); // 删除目标列表信息中的数据
        }
      }
      console.log(this.targets);
      // 筛选总列表的数据
      this.targets.forEach((info) => {
        let speed = info.speed == undefined ? 0 : info.speed;
        let alt = info.alt == undefined ? 0 : info.alt;

        if (
          speed >= this.speedSlider[0] &&
          speed <= this.speedSlider[1] &&
          alt >= this.altSlider[0] &&
          alt <= this.altSlider[1]
        ) {
          // 全部符合条件 && this.checkAlarmLevel(info.alarmLevel)
          let index_filter = this.targets_filter.findIndex(
            (item) => item.id == info.id
          );

          if (index_filter == -1) {
            //如果不存在
            this.targets_filter.push(info);
            Draw_Icon_Marker(
              info.id,
              { lat: info.lat, lng: info.lng },
              {
                id: info.id,
                status: info.alarmLevel,
                deviceId: info.deviceId,
                alarmLevel: info.alarmLevel,
                uavSn: info.uavSn,
              },
              featureType_enum.target
            );
          }
        }
      });

      this.set_target_0level_num(0);
      this.set_target_1level_num(0);
      this.set_target_2level_num(0);
      this.set_target_3level_num(0);
      this.set_target_out_level_num(0);

      this.targets_filter.forEach((info) => {
        switch (info.alarmLevel) {
          case 0:
            this.set_target_0level_num(this.targets_0level_num + 1);
            break;
          case 1:
            this.set_target_1level_num(this.targets_1level_num + 1);
            break;
          case 2:
            this.set_target_2level_num(this.targets_2level_num + 1);
            break;
          case 3:
            this.set_target_3level_num(this.targets_3level_num + 1);
            break;
          case 4:
            this.set_target_out_level_num(this.targets_out_level_num + 1);
            break;
          default:
            this.set_target_out_level_num(this.targets_out_level_num + 1);
            break;
        }
      });

      console.log("筛选后为" + this.targets_filter.length);
    },
    doChecked(info) {
      let SelectFeature = Get_Target_Layer()
        .getSource()
        .getFeatureById(info.id.toString());

      if (null == SelectFeature) {
        show_message(msg_enum.error, this.$t("m.tgDisOrHiddeen"));
        return;
      }

      let ids = Select_TargetDraw(SelectFeature, featureType_enum.target);

      this.set_select_target_id({ id: ids[0], did: ids[1] });

      if (ids[0] == "") {
        this.do_select_target_info({});
      } else {
        this.do_select_target_info(info);
      }

      Fly_ToByTaergetID(info.id, featureType_enum.target);
    },
    getTargetStatus(id) {
      let info = this.target_status_list.filter((t) => t.id == id)[0];
      return info;
    },
  },
  mounted() {
    this.InitialHeight = document.documentElement.clientHeight - 276;
    this.heightDataTargetList = this.InitialHeight + "px";
    this.SwtichText = "+";
    this.divDisplay = "none";

    bus.$on("ResetTargetListFilter", (Filter) => {
      //console.log('TargetList收到消息开始加载筛选条件', Filter);

      this.speedSlider = [Filter.minSpeed, Filter.maxSpeed];
      this.altSlider = [Filter.minAlt, Filter.maxAlt];

      this.zeroLevelCheck = Filter.zeroLevelCheck;
      this.oneLevelCheck = Filter.oneLevelCheck;
      this.twoLevelCheck = Filter.twoLevelCheck;
      this.threeLevelCheck = Filter.threeLevelCheck;
      this.outLevelCheck = Filter.outLevelCheck;

      this.set_zero_level_check(Filter.zeroLevelCheck);
      this.set_one_level_check(Filter.oneLevelCheck);
      this.set_two_level_check(Filter.twoLevelCheck);
      this.set_three_level_check(Filter.threeLevelCheck);
      this.set_out_level_check(Filter.outLevelCheck);
    });

    get_app_config().then((data) => {
      console.log(data);
      this.flyerAddress =
        data.flyer_url || "http://192.168.2.111/flyer/index.html";
      //this.openFlyerPosition({});
    });

    // this.speedSlider
    // this.altSlider
    // F11 全屏
    let that = this;
    window.onresize = function temp() {
      console.log("全屏改变高度");
      let heightSwtichDiv = that.heightDataSwtich; //document.getElementById('SwtichDataDiv').style.height;

      if (heightSwtichDiv == "30px") {
        that.heightDataTargetList =
          (document.documentElement.clientHeight - 303 + 27).toString() + "px";
      } else {
        that.heightDataTargetList =
          (document.documentElement.clientHeight - 443 - 10 + 97).toString() +
          "px";
      }
    };
  },
  components: {},
};
</script>

<style scoped>
.target-icon-box {
  position: relative;
  display: inline-block;
  width: 75px;
  line-height: 44px;
  padding-left: 40px;
  font-weight: bold;
}
.target-green{
  color: #23aa55;
}
.target-blue{
  color: #40cef9;
}
.target-red{
  color: #ff0000;
}
.target-red::before,
.target-blue::before,
.target-green::before {
  content: "";
  position: absolute;
  height: 16px;
  width: 16px;
  left: 16px;
  border-radius: 10px;
  top: 48%;
  transform: translateY(-50%);
}
.target-red::before {
  background-color: #ff0000;
}
.target-blue::before {
  background-color: #40cef9;
}
.target-green::before {
  background-color: #23aa55;
}
.targetIconSign {
  position: absolute;
  top: 30px;
  left: 50px;
}
a,
a:link,
a:active {
  color: #1d8af9;
  position: relative;
  padding: 0 24px;
}
a:before,
a:after {
  position: absolute;
  height: 2px;
  width: 24px;
  content: " -- ";
  top: 2px;
}
a:before {
  left: 0px;
}
a:after {
  right: -10px;
}
.InputYellow {
  margin-top: 2px;
  font-size: 15px;
  height: 25px;
  width: 165px;
  box-shadow: 0px 0px 6px rgba(255, 204, 0, 0.5) inset;
  border: none;
  outline: none;
  margin-left: 30px;
  color: red;
  background-color: unset;
}

.NoTGClass {
  padding-top: 2px;
  color: #40cef9;
  margin-bottom: 10px;
  cursor: pointer;
  box-shadow: 0px 0px 4px rgba(77, 220, 254, 0.7) inset;
}

.TargetListClass {
  padding-top: 2px;
  color: #40cef9;
  margin-bottom: 10px;
  cursor: pointer;
  box-shadow: 0px 0px 4px rgba(77, 220, 254, 0.7) inset;
}

.TargetListClass:hover {
  box-shadow: 0px 0px 10px rgba(17, 235, 255, 1) inset;
}

.TargetListClass_Checked {
  color: rgba(255, 221, 0, 1);
  box-shadow: 0px 0px 4px rgba(255, 221, 0, 1) inset;
}

.YellowSpan {
  margin-left: 12px;
  width: 80px;
  font-size: 14px;
  font-family: "Microsoft YaHei";
  color: #ffdd00;
}

.title {
  font-family: Microsoft YaHei;
  font-size: 15px;
  font-weight: 700;
  color: red;
}

.TotalNumLable {
  font-family: Microsoft YaHei;
  color: #43dcff;
  font-size: 14px;
  font-weight: 700;
}

.TotalAlarmNumItemClass {
  display: inline-block;
  margin-left: 25px;
  margin-top: 10px;
}

.TargerTable {
  font-family: "Microsoft YaHei UI Light";
  cursor: pointer;
  font-size: 13px;
}

.TargerTable_Check {
  color: rgba(214, 185, 11, 1) !important;
}

.FontFamily {
  font-family: Microsoft YaHei;
  font-weight: 700;
}

.bg-right {
  height: 8px;
  width: 47px;
  left: 80px;
  top: 76px;
  background-image: url("../assets/table/bg-right.png");
}

.bg-left {
  height: 8px;
  width: 47px;
  left: 298px;
  top: 76px;
  background-image: url("../assets/table/bg-left.png");
}

.HeaderBackImg {
  background: url("../assets/TargetListHeader.jpg");
}

.BottomBackImg {
  height: 61px;
  margin-left: 5px;
  margin-right: 5px;
  border-right: 1px solid #43dcff;
  border-left: 1px solid #43dcff;
  margin-top: 0px;
  border-bottom: 1px solid #43dcff;
  border-top: 1px solid rgba(77, 220, 254, 0.3);
  box-shadow: 0px 0px 15px rgba(77, 220, 254, 0.3) inset;
}

.bbox {
  position: absolute;
  display: inline-block;
  height: 54px;
  line-height: 54px;
  padding-left: 3px;
  background-image: url("../assets/table/box-bg.png");
  background-repeat: repeat-x;
}

.bbox::before {
  content: "";
  width: 35px;
  height: 54px;
  background-image: url("../assets/table/box-left.png");
  position: absolute;
  left: -35px;
}

.bbox::after {
  content: "";
  width: 35px;
  height: 54px;
  background-image: url("../assets/table/box-right.png");
  position: absolute;
  right: -35px;
}

.list-title {
  width: 80px;
  left: 105px;
  font-size: 18px;
  font-family: MF-Lihei;
}

.tb-border-lt,
.tb-border-rb,
.tb-border-rt,
.tb-border-lb,
.bg-left,
.bg-right {
  /*height: 44px;*/
  /*width: 40px;*/
  /*background-color: #020e2c;*/
  /*position: absolute;*/
  position: absolute;
  margin: -53px -48px;
  z-index: 90;
}

.tb-border-lt {
  height: 33px;
  width: 25px;
  left: 42px;
  top: 66px;
  background-image: url("../assets/table/border-lt.png");
}

.tb-border-lb {
  height: 33px;
  width: 25px;
  bottom: 90px;
  left: 42px;
  background-image: url("../assets/table/border-lb.png");
}

.tb-border-rt {
  height: 31px;
  width: 26px;
  left: 406px;
  top: 67px;
  background-image: url("../assets/table/border-rt.png");
}

.tb-border-rb {
  height: 34px;
  width: 26px;
  left: 404px;
  bottom: 78px;
  background-image: url("../assets/table/border-rb.png");
}
/*.target_list_title__:hover+div{*/
/*  display: block;*/
/*}*/
.border_ {
  /*display: none;*/
  box-shadow: 0px 6px 3px -3px rgba(64, 204, 249, 0.2),
    -6px 0px 3px -3px rgba(64, 204, 249, 0.2),
    6px 0px 3px -3px rgba(64, 204, 249, 0.2);
  -webkit-border: 0px 6px 3px -3px rgba(64, 204, 249, 0.2),
    -6px 0px 3px -3px rgba(64, 204, 249, 0.2),
    -6px 0px 3px -3px rgba(64, 204, 249, 0.2),
    6px 0px 3px -3px rgba(64, 204, 249, 0.2);
  -moz-border: 0px 6px 3px -3px rgba(64, 204, 249, 0.2),
    -6px 0px 3px -3px rgba(64, 204, 249, 0.2),
    6px 0px 3px -3px rgba(64, 204, 249, 0.2);
}

.border_top {
  box-shadow: 0px -6px 3px -3px rgba(64, 204, 249, 0.2);
  -webkit-border: 0px -6px 3px -3px rgba(64, 204, 249, 0.2);
  -moz-border: 0px -6px 3px -3px rgba(64, 204, 249, 0.2);
}

.v-line__ {
  display: inline-block;
  width: 2px;
  height: 35px;
  background: radial-gradient(#43dcfe 24%, #0f152f 70%);
}

.v-line__Yellow {
  display: inline-block;
  width: 2px;
  height: 35px;
  background: radial-gradient(#ffcc00 24%, #0f152f 70%);
}

.scrollbox {
  position: absolute;
  height: 100%;
  width: 100%;
  overflow-x: hidden;
  overflow-y: auto;
  padding: 5px 0;
  /*border-top: 1px solid rgba(64,204,249,0.2);*/
  border-bottom: 1px solid rgba(64, 204, 249, 0.2);
  /*box-shadow: 0 0 5px 5px rgba(64,204,249,0.2);*/
  /*-webkit-box-shadow:  0 0 5px 5px rgba(64,204,249,0.2);*/

  scrollbar-color: transparent transparent;
  scrollbar-track-color: transparent;
  -ms-scrollbar-track-color: transparent;
}

.wapper {
  position: relative;
  height: auto;
  background-color: rgba(15, 21, 47, 0.7);

}

.header {
  height: 82px;
}

.con {
  height: 200px;
  margin-bottom: 50px;
  background-color: #f5f5f5;
}

.footer {
  /*flex-direction: row;*/
  /*font-size: 12px;*/
  height: 45px;
  /*line-height: 60px;*/
  /*position: absolute;*/
  display: flex;
  /*bottom: 0;*/
  /*left: 0;*/
  /*right: 0;*/
}
</style>
