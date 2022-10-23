<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" style="width: 120px" is-title
        ><img src="../../assets/table/title-arrow.png" alt="" /> 历史数据</span
      >
      <div class="bbox list-add" style="right: 620px; width: 720px">
        <!-- 飞机类型 -->
        <el-select
          style="width: 140px"
          v-model="selectUAVType"
          placeholder="飞机类型"
        >
          <el-option
            v-for="item in uavTypes"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          >
          </el-option>
        </el-select>
        <!-- 无人机SN码 -->
        <el-input style="width: 140px" v-model="key" placeholder="SN码" />
        <!-- 选择日期 -->
        <el-date-picker
          style="width: 400px"
          v-model="selectDateRang"
          type="datetimerange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
        >
        </el-date-picker>
        <!-- 搜索按钮 -->
        <img
          src="../../assets/table/search.png"
          class="link-button"
          style="margin-left: 2px；margin-top:2px"
          @click.stop="doSearchHistoryData"
        />
      </div>
      <span
        class="bbox list-add"
        style="right: 80px; width: 100px"
        @click.prevent="ClearAllHisData()"
      >
        删除选中数据
      </span>
      <span
        class="bbox list-add"
        style="right: 260px; width: 100px"
        @click.prevent="GetUserOperHisData()"
      >
        导出操作记录
      </span>
      <span
        class="bbox list-add"
        style="right: 440px; width: 100px"
        @click.prevent="GetHisData()"
      >
        导出历史数据
      </span>
    </div>

    <div class="list-container">
      <div class="tb-border-lt"><div></div></div>
      <div class="tb-border-rt"><div></div></div>
      <div class="tb-border-rb"><div></div></div>
      <div class="tb-border-lb"><div></div></div>

      <div class="list-header">
        <span
          class="list-header-item"
          style="cursor: pointer; flex: 0.2"
          @click="CheckAllData()"
          >选择</span
        >
        <span class="list-header-item">无人机SN</span>
        <span class="list-header-item" style="flex: 0.5">无人机类型</span>
        <span class="list-header-item" style="flex: 0.5">威胁等级</span>
        <span class="list-header-item" style="flex: 0.5">反制情况</span>
        <span class="list-header-item" style="flex: 0.5">轨迹点(数量)</span>
        <span class="list-header-item">起始时间</span>
        <span class="list-header-item">结束时间</span>
        <span class="list-header-item" style="flex: 0.5">持续时长(秒)</span>
        <span class="list-header-item" style="flex: 0.3">回放</span>
        <span class="list-header-item" style="flex: 0.3">删除</span>
      </div>

      <div class="list-scroll-box">
        <div class="scroll-box">
          <div class="list-item" v-for="(item, key) in tableData" :key="key">
            <el-checkbox
              v-model="item.check"
              :checked="item.check"
              @change="ChangeCheck(item)"
              style="margin-left: 15px"
            />
            <span class="list-item-cell" style="margin-left: 25px">{{
              item.sn
            }}</span>
            <span class="list-item-cell ellipsis" :title="UAVModelLabel(item.uavModel)" style="flex: 0.5">{{
              UAVModelLabel(item.uavModel)
            }}</span>
            <span
              class="list-item-cell"
              :style="`color:${getThreat(item.threatMax).color}`"
              style="flex: 0.5"
              >{{ getThreat(item.threatMax).label || "无告警" }}</span
            >
            <span class="list-item-cell" style="flex: 0.5">{{
              item.hitMark?"已反制":"未操作"
            }}</span>
            <span class="list-item-cell" style="flex: 0.5">{{
              item.count
            }}</span>
            <span class="list-item-cell">{{ item.starttime }}</span>
            <span class="list-item-cell">{{ item.endtime }}</span>
            <span class="list-item-cell" style="flex: 0.5"
              >{{ item.keepScounds }}s</span
            >
            <span class="list-item-cell" style="flex: 0.3">
              <img
                src="../../assets/table/bofang.png"
                class="link-button"
                style="margin-top: 11px; margin-left: 22px"
                @click.stop="doReplay(item.tgId)"
              />
            </span>
            <span class="list-item-cell" style="flex: 0.3">
              <img
                src="../../assets/table/delete.png"
                class="link-button"
                style="margin-left: 25px"
                @click.stop="ClearSingleHisData(item.id)"
              />
            </span>
          </div>
        </div>
      </div>
    </div>

    <div class="list-paging-box">
      <div class="paging-box">
        <el-pagination
          background
          :pager-count="5"
          :hide-on-single-page="false"
          class="page-layout"
          :total="total"
          :page-size="size"
          :current-page.sync="index"
          :prev-text="$t('m.lastPage')"
          :next-text="$t('m.nextPage')"
          layout="prev, pager, next"
          @current-change="initData"
        ></el-pagination>
      </div>
    </div>

    <el-dialog
      width="96%"
      top="50px"
      :visible.sync="isShowFly"
      direction="rtl"
      model
    >
      <tg-play ref="tgPlay" />
    </el-dialog>
  </div>
