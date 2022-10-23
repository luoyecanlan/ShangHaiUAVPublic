<template>
  <div class="langBox"
       :style="{pointerEvents:SeDisabled ? 'none':''}"
       :class="{'langBox_dis': SeDisabled}">
    <div class="Select_"
         @click.stop="changeVis()">

    <span style="font-weight: 700;font-size: 16px;position:absolute;
          margin-left: 8px;vertical-align: 28%;margin-top: -10px;"
          @click.stop="changeVis()">{{select.ip==''? $t('m.selectForwardingAddress'):select.ip+':'+select.port}}</span>

    <span style="
          position:absolute;display: inline-block;margin-top: -10px;
          font-weight: 700;left: 155px;"
          @click.stop="changeVis()">▾</span>
    </div>

    <transition name="fade">
      <div v-show="openSetting" style="height:auto;background: #40ccf9">
        <div v-for="(item,key) in langs" :key="key">
          <div class="menu_item" style="height: 30px;width: 175px;
                      margin-top: 2px;
                      box-shadow: 0 0 5px 5px rgba(15, 21, 47,0.1) inset;">
            <span style="margin-left: 10px;
                         position:absolute;margin-top:-5px;
                         font-size: 16px;color: black;padding-top: -20px"
                         @click.stop="handleCommand(item)">
                 {{item.ip}}:{{item.port}}</span>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
  import {http_request, request_tgturn_list,create_tgturn_list} from "../modes/api";
  import {mapActions, mapState} from "vuex";
  export default {
    name: "Language",
    props: {
      SeDisabled: {
        type: Boolean,
        default: true
      },
    },
    data() {
      return {
        openSetting:false,
        langs: [],
        select:{
          ip: "",
          port: 0,
          remark: "",
          id: 0
        }
      };
    },
    methods: {
      ...mapActions(["set_target_turn"]),
      handleCommand(info)
      {
        this.select = info;
        this.openSetting=false;
        this.set_target_turn({ip:info.ip,port:info.port});
      },
      changeVis()
      {
        this.openSetting=!this.openSetting;
      }
    },
    mounted() {
      http_request(request_tgturn_list, '', data =>
      {
        data.forEach(info =>
        {
          this.langs.push(info);
        })
        if(data.length>0){
          this.select = data[0];
          this.set_target_turn({ip:data[0].ip,port:data[0].port});
        }
      });
    }

  };
</script>

<style scoped>
  .Select_{
    background-color: rgba(15, 21, 47, 0.9);
    box-shadow: 0 0 3px 3px rgba(64,204,249,0.4) inset;
    border-radius: 5px;
    width: 175px;height: 24px;
    cursor: pointer
  }

  .menu_item :hover {
    color: white !important;
  }
  .el-dropdown-link span{
    display: block;
    float: left;
  }
  .el-dropdown-link img{
    display: block;
    float: left;
  }
  .langBox{
    cursor: pointer;
    z-index:40;
    color: #40cef9;
  }
  .langBox_dis{
    cursor: not-allowed;
    z-index:40;
    color: rgba(64, 206, 249,0.6);
  }
  .el-dd{
    width: 80px;
    color: #40cef9;
  }
</style>
