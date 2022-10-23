<template>
  <div class="header" v-if="isShow">
    <router-link to="/" class="hot-area">
      <img src="../assets/main/logo.png"  class="header-logo">
    </router-link>
    <div class="menu-box">
      <w-menu/>
    </div>
    <div class="info-box">
      <w-login-info/>
    </div>
  </div>
</template>

<script>
  import LoginInfo from './LoginInfo'
  import Menu from './Menu'
  import DeviceList from '../map/DeviceList'
  export default {
    name: "w_Header",
    components: {
      wMenu: Menu,
      wLoginInfo: LoginInfo,
      //DeviceList
    },
    computed:{
      isShow(){
        if(this.$route.meta)
        {
          return !!!this.$route.meta.hideMenu;
        }
        return true;
      }
    }
  }
</script>

<style scoped>
  .header{
    height: 79px;
    background-image: url("../assets/main/logo-back.png");
    background-repeat: no-repeat;
  }
  .header-logo{
    position: absolute;
    top: 5px;
    left: 270px;
  }
  .hot-area{
    width:600px ;
    height: 129px;
    z-index: 10;
  }
  .menu-box{
    position: absolute;
    top: 33px;
    left:500px ;
  }
  .info-box{
    position: absolute;
    right: 190px;
    top: 4px;
  }
</style>
