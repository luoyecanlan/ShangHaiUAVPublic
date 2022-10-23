<template>
  <div class="page-box">
    <div class="center-box">
      <div>
        <img src="../assets/main/limit.png" style="margin-top: -20px" alt="">
      </div>
      <p style="margin-top: -30px;text-align: center;color: #40cef9;">
        {{$t('m.noPermissionsExist')}}！
      </p>
      <router-link to="/login">
          <div class="to-login">{{$t('m.reLogin')}}</div>
      </router-link>
      <router-link to="/">
        <div class="to-home">{{$t('m.backHome')}}</div>
      </router-link>
    </div>
  </div>
</template>

<script>
    export default {
        name: "Limit"
    }
</script>

<style scoped>
  .to-login,.to-home{
    line-height: 40px;
    width: 186px;
    text-align: center;
    font-size: 18px;
    margin: 17px auto;
    box-shadow: 0 0 5px 5px rgba(64,204,249,0.2) inset;
  }
  .to-login{
    margin-top: 80px;
    background-color: #40cef9;
    color: #020e2c;
  }
  .to-login:hover{
    background-color: #84e3ff;
  }
  .to-home{
    border: 1px solid #40cef9;
    background-color: #020e2c;
    color: #40cef9;
  }
  .to-home:hover{
    box-shadow: 0 0 5px 5px rgba(64,204,249,0.4) inset;
    transition: all 0.2s;
  }
  .center-box{
    position: absolute;
    margin: 0 auto;
    left: 50%;
    top: 25%;
    transform: translateX(-50%);
  }
</style>
