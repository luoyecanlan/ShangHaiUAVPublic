<template>
  <el-form
    class="dialog-box__"
    ref="UpDateUserPassWordForm"
    :model="UpDateUserPassWordForm"
    label-width="150px">
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
      <el-form-item :label="$t('m.originalPassword')" >
        <el-input size="mini" v-model="UpDateUserPassWordForm.oldPwd" show-password></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.newPassword')" >
        <el-input size="mini" v-model="UpDateUserPassWordForm.newPwd" show-password></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.SurePassword')">
        <el-input size="mini"  v-model="confirmNewPwd" show-password></el-input>
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
    import MD5 from 'md5';

    export default {
        name: "Index",
        data() {
            return {
                confirmNewPwd: "",
                UpDateUserPassWordForm:
                    {
                        uid: 0,
                        oldPwd: "",
                        newPwd: ""
                    },
            };
        },
        methods: {
            submitHandle() {

              if(this.confirmNewPwd != this.UpDateUserPassWordForm.newPwd ||
                this.UpDateUserPassWordForm.newPwd==''||
                this.confirmNewPwd==''){

                    show_message(msg_enum.error, this.$t('m.newPasswordIsInconsistent'));
                    return;
                }

                const {uid,oldPwd,newPwd}=this.UpDateUserPassWordForm;

                http_request(update_current_pwd, {uid, oldPwd: MD5(oldPwd),newPwd: MD5(newPwd)}, () =>
                {
                    show_message(msg_enum.success, this.$t('m.PasswordChangedSuccessfully'));

                    this.UpDateUserPassWordForm.oldPwd="";
                    this.UpDateUserPassWordForm.newPwd="";
                    this.confirmNewPwd = "";

                    this.$emit("success");
                });

            }
        },
        mounted() {
            //获取当前用户信息
            http_request(request_current, '', (data) =>
            {
                this.UpDateUserPassWordForm.uid = data.uid;
            });
        }
    }

</script>

<style scoped src="../../assets/dialog/dialog.css">
