<template>
  <transition name="fade">
    <div class="deviceBox">
      <div class="device-info-box">
        <div class="device-pic"
             :style="{border:statusDevColor}">
          <img :src="deviceHeader" alt="">
        </div>
        <div class="device-info-box__">
          <div>
            <img :src="statusIcon" style="vertical-align: -10%" alt="">
            <span class="bold-font__ device-title__"
                  :style="{color:statusSapnColor}"
                  style="font-size: 22px;">{{device.name}}</span>
          </div>
          <div class="device-info-item__">ID {{device.id}} ({{category_name}})</div>
          <div>
            <img src="../../assets/device/position.png" alt="">
            <span class="device-info-item__">{{device.lng.toFixed(6)}}°,{{device.lat.toFixed(6)}}°</span>
          </div>
          <div>
            <img src="../../assets/device/alt.png" alt="">
            <span class="device-info-item__">A{{device.alt.toFixed(2)}}m</span>
          </div>
        </div>
      </div>
      <div class="device-option-box">
        <div class="device-status-box">
          <span class="img-box__" :active="deviceState.isGuidance">
            <img src="../../assets/home/yd.png" alt="">
          </span>
          <span class="img-box__" :active="deviceState.isBeGuidance">
            <img src="../../assets/home/byd.png" alt="">
          </span>
          <span class="img-box__" :active="deviceState.isTurnTarget">
            <img src="../../assets/home/zf.png" alt="">
          </span>
        </div>

        <div class="device-switch">
          <s-s-w-b :device="device.id"
                   :active="deviceState.code == 1 || deviceState.code == 2 || deviceState.code == 3? true:false"/>
          <!--<switch-button @onChange="serverSwitch"-->
                         <!--:active="deviceState.code == 1 || deviceState.code == 2 || deviceState.code == 3? true:false"/>-->
        </div>

        <div class="device-switch">
          <s-w-b :device="device.id"
          :active="deviceState.code== 2 ? true:false"
          yes-string="Work" no-string="Stop"/>
          <!--<switch-button @onChange="deviceSwitch"-->
                         <!--:active="deviceState.code== 2 ? true:false"-->
                         <!--yes-string="Work" no-string="Stop"/>-->
        </div>

      </div>
      <div class="device-status-switch">
        <div class="button-switch" @click.prevent="isOpen=!isOpen" :class="isOpen?'is-open':''"></div>
      </div>
      <div class="device-config-box" v-show="!isOpen">
        <p><span title>{{$t('m.device')}}：</span><span>{{device.ip||'0.0.0.0'}}:{{device.port||'0000'}}</span></p>
        <p><span title>{{$t('m.sevice')}}：</span><span>{{device.lip||'0.0.0.0'}}:{{device.lport||'0000'}}</span></p>
        <p style="margin-top: 10px;display: flex;flex-direction: row">
          <span title style="width: 135px;">{{$t('m.timeout')}}：</span>
          <span style="width: 30px;">{{device.targetTimeOut}}″</span>
          <span title style="width: 135px;">{{$t('m.targetReport')}}：</span>
          <span style="width: 10px;">{{device.probeReportingInterval}}″</span>
        </p>
        <p style="display: flex;flex-direction: row">
          <span title style="width: 135px;">{{$t('m.statusReport')}}：</span>
          <span style="width: 30px;">{{device.statusReportingInterval}}″</span>
          <span title style="width: 135px;">{{$t('m.threatDetermin')}}：</span>
          <span style="width: 10px;">{{device.threadAssessmentCount}}″</span>
        </p>
        <div class="device-config-bottom-box">
          <span title style="line-height: 64px;">{{$t('m.rectify')}}</span>
          <div border-not-right></div>
          <div>
            <p ><img style="vertical-align: -5px" src="../../assets/device/el.png" alt="">
              <span style="padding-left: 4px;">{{device.rectifyAz}}°</span></p>
            <p style="margin-top: 12px;"><img style="vertical-align: -5px" src="../../assets/device/az.png" alt="">
              <span style="padding-left: 4px;">{{device.rectifyEl}}°</span></p>
          </div>
          <div class="device-cover-box">
            <p cover>{{device.coverS}}°-{{device.coverE}}°</p>
            <p alt>R:{{device.coverR>=1000?`${(device.coverR/1000).toFixed(2)}km`:`${device.coverR}m`}}</p>
          </div>
        </div>
      </div>
      <div class="device-work-info" v-show="isOpen">
        <div class="work-rt-info-box" v-if="deviceState.runInfo">
          <div class="az-box_">{{deviceState.runInfo.currentAz}}°</div>
          <div class="el-box_">{{deviceState.runInfo.currentEl}}°</div>
        </div>
        <div class="work-error-box">
          <img src="../../assets/home/error-icon.png" alt="">
          <p>

          </p>
        </div>
      </div>
    </div>
  </transition>
