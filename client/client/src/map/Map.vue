<template>

    <div id="olMap" class="map"></div>


</template>

<script>
  import {Init_Map} from "./js";

  export default {
    mounted() {
       Init_Map("olMap");
    }
  }

</script>

<style scoped>
  .map {
    background-color: unset;
    height: 100%;
    width: 100%;
    z-index: auto;
  }
</style>
