<template>
  <div class="TargetInfoViewer" style="background: gray">
    <video class="h5video" style="width: 100%;height: 100%;" :id="videoId"></video>
    <div class="operlist">

      <span class="textBlue" style="position: absolute;left:15px;">{{$t('m.rectify')}}</span>
      <input class="input_H5" v-model="rectifyAZData"
             onkeyup="value=value.replace(/[^\d\.]/g,'')"
             style="left: 120px;width: 50px" placeholder="1~100"/>
      <input class="input_H5" v-model="rectifyELData"
             onkeyup="value=value.replace(/[^\d\.]/g,'')"
             style="left: 190px;width: 50px" placeholder="1~100"/>
      <button class="buttonBlue"
              @mousedown="SetRectify()"
              style="left:260px;width:70px"
      >{{$t('m.set')}}</button>

      <span class="textBlue" style="position: absolute;left:350px;">{{$t('m.shotChange')}}</span>
      <div class="switch-box"
           style="left: 470px;"
           :class="isCamera?'active':''"
           @click.stop="doChangeCamera()">
        <div class="switch-silder"></div>
        <span :class="isCamera?'black-color':''">{{$t('m.visibleLight')}}</span>
        <span :class="!isCamera?'black-color':''">{{$t('m.infraRed')}}</span>
      </div>

      <span class="textBlue" style="position: absolute;left:700px;">{{$t('m.track')}}</span>
      <div class="switch-box"
           style="left: 770px;"
           :class="isActive?'active':''"
           @click.stop="doSwitch">
        <div class="switch-silder"></div>
        <span :class="isActive?'black-color':''">{{'ON'}}</span>
        <span :class="!isActive?'black-color':''">{{'OFF'}}</span>
      </div>

      <button class="buttonBlue"
              @mousedown="doPTZ('zero')"
              style="left:1000px;"
              >{{$t('m.returnZero')}}</button>

      <button class="buttonBlue"
              @mousedown="doPTZ('toNorth')"
              style="left:1110px;"
              >{{$t('m.toNorth')}}</button>

      <span class="textBlue" style="position: absolute;left:1220px;">{{$t('m.turnSpeed')}}</span>
      <input class="input_H5" v-model="speed"
             onkeyup="value=value.replace(/[^\d\.]/g,'')"
             @change="TurntableChange"
             style="left: 1330px" placeholder="1~100"/>

      <span class="textBlue" style="position: absolute;left:1460px;">{{$t('m.turn')}}</span>

      <button class="directionBlue"
              @mousedown="doPTZ('up')"
              @mouseup="doStopPTZ(1)"
              style="left:1540px;top:5px;"
      >▲</button>

      <button class="directionBlue"
              @mousedown="doPTZ('left')"
              @mouseup="doStopPTZ(1)"
              style="left:1510px;top:32px;"
      >◄</button>

      <button class="directionBlue"
              @click="doStopPTZ(1)"
              style="font-weight: 900;
              font-size: 10px;
              left:1540px;top:32px;"
      >| |</button>

      <button class="directionBlue"
              @mousedown="doPTZ('right')"
              @mouseup="doStopPTZ(1)"
              style="left:1570px;top:32px;"
      >►</button>

      <button class="directionBlue"
              @mousedown="doPTZ('down')"
              @mouseup="doStopPTZ(1)"
              style="left:1540px;top:59px;"
      >▼</button>

      <span class="textBlue" style="position: absolute;left:1620px;">{{$t('m.view')}}</span>

      <button class="directionBlue"
              @mousedown="doPTZ('watchAdd')"
              @mouseup="doStopPTZ(5)"
              style="font-weight: 900;left:1675px;top:34px;"
      >✚</button>
      <!--<input class="input_H5" v-model="watchNum"-->
             <!--@change="TurntableChange"  readonly="true"-->
             <!--style="left: 1580px;height: 18px;top:34px;width: 64px"-->
             <!--placeholder="0.1~10.9"/>-->
      <button class="directionBlue"
              @mousedown="doPTZ('watchSub')"
              @mouseup="doStopPTZ(5)"
              style="font-weight: 900;left:1705px;top:34px;"
      >━</button>

      <span class="textBlue" style="position: absolute;left:1750px;">{{$t('m.zoom')}}</span>
      <button class="directionBlue"
              @mousedown="doPTZ('focalAdd')"
              style="font-weight: 900;left:1805px;top:34px;"
      >✚</button>

      <button class="directionBlue"
              @mousedown="doPTZ('focalSub')"
              style="font-weight: 900;left:1835px;top:34px;"
      >━</button>

    </div>
  </div>
