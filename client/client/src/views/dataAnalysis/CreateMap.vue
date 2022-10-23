<template>
  <div class="link right-box">
    <p class="info-title"><i class="iconfont icon-info"></i>设备基本信息</p>
    <el-row class="info-item" :gutter="10">
      <el-col class="info-item-title" :span="8">设备名称：</el-col>
      <el-col class="info-item-content" :span="16">{{ device.name }}</el-col>
    </el-row>
    <el-row class="info-item" :gutter="10">
      <el-col class="info-item-title" :span="8">关联服务用户：</el-col>
      <el-col class="info-item-content" :span="16">
        <el-select v-model="select" placeholder="请选择">
          <el-option
            v-for="item in users"
            :key="item.uid"
            :label="item.nick"
            :value="item.uid">
          </el-option>
        </el-select>
      </el-col>
    </el-row>
    <el-row class="info-item" :gutter="10" v-if="canLinkZone">
      <el-col class="info-item-title" :span="8">关联告警区域：</el-col>
      <el-col class="info-item-content" :span="16">
        <el-select v-model="selectZones" multiple placeholder="请选择">
          <el-option
            v-for="item in zones"
            :key="item.id"
            :label="item.name"
            :value="item.id">
          </el-option>
        </el-select>
      </el-col>
    </el-row>
    <el-row :gutter="10" class="info-item" style="margin-top: 10px;">
      <el-button
        type="primary"
        class="full-row"
        @click.stop="doSave"
        size="small"
      >
        保 存 <i class="iconfont icon-save"></i>
      </el-button>
    </el-row>
  </div>
</template>

<script>
  import {
    http_request,
    // set_map_device_s_user,
    // set_map_device_zones,
    // request_device_map_s_users,
    // request_device_map_zones,
    // request_simple_users,
    // request_simple_warn_zones
  } from "../../modes/api";
  import {show_message,msg_enum} from "../../modes/elementUI";

  export default {
    name: "LinkUser",
    data(){
      return {
        users:[],
        select:undefined,
        zones:[],
        selectZones:[]
      }
    },
    props: {
      device: {
        required: true
      }
    },
    computed:{
      canLinkZone() {
        const {category} = this.device;
        return parseInt(category / 10000) === 1;
      }
    },
    methods:{
      // doSave(){
      //   const {id}=this.device;
      //   const {select,selectZones,canLinkZone}=this;
      //   if(select>0){
      //     http_request(set_map_device_s_user,{devId:id,uid:select},()=>{
      //       show_message(msg_enum.success,"服务用户关联成功!");
      //       this.$emit("success");
      //     });
      //   }
      //   //保存设备关联的区域信息
      //   if(canLinkZone&& selectZones.length) {
      //     if(selectZones.length){
      //       console.log({devId:id,zoneIds:selectZones})
      //       http_request(set_map_device_zones,{devId:id,zoneIds:selectZones},()=>{
      //         show_message(msg_enum.success,"告警区域关联成功!");
      //         this.$emit("success");
      //       });
      //     }
      //   }
      // },
      // InitView(){
      //   this.select=undefined;
      //   //获取当前用户关联的设备
      //   http_request(request_device_map_s_users,this.device.id,(data)=>{
      //     if(data){
      //       const {uid}=data;
      //       this.select=uid;
      //     }
      //   });
      //   //获取设备关联的告警区域
      //   this.selectZones=[];
      //   http_request(request_device_map_zones,this.device.id,(data)=>{
      //     console.log('devices:',data);
      //     if(data&&data.length>0){
      //       data.forEach(zone=>{
      //         this.selectZones.push(zone.id);
      //       });
      //     }
      //   })
      // }
    },
    mounted() {
      //获取全部设备
      // http_request(request_simple_users,'',(data)=>{
      //   if(data.length){
      //     data.forEach(f=>{
      //       if(f.role==='device'){
      //         this.users.push(f);
      //       }
      //     });
      //   }
      // });
      // if(this.canLinkZone){
      //   //获取全部告警区域信息
      //   http_request(request_simple_warn_zones,'',(data)=>{
      //     this.zones=data;
      //   })
      // }
    }
  };
</script>

<style scoped>

</style>
