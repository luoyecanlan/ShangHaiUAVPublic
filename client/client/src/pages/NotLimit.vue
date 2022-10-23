<template>
  <div class="page-box">
    <div class="center-box">
      <div>
        <i class="iconfont icon-limit"></i>
      </div>
      <p style="line-height: 60px;text-align: center;color: #666666;">
        <span style="font-size: 24px">Sorry,</span>
        <span style="font-size: 16px">{{$t('m.noPermissionsExist')}}！</span>
      </p>
      <router-link to="/login">
        <p style="line-height: 44px;text-align: center;color: #1DA57A;">>> {{$t('m.reLogin')}} <<</p>
      </router-link>
    </div>
  </div>
</template>

<script>
  export default {
    name: "Limit"
  }
</script>

<style scoped>
  .iconfont{
    font-size: 160px;
    color: #1DA57A;
    text-align: center;
  }
  .center-box{
    position: absolute;
    margin: 0 auto;
    left: 50%;
    top: 25%;
    transform: translateX(-50%);
  }
</style>
