<template>
  <div class="login-info" v-if="login_info" >

    <el-popover v-model="openMenu"
                placement="bottom"
                trigger="click"
                style="margin-top: -3px">
      <div>
        <el-button type="text" class="login-info-btn" @click.stop="openUpdateInfo">{{$t('m.modifyUserInfo')}}</el-button>
        <el-divider direction="vertical"/>
        <el-button type="text" class="login-info-btn" @click.stop="openUpdatePwd">{{$t('m.modifyPassword')}}</el-button>
      </div>
      <span slot="reference" style="margin-bottom: 5px;">
        <img src="../assets/main/user-icon.png" style="vertical-align: -10%" alt="">
        <span class="login-nick ellipsis" style="vertical-align: 4%">{{login_info.nick }} ({{login_info.username}})</span>
      </span>
    </el-popover>

    <span class="login-nick" @click.stop="loginOut">
      <img src="../assets/main/exit.png" class="link-button" style="margin-top: 2px;margin-right: 1px" alt="">
    </span>

    <span class="v-line__"></span>
    <!--{{$t('m.bate')}}-->
    <language style="right: -89px;top: 0px;position:absolute"/>
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
    import {http_request, distory_access} from "../modes/api";
    import {clear_token, directLogin} from "../modes/api";
    import {show_confirm} from "../modes/elementUI";
    import {mapState, mapActions} from 'vuex';
    import UpdateUserPassword from '../views/center/UpadtePassword';
    import PerfectAccess from '../views/center/PerfectAccess';
    import {Clear_Aspnet_Signalr} from "../map/mapHandle/mapService";
    import Language from "../components/Language";
    import bus from "../modes/tool/bus";

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
            ...mapState(['map_type']),
            ...mapState(['login_info']),
            ...mapState(['user_config_info']),
        },
        methods: {
            ...mapActions(['commit_login_info']),
            ...mapActions(['set_user_config_info']),
            ...mapActions(['set_map_info']),
            ...mapActions(['set_map_type']),
            ...mapActions(['set_user_map_info']),
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
                show_confirm(this.$t('m.quitSys') ,this.$t('m.sureQuit'),()=>
                {
                    bus.$emit('DelVideo');
                    Clear_Aspnet_Signalr();
                    this.distoryAndExit();
                });
            }
        },
        components: {
            PerfectAccess: PerfectAccess,
            UpdatePassword: UpdateUserPassword,
            Language:Language,
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
    vertical-align: -0%;
    margin-right: 7px;
  }
  .link-button{
    height: 14px;
    width: 14px;
    text-align: center;
    margin: 0;
    cursor: pointer;
    transform-origin: center center;
  }
  .link-button:hover {
    animation: rotate_left_to_right 0.5s;
  }
</style>
