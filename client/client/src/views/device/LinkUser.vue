<template>
<!--  <div class="link right-box">-->
<!--    <p class="info-title"><i class="iconfont icon-info"></i>用户基本信息</p>-->
<!--    <el-row class="info-item" :gutter="10">-->
<!--      <el-col class="info-item-title" :span="8">用户姓名：</el-col>-->
<!--      <el-col class="info-item-content" :span="16">{{ device.name }}</el-col>-->
<!--    </el-row>-->
<!--    <el-row class="info-item" :gutter="10">-->
<!--      <el-col class="info-item-title" :span="8">关联设备：</el-col>-->
<!--      <el-col class="info-item-content" :span="16">-->
<!--        <el-select v-model="select" multiple placeholder="请选择">-->
<!--          <el-option-->
<!--            v-for="item in users"-->
<!--            :key="item.uid"-->
<!--            :label="item.nick"-->
<!--            :value="item.uid">-->
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
  // set_devic_users_map,request_map_d_users,request_simple_users
  import { http_request } from "../../modes/api";
  import {show_message,msg_enum} from "../../modes/elementUI";

  export default {
    // name: "LinkUser",
    // data(){
    //   return {
    //     users:[],
    //     select:[]
    //   }
    // },
    // props: {
    //   device: {
    //     required: true
    //   }
    // },
    // methods:{
    //   doSave(){
    //     const {id}=this.device;
    //     const {select}=this;
    //     // if(select.length){
    //     //   http_request(set_devic_users_map,{did:id,ids:select},(data)=>{
    //     //     show_message(msg_enum.success,"关联设备设置成功!");
    //     //     this.$emit("success");
    //     //   });
    //     // }
    //   },
    //   InitView(){
    //     //获取当前用户关联的设备
    //     // http_request(request_map_d_users,this.device.id,(data)=>{
    //     //   this.select=[];
    //     //   if(data.length){
    //     //     data.forEach(user=>{
    //     //       this.select.push(user.uid);
    //     //     });
    //     //   }
    //     // });
    //   }
    // },
    // mounted() {
    //   //获取全部设备
    //   // http_request(request_simple_users,'',(data)=>{
    //   //   this.users=data;
    //   // });
    // }
  };
</script>

<style scoped>

</style>