</template>

<script>
  import {rtsp_login} from "../video/helper";
  import {H5sPlayerWS} from '../video/h5splayer'
  import Md5 from 'md5'
  import {http_request,
          request_Rectify,
          request_Follow,
          request_system_config,
          request_PTZ,
          http_request_await} from "../modes/api";
  import Vue from 'vue'

  export default {
    data() {
      return {
        rectifyAZData:0,
        rectifyELData:0,
        speed:10,
        watchNum:4.5,
        zoomNum:5.5,
        isCamera:false,
        isActive:false,
        h5handler: undefined,
        session: '',
        proto: 'WS',
        token: 'token2',//c39f 1b03
        videoId: 'token2', //d479
        RTSP_CONF: {
          username: 'admin',
          password: 'JZGPyyds2022@', //'12345',
          url: 'http://120.48.147.194:7070/', //'http://47.92.98.138:8080/',
          wshost: '' //"47.92.98.138:8080"
        }
      }
    },
    methods: {
      SetRectify(){
        //AZ 水平
        //EL 俯仰
        let _data = {
          lat: 0,
          lng: 0,
          alt: 0,
          rectifyAz: parseFloat(this.rectifyAZData),
          rectifyEl: parseFloat(this.rectifyELData)}

        http_request(request_Rectify,_data,data =>{});
      },
      TurntableChange(){
        if(this.speed>100)
        {
          this.speed = 100;
        }else if(this.speed<1){
          this.speed = 1;
        }
      },
      async playVideo() {
        //登录
        const {url, username, password} = this.RTSP_CONF;
        let api = `${url}api/v1/Login`;
        let result = await rtsp_login(api, {user: username, password: Md5(password)})
        if (result) {
          //登录成功
          this.session = result.strSession;
          this.doPlay();
        } else {
          //登录失败
          alert("视频服务异常，Code:LOGIN_001");
        }
      },
      doChangeCamera(){
        this.isCamera = !this.isCamera;

        // 改变 token
        if(this.isCamera)
        {
          // 可见光
          this.videoId = Vue.prototype.$visibleLight;
          this.token = Vue.prototype.$visibleLight;
        }
        else{
          // 红外
          this.videoId = Vue.prototype.$redLLight;
          this.token = Vue.prototype.$redLLight;
        }
        this.h5handler.disconnect();
        this.h5handler = undefined;

        this.playVideo();
      },
      doSwitch(){
        this.isActive=!this.isActive;
        let _data = {operateCode: 0};
        if(this.isActive)
        {
          _data.operateCode = 1;
        }else{
          _data.operateCode = 2;
        }
        http_request(request_Follow,_data,data =>{});
      },
      rectifyOper(){
        let _data={
          lat: 0,
          lng: 0,
          alt: 0,
          rectifyAz: 0,
          rectifyEl: 0
        }
        http_request(request_Rectify,_data,data => {});
      },
      doStopPTZ(data){
        let _data = {
          az: 0,
          el: 0,
          speed: 0,
          operateCode: 2,
          operateItem: data
        }
        http_request(request_PTZ,_data,data => {});
      },
      doPTZ(oper){
        let _data = {
          az: 0,
          el: 0,
          speed: 0,
          operateCode: 0,
          operateItem: 0
        }

        //operateCode 1开始 2结束
        //operateItem 1水平 2俯仰 3归零 4指北 5视场 6焦距
        switch (oper) {
          case 'up':
            _data.speed = parseInt(this.speed);
            _data.operateCode = 1;
            _data.operateItem = 2 ;
            break;
          case 'down':
            _data.speed = (parseInt(this.speed)-(2*parseInt(this.speed)));
            _data.operateCode = 1;
            _data.operateItem = 2;
            break;
          case 'left':
            _data.speed = (parseInt(this.speed)-(2*parseInt(this.speed)));
            _data.operateCode = 1;
            _data.operateItem = 1;
            break;
          case 'right':
            _data.speed = parseInt(this.speed);
            _data.operateCode = 1;
            _data.operateItem = 1 ;
            break;
          case 'zero':
            _data.operateItem = 3;
            break;
          case 'toNorth':
            _data.operateCode = 1;
            _data.operateItem = 4;
            break;
          case 'watchAdd':
            _data.speed = parseInt(this.speed);
            _data.operateCode = 1;
            _data.operateItem = 5;
            break;
          case 'watchSub':
            _data.speed = (parseInt(this.speed)-(2*parseInt(this.speed)));
            _data.operateCode = 1;
            _data.operateItem = 5;
            break;
          case 'focalAdd':
            _data.speed = parseInt(this.speed);
            _data.operateCode = 1;
            _data.operateItem = 6;
            break;
          case 'focalSub':
            _data.speed = (parseInt(this.speed)-(2*parseInt(this.speed)));
            _data.operateCode = 1;
            _data.operateItem = 6;
            break;
        }
        http_request(request_PTZ,_data,data =>{});
      },
      doPlay(){
        let {token,session,videoId,h5handler}=this;
        if(h5handler){
          h5handler.disconnect();
          // delete h5handler;
          h5handler=undefined;
        }
        let conf={
          videoid:videoId,
          protocol:window.location.protocol,
          host:this.RTSP_CONF.wshost,
          rootpath:'/',
          token:token,
          hlsver:'v1',
          session:session
        };
        this.h5handler = new H5sPlayerWS(conf);
        this.h5handler.connect();
      }
    },
    mounted()
    {
      http_request(request_system_config,null,_data =>
      {
        this.RTSP_CONF.url = _data[0].info + '/';
        this.RTSP_CONF.wshost = _data[0].info.slice(7);

        //默认加载红外
        this.videoId = Vue.prototype.$redLLight;
        this.token = Vue.prototype.$redLLight;

        this.playVideo();
      });
    }
  }
