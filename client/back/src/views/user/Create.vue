<template>
  <div class="drawer-box__">
    <div class="list-box">
      <div class="list-title-box">
        <span class="bbox list-title" is-title><img src="../../assets/table/title-arrow.png" alt=""> {{$t('m.addUser')}}</span>
      </div>
      <el-form
        class="list-container" style="margin-bottom:0px;padding: 45px 0px 20px"
        ref="userForm"
        :model="userForm"
        :rules="userRules"
        label-width="120px"
      >
        <div class="tb-border-lt"></div>
        <div class="tb-border-rt"></div>
        <div class="tb-border-rb" style="bottom: 46px"></div>
        <div class="tb-border-lb" style="bottom: 46px"></div>
        <div class="scroll-box">
          <div class="scroll" style="border-top: none">
            <div class="group-box__" title="">
              <el-form-item :label="$t('m.account')+'：'" prop="username">
                <el-input size="mini" v-model="userForm.username"></el-input>
              </el-form-item>
              <el-form-item :label="$t('m.userRole')+'：'" prop="role">
                <el-select
                  size="mini"
                  class="full-row"
                  v-model="userForm.role"
                  :placeholder="$t('m.pleaseSelectUserRole')"
                >
                  <el-option
                    v-for="item in roleType"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                  >
                  </el-option>
                </el-select>
              </el-form-item>
            </div>

            <div class="group-box__">
              <el-form-item :label="$t('m.name')+'：'" prop="name">
                <el-input size="mini" v-model="userForm.name"></el-input>
              </el-form-item>
              <el-form-item :label="$t('m.nick')+'：'" prop="nick">
                <el-input size="mini" v-model="userForm.nick"></el-input>
              </el-form-item>
              <el-form-item :label="$t('m.phoneNum')+'：'" prop="phone">
                <el-input size="mini" v-model="userForm.phone"></el-input>
              </el-form-item>
              <el-form-item :label="$t('m.email')+'：'" prop="email">
                <el-input size="mini" v-model="userForm.email"></el-input>
              </el-form-item>
            </div>
          </div>
        </div>

        <div class="fixed-button-box__">
          <button class="option-button__ _ok__" @click.stop="doSave">
            {{$t('m.save')}}
          </button>
          <button class="option-button__ _cancel__" @click.stop="doReset">
            {{$t('m.reset')}}
          </button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
  import {roleType,validator_required_rang,validator_type,validator_phone,validator_required,type_enum} from "../../modes/tool";
  import {show_message,msg_enum} from "../../modes/elementUI";
  import {http_request,create_user} from "../../modes/api";

  export default {
    name: "Create",
    data() {
      return {
        roleType,
        userForm: {
          name:"",
          username: "",
          // password: "",
          nick: "",
          phone: "",
          email: "",
          role: 'client'
        },
        userRules: {
          name:validator_required_rang(1, 20, "name"),
          username: validator_required_rang(6, 20, "username"),
          // password: validator_required_rang(6, 25, "password"),
          nick: validator_required_rang(1, 20, "nick"),
          email: validator_type(type_enum.email),
          // phone: validator_phone(),
          role:validator_required('role')
        }
      };
    },
    methods: {
      doSave() {
        this.$refs["userForm"].validate(valid => {
          if (valid) {
            http_request(create_user, this.userForm, () => {
              show_message(msg_enum.success, this.$t('m.addSuccess'));
              this.$emit("success");
            });
          }
        });
      },
      doReset(){
        this.userForm={
          name:"",
          username: "",
          // password: "",
          nick: "",
          phone: "",
          email: "",
          role: 'client'
        };
      }
    }
  };
</script>

<style scoped src="../../assets/table/table.css"></style>
