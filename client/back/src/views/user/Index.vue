<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" is-title
        ><img src="../../assets/table/title-arrow.png" alt="" />{{
          $t("m.userList")
        }}
      </span>
      <span class="bbox list-add" @click.prevent="isCreate = true"
        ><img src="../../assets/table/option-add.png" />{{
          $t("m.addUser")
        }}</span
      >
      <span class="bbox list-search">
        <input
          type="text"
          v-model="key"
          @keyup.enter="initData"
          :placeholder="$t('m.clickSearch')"
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
        <span class="list-header-item">{{ $t("m.name") }}</span>
        <span class="list-header-item">{{ $t("m.nick") }}</span>
        <span class="list-header-item">{{ $t("m.account") }}</span>
        <span class="list-header-item">{{ $t("m.userRole") }}</span>
        <span class="list-header-item">{{ $t("m.phoneNum") }}</span>
        <span class="list-header-item">{{ $t("m.email") }}</span>
        <span class="list-header-item">{{ $t("m.modifyTime") }}</span>
        <span class="list-header-item">{{ $t("m.operInfo") }}</span>
      </div>
      <div class="list-scroll-box">
        <div class="scroll-box">
          <div class="list-item" v-for="(item, key) in tableData" :key="key">
            <span class="list-item-cell ellipsis">{{ item.name }}</span>
            <span class="list-item-cell ellipsis">{{ item.nick }}</span>
            <span class="list-item-cell ellipsis">{{ item.username }}</span>
            <span class="list-item-cell">{{ item.role }}</span>
            <span class="list-item-cell">{{ item.phone }}</span>
            <span class="list-item-cell ellipsis">{{ item.email }}</span>
            <span class="list-item-cell">{{ item.updateTime }}</span>
            <span class="list-item-cell">
              <!--              <img src="../../assets/table/view.png" class="link-button" @click.stop="doView(item)">-->
              <img
                src="../../assets/table/resetpwd.png"
                style="margin-left: 70px"
                class="link-button"
                @click.stop="resetPwd(item.id)"
              />
              <img
                src="../../assets/table/delete.png"
                class="link-button"
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
    <!--    查看用户-->
    <el-drawer
      :visible.sync="isView"
      @opened="openViewer"
      direction="rtl"
      model
    >
      <user-viewer ref="viewer" :info="select" />
    </el-drawer>
    <!--    关联设备-->
    <!--        <el-drawer title="关联设备" :visible.sync="isLink" @opened="openLinker" direction="rtl" model>-->
    <!--          <link-device ref="linker" @success="addSuccess" :user="select"/>-->
    <!--        </el-drawer>-->
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
  request_users,
  delete_user,
  reset_user_pwd,
} from "../../modes/api";
import Create from "./Create";
import Viewer from "./Viewer";
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
    resetPwd(id) {
      show_confirm(this.$t("m.tips"), this.$t("m.sureResetPassword"), () => {
        http_request(reset_user_pwd, id, () => {
          show_message(msg_enum.success, this.$t("m.passwordResetSuccessful"));
          this.initData();
        });
      });
    },
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
    doDelete(id) {
      show_delete(this.$t("m.tips"), this.$t("m.sureDel"), () => {
        http_request(delete_user, id, () => {
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
      http_request(request_users, { index, size, key }, (_data) => {
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
    UserViewer: Viewer,
  },
};
</script>

<style scoped src="../../assets/table/table.css"></style>