</script>

<style scoped>
  .input_H5{
    line-height: 26px;
    border:1px solid #43DCFF;
    background: unset;
    font-size: 14px;
    color: #43DCFF;
    width: 100px;
    height: 26px;
    position: absolute;
    top: 30px
  }
  .directionBlue{
    color: black;
    font-size: 16px;
    position: absolute;
    height: 22px;
    width: 25px;
    background: #49ddff;
  }
  .directionBlue:hover{
    background:#40ccf9;
    /*border:1px solid #FFFF00;*/
  }
  .directionBlue:active{
    background:#02A5D5;
  }
  .buttonBlue{
    color: black;
    font-size: 20px;
    position: absolute;
    height: 30px;
    top:30px;
    width: 100px;
    background: #49ddff;
  }
  .buttonBlue:hover{
    background:#40ccf9;
   /*border:1px solid #FFFF00;*/
  }
  .buttonBlue:active{
    background:#02A5D5;
  }
  .TargetInfoViewer{
    position:fixed;
    /*background-color: rgba(15, 21, 47, 0.4);*/
    /*box-shadow: 0px 0px 35px #43DCFF inset;*/
    /*border:1px solid #43DCFF;*/
    height: 950px;
    width: 1920px;
    overflow-y: hidden;
    top: 0;
    left: 0;
  }
  .switch-box{
    top: 30px;
    position: absolute;
    height: 31px;
    width: 210px;
    padding: 0px 2px;
    border-radius: 3px;
    border: 1px solid rgba(64,204,249,0.4);
    display: flex;
    flex-direction: row;
    cursor: pointer;
  }
  .switch-silder{
    position: absolute;
    height: 25px;
    width: 48%;
    border-radius: 2px;
    margin-top: 2px;
  }
  .switch-silder{
    /*right: 2px;*/
    left: 99%;
    transform: translateX(-100%);
    background: linear-gradient(#c7c7c7,#6f6f6f);
    transition: all 0.2s;
  }
  .operlist{
    width: 100%;
    background-color: rgba(15, 21, 47, 0.5);
    position: absolute;
    z-index:99;
    bottom: 0px;
    height: 85px
  }
  .active .switch-silder{
    left: 50%;
    background: linear-gradient(#8be9ff,#49ddff);
    transition: all 0.2s;
  }
  .textBlue{
    color: #49ddff;
    font-size: 20px;
    top:34px;
  }
  .switch-box span{
    line-height: 26px;
    text-align: center;
    font-size: 16px;
    flex: 1;
    color: rgba(64,204,249,0.4);
    z-index: 10;
  }
  span.black-color{
    color: #020e2c;
    transition: color 0.5s;
  }
</style>
