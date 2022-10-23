<template>
  <div class="mapbox">
    <div class="target-list-box__">
      <target-list />
    </div>

    <div class="map-box__">
      <div class="map-box-container__">
        <div class="map-option-box__">
          <div class="target-operation-list__"><target-operation-list /></div>
          <target-search />
        </div>
        <div class="map">

          <!--目标信息窗口-->
          <target-info-viewer
            id="TargetInfoViewerDisplay"
            style="position: absolute; z-index: 10; right: 4px; bottom: 28px"
          />
          <!--style="position:absolute;z-index: 10;top: 52px;right: 8px;"/>-->
          <h5-video-column
            style="position: absolute; left: 3px; top: 55px; z-index: 10"
          />
          <!--视频窗口-->
          <video-info-wiewer
            id="VideoViewerDisplay"
            style="
              position: absolute;
              top: 55px;
              display: none;
              left: 12px;
              z-index: 10;
            "
          />
          <!--信息标识窗口-->
          <w-legend
            id="LegendViewer"
            style="position: absolute; bottom: 29px; left: 4px"
          />
          <hawkeye-viewer
            id="HawkeyeViewer"
            style="position: absolute; top: 42px; right: 0px; z-index: 10"
          />
          <w-map style="cursor: pointer" />
        </div>

        <div>
          <!--工具栏-->
          <map-tools-viewer style="padding-top: 5px" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
import {Draw_DevIcon_Marker, Draw_DevListIcon_Marker, Draw_PositionDev_Icon_Marker} from "./mapHandle/mapDrawDev";
import { Draw_Zone } from "./mapHandle/mapDraw";
import { Init_Aspnet_Signalr } from "./mapHandle/mapService";
import DeviceList from "./DeviceList";
import DeviceViewer from "./DeviceViewer";
import TargetInfoViewer from "./TargetInfoViewer";
import VideoInfoWiewer from "./VideoInfoWiewer";
import MapToolsViewer from "./MapToolsViewer";
import TargetSearch from "../components/TargetInputSearch";
import Legend from "../components/Legend";
import Map from "./Map";
import Footer from "../components/Footer";
import TargetList from "./TargetList";
import TargetOperationList from "./TargetOperationList";
import H5VideoColumn from "./H5VideoColumn";
import HawkeyeViewer from "./HawkeyeViewer";
import bus from "../modes/tool/bus";
import { RemoveTargetById } from "./mapHandle/mapDraw";
import {http_request, request_all_warn_zones} from "../modes/api";

