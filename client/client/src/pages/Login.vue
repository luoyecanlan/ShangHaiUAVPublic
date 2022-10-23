<template>
  <div class="login-box fixed-box">
    <language  style="right: 1px;top: 4px"/>
    <night-sky style="position: fixed"/>
    <div class="login-main">
      <div class="form-box">
        <div class="login-container">
          <div class="top_outline"></div><div class="top_inline"></div>
          <div class="lb"></div><div class="rb"></div>
          <span class="login-header">
              <!--<img src="../assets/login/login-logo.png" class="logo" alt="">-->
              <span class="logo-title">系统登陆</span>
            </span>
          <el-form :model="loginForm" ref="loginForm" class="login-form"
                   :rules="loginRules" @submit.native.prevent>
            <el-form-item prop="username">
              <el-input size="medium"  v-model="loginForm.username"
                        :placeholder="$t('m.enterUserName')">
                <div slot="prefix" class="input-icon___">
                  <img src="../assets/login/uname.png" alt="" style="margin-bottom: 2px">
                </div>
              </el-input>
            </el-form-item>
            <el-form-item prop="password" style="margin: 40px 0 10px;">
              <el-input size="medium" show-password v-model="loginForm.password"
                        :placeholder="$t('m.enterPassWord')" style="height: 55px">
                <div slot="prefix" class="input-icon___">
                  <img src="../assets/login/pwd.png" alt="" style="margin-bottom: 2px;">
                </div>
              </el-input>
            </el-form-item>

            <el-form-item>
              <el-checkbox :checked="remember" style="float: right;">{{$t('m.rememberPassword')}}</el-checkbox>
            </el-form-item>

            <el-form-item >
              <el-input
                size="medium"
                v-model="loginForm.videntifyCode"
                auto-complete="off"
                placeholder="验证码"
                style="width: 63%"
              />
              <VerificationCode :changeCode.sync='identifyCode'></VerificationCode>

            </el-form-item>

            <el-form-item style="margin-top: 40px">

              <button
                type="primary"
                class="LoginButtonClass"
                @click.prevent="handleSubmit">
                {{$t('m.login')}}
              </button>

<!--              <el-button style="width: 100%" size="medium" type="primary"-->
<!--                         native-type="submit" @click.prevent="handleSubmit">-->
<!--                <span class="submit-font">登录</span>-->
<!--              </el-button>-->
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
  } from "../modes/elementUI";
  import NightSky from '../components/NightSky'
  import {validator_required_rang } from "../modes/tool";
  import { request_access ,save_token,clear_token } from "../modes/api";
  import MD5 from 'md5'
  import {http_request,request_map_info_list,create_person_info,request_person_info_list} from "../modes/api";
  import {mapActions, mapState} from "vuex";
  import Language from "../components/Language";
  import VerificationCode from "../components/VerificationCode";
  export default {
    name: "Login-Index",
    data() {
      return {
        identifyCode:'',    //当前生成的验证码
        isLoginButton :true,
        loginForm:{
          username:'',
          password:'',
          videntifyCode:''
        },
        loginRules: {
          username: validator_required_rang(1,25,'username'),
          password: validator_required_rang(5,25,'password'),
          videntifyCode: [{ required: true, trigger: "change", message: "验证码不能为空" }]        },
        remember: true
      };
    },
    components:{
      VerificationCode,
      NightSky,
      Language:Language,
    },
    computed:
      {
        ...mapState(['login_info']),
        ...mapState(['select_target_id']),
      },
    methods: {



      setLoginButton(){
        this.isLoginButton = true;
      },
      /**
       * 登录方法
       */
      doLogin(){
        const {username,password,videntifyCode}=this.loginForm;
        let result=request_access({
          username,
          password: MD5(password)
        });
        if(videntifyCode==null){

          show_message(msg_enum.error,  "验证码不能为空");
          return;
        }
        if(videntifyCode!==this.identifyCode){
          console.log(videntifyCode+"----------"+this.identifyCode)
          show_message(msg_enum.error,  "验证码错误");
          return;
        }
        result.then(data=>{
          if(data.code===0){
            let _data=data.data;
            console.log(_data);
            if(_data)
            {
              const {access_token,refresh_token,nbf,expire_in} = _data;
              //保存token
              if(this.remember)
              {
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
          }else {
            show_message(msg_enum.error, data.message)
          }
        }).catch(error=>{
          show_message(msg_enum.error, error)
        })
      },
      /**
       * 数据验证
       */
      handleSubmit(){
        // 避免重复点击
        if(this.isLoginButton)
        {
          this.isLoginButton = false;
          let timer = setTimeout(this.setLoginButton,4000);
          //数据验证
          this.$refs['loginForm'].validate(valid=>{
            if(valid){
              this.doLogin()
            }
          })
        } else
          {
            show_message(msg_enum.success, this.$t('m.pleaseTryAgain2Seconds'));
          }
      }
    },
    mounted(){}
  };
</script>

<style scoped src="../assets/login/login.css">

</style>