</template>

<script>
import {
  show_delete,
  show_message,
  show_confirm,
  msg_enum,
} from "../../modes/elementUI";
import {
  http_request,
  request_his_targets,
  get_his_list,
  get_his_user_list,
  delete_all_hisdata,
  delete_single_hisdata,
  query_uav_types,
  query_history_ids,
} from "../../modes/api";
import TgPlay from "./tgReplay/ReplayTGWnd";
import bus from "../../modes/tool/bus";
import { mapState, mapActions } from "vuex";
const threatOption = [
  {
    id: 0,
    color: "#00A518",
    label: "无告警",
  },
  {
    id: 1,
    color: "#42D8FC",
    label: "蓝色告警",
  },
  {
    id: 2,
    color: "#42D8FC",
    label: "蓝色告警",
  },
  {
    id: 3,
    color: "#F50000",
    label: "红色告警",
  },
];
export default {
  name: "Index",
  data() {
    return {
      TrueOrFlase: true,
      pageCount: 3,
      isShowFly: false,
      tableData: [],
      select: {},
      isCreate: false,
      isView: false,
      isLink: false,
      key: "",
      index: 1,
      size: 9,
      total: 50,
      selectDateRang: [],
      selectUAVType: -99,
      uavTypes: [],
    };
  },
  methods: {
    ...mapActions(["update_tgId"]),
    // 无人机类型转换
    UAVModelLabel(value) {
      let item = this.uavTypes.find((f) => f.id === value);
      return item ? item.name : "";
    },
    // 搜索历史记录
    doSearchHistoryData() {
      this.index = 1;
      this.initData();
    },
    ChangeCheck(item) {
      for (let i = 0; i < this.tableData.length; i++) {
        if ((this.tableData[i].id = item.id)) {
          this.tableData[i].check = !item.check;
          this.$forceUpdate();
          return;
        }
      }
    },
    CheckAllData() {
      for (let i = 0; i < this.tableData.length; i++) {
        this.tableData[i].check = this.TrueOrFlase;
      }
      this.TrueOrFlase = !this.TrueOrFlase;
      this.$forceUpdate();
    },
    InitCheckData(flag) {
      for (let i = 0; i < this.tableData.length; i++) {
        this.tableData[i].check = false;
      }
      //console.log(this.tableData);
      this.$forceUpdate();
    },
    GetTime(date = undefined) {
      console.log("date 88888:>> ", date);
      date = new Date(date) || new Date();
      let yy = date.getFullYear();
      let mm = date.getMonth() + 1;
      let dd = date.getDate();
      let hh = date.getHours();
      let mf =
        date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
      let ss =
        date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
      return yy + "-" + mm + "-" + dd + " " + hh + ":" + mf + ":" + ss;
    },
    GetHisData() {
      const { index, size, key, selectUAVType } = this;
      let query = {
        key,
        start: this.queryStartTime,
        end: this.queryEndTime,
        uavModel: selectUAVType,
        desc: true,
      };
      http_request(query_history_ids, query, (ids) => {
        if (ids && ids.length) {
          console.log('ids :>> ', ids);
          http_request(get_his_list, ids, (data) => {
            let fileName = "目标历史数据" + this.GetTime() + ".xlsx";
            const link = document.createElement("a");
            if ("download" in link) {
              link.download = fileName;
              link.style.display = "none";
              link.href = this.$baseUrl + "/api" + data;
              document.body.appendChild(link);
              link.click();
              URL.revokeObjectURL(link.href);
              document.body.removeChild(link);
            } else {
              navigator.msSaveBlob(data, fileName);
            }
          });
        }
      });
    },
    GetUserOperHisData() {
      let today = new Date();
      const dateStr = `${today.getFullYear()}-${
        today.getMonth() + 1
      }-${today.getDate()}`;
      http_request(get_his_user_list, dateStr, (data) => {
        console.log("data :>> ", data, typeof data);
        let fileName = "用户操作信息" + this.GetTime() + ".xlsx";

        const link = document.createElement("a");
        if ("download" in link) {
          link.download = fileName;
          link.style.display = "none";
          link.href = this.$baseUrl + "/api" + data;
          document.body.appendChild(link);
          link.click();
          URL.revokeObjectURL(link.href);
          document.body.removeChild(link);
        } else {
          navigator.msSaveBlob(data, fileName);
        }
      });
    },
    ClearAllHisData() {
      let list = [];
      this.tableData.forEach((item) => {
        if (item.check) {
          list.push(item.id);
        }
      });

      if (list.length == 0) {
        show_message(msg_enum.success, "请先勾选要删除的数据！");
        return;
      }
      show_confirm(
        "提示信息",
        "确认要删除所有历史数据吗？删除后不可恢复！",
        () => {
          show_confirm("提示信息", "真的确认删除吗？", () => {
            http_request(delete_all_hisdata, list, (data) => {
              if (data) {
                show_message(msg_enum.success, "清空成功！");
                this.initData();
              }
            });
          });
        }
      );
    },
    ClearSingleHisData(tgId) {
      show_confirm(
        "提示信息",
        "确认要删除该条历史数据吗？删除后不可恢复！",
        () => {
          http_request(delete_single_hisdata, tgId, (data) => {
            console.log("delete_single_hisdata", data);
            if (data) {
              show_message(msg_enum.success, "删除成功！");
              this.initData();
            }
          });
        }
      );
    },
    changePage() {
      this.initData();
    },
    resetPwd(id) {},
    openLinker() {
      this.$refs.linker.initView();
    },
    openViewer() {
      this.$refs.viewer.initView();
    },
    doLink(info) {
      this.select = info;
      this.isLink = true;
    },
    doView(info) {
      this.select = info;
      this.isView = true;
    },
    doReplay(id) {
      this.isShowFly = true;
      this.update_tgId(id);
      bus.$emit("TGPlay__");
    },
    doReplays(id) {
      show_message(msg_enum.success, "点击成功成功！");
    },
    getThreat(threat) {
      let ttag = 0;
      if (threat < 30) ttag = 0;
      else if (threat < 60) ttag = 1;
      else ttag = 3;
      return threatOption.find((f) => f.id === ttag) || {};
    },
    initData() {
      const { index, size, key, selectUAVType } = this;
      let query = {
        key,
        start: this.queryStartTime,
        end: this.queryEndTime,
        uavModel: selectUAVType,
        desc: true,
      };
      http_request(
        request_his_targets,
        { index, size, data: query },
        (_data) => {
          const { data, snumSize } = _data;
          this.tableData = data;
          this.total = snumSize;
          this.TrueOrFlase = true;
          this.InitCheckData(false);
        }
      );
    },
    queryUAVtypes() {
      http_request(query_uav_types, null, (data) => {
        this.uavTypes = data;
      });
    },
    addSuccess() {
      this.isCreate = false;
      this.isLink = false;
      this.initData();
    },
  },
  created() {
    const defSDate = new Date();
    this.selectDateRang = [
      new Date(defSDate.setDate(defSDate.getDate() - 1)),
      new Date(),
    ];

    this.$forceUpdate(this.selectDateRang);
  },
  mounted() {
    this.queryUAVtypes();
    this.initData();
  },
  computed: {
    ...mapState(["tgId"]),
    queryStartTime() {
      if (this.selectDateRang && this.selectDateRang.length)
        return this.GetTime(this.selectDateRang[0]);
      else {
        return "";
      }
    },
    queryEndTime() {
      if (this.selectDateRang && this.selectDateRang.length)
        return this.GetTime(this.selectDateRang[1]);
      else {
        return "";
      }
    },
  },
  components: {
    TgPlay: TgPlay,
  },
};
</script>

<style scoped src="../../assets/table/table.css"></style>