</template>
<script>
  import {
    msg_enum,
    show_message,
    open_notification
  } from "../../modes/elementUI";
  import SwitchButton from '../../components/SwitchButton'
  import SSWB from '../../components/SSWSwitchButton'
  import SWB from '../../components/SWSwitchButton'
  import {mapState} from 'vuex'
  import {http_request,device_server_switch,device_power_switch} from "../../modes/api";
  import {DEVSTATE_OFFLINE , DEVSTATE_WORK ,
    DEVSTATE_ONLINE ,  DEVSTATE_ALARM ,
    DEVSTATE_OFFLINE_BorderColor ,
    DEVSTATE_ONLINE_BorderColor ,
    DEVSTATE_WORK_BorderColor ,
    DEVSTATE_ALARM_BorderColor ,} from '../../../static/css/GeneralStyle'

  export default {
    name: "DeviceItem",
    data() {
      return {
        isOpen: false,
        deviceStateCodeSSW: false,
        deviceStateCodeSW: false,
      }
    },
    props:{
      device:{
        required:true
      }
    },
    computed:{
      ...mapState(['device_status','device_categories']),
      category_name() {
        if (this.device_categories) {
          let mode = this.device_categories.find(f => f.id === this.device.category);
          return mode ? mode.name : this.$t('m.unknown');
        }
        return this.$t('m.unknown');
      },
       deviceHeader() {
        let category = this.device.category
        let src = '';

        if(category<=20000)
        {
          if(category>10200&&category<10400)
          {
            src = require('../../../static/res/pic/10200.png');
          }
          else if(category>10400) {
            src = require('../../../static/res/pic/10400.png');
          }
          else{
            src = require('../../../static/res/pic/10100.png');
          }
        }

        else if(category>20000 && category<=30000)
        {
          src = require('../../../static/res/pic/20100.png');
        }
        else if(category>30000 && category<=30500)
        {
          src = require('../../../static/res/pic/30100.png');
        }
        else if(category>30500)
        {
          src = require('../../../static/res/pic/30500.png');
        }

        return src;
      },
      deviceState(){
        if(this.device_status){
          return !!this.device_status[this.device.id]?this.device_status[this.device.id]:{};
        }
        return {};
      },
      statusDevColor() {
        let color ='';
        let _state='';
        if(this.device_status){
          _state = !!this.device_status[this.device.id]?this.device_status[this.device.id]:{};
        }
        switch (_state.code) {
          case 0:
            color = DEVSTATE_OFFLINE_BorderColor
            break;
          case 1:
            color = DEVSTATE_ONLINE_BorderColor
            break;
          case 2:
            color =DEVSTATE_WORK_BorderColor
            break;
          case 3:
            color = DEVSTATE_ALARM_BorderColor
            break;
          default:
            color = DEVSTATE_OFFLINE_BorderColor
            break;
        }
        return color;
      },
      statusSapnColor() {
        let color ='';
        let _state='';
        if(this.device_status){
          _state = !!this.device_status[this.device.id]?this.device_status[this.device.id]:{};
        }
        switch (_state.code) {
          case 0:
            color = DEVSTATE_OFFLINE;
            break;
          case 1:
            color = DEVSTATE_ONLINE;
            break;
          case 2:
            color = DEVSTATE_WORK;
            break;
          case 3:
            color = DEVSTATE_ALARM;
            break;
          default:
            color = DEVSTATE_OFFLINE;
            break;
        }
        return color;
      },
      statusIcon() {
        let _icon='0';
        if (this.device_status) {
          let _state = this.device_status[this.device.id];
          if (_state) {
            //deviceState 0:离线;1:待机;2:工作;
            const {code} = _state;
            _icon= code;
          }
        }
        try {
          return require('../../assets/home/status/'+_icon+'.png');
        }catch (e) {
          return require('../../assets/home/status/0.png');
        }
      }
    },
    methods: {
      serverSwitch(val) {
        http_request(device_server_switch,{id:this.device.id,state:val},data=>
        {
          if(val){
            show_message(msg_enum.success, '开启成功');
          }else{
            show_message(msg_enum.success, '关闭成功');
          }
        });
      },
      deviceSwitch(val) {
        http_request(device_power_switch,{id:this.device.id,open:val},data=>
        {
          if(val){
            show_message(msg_enum.success, '开启成功');
          }else{
            show_message(msg_enum.success, '关闭成功');
          }
        });
      }
    },
    components:{
      SwitchButton,
      SSWB,
      SWB
    }
  }
