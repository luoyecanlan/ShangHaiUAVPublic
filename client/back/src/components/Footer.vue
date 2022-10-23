<template>
  <div class="footer" v-if="isShow">
    <span class="footer-font">上海寻航者智能科技有限公司 - ALL Rights Res</span>
    <span class="footer-font">Copyright ©2020</span>
  </div>
</template>

<script>
    export default {
      name: "Footer",
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

  /* 页脚版权样式 */
  .footer{
    margin: 0 47px 0 42px;
    height: 32px;
    font-size: 14px;
    text-align: center;
    line-height:32px ;
    background-image: url("../assets/main/footer-bg.png");
    background-repeat: repeat-x;
  }
  .footer::before{
    content:"";
    width: 42px;
    height: 32px;
    background-image: url("../assets/main/footer-left.png");
    position: absolute;
    left:0px;
  }
  .footer::after{
    content:"";
    width: 47px;
    height: 32px;
    background-image: url("../assets/main/footer-right.png");
    position: absolute;
    right:0px;
  }
  .footer-font{
    padding: 0 20px;
    color: #40cef9;
  }
</style>
