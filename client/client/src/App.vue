<template>
  <div id="app" v-loading="isLoading" style="overflow-x: hidden">
    <w-header class="flex-header" />
    <!--<language/>-->
    <div class="flex-main">
      <router-view />
    </div>
    <w-footer class="flex-footer"/>
  </div>
</template>

<script>
  import Header from "./components/Header";
  import Footer from "./components/Footer";
  import Language from './components/Language'
  import { event_type } from "./modes/tool/bus";
  import bus from "./modes/tool/bus";
  import {mapActions} from 'vuex'
  import 'ol/ol.css'

  export default {
    name: "App",
    data() {
      return {
        isLoading: false
      };
    },
    methods:{
      ...mapActions(['request_device_categories'])
    },
    mounted() {
      this.request_device_categories();
      bus.$off(event_type.loading);
      bus.$on(event_type.loading, state => {
        this.isLoading = state
      });
    },
    components: {
      wHeader: Header,
      wFooter: Footer,
      Language
    }
  };
</script>

<style>
  #app {
    position: absolute;
    height: 100%;
    width: 100%;
    background-color: #0F152F;
    display: flex;
    flex-direction: column;
  }
  .flex-header {
    height: 70px;
  }
  .flex-main {
    flex: 1;
    margin: 0px 0px 0px 0px;
    position: relative;
  }
  .flex-footer {
    height: 0px;
  }
  .loading {
    background-color: rgba(0, 0, 0, 0.8);
    position: absolute;
    top: 0px;
    height: 100%;
    width: 100%;
  }
  .loading-content {
    margin-left: 50%;
    transform-origin: center;
    transform: translateX(-50%);
    margin-top: 360px;
  }
  .loading-enter-active,
  .loading-leave-active {
    transition: opacity 0.2s;
  }
  .loading-enter, .loading-leave-to /* .fade-leave-active below version 2.1.8 */
  {
    opacity: 0;
  }
</style>
