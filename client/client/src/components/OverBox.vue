<template>
  <div class="dialog-box__" :style="positionStyle">
<!--    <div class="dialog-lt"></div>-->
<!--    <div class="dialog-lb"></div>-->
<!--    <div class="dialog-rt"></div>-->
<!--    <div class="dialog-rb"></div>-->

<!--    <div class="__dialog-box-header" v-if="title">-->
<!--      <span class="title">{{title}}</span>-->
<!--    </div>-->
    <div class="__dialog-container">
      <div class="__dialog-scroll-box">
        <slot name="content"></slot>
      </div>
    </div>

  </div>
</template>

<script>
    export default {
      name: "OverBox",
      props: ['positionStyle','title']
    }
</script>

<style scoped>
  .dialog-box__{
    position: absolute;
    top: 40px;
    margin-left: -220px;
    display: flex;
    flex-direction: column;
    width: 370px;
    border: 1px solid #40CEF9;
    background-color:#0F152F;
    z-index: 99;
    box-shadow: 0 0 5px 5px rgba(64,204,249,0.4);
  }

  .device-info-box__ .device-info-item__{
    font-size: 14px;
    line-height: 22px;
    letter-spacing: 0.5px;
  }

  /*.dialog-lt,.dialog-lb,.dialog-rb,.dialog-rt{*/
  /*  height: 72px;*/
  /*  width: 60px;*/
  /*  position: absolute;*/
  /*  border: 3px solid #40CEF9;*/
  /*  margin: -2px;*/
  /*}*/
  /*.dialog-lt{*/
  /*  border-width: 3px 0px 0px 3px;*/
  /*}*/
  /*.dialog-lb{*/
  /*  border-width: 0px 0px 3px 3px;*/
  /*  bottom: 0px;*/
  /*}*/
  /*.dialog-rt{*/
  /*  border-width: 3px 3px 0px 0px;*/
  /*  right: 0px;*/
  /*}*/
  /*.dialog-rb{*/
  /*  border-width: 0px 3px 3px 0px;*/
  /*  right: 0px;*/
  /*  bottom: 0px;*/
  /*}*/
  .__dialog-box-header{
    height: 140px;
    line-height: 140px;
    box-shadow-bottom: 0 0 10px 10px rgba(64,204,249,0.2) inset;
  }
  .__dialog-box-header:after{
    content: "";
    height: 26px;
    width: 26px;
    position: absolute;
    top: 13px;
    left: 14px;
    background-image: url("../assets/table/title-arrow.png");
    background-repeat: no-repeat;
    background-position: center center;
  }
  .__dialog-box-header .title{
    font-size: 18px;
    font-weight: bold;
    margin-left: 44px;
  }
  .info-content__{
    padding-left: 16px;
    text-align: left;
  }
  .__dialog-container{
    flex: 1;
    position: relative;
  }
  .__dialog-scroll-box{
    position: absolute;
    width: 100%;
    height: 100%;
    overflow-y: auto;
  }
</style>
