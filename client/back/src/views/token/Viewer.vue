<template>
  <transition name="fade">
    <div class="drawer-box__">
      <div class="list-box">
        <div class="list-title-box">
          <span class="bbox list-title" is-title><img src="../../assets/table/title-arrow.png" alt=""> 令牌信息</span>
        </div>
        <div v-if="info" class="info list-container" style="margin-bottom:0px;padding: 45px 30px 20px">
          <div class="tb-border-lt"></div>
          <div class="tb-border-rt"></div>
          <div class="tb-border-rb" style="bottom: 46px"></div>
          <div class="tb-border-lb" style="bottom: 46px"></div>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">用户名：</el-col>
            <el-col class="info-item-content" :span="16">{{info.username}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">用户角色：</el-col>
            <el-col class="info-item-content" :span="16" >{{roleName}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">授权时间：</el-col>
            <el-col class="info-item-content" :span="16">{{info.nbf}}</el-col>
          </el-row>
          <el-row class="info-item" :gutter="10">
            <el-col class="info-item-title" :span="8">生效时长：</el-col>
            <el-col class="info-item-content" :span="16">{{`${info.expire_in}s`}}</el-col>
          </el-row>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
  import {roleType} from "../../modes/tool";

  export default {
    name: "Viewer",
    data(){
      return {
        roleType
      };
    },
    computed:{
      roleName(){
        return this.roleType.find(f=>f.value===this.info.rold).label||'未知';
      }
    },
    props: {
      info: {
        required: true
      }
    },
    methods:{
      // initView(){
      //   http_request(request_map_user_d,this.info.uid,(data)=>{
      //     this.devices=data;
      //   });
      // }
    }
  }
</script>


