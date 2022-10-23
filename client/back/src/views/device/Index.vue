<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" style="width: 250px" is-title="">
        <img src="../../assets/table/title-arrow.png" alt="">{{'  '+$t('m.deviceList')}}
      </span>

      <span class="bbox list-add" style="right: 450px;width: 145px" @click.prevent="isCreate=true">
        <img src="../../assets/table/option-add.png">{{$t('m.deviceAdd')}}
      </span>

      <span class="bbox list-search">
					<input type="text" :placeholder="$t('m.clickSearch')" v-model="key" @keyup.enter="initData">
					<img src="../../assets/table/search.png" class="link-button"
               @click.prevent="initData" height="16">
				</span>
    </div>

    <div class="list-container">
      <div class="tb-border-lt"><div></div></div>
      <div class="tb-border-rt"><div></div></div>
      <div class="tb-border-rb"><div></div></div>
      <div class="tb-border-lb"><div></div></div>
      <div class="list-header">
        <span class="list-header-item">{{$t('m.name')}}</span>
        <span class="list-header-item">{{$t('m.deviceType')}}</span>
        <span class="list-header-item">{{$t('m.deviceAddress')}}</span>
        <span class="list-header-item">{{$t('m.equipmentcoordinates')}}</span>
        <span class="list-header-item">{{$t('m.coverZone')}}</span>
        <span class="list-header-item">{{$t('m.rectificationInfo')}}</span>
        <span class="list-header-item">{{$t('m.operInfo')}}</span>
      </div>
      <div class="list-scroll-box">
        <div class="scroll-box">
          <div class="list-item" v-for="(item,key) in tableData" :key="key">
            <span class="list-item-cell">{{item.name}}</span>
            <span class="list-item-cell">{{category_name(item.category)}}</span>
            <span class="list-item-cell">{{item.ip}}</span>
            <span class="list-item-cell ellipsis" :title="[`${item.lng},${item.lat}`]">({{item.lng}},{{item.lat}})</span>
            <span class="list-item-cell">{{item.coverS}}-{{item.coverE}}</span>
            <span class="list-item-cell">{{item.rectifyAz}},{{item.rectifyEl}}</span>
            <span class="list-item-cell" >
              <img src="../../assets/table/view.png" class="link-button" style="margin-left: 70px" @click.stop="doView(item)">
              <img src="../../assets/table/edit.png" class="link-button" @click.stop="doUpdate(item)">
              <img src="../../assets/table/delete.png" class="link-button" @click.stop="doDelete(item.id)">
							</span>
          </div>
        </div>
      </div>
    </div>
    <div class="list-paging-box">
      <div class="paging-box">
        <el-pagination background :pager-count="5"
                       :hide-on-single-page="false" class="page-layout"
                       :total="total" :page-size="size" :current-page.sync="index"
                       :prev-text="$t('m.lastPage')" :next-text="$t('m.nextPage')"
                       layout="prev, pager, next"
                       @current-change="initData"></el-pagination>
      </div>
    </div>
    <!--    新增设备-->
    <el-drawer
      :visible.sync="isCreate"
      direction="rtl"
      model
    >
      <create-device @success="onSuccess"/>
    </el-drawer>
    <!--    设备信息-->
    <el-drawer
      :visible.sync="isView"
      direction="rtl"
      model
      @opened="openViewer"
    >
      <device-viewer ref="viewer" :info="select"/>
    </el-drawer>
    <!--    修改设备信息-->
    <el-drawer
      :visible.sync="isUpdate"
      direction="rtl"
      model
    >
      <update-device :info="select" :old-data="updateBackup" @success="onSuccess"/>
    </el-drawer>
<!--    <el-drawer-->
<!--      :visible.sync="isLink"-->
<!--      direction="rtl"-->
<!--      model-->
<!--      @opened="openLinker"-->
<!--    ></el-drawer>-->
  </div>
</template>

<script>
import Create from "./Create";
import Viewer from "./Viewer";
import Update from "./Update";
import { http_request, request_devices, delete_device } from "../../modes/api";
import { show_delete, show_message, msg_enum } from "../../modes/elementUI";
import { mapState,mapActions } from "vuex";

export default {
  name: "Index",
  data() {
    return {
      select: {},
      updateBackup:{},
      isView: false,
      isCreate: false,
      isUpdate: false,
      isLink:false,
      key:'',
      tableData: [],
      index:1,
      size:10,
      total: 50
    };
  },
  computed:{
    ...mapState(['device_categories'])
  },
  methods: {
    // this.request_device_categories();
    ...mapActions(['request_device_categories']),
    category_name(category) {
      if (this.device_categories) {
        let mode = this.device_categories.find(f => f.id === category);
        return mode ? mode.name : this.$t('m.unknown');
      }
      return this.$t('m.unknown')
    },
    openViewer(){
      this.$refs.viewer.InitView();
    },
    openLinker(){
      this.$refs.linker.InitView();
    },
    doLink(info){
      this.select=info;
      this.isLink=true;
    },
    doView(info) {
      this.select =info;
      this.isView = true;
    },
    doDelete(id) {
      show_delete(this.$t('m.tips'), this.$t('m.sureDel'), () => {
        http_request(delete_device, id, () => {
          show_message(msg_enum.success, this.$t('m.delSuccess'));
          if(this.total%this.size===1) {
            this.index = this.index > 1 ? this.index - 1 : 1;
          }
          this.initData();
        });
      });
    },
    doUpdate(info) {
      this.updateBackup= JSON.parse(JSON.stringify(info));
      this.select = JSON.parse(JSON.stringify(info));
      this.isUpdate = true;
    },
    initData() {
      if(!this.device_categories||this.device_categories.length==0){
        this.request_device_categories();
      }
      const {index,size,key}=this;
      http_request(request_devices, {index,size,key}, _data => {
        console.log(_data);
        const {data, snumSize} = _data;
        this.tableData = data;
        this.total = snumSize;
      });
    },
    onSuccess() {
      this.isCreate = false;
      this.isUpdate = false;
      this.isLink=false;
      this.initData();
    }
  },
  mounted() {
    this.initData();
  },
  components: {
    CreateDevice: Create,
    UpdateDevice: Update,
    DeviceViewer: Viewer
  }
};
</script>

<style src="../../assets/table/table.css" scoped>
</style>
