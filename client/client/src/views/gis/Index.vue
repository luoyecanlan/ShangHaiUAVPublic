<template>
  <div class="gis-box">
    <div class="common-box">
      <common/>
    </div>
    <div class="correct-box">
      <correct/>
    </div>
  </div>
</template>

<script>
  import '../../assets/table/table.css'
  import '../../assets/gis/gis.css'
  import Correct from './Correct'
  import Common from './Common'
  export default {
    name: "Gis",
    components:{
      Common,
      Correct
    }
  }
</script>

<style scoped>
  .gis-box{
    position: relative;
    height: 100%;
    display: flex;
    flex-direction: column;
  }
  .common-box{
    flex: 55;
    margin-bottom: 36px;
  }
  .correct-box{
    flex: 45;
    max-height: 360px;
    min-height: 300px;
    display: flex;
    flex-direction: row;
    margin-bottom: 30px;
  }
</style>
