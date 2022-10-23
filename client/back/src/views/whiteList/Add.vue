<template>
  <div class="drawer-box__">
    <div class="list-box">
      <div class="list-title-box">
        <span class="bbox list-title" is-title
          ><img
            src="../../assets/table/title-arrow.png"
            alt=""
          />新增白名单</span
        >
      </div>
      <el-form
        class="list-container"
        style="margin-bottom: 0px; padding: 45px 0px 20px"
        ref="whiteForm"
        :model="whiteForm"
        :rules="whiteRules"
        label-width="120px"
      >
        <div class="tb-border-lt"></div>
        <div class="tb-border-rt"></div>
        <div class="tb-border-rb" style="bottom: 46px"></div>
        <div class="tb-border-lb" style="bottom: 46px"></div>
        <div class="scroll-box">
          <div class="scroll" style="border-top: none">
            <div class="group-box__" title="">
              <!-- SN 码 -->
              <el-form-item label="SN 码：" prop="sn">
                <el-input size="mini" v-model="whiteForm.sn"></el-input>
              </el-form-item>

              <!-- 报备部门 -->
              <el-form-item label="报备部门：" prop="department">
                <el-input size="mini" v-model="whiteForm.department"></el-input>
              </el-form-item>

              <!-- 报备时间 -->
              <el-form-item label="报备时间：" prop="starTime">
                <el-date-picker style="width:200px"
                  v-model="whiteForm.starTime"
                  type="datetime"
                  placeholder="开始时间"
                >
                </el-date-picker>
              </el-form-item>
              <el-form-item label="" prop="endTime">
                <el-date-picker style="width:200px"
                  v-model="whiteForm.endTime"
                  type="datetime"
                  placeholder="结束时间"
                >
                </el-date-picker>
              </el-form-item>
            </div>
          </div>
        </div>

        <div class="fixed-button-box__">
          <button class="option-button__ _ok__" @click.stop="doSave">
            {{ $t("m.save") }}
          </button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import {
  roleType,
  validator_required_rang,
  validator_type,
  validator_phone,
  validator_required,
  type_enum,
} from "../../modes/tool";
import { show_message, msg_enum } from "../../modes/elementUI";
import { http_request, create_white_item } from "../../modes/api";

export default {
  name: "Create",
  data() {
    return {
      roleType,
      whiteForm: {
        sn: "",
        department: "",
        starTime:'',
        endTime:''
      },
      whiteRules: {
        sn: validator_required_rang(1, 20, "sn"),
        department: validator_required("department"),
        starTime: validator_required("starTime"),
        endTime: validator_required("endTime"),
      },
    };
  },
  methods: {
    doSave() {
      this.$refs["whiteForm"].validate((valid) => {
        if (valid) {
          http_request(create_white_item, this.whiteForm, () => {
            show_message(msg_enum.success, this.$t("m.addSuccess"));
            this.$emit("success");
          });
        }
      });
    },
  },
};
</script>

<style scoped src="../../assets/table/table.css"></style>
