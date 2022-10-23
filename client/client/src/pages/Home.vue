<template>
    <div class="home">
      <w-map/>
    </div>
</template>

<script>
  import Map from '../map/Index'

    export default {
      name: "Home",
      data() {
        return {
          // randomImg:''
        }
      },
      components: {
        wMap: Map
      },
      mounted() {
        // setInterval(()=>{
        //   this.randomImg=Mock.Random.dataImage('44x44',Mock.mock('@cfirst'));
        // },1000);
      }
    }
</script>

<style scoped>
  .home{
    position: absolute;
    height: 100%;
    width: 100%;
  }
</style>
