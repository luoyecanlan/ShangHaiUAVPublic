<template>
  <div class="item-box">
    <div lt></div>
    <div lb></div>
    <div rt></div>
    <div rb></div>
    <slot></slot>
  </div>
</template>

<script>
    export default {
        name: "DeviceBox"
    }
</script>

<style scoped>
  /*边框样式*/
  .item-box{
    height: 100%;
    width: 100%;
    position: relative;
    border: 1px solid rgba(64,204,249,0.2);
    box-shadow: 0 0 5px 3px rgba(64,204,249,0.2);
    -webkit-box-shadow: 0 0 5px 3px rgba(64,204,249,0.2);
    padding: 16px;
    display: flex;
    flex-direction: column;
  }
  div[lt],div[lb],div[rb],div[rt]{
    position: absolute;
    height: 32px;
  }
  div[lt]{
    left: -11px;
    top: -10px;
    width: 33px;
    background-image: url("../assets/home/lt.png");
  }
  div[lb]{
    left: -11px;
    bottom: -11px;
    width: 33px;
    background-image: url("../assets/home/lb.png");
  }
  div[rt]{
    right: -10px;
    top: -10px;
    width: 32px;
    background-image: url("../assets/home/rt.png");}
  div[rb]{
    right: -10px;
    bottom: -11px;
    width: 32px;
    background-image: url("../assets/home/rb.png");
  }
</style>