</script>

<style scoped>
  .deviceBox{
    display: flex;
    height: 100%;
    width: 100%;
    position: relative;
    flex-direction: column;
  }
  /*!*边框样式*!*/
  /*.item-box{*/
  /*  height: 100%;*/
  /*  width: 100%;*/
  /*  position: relative;*/
  /*  border: 1px solid rgba(64,204,249,0.2);*/
  /*  box-shadow: 0 0 5px 3px rgba(64,204,249,0.2);*/
  /*  -webkit-box-shadow: 0 0 5px 3px rgba(64,204,249,0.2);*/
  /*  padding: 16px;*/
  /*  display: flex;*/
  /*  flex-direction: column;*/
  /*}*/
  /*div[lt],div[lb],div[rb],div[rt]{*/
  /*  position: absolute;*/
  /*  height: 32px;*/
  /*}*/
  /*div[lt]{*/
  /*  left: -11px;*/
  /*  top: -10px;*/
  /*  width: 33px;*/
  /*  background-image: url("../../assets/home/lt.png");*/
  /*}*/
  /*div[lb]{*/
  /*  left: -11px;*/
  /*  bottom: -11px;*/
  /*  width: 33px;*/
  /*  background-image: url("../../assets/home/lb.png");*/
  /*}*/
  /*div[rt]{*/
  /*  right: -10px;*/
  /*  top: -10px;*/
  /*  width: 32px;*/
  /*  background-image: url("../../assets/home/rt.png");}*/
  /*div[rb]{*/
  /*  right: -10px;*/
  /*  bottom: -11px;*/
  /*  width: 32px;*/
  /*  background-image: url("../../assets/home/rb.png");*/
  /*}*/
  /*基本信息样式*/
  .device-info-box{
    position: relative;
    display: flex;
    flex-direction: row;
    height: 100px;
  }
  .device-pic{
    width: 100px;
    height: 100px;
    border-radius: 50%;
    position: relative;
    border: 2px solid #40cef9;
    margin-right: 20px;
  }
  .device-pic img{
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    margin: auto;
  }
  /*操作按钮样式*/
  .device-option-box{
    height: 60px;
    display: flex;
    flex-direction: row;
  }
  .device-status-box{
    margin: 20px 13px;
    width: 75px;
    height: 20px;
    border: 1px solid rgba(64,204,249,0.5);
    border-radius: 2px;
    background-color:#12213b;
    display: flex;
    flex-direction: row;
    position: relative;
  }
  .device-status-box:before{
    content: "";
    height: 8px;
    width: 8px;
    /*background-color: #ff4d51;*/
    border-left: 1px solid rgba(64,204,249,0.5);
    border-top: 1px solid rgba(64,204,249,0.5);
    box-sizing: border-box;
    background-color: #12213b;
    top: -4px;
    left: 50%;
    position: absolute;
    transform-origin: center center;
    transform: translateX(-50%) rotate(45deg);
  }
  .img-box__{
    flex: 1;
    position: relative;
  }
  .img-box__ img{
    position: absolute;
    top: 50%;left: 50%;
    opacity: 0.5;
    transform: translate(-50%,-50%);
  }
  .img-box__[active='true'] img{
    opacity: 1;
    transition: all 0.5s;
  }

  /*显示隐藏设备运行参数*/
  .device-switch{
    flex: 1;
    height: 20px;
    margin: 20px 0;
    margin-left: 20px;
    border-radius: 2px;
    /*border: 1px solid #40cef9;*/
  }
  .device-switch:last-child{
    margin-right: 10px;
  }
  .device-status-switch:hover{
    opacity: 1;
    transition: all 0.5s;
  }
  .device-status-switch{
    height: 26px;
    opacity: 0.8;
    cursor: pointer;
    position: relative;
  }
  .device-status-switch:before{
    height: 4px;
    content: '';
    margin-top: 11px;
    width: 100%;
    background: linear-gradient(to right,transparent,#12213b,transparent);
    position: absolute;
  }
  .button-switch{
    height: 24px;
    width: 24px;
    background: #12213b;
    margin: 0 auto;
    position: relative;
    transform: rotate(45deg);
  }
  .is-open.button-switch:before{
    content: '';
    height: 6px;
    width: 6px;
    right: 0px;
    bottom: 0px;
    border: 1px solid transparent;
    border-right-color: #40cef9;
    border-bottom-color: #40cef9;
    position: absolute;
  }
  .button-switch:before{
    content: '';
    height: 6px;
    width: 6px;
    border: 1px solid transparent;
    border-left-color: #40cef9;
    border-top-color: #40cef9;
    position: absolute;
    margin: 6px;
  }
  .button-switch:after{
    content: '';
    height: 4px;
    width: 4px;
    border-radius: 50%;
    background-color: #40cef9;
    position: absolute;
    margin: 10px;
  }

  /*设备参数详情样式*/
  .device-config-box,.device-work-info{
    flex: 1;
    margin: 5px;
    border-radius: 3px;
  }
  .device-config-box{
    background-color: rgba(64,204,249,0.1);
    padding: 10px;
    transition: all 0.5s;
  }
  .device-config-box span{
    font-size: 14px;
    color: rgba(64,204,249,0.5);
    line-height: 20px;
    letter-spacing: 1px;
  }
  .device-config-box span[title]{
    font-size: 16px;
    color: #40cef9;
  }
  .device-config-bottom-box{
    height: 64px;
    margin-top: 10px;
    display: flex;
    flex-direction: row;
  }
  div[border-not-right]{
    height: 42px;
    width: 8px;
    border: 1px solid #40cef9;
    border-right-color: transparent;
    margin: auto 5px;
  }
  .device-cover-box{
    flex: 1;
    margin-left: 30px;
    background-image: url("../../assets/device/cover.png");
    background-repeat: no-repeat;
    background-position: left center;
    position: relative;
  }
  .device-cover-box p[cover],.device-cover-box p[alt]{
    position: absolute;
  }
  .device-cover-box p[cover]{
    color: #40cef9;
    top: 6px;
    left: 66.66%;
    transform: translateX(-50%);
  }
  .device-cover-box p[alt]{
    color: rgba(64,204,249,0.5);
    bottom: 16px;
    left: 66.66%;
    transform: translateX(-50%);
  }
  /*设备运行状态样式*/
  .device-work-info{
    display: flex;
    flex-direction: column;
    position: relative;
    transition: all 0.5s;
  }
  .work-rt-info-box{
    height: 50px;
    background-color: rgba(64,204,249,0.2);
    padding: 5px;
    position: relative;
    display: flex;
    flex-direction: row;
  }
  .work-rt-info-box:before{
    content: "";
    width: 1px;
    height: 100%;
    position: absolute;
    left: 50%;
    top: 0;
    background: linear-gradient(transparent,rgba(64,204,249,0.4),transparent);
  }
  .az-box_,.el-box_{
    flex: 1;
    text-align: center;
    line-height: 40px;
    color: #40cef9;
    background-repeat: no-repeat;
    background-position: 5px 0px;
  }
  .az-box_{
    background-image: url("../../assets/home/az.png");
  }
  .el-box_{
    background-position: 10px 3px;
    background-image: url("../../assets/home/el.png");
  }
  .work-error-box{
    border-radius: 3px;
    flex: 1;
    background-color: rgba(64,204,249,0.1);
    margin-top: 15px;
    color: #40cef9;
    overflow-y: auto;
    padding: 10px 15px;
  }
  .work-error-box p{
    white-space: normal;
    line-height: 22px;
    display: block;
    width: 100%;
    margin-top: 10px;
    height: 80px;
    overflow-x: hidden;
    overflow-y: auto;
  }
</style>
