<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" is-title
        ><img src="../../assets/table/title-arrow.png" alt="" />白名单列表
      </span>
      <span
        class="bbox list-add"
        style="right: 450px; width: 114px"
        @click.prevent="isCreate = true"
        ><img src="../../assets/table/option-add.png" /> 新增白名单
      </span>
      <span class="bbox list-search">
        <input
          type="text"
          v-model="key"
          @keyup.enter="initData"
          placeholder="请输入关键字"
        />
        <img
          src="../../assets/table/search.png"
          class="link-button"
          @click.stop="initData"
          alt=""
          height="16"
        />
      </span>
    </div>
    <div class="list-container">
      <div class="tb-border-lt"><div></div></div>
      <div class="tb-border-rt"><div></div></div>
      <div class="tb-border-rb"><div></div></div>
      <div class="tb-border-lb"><div></div></div>
      <div class="list-header">
        <span class="list-header-item">序号</span>
        <span class="list-header-item">SN码</span>
        <span class="list-header-item">报备部门</span>
        <span class="list-header-item">开始时间</span>
        <span class="list-header-item">结束时间</span>
        <span class="list-header-item">操作</span>
      </div>
      <div class="list-scroll-box">
        <div class="scroll-box">
          <div class="list-item" v-for="(item, key) in tableData" :key="key">
            <span class="list-item-cell ellipsis">{{ key + 1 }}</span>
            <span class="list-item-cell ellipsis">{{ item.sn }}</span>
            <span class="list-item-cell ellipsis">{{ item.department }}</span>
            <span class="list-item-cell ellipsis"
              >{{ item.starTime }}</span
            >
            <span class="list-item-cell ellipsis"
              >{{ item.endTime }}</span
            >
            <span class="list-item-cell">
              <!--              <img src="../../assets/table/view.png" class="link-button" @click.stop="doView(item)">-->
              <!-- <img src="../../assets/table/resetpwd.png" style="margin-left: 70px" class="link-button" @click.stop="resetPwd(item.id)"> -->
              <img
                src="../../assets/table/delete.png"
                class="link-button"
                style="margin-left: 45%"
                @click.stop="doDelete(item.id)"
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
          :pager-count="3"
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
    <!--    新增用户-->
    <el-drawer :visible.sync="isCreate" direction="rtl" model>
      <create-user @success="addSuccess" />
    </el-drawer>
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
  request_white_list,
  delete_white_item,
} from "../../modes/api";
import Create from "./Add";
export default {
  name: "Index",
  data() {
    return {
      tableData: [],
      select: {},
      isCreate: false,
      isView: false,
      isLink: false,
      key: "",
      index: 1,
      size: 9,
      total: 50,
    };
  },
  methods: {
    changePage() {
      console.log("changePage");
      this.initData();
    },
    doDelete(id) {
      show_delete(this.$t("m.tips"), this.$t("m.sureDel"), () => {
        http_request(delete_white_item, id, () => {
          show_message(msg_enum.success, this.$t("m.delSuccess"));
          //当删除当前页最后一条  则跳转到上一页刷新数据
          if (this.total % this.size == 1) {
            this.index = this.index > 1 ? this.index - 1 : 1;
          }
          this.initData();
        });
      });
    },
    initData() {
      const { index, size, key } = this;
      http_request(request_white_list, { index, size, key }, (_data) => {
        const { data, snumSize } = _data;
        this.tableData = data;
        this.total = snumSize;
      });
    },
    addSuccess() {
      this.isCreate = false;
      this.isLink = false;
      this.initData();
    },
  },
  mounted() {
    this.initData();
  },
  components: {
    CreateUser: Create,
  },
};
</script>

<style scoped src="../../assets/table/table.css"></style>

