<template>
  <div class="legend-box">
    <div class="legend-box">
      <div style="width: 165px;">
        <!-- <div class="itemRow" style="margin-top: 5px">
          <img src="../assets/sign/target.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.UAV')}}</span>
        </div> -->
        <div class="itemRow">
          <img src="../assets/sign/1levelSign.png" class="itemImg" alt="">
          <span class="itemText">红色告警</span>
        </div>
        <!--<div class="itemRow">-->
          <!--<img src="../assets/sign/2levelSign.png" class="itemImg" alt="">-->
          <!--<span class="itemText">{{$t('m.twoLevelAlarm_')}}</span>-->
        <!--</div>-->
        <!--<div class="itemRow">-->
          <!--<img src="../assets/sign/3levelSign.png" class="itemImg" alt="">-->
          <!--<span class="itemText">{{$t('m.threeLevelAlarm_')}}</span>-->
        <!--</div>-->
        <div class="itemRow">
          <img src="../assets/sign/0levelSign.png" class="itemImg" alt="">
          <span class="itemText">蓝色告警</span>
        </div>
        <div class="itemRow">
          <img src="../assets/sign/00levelSign.png" class="itemImg" alt="">
          <span class="itemText">白名单</span>
        </div>
        <!-- <div class="itemRow">
          <img src="../assets/sign/outlevelSign.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.extLevelAlarm_')}}</span>
        </div> -->
        <!--<div class="itemRow">
          <img src="../assets/sign/decoyPoint.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.decoyPoint')}}</span>
        </div>-->
      </div>

      <!-- <div style="width: 180px;">
        <!--<div class="itemRow" style="margin-top: 5px">
          <img src="../assets/sign/radar.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.radar')}}</span>
        </div>-->
        <!-- <div class="itemRow" style="margin-top: 5px">
          <img src="../assets/sign/photoelectricity.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.photoelectricDev')}}</span>
        </div>
        <div class="itemRow">
          <img src="../assets/sign/jam.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.interfere')}}</span>
        </div>
        <div class="itemRow">
          <img src="../assets/sign/tempMarker.png" class="itemImg"
               style="margin-left: 14px;margin-top: 6px"  alt="">
          <span class="itemText" style="top: -6px;">{{$t('m.tempPoint')}}</span>
        </div> -->
<!--        <div class="itemRow">
          <img src="../assets/sign/spectrum.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.spectrum')}}</span>
        </div>
        <div class="itemRow">
          <img src="../assets/sign/bird.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.bird')}}</span>
        </div>
        <div class="itemRow">
          <img src="../assets/sign/unknow.png" class="itemImg" alt="">
          <span class="itemText">{{$t('m.unknownTarget')}}</span>
        </div>-->

      <!-- </div>  -->
    </div>
  </div>
</template>

<script>
    export default {
        name: "Legend",
        data() {
            return {};
        }
    };
</script>

<style scoped>

  .itemImg{
    margin-left: 16px;
    margin-top: 10px
  }
  .itemText {
    font-family: "Microsoft YaHei UI Light";
    font-size: 13px;
    color: #43DCFF;
    position: relative;
    top: -3px;
    cursor: default;
  }

  .itemRow {
    height: 37px;
  }

  .legend-box {
    display: flex;
    flex-direction: row;
    width: 130px;
    height: 120px;
    background-color: rgba(15, 21, 47, 0.5);
    border: 1px solid #43DCFF;
    z-index: 10;
  }

</style>
