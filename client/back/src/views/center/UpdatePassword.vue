<template>
  <el-form
    class="dialog-box__"
    ref="UpDateUserPassWordForm"
    :model="pwdForm"
    :rules="pwdRules"
    label-width="170px">
    <div class="dialog-lt"></div>
    <div class="dialog-lb"></div>
    <div class="dialog-rt"></div>
    <div class="dialog-rb"></div>
    <div class="__dialog-box-header">
      <span class="bg_01"></span>
      <span class="bg_02"></span>
      <span class="bg_03"></span>
      <span class="bg_04"></span>
      <span class="bg_05"></span>
      <span class="title">{{$t('m.modifyPassword')}}</span>
    </div>
    <div class="__dialog-container">
      <el-form-item :label="$t('m.originalPassword')" prop="oldPwd">
        <el-input size="mini" v-model="pwdForm.oldPwd" show-password></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.newPassword')" prop="newPwd">
        <el-input size="mini" v-model="pwdForm.newPwd" show-password></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.SurePassword')" prop="newPwd1">
        <el-input size="mini"  v-model="pwdForm.newPwd1" show-password></el-input>
      </el-form-item>
      <el-button
        type="primary"
        size="small"
        class="__dialog-button"
        @click="submitHandle">
        {{$t('m.save')}}
      </el-button>
    </div>
  </el-form>
</template>

<script>
    import {http_request, request_current, update_current_pwd} from "../../modes/api";
    import {show_message} from "../../modes/elementUI";
    import {msg_enum} from "../../modes/elementUI/message";
    import {validator_required} from "../../modes/tool";
    import MD5 from 'md5';

    export default {
        name: "Index",
        data() {
            return {
              confirmNewPwd: "",
              pwdForm:
                {
                  uid: 0,
                  oldPwd: "",
                  newPwd: "",
                  newPwd1:""
                },
              pwdRules:{
                oldPwd: validator_required('原密码'),
                newPwd: validator_required('新密码'),
                newPwd1:validator_required('确认密码'),
              }
            };
        },
        methods: {
            submitHandle() {
              const {uid,oldPwd,newPwd,newPwd1}=this.pwdForm;
                if(newPwd !== newPwd1)
                {
                    show_message(msg_enum.error,  this.$t('m.newPasswordIsInconsistent'));
                    return;
                }
                http_request(update_current_pwd, {uid, oldPwd: MD5(oldPwd),newPwd: MD5(newPwd)}, () => {
                  show_message(msg_enum.success,  this.$t('m.PasswordChangedSuccessfully'));
                  this.pwdForm = {
                    uid: 0,
                    oldPwd: "",
                    newPwd: "",
                    newPwd1: ""
                  };
                  this.$emit("success");
                });

            }
        },
        mounted() {
            //获取当前用户信息
            http_request(request_current, '', (data) =>
            {
                this.pwdForm.uid = data.uid;
            });
        }
    }

</script>

<style scoped src="../../assets/dialog/dialog.css">

</style>
