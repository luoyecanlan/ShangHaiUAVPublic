<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" style="width: 150px;" is-title>
        <img src="../../assets/table/title-arrow.png" alt=""> {{$t('m.set')}}
      </span>
    </div>
    <div class="list-container" style="padding-left:40px;padding-right:40px;padding-bottom: 16px;">
      <div class="tb-border-lt"></div>
      <div class="tb-border-rt"></div>
      <div class="tb-border-rb" style="bottom: 30px;"></div>
      <div class="tb-border-lb" style="bottom: 30px;"></div>
      <div class="config-middle-line"><p></p></div>
      <div class="config-box">
        <turn-address style="flex: 48;height: 100%;"/>
        <div class="map-rtsp-box">
          <map-config/>
          <rtsp-config/>
        </div>
      </div>

    </div>
  </div>
</template>

<script>
  import TurnAddress from './TurnAddress'
  import MapConfig from './MapConfig'
  import RtspConfig from './RtspConfig'
  export default {
    name: "SystemConfig",
    components: {
      TurnAddress,
      MapConfig,
      RtspConfig
    }
  }
</script>

<style scoped>
  .config-box{
    margin: -10px;
    display: flex;
    flex-direction: row;
    position: relative;
    height: 100%;
  }
  .config-box:before{
    position: absolute;
    content: "";
    height: 100%;
    width: 1px;
    background-color: rgba(64,204,249,0.3);
    left: 48%;
    top: 0;
    transform: translateX(-1px);
    z-index: 99;
  }
  .map-rtsp-box{
    flex: 52;
    display: flex;
    margin-left: 40px;
    flex-direction: column;
  }
  .config-middle-line{
    width: 34px;
    height: 100%;
    top: 16px;
    /*background-color: #40cef9;*/
    position: absolute;
    left: 48%;
    margin-top: 0px;
    margin-left: -17px;
  }
  /*.config-middle-line p{*/
  /*  width: 1px;*/
  /*  position: relative;*/
  /*  height: 100%;*/
  /*  background-color: #40cef9;*/
  /*  margin: 0px auto;*/
  /*}*/
  .config-middle-line:before,.config-middle-line:after{
    content: "";
    height: 18px;
    width: 100%;
    position: absolute;
  }
  .config-middle-line:before{
    background-image: url("../../assets/home/set-top.png");
    top: 4px;
  }
  .config-middle-line:after{
    background-image: url("../../assets/home/set-bottom.png");
    bottom: -7px;
  }
</style>
