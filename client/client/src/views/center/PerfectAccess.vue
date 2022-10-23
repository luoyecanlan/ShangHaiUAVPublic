<template>
  <el-form
    class="dialog-box__"
    ref="UpDateUserInfoForm"
    :model="UpDateUserInfoForm"
    :rules="UpDateUserInfoRules"
    label-width="100px">

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
      <span class="title">{{$t('m.modifyUserInfo')}}</span>
    </div>

    <div class="__dialog-container">
      <el-form-item :label="$t('m.name')" prop="name">
        <el-input size="mini" v-model="UpDateUserInfoForm.name"></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.nick')" prop="nick">
        <el-input size="mini" v-model="UpDateUserInfoForm.nick"></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.Email')" prop="email">
        <el-input size="mini" v-model="UpDateUserInfoForm.email"></el-input>
      </el-form-item>
      <el-form-item class="__dialog-row" :label="$t('m.phoneNum')" prop="phone">
        <el-input size="mini" v-model="UpDateUserInfoForm.phone"></el-input>
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
    import {http_request, request_current, update_current} from "../../modes/api";
    import {show_message} from "../../modes/elementUI";
    import {msg_enum} from "../../modes/elementUI/message";
    import {validator_phone, validator_required_rang, validator_type} from "../../modes/tool";
    import {type_enum} from "../../modes/tool/ruleType";

    export default {
        name: "Index",
        data()
        {
            return {
                UpDateUserInfoForm:
                    {
                        name: "",
                        nick: "",
                        phone: "",
                        email: "",
                        id: 0,
                    },
                UpDateUserInfoRules:
                    {
                        name: validator_required_rang(1, 25, "name"),
                        nick: validator_required_rang(1, 25, "nick"),
                        phone: validator_phone(),
                        email: validator_type(type_enum.email)
                    }
            };
        },
        methods: {
            submitHandle()
            {
                this.$refs["UpDateUserInfoForm"].validate(valid =>
                {
                    if (valid)
                    {
                        http_request(update_current, this.UpDateUserInfoForm, () =>
                        {
                            show_message(msg_enum.success, this.$t('m.modifySuccess'));
                            this.$emit("success");
                        });
                    }
                });
            }
        },
        mounted()
        {
            //获取当前用户信息
            http_request(request_current,'',(data)=>
            {
                this.UpDateUserInfoForm.name = data.name;
                this.UpDateUserInfoForm.email = data.email;
                this.UpDateUserInfoForm.phone = data.phone;
                this.UpDateUserInfoForm.nick = data.nick;
                this.UpDateUserInfoForm.id = data.uid;
            });
        }
    }
</script>

<style scoped src="../../assets/dialog/dialog.css">
</style>