export default {
  components: {
    wFooter: Footer,
    TargetList: TargetList,
    TargetInfoViewer: TargetInfoViewer,
    TargetOperationList: TargetOperationList,
    wMap: Map,
    VideoInfoWiewer: VideoInfoWiewer,
    wLegend: Legend,
    wDeviceList: DeviceList,
    wDeviceViewer: DeviceViewer,
    MapToolsViewer: MapToolsViewer,
    TargetSearch: TargetSearch,
    H5VideoColumn: H5VideoColumn,
    HawkeyeViewer: HawkeyeViewer,
  },
  name: "Index",
  data() {
    return {
      audio: new Audio(),
      selectDevice: true,
      selectText: "",
      DevTimer: "",
      TGTimer: "",
      TGTimeOutSecond: 25,
      Logo: "../assets/logo.png",
      targetInfoForm: {
        id: "",
        lat: 0.0,
        lng: 0.0,
        alt: 100.0,
        category: "",
        mode: "",
        pCount: "",
        threat: "",
        trackTime: "",
      },
    };
  },
  computed: {
    ...mapState(["targets"]),
    ...mapState(["targets_filter"]),
    ...mapState(["devices"]),

    ...mapState(["targets_0level_num"]),
    ...mapState(["targets_1level_num"]),
    ...mapState(["targets_2level_num"]),
    ...mapState(["targets_3level_num"]),
    ...mapState(["targets_out_level_num"]),
  },
  methods: {
    ...mapActions(["query_devices"]),
    ...mapActions(["set_select_target_id"]),
    ...mapActions(["set_target_0level_num"]),
    ...mapActions(["set_target_1level_num"]),
    ...mapActions(["set_target_2level_num"]),
    ...mapActions(["set_target_3level_num"]),
    ...mapActions(["set_target_out_level_num"]),
    ...mapActions(["del_target_info"]),
    ...mapActions(["del_target_filter_info"]),
    PlayAlarmMp3() {
      let flag = this.audio.paused;
      if (flag) {
        new Promise((slov) => {
          this.audio.src = require("../../static/mp3/1.wav");
          slov();
        }).then(() => {
          this.audio.play();
        });
      }
    },
    CheckTGLevelNum() {
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
            break;
        }
      });
    },

    DevTimerQuery() {
      //console.log("定时查询设备状态", this.devices);
      Draw_DevListIcon_Marker(this.devices); // 自动查询设备信息
    },
    // 初始化地图事件
    TGTimerCheckDel() {
      let isHavaDelData = false;
      console.log("开始检查目标的更新时间", this.targets_filter);

      let nowTime = new Date();
      nowTime.setTime(nowTime.getTime());

      this.targets_filter.forEach((info) => {
        // console.log(info.trackTime);
        let num =
          (nowTime.getTime() - new Date(info.trackTime).getTime()) / 1000;
        //console.log('num',num);
        if (num > this.TGTimeOutSecond) {
          isHavaDelData = true;
          // 移除目标
          RemoveTargetById(info.id);
          // 移除列表中目标信息
          this.del_target_info(info.id);
          this.del_target_filter_info(info.id);
        }
      });

      this.targets.forEach((info) => {
        let num =
          (nowTime.getTime() - new Date(info.trackTime).getTime()) / 1000;
        //console.log('num',num);
        if (num > this.TGTimeOutSecond) {
          console.log(info.trackTime);
          isHavaDelData = true;
          // 移除目标
          RemoveTargetById(info.id);
          //移除列表中目标信息
          this.del_target_info(info.id);
          this.del_target_filter_info(info.id);
        }
      });

      // 需要重置数量
      if (isHavaDelData == true) {
        console.log("检查到有未更新的目标->删除");
        this.CheckTGLevelNum();
      }
    },
  },
  created() {},
  mounted() {
    document.getElementById("TargetInfoViewerDisplay").style.display = "none";
    document.getElementById("LegendViewer").style.display = "none";

    bus.$on("MapInitSuccess", () => {
      console.log("地图收到消息开始加载");
      Init_Aspnet_Signalr(); // 初始化接收推送方法
      this.query_devices(); // 查询设备

      // 定时器开启 查询设备
      this.DevTimer = setInterval(this.DevTimerQuery, 2500);
      this.TGTimer = setInterval(this.TGTimerCheckDel, 3000); //120000);

      http_request(request_all_warn_zones, null, (_data) => {
        //console.error("999999999999999999999999999999999999999999999999999999999999999999999999999999"+_data)
        if (_data && _data.length) {
          _data.forEach((__info_) => {
            Draw_Zone(__info_);
          });
        }
      });
    });

    bus.$on("ClearTimer", () => {
      console.log("清除【目标定时删除定时器】成功");
      window.clearInterval(this.TGTimer);
    });

    bus.$on("TargetAlarmPlayMp3", () => {
      this.PlayAlarmMp3();
    });
  },
};
</script>

<style scoped>
.mapbox {
  height: 100%;
  width: 100%;
  display: flex;
  flex-direction: row;
}

.target-list-box__ {
  /*width: 376px;*/

  margin-left: 0px;
  /*height: 500px;*/
  position: absolute;
  z-index: 99;
  /*opacity: 100;*/
}
.target-operation-list__{
  margin-left: 400px;
  position: relative;
}
.map-box__ {
  flex: 1;
  position: relative;
}

.map-box-container__ {
  position: absolute;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.map-option-box__ {
  position: relative;
  height: 45px;
  display: flex;
  flex-direction: row;
  /*background-image: url("../assets/main/TargetOperationImg.png");*/
}

.map {
  flex: 1;
  border-left: 1px solid #40cef9;
  border-right: 1px solid #40cef9;
}

.mapGridStyle {
}

.itemFormColor {
  font-size: 22px;
  color: #43dcff;
}

.device-list {
  position: absolute;
  top: 70px;
  left: 10px;
}

.mapDialog {
  display: none;
  background: rgba(0, 0, 0, 0.5);
  padding: 8px;
  width: 310px;
  height: 510px;
}

.el-header,
.el-footer {
  background-color: #b3c0d1;
  color: #333;
  text-align: center;
  line-height: 60px;
}

.el-aside {
  background-color: #505050;
  color: #333;
  text-align: center;
  line-height: 200px;
}

.el-main {
  background-color: #e9eef3;
  color: #333;
  text-align: center;
  line-height: 160px;
}

body > .el-container {
  margin-bottom: 40px;
}

.el-container:nth-child(5) .el-aside,
.el-container:nth-child(6) .el-aside {
  line-height: 260px;
}

.el-container:nth-child(7) .el-aside {
  line-height: 320px;
}

.ol-custom-overviewmap,
.ol-custom-overviewmap.ol-uncollapsible {
  bottom: auto;
  left: auto;
  right: 5px;
  top: 5px;
  padding: 2px;
  margin: 0;
  width: 300px;
  height: 200px;
  background-color: #ffffff;
  border: 1px solid rgba(51, 51, 51, 0.8);
  box-sizing: border-box;
  box-shadow: 0 0 3px 3px rgba(0, 0, 0, 0.5);
  -weblit-box-shadow: 0 0 3px 3px rgba(0, 0, 0, 0.5);
}
</style>
