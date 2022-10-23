<template>
    <div class="build">
      <div style="text-align: center"><i class="iconfont icon-build"></i></div>
      <div class="build-text">网站建设中...</div>
    </div>
</template>

<script>
    export default {
        name: "Building"
    }
</script>

<style scoped>
  .build{
    position: absolute;
    top: 36%;
    left: 50%;
    transform: translateX(-50%);
  }
  .icon-build{
    color: #40cef9;
    font-size: 8em;
    opacity: 0.2;
  }
  .build-text{
    line-height: 72px;
    font-size: 24px;
    color: #40cef9;
    opacity: 0.2;
  }
</style>
