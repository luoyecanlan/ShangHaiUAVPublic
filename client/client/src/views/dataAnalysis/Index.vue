<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" style="width: 250px" is-title="">
        <img src="../../assets/table/title-arrow.png" alt="">{{'  '+$t('m.analysis')}}
      </span>

<!--      <span class="bbox list-add" style="right: 450px;width: 145px" @click.prevent="isCreate=true">-->
<!--        <img src="../../assets/table/option-add.png">{{$t('m.deviceAdd')}}-->
<!--      </span>-->

<!--      <span class="bbox list-search">-->
<!--					<input type="text" :placeholder="$t('m.clickSearch')" v-model="key" @keyup.enter="initData">-->
<!--					<img src="../../assets/table/search.png" class="link-button"-->
<!--               @click.prevent="initData" height="16">-->
<!--				</span>-->
    </div>
    <div class="heatmap-container"><HeatMap></HeatMap></div>

    <div style="width:550px;height:400px;position:absolute;top:0%;right:10%;z-index:999;">
      <weigui2></weigui2>
    </div>

    <div style="width:550px;height:400px;position:absolute;top:50%;right:10%;z-index:999;">
      <river></river>
    </div>
<!--    <div class="card_wrap"  style="width:550px;height:400px;position:absolute;top:50%;right:10%;z-index:999;">-->
<!--      <v-card class="r_card">-->
<!--        <v-card-title>Ve-histogram</v-card-title>-->
<!--        <v-divider></v-divider>-->
<!--        <div class="item_content">-->
<!--          <div class="chart_wrap">-->
<!--            <ve-histogram-->
<!--              :data="chartData"-->
<!--              :extend="chartExtend"-->
<!--              height="240px"-->
<!--            ></ve-histogram>-->
<!--          </div>-->
<!--        </div>-->
<!--      </v-card>-->
<!--      <v-card class="r_card">-->
<!--        <v-card-title>Ve-line</v-card-title>-->
<!--        <v-divider></v-divider>-->
<!--        <div class="item_content">-->
<!--          <div class="tool">-->
<!--            <div class="t">-->
<!--              <div class="i">Daily Income</div>-->
<!--              <div class="r">-->
<!--                <v-icon>mdi-arrow-up</v-icon>38%-->
<!--              </div>-->
<!--            </div>-->
<!--            <div class="v">584</div>-->
<!--          </div>-->
<!--          <div class="chart_wrap chart_wrap1">-->
<!--            <ve-line-->
<!--              :data="chartData2"-->
<!--              :extend="chartExtend2"-->
<!--              :settings="chartSettings"-->
<!--              height="240px"-->
<!--            ></ve-line>-->
<!--          </div>-->
<!--        </div>-->
<!--      </v-card>-->
    </div>
<!--    <div class="list-paging-box">-->
<!--      <div class="paging-box">-->
<!--        <el-pagination background :pager-count="5"-->
<!--                       :hide-on-single-page="false" class="page-layout"-->
<!--                       :total="total" :page-size="size" :current-page.sync="index"-->
<!--                       :prev-text="$t('m.lastPage')" :next-text="$t('m.nextPage')"-->
<!--                       layout="prev, pager, next"-->
<!--                       @current-change="initData"></el-pagination>-->
<!--      </div>-->
<!--    </div>-->
<!--    &lt;!&ndash;    新增设备&ndash;&gt;-->
<!--    <el-drawer-->
<!--      :visible.sync="isCreate"-->
<!--      direction="rtl"-->
<!--      model-->
<!--    >-->
<!--      <create-device @success="onSuccess"/>-->
<!--    </el-drawer>-->
<!--    &lt;!&ndash;    设备信息&ndash;&gt;-->
<!--    <el-drawer-->
<!--      :visible.sync="isView"-->
<!--      direction="rtl"-->
<!--      model-->
<!--      @opened="openViewer"-->
<!--    >-->
<!--      <device-viewer ref="viewer" :info="select"/>-->
<!--    </el-drawer>-->
<!--    &lt;!&ndash;    修改设备信息&ndash;&gt;-->
<!--    <el-drawer-->
<!--      :visible.sync="isUpdate"-->
<!--      direction="rtl"-->
<!--      model-->
<!--    >-->
<!--      <update-device :info="select" :old-data="updateBackup" @success="onSuccess"/>-->
<!--    </el-drawer>-->

  </div>
</template>

<script>
import Create from "./Create";
import Viewer from "./Viewer";
import Update from "./Update";
import HeatMap from "./HeatMap";
import { http_request, request_devices, delete_device } from "../../modes/api";
import { show_delete, show_message, msg_enum } from "../../modes/elementUI";
import { mapState,mapActions } from "vuex";
import River from './river'
import Weigui2 from './weigui2'
import echartMixins from '@/mixins/echart-settings';
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
  mixins: [echartMixins],
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
    DeviceViewer: Viewer,
    River,
    Weigui2,
    HeatMap
  }
};
</script>

<style src="../../assets/table/table.css" scoped>
</style>
