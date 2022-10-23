<template>
<!--  <div class="link right-box">-->
<!--    <p class="info-title"><i class="iconfont icon-info"></i>用户基本信息</p>-->
<!--    <el-row class="info-item" :gutter="10">-->
<!--      <el-col class="info-item-title" :span="8">用户姓名：</el-col>-->
<!--      <el-col class="info-item-content" :span="16">{{ user.name }}</el-col>-->
<!--    </el-row>-->
<!--    <el-row class="info-item" :gutter="10" v-show="false">-->
<!--      <el-col class="info-item-title" :span="8">关联设备：</el-col>-->
<!--      <el-col class="info-item-content" :span="16">-->
<!--        <el-select v-model="select" multiple placeholder="请选择">-->
<!--          <el-option-->
<!--            v-for="item in devices"-->
<!--            :key="item.deviceId"-->
<!--            :label="item.deviceName"-->
<!--            :value="item.deviceId">-->
<!--          </el-option>-->
<!--        </el-select>-->
<!--      </el-col>-->
<!--    </el-row>-->
<!--    <el-row :gutter="10" class="info-item" style="margin-top: 10px;">-->
<!--      <el-button-->
<!--        type="primary"-->
<!--        class="full-row"-->
<!--        @click.stop="doSave"-->
<!--        size="small"-->
<!--      >-->
<!--        保 存 <i class="iconfont icon-save"></i>-->
<!--      </el-button>-->
<!--    </el-row>-->
<!--  </div>-->
</template>

<script>
  // , set_map_user_devices,request_user_map_devices,request_simple_devices
  import { http_request} from "../../modes/api";
import {show_message,msg_enum} from "../../modes/elementUI";

export default {
  // name: "LinkDevice",
  // data(){
  //   return {
  //     devices:[],
  //     select:[]
  //   }
  // },
  // props: {
  //   user: {
  //     required: true
  //   }
  // },
  // methods:{
  //   doSave(){
  //     const {uid}=this.user;
  //     const {select}=this;
  //     // if(select.length){
  //     //   http_request(set_map_user_devices,{uid,ids:select},()=>{
  //     //     show_message(msg_enum.success,"关联设备设置成功!");
  //     //     this.$emit("success");
  //     //   });
  //     // }
  //   },
  //   initView(){
  //     //获取当前用户关联的设备
  //     // http_request(request_user_map_devices,this.user.uid,(data)=>{
  //     //   this.select=[];
  //     //   if(data.length){
  //     //     data.forEach(dev=>{
  //     //       this.select.push(dev.deviceId);
  //     //     });
  //     //   }
  //     // });
  //   }
  // },
  // mounted() {
  //   //获取全部设备
  //   // http_request(request_simple_devices,'',(data)=>{
  //   //   this.devices=data;
  //   // });
  // }
};
</script>

<style scoped></style>
