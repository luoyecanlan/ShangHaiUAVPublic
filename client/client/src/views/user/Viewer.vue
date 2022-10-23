<template>
  <transition name="fade">

    <div class="drawer-box__">
      <div class="list-box">
        <div class="list-title-box">
          <span class="bbox list-title" is-title><img src="../../assets/table/title-arrow.png" alt=""> {{$t('m.info')}}</span>
        </div>
        <div v-if="info" class="info list-container" style="margin-bottom:0px;padding: 45px 30px 20px">
          <div class="tb-border-lt"></div>
          <div class="tb-border-rt"></div>
          <div class="tb-border-rb" style="bottom: 46px"></div>
          <div class="tb-border-lb" style="bottom: 46px"></div>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.account')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.username}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.name')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.name}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.nick')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.nick}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.userRole')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{roleName}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.phoneNum')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.phone}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.email')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.email}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.creatTime')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.createTime}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">{{$t('m.modifyTime')}}：</el-col>
            <el-col class="info-item-content" :span="16">{{info.updateTime}}</el-col>
          </el-row>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
  // ,request_user_map_devices
  import {roleType} from "../../modes/tool";
  import {http_request} from "../../modes/api";

  export default {
    name: "Viewer",
    data(){
      return {
        roleType,
        devices:[]
      };
    },
    computed:{
      roleName(){
        return this.roleType.find(f=>f.value===this.info.role).label||this.$t('m.unknown');
      }
    },
    props: {
      info: {
        required: true
      }
    },
    methods:{
      initView(){
        // http_request(request_user_map_devices,this.info.uid,(data)=>{
        //   this.devices=data;
        // });
      }
    }
  }
</script>

