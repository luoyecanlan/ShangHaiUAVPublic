<template>
    <div class="login-box fixed-box">
      <night-sky style="position: fixed"/>
      <div class="login-main">
        <div class="form-box">
          <div class="login-container">
            <div class="top_outline"></div><div class="top_inline"></div>
            <div class="lb"></div><div class="rb"></div>
            <span class="login-header">
              <!--<img src="../../assets/login/login-logo.png" class="logo" alt="">-->
              <span class="logo-title">后台登录</span>
            </span>
            <el-form :model="loginForm" ref="loginForm" class="login-form"
                     :rules="loginRules" @submit.native.prevent>
              <el-form-item prop="username">
                <el-input size="medium"  v-model="loginForm.username"
                          :placeholder="$t('m.enterUserName')">
                  <div slot="prefix" class="input-icon___">
                    <img src="../../assets/login/uname.png" alt="" style="margin-bottom: 2px">
                  </div>
                </el-input>
              </el-form-item>
              <el-form-item prop="password" style="margin: 40px 0 10px;">
                <el-input size="medium" show-password v-model="loginForm.password"
                          :placeholder="$t('m.enterPassWord')" >
                  <div slot="prefix" class="input-icon___">
                    <img src="../../assets/login/pwd.png" alt="" style="margin-bottom: 2px;">
                  </div>
                </el-input>
              </el-form-item>
              <el-form-item>
                <el-checkbox :checked="remember" style="float: right;">{{$t('m.rememberPassword')}}</el-checkbox>
              </el-form-item>
              <el-form-item style="margin-top: 40px">
                <el-button style="width: 100%" size="medium" type="primary"
                           native-type="submit" @click.prevent="handleSubmit">
                  <span class="submit-font"><i :class="{'el-icon-loading':busy}" style="width: 24px"></i>{{$t('m.login')}}</span>
                </el-button>
              </el-form-item>
            </el-form>
          </div>
        </div>
      </div>
    </div>
</template>

<script>
  import {
    msg_enum,
    show_message,
    open_notification
  } from "../../modes/elementUI";
  import NightSky from '../../components/NightSky'
  import {validator_required_rang } from "../../modes/tool";
  import { request_access ,save_token,clear_token } from "../../modes/api";
  import MD5 from 'md5'
  import {mapActions} from 'vuex'

  export default {
    name: "Login-Index",
    data() {
      return {
        busy:false,
        loginForm:{
          username:'',
          password:''
        },
        loginRules: {
          username: validator_required_rang(1,25,'username'),
          password: validator_required_rang(5,25,'password')
        },
        remember: true
      };
    },
    methods: {
      /**
       * 登录方法
       */
      doLogin(){
        this.busy=true;
        const {username,password}=this.loginForm;
        let result=request_access({
          username,
          password: MD5(password)
        });
        result.then(data=>{
          console.log(data)
          if(data.code===0){
            let _data=data.data;
            if(_data){
              const { access_token,refresh_token,nbf,expire_in} = _data;
              //保存token
              if(this.remember){
                save_token({access_token,refresh_token,nbf,expire_in});
              }else{
                clear_token();
              }
              show_message(msg_enum.success, this.$t('m.userLoginSucceeded'));
              //open_notification(msg_enum.success,'提示','用户登录成功!',3000)
              this.$router.replace('/home')
            }else {
              show_message(msg_enum.error,  this.$t('m.loginDataParsingError'));
            }
            this.busy=false;
          }else {
            show_message(msg_enum.error, data.message)
            this.busy=false;
          }
        }).catch(error=>{
          show_message(msg_enum.error, error)
          this.busy=false;
        })
      },
      /**
       * 数据验证
       */
      handleSubmit(){
        //数据验证
        this.$refs['loginForm'].validate(valid=>{
          if(valid){
            this.doLogin()
          }
        })
      }
    },
    components:{
      NightSky
    }
  };
</script>

<style scoped src="../../assets/login/login.css"></style>
