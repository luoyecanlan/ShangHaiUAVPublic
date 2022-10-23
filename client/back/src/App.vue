<template>
  <div id="app" v-loading="isLoading">
    <w-header class="flex-header" />
    <language/>
    <div class="flex-main" :class="{'flex-login':$route.name==='login'}">
      <router-view />
    </div>
    <w-footer class="flex-footer" />
  </div>
</template>

<script>
import Header from "./components/Header";
import Footer from "./components/Footer";
import { event_type } from "./modes/tool/bus";
import Language from './components/Language';
import bus from "./modes/tool/bus";

export default {
  name: "App",
  data() {
    return {
      isLoading: false
    };
  },
  mounted() {
    bus.$off(event_type.loading);
    bus.$on(event_type.loading, state => {
      this.isLoading = state;
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
  background: #0f152f;
  display: flex;
  flex-direction: column;
}
.flex-header {
  height: 79px;
}
.flex-main {
  flex: 1;
  margin: 5px 100px 20px;
}
.flex-login{
  margin: 0px;
}
.flex-footer {
  height: 32px;
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
