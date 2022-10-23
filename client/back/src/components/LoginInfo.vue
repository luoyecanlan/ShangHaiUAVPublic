<template>
  <div class="login-info" v-if="login_info">
    <el-popover v-model="openMenu"
      placement="bottom"
      trigger="click">
      <div >
        <el-button type="text" class="login-info-btn" @click.stop="openUpdateInfo">{{$t('m.modifyInfo')}}</el-button>
        <el-divider direction="vertical"/>
        <el-button type="text" class="login-info-btn" @click.stop="openUpdatePwd">{{$t('m.modifyPW')}}</el-button>
      </div>
      <span slot="reference" style="width: 160px;margin-bottom: 5px;">
        <img src="../assets/main/user-icon.png" style="vertical-align: -10%" alt="">
        <span class="login-nick ellipsis">{{login_info.nick}}({{login_info.name}})</span>
      </span>
    </el-popover>
    <span class="login-nick" @click.stop="loginOut">
      <img src="../assets/main/exit.png" class="link-button" style="vertical-align: -15%" alt="">
    </span>

    <span class="v-line__"></span>
<!--    <el-divider direction="vertical" style="height: 14px;"/>-->

    <el-dialog width="380px" :visible.sync="isPerfectAccess" direction="rtl" model>
      <perfect-access @success="ModifyInfoSuccess"/>
    </el-dialog>

    <el-dialog width="380px" :visible.sync="isUpdatePassWord" direction="rtl" model>
      <update-password @success="ModifyPasswordSuccess"/>
    </el-dialog>
  </div>
</template>

<script>
    import {http_request, request_current,distory_access} from "../modes/api";
    import {clear_token, directLogin} from "../modes/api";
    import {show_confirm} from "../modes/elementUI";
    import {mapState, mapActions} from 'vuex';
    import UpdateUserPassword from '../views/center/UpdatePassword';
    import PerfectAccess from '../views/center/PerfectAccess';

    export default {
        name: "LoginInfo",
        data() {
          return {
            openMenu: false,
            isPerfectAccess: false,
            isUpdatePassWord: false
          }
        },
        computed: {
            ...mapState(['login_info'])
        },
        methods: {
          ...mapActions(['commit_login_info']),
          openUpdateInfo(){
            this.isPerfectAccess=true;
            this.openMenu=false;
            return false;
          },
          openUpdatePwd(){
            this.isUpdatePassWord=true;
            this.openMenu=false;
            return false;
          },
          ModifyInfoSuccess() {
            this.isPerfectAccess = false;
          },
          ModifyPasswordSuccess() {
            this.isUpdatePassWord = false;
            this.distoryAndExit();
          },
          distoryAndExit(){
            //注销访问令牌
            http_request(distory_access, null, () => {
              //清理token
              clear_token();
              //定向到登录界面
              directLogin();
            });
          },
          loginOut() {
            show_confirm(this.$t('m.quitSys'),this.$t('m.sureQuit'),()=>{
              this.distoryAndExit();
            });
          }
        },
        mounted() {
            http_request(request_current, null, _info => {
                this.commit_login_info(_info)
            })
        },
        components: {
            PerfectAccess: PerfectAccess,
            UpdatePassword: UpdateUserPassword,
        }
    }
</script>

<style scoped>
  .login-nick {
    font-size: 14px;
    cursor: pointer;
    color: #40cef9;
    padding-right: 12px;
  }
  .login-nick:hover{
       color: #84e3ff;
  }
  .login-info-btn{
    font-size: 14px;
    padding: 5px 0 5px 10px;
  }
  .login-info-btn:last-child{
    padding: 5px 10px 5px 0;
  }
  .v-line__{
    display: inline-block;width: 2px;height: 14px;background-color: #40cef9;
    vertical-align: -10%;
    margin-right: 7px;
  }
</style>
