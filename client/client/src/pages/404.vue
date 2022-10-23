<template>
  <div class="notfound page-box">
    <div class="center-box">
      <div>
        <i class="iconfont icon-404"></i>
      </div>
      <router-link to="/home">
        <p style="line-height: 44px;text-align: center;color: #1DA57A;">
          >> {{$t('m.reLogin')}} <<
        </p>
      </router-link>
    </div>
  </div>
</template>

<script>
  export default {
    name: "404"
  };
</script>

<style scoped>
  .iconfont {
    font-size: 200px;
    color: #1da57a;
    text-align: center;
  }
  .center-box {
    position: absolute;
    margin: 0 auto;
    left: 50%;
    top: 25%;
    transform: translateX(-50%);
  }
</style>
