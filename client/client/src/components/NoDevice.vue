<template>
    <div class="noDataBox">
    </div>
</template>
<script>
    export default {
      name: "NoDevice",
      data(){
          return{
          }
      }
    }
</script>

<style scoped>
  .noDataBox{
    display: flex;
    height: 26%;
    width: 100%;
    position: relative;
  }
  .noDataBox:before{
    content: "";
    position: absolute;
    height: 420px;
    width: 100%;
    top: -24px;
    left: 0;
    background: url("../assets/home/nodevice.png");
    background-repeat: no-repeat;
    background-position: center center;
    background-size: 98%;
  }
</style>
