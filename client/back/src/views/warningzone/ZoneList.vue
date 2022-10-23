<template>
    <transition name="fade">
      <div class="list-box">
        <div class="list-title-box">
          <span class="bbox list-title" is-title style="letter-spacing: 0"><img src="../../assets/table/title-arrow.png" alt="">{{$t('m.alarmZoneList')}} </span>
          <span class="bbox list-add" style="right:52px " @click.prevent="isCreate=true">
            <img src="../../assets/table/option-add.png"> {{$t('m.addZone')}}</span>
        </div>

        <div class="list-container" style="padding-left:40px;padding-right:40px;">
          <div class="tb-border-lt"><div></div></div>
          <div class="tb-border-rt"><div></div></div>
          <div class="tb-border-rb"><div></div></div>
          <div class="tb-border-lb"><div></div></div>
          <div class="list-header">
            <span class="list-header-item" style="padding-left: 10px">{{$t('m.number')}}</span>
            <span class="list-header-item" style="padding-left: 10px">{{$t('m.name')}}</span>
            <span class="list-header-item" style="padding-left: 10px">{{$t('m.operInfo')}}</span>
          </div>
          <div class="list-scroll-box">
            <div class="scroll-box">
              <div class="list-item" :class="{'list-item_checked':currentZId===item.id}" @click.stop="doChecked(item.id)" v-for="(item,key) in tableData" :key="key">
                <span class="list-item-cell" style="padding-left: 10px">{{item.id}}</span>
                <span class="list-item-cell ellipsis" style="padding-left: 10px">{{item.name}}</span>
                <span class="list-item-cell" style="padding-left: 10px">
<!--                  <img src="../../assets/table/view.png" class="link-button" @click.stop="doView(item)">-->
<!--                  <img src="../../assets/table/edit.png" class="link-button" @click.stop="resetPwd(item.id)">-->
                  <img src="../../assets/table/delete.png" class="link-button" style="margin-left: 45px" @click.stop="doDelete(item.id)">
							  </span>
              </div>
            </div>
          </div>
        </div>
        <div class="list-paging-box">
          <div class="paging-box">
            <el-pagination :pager-count="5" background
                           :hide-on-single-page="false" class="page-layout"
                           :total="total" :page-size="size" :current-page.sync="index"
                           :prev-text="$t('m.lastPage')" :next-text="$t('m.nextPage')"
                           layout="prev, pager, next"
                           @current-change="_initData"></el-pagination>
          </div>
        </div>
        <!--    新增用户-->
        <el-drawer :visible.sync="isCreate" direction="rtl" model>
          <create-zone @success="onSuccess"/>
        </el-drawer>
      </div>
    </transition>
</template>

<script>
import { http_request, request_warn_zones,delete_warn_zone } from "../../modes/api";
import {show_message,msg_enum,show_confirm} from "../../modes/elementUI";
import Bus from "../../modes/tool/bus";
import { event_type,zoneInitType } from "../../modes/tool";
import Create from './Create'

export default {
  name: "ZoneList",
  data() {
    return {
      isCreate:false,
      tableData: [],
      index:1,
      size:10,
      total:10,
      currentZId:undefined
    };
  },
  methods: {
    //删除成功
    doDelete(id){
      show_confirm(this.$t('m.tips'), this.$t('m.sureDel'),()=>{
        http_request(delete_warn_zone,id,()=> {
          show_message(msg_enum.success,this.$t('m.delSuccess'));
          this._initData({type:zoneInitType.removed});
          this.$emit('onDataChanged',{type: zoneInitType.removed});
          // Bus.$emit(event_type.reload_zone_data, {type: zoneInitType.removed})
        })
      });
    },
    //选中
    doChecked(id){
      Bus.$emit(event_type.begin_draw_zone,id);
      if(id){
        this.currentZId=id;
      }
    },
    // 添加成功
    onSuccess(id) {
      show_message(msg_enum.success, this.$t('m.addSuccess'));
      this.isCreate = false;
      this._initData({id,type:zoneInitType.created});
      this.$emit('onDataChanged',{type: zoneInitType.created,id});
      // Bus.$emit(event_type.reload_zone_data, {type: zoneInitType.created,id});
    },
    //初始化数据
    _initData(_selectData) {
      const {index,size}=this
      let newIndex=index;
      //添加成功后应当加载最后一页数据
      if(_selectData&&_selectData.type===zoneInitType.created){
        // debugger
        if(this.total%size>=0) {
          newIndex = parseInt(this.total / size) + 1;
        }
      }
      //删除当前页最后一条
      if(_selectData&&_selectData.type===zoneInitType.removed) {
        // debugger
        if (this.total % size === 1) {
          newIndex -= 1;
        }
      }
      //刷新数据
      http_request(request_warn_zones, {index:newIndex,size}, _data => {
        // console.log(_data);
        const {data,snumSize,pageIndex}=_data;
        this.tableData = data;
        this.total=snumSize;
        this.index=pageIndex;
        if(data.length==0) return;
        this.currentZId=this.tableData[0].id;
        if(_selectData){
          const {id}=_selectData;
          if(id){
            this.currentZId=id;
          }
        }
        else{
          this.$emit('onDataChanged',{type:zoneInitType.initData});
        }
        // setTimeout(()=>{
        //   // debugger
        //   Bus.$emit(event_type.reload_zone_data,{type:zoneInitType.initData});
        //   },10);

        // debugger
        // if(data.length){
        //   // if(!_selectData) this.doChecked(data[0].id);
        //   // return false;
        //   //选中第一条
        //   // if(_selectData) {
        //   //   const {id} = _selectData;
        //   //   if(id){
        //   //     this.doChecked(id);
        //   //     return false;
        //   //   }
        //   // }
        //   // this.doChecked(data[0].id);
        //   // return false;
        // }
      });
    }
  },
  mounted() {
    // Bus.$off(event_type.init_zone_data);
    // Bus.$on(event_type.init_zone_data,data=>{
    //   this._initData(data);
    // });
    this._initData();
  },
  components:{
    CreateZone:Create
  }
};
</script>

<style scoped src="../../assets/table/table.css">
</style>
