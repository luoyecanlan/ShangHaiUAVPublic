<template>
  <transition name="fade">
    <div class="loading">
      <div class="load-box">
        <p class="load-icon"><i class="el-icon-loading"></i></p>
        <p class="load-title">{{$t('m.loading')}}</p>
      </div>
    </div>
  </transition>
</template>

<script>
    export default {
        name: "Loading"
    }
</script>

<style scoped>
.loading{
  background-color: rgba(0,0,0,.5);
  position: fixed;
  width: 100%;
  height: 100%;
  top: 0px;
  left: 0px;
  z-index: 2000;
}
  .load-box{
    position: absolute;
    left: 50%;
    top: 36%;
    transform: translateX(-50%);
    color: #ffffff;
  }
  .load-title{
    line-height: 24px;
    font-size: 14px;
  }
  .load-icon{
    padding-right:10px;
    text-align: center;
  }
  .el-icon-loading{
    font-size: 30px;
  }
</style>
