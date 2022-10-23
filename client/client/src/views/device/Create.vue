<template>
  <div class="drawer-box__">
        <div class="list-box">
          <div class="list-title-box">
            <span class="bbox list-title bbox-bold"  is-title><img src="../../assets/table/title-arrow.png" alt="">{{$t('m.add')}} </span>
          </div>
              <el-form
                class="list-container" style="margin-bottom:0px;padding: 45px 0px 20px"
                ref="deviceForm"
                :model="deviceForm"
                :rules="deviceRules"
                label-width="120px"
              >
                <div class="tb-border-lt"></div>
                <div class="tb-border-rt"></div>
                <div class="tb-border-rb" style="bottom: 46px"></div>
                <div class="tb-border-lb" style="bottom: 46px"></div>
                <div class="scroll-box" style="overflow-x: hidden">
                  <div class="scroll" style="width: 500px">
                    <div style="width: 360px;">
                      <div class="group-box__" :title="$t('m.baseinfo')">
                        <el-form-item :label="$t('m.name')+':'" prop="name">
                          <el-input size="mini" v-model="deviceForm.name"></el-input>
                        </el-form-item>
                        <el-form-item :label="$t('m.type')+':'" prop="role">
                          <el-select
                            size="mini"
                            class="full-row"
                            v-model="deviceForm.category"
                            :placeholder="$t('m.pleaseSelectDeviceType')"
                          >
                            <el-option
                              v-for="item in device_categories"
                              :key="item.id"
                              :label="item.name"
                              :value="item.id"
                            >
                            </el-option>
                          </el-select>
                        </el-form-item>

                        <el-form-item :label="$t('m.lat')+'：'" prop="lat">
                          <el-input :title="$t('m.lat')" size="mini" v-model="deviceForm.lat">
                            <span slot="suffix">°</span>
                          </el-input>
                        </el-form-item>

                        <el-form-item :label="$t('m.lng')+'：'" prop="lng">
                          <el-input :title="$t('m.lng')" size="mini" v-model="deviceForm.lng">
                            <span slot="suffix">°</span>
                          </el-input>
                        </el-form-item>

                        <el-form-item :label="$t('m.alt')+'：'" prop="alt">
                          <el-input size="mini" v-model="deviceForm.alt">
                            <span slot="suffix">m</span>
                          </el-input>
                        </el-form-item>

                      </div>
                      <div class="group-box__" title="">
                        <el-form-item :label="$t('m.device')+'：'" style="margin-bottom: 0px">
                          <el-row>
                            <el-col :span="14">
                              <el-form-item prop="ip">
                                <el-input placeholder="IP" title="" size="mini" v-model="deviceForm.ip"></el-input>
                              </el-form-item>
                            </el-col>
                            <el-col :span="2" style="text-align: center">:</el-col>
                            <el-col :span="8">
                              <el-form-item prop="port">
                                <el-input  title="" size="mini" v-model="deviceForm.port"></el-input>
                              </el-form-item>
                            </el-col>
                          </el-row>
                        </el-form-item>
                        <el-form-item :label="$t('m.sevice')+'：'" style="margin-bottom: 0px">
                          <el-row>
                            <el-col :span="14">
                              <el-form-item prop="lip">
                                <el-input  placeholder="IP" title="" size="mini" v-model="deviceForm.lip"></el-input>
                              </el-form-item>
                            </el-col>
                            <el-col :span="2" style="text-align: center">:</el-col>
                            <el-col :span="8">
                              <el-form-item prop="lport">
                                <el-input title="" size="mini" v-model="deviceForm.lport"></el-input>
                              </el-form-item>
                            </el-col>
                          </el-row>
                        </el-form-item>
                      </div>
                      <div class="group-box__" title="">
                        <el-form-item :label="$t('m.coverRadius')" prop="coverR">
                          <el-input size="mini" v-model="deviceForm.coverR"></el-input>
                        </el-form-item>
                        <el-form-item :label="$t('m.coverZone')" style="margin-bottom: 0px;">
                          <el-row>
                            <el-col :span="11">
                              <el-form-item prop="coverS">
                                <el-input :title="$t('m.startAngleRange')" size="mini" v-model="deviceForm.coverS">
                                  <span slot="suffix">°</span>
                                </el-input>
                              </el-form-item>
                            </el-col>
                            <el-col :span="2" style="text-align: center">-</el-col>
                            <el-col :span="11">
                              <el-form-item prop="coverE">
                                <el-input :title="$t('m.endAngleRange')" size="mini" v-model="deviceForm.coverE">
                                  <span slot="suffix">°</span>
                                </el-input>
                              </el-form-item>
                            </el-col>
                          </el-row>
                        </el-form-item>
                        <el-form-item :label="$t('m.correInfo')+'：'" style="margin-bottom: 0px;">
                          <el-row>
                            <el-col :span="11">
                              <el-form-item prop="rectifyAz">
                                <el-input :title="$t('m.horizontalCorr')" size="mini" v-model="deviceForm.rectifyAz"></el-input>
                              </el-form-item>
                            </el-col>
                            <el-col :span="2" style="text-align: center">,</el-col>
                            <el-col :span="11">
                              <el-form-item prop="rectifyEl">
                                <el-input :title="$t('m.PitchCorr')" size="mini" v-model="deviceForm.rectifyEl"></el-input>
                              </el-form-item>
                            </el-col>
                          </el-row>
                        </el-form-item>
                      </div>
                      <div class="group-box__" title="">
                        <el-form-item :label="$t('m.timeout')+'：'" prop="targetTimeOut">
                          <el-input size="mini" v-model="deviceForm.targetTimeOut">
                            <span slot="suffix">″</span>
                          </el-input>
                        </el-form-item>
                        <el-form-item :label="$t('m.targetReport')+'：'" prop="probeReportingInterval">
                          <el-input size="mini" v-model="deviceForm.probeReportingInterval">
                            <span slot="suffix">″</span>
                          </el-input>
                        </el-form-item>
                        <el-form-item :label="$t('m.statusReport')+'：'" prop="statusReportingInterval">
                          <el-input size="mini" v-model="deviceForm.statusReportingInterval">
                            <span slot="suffix">″</span>
                          </el-input>
                        </el-form-item>
                        <el-form-item :label="$t('m.threatDetermin')+'：'" prop="threadAssessmentCount">
                          <el-input size="mini" v-model="deviceForm.threadAssessmentCount"></el-input>
                        </el-form-item>
                        <el-form-item :label="$t('m.descInfo')+'：'" prop="display">
                          <el-input
                            type="textarea"
                            maxlength="100" style="margin: 10px 0;"
                            v-model="deviceForm.display"
                          ></el-input>
                        </el-form-item>
                      </div>
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
import {
  validator_required_rang,
  validator_rang,
  validator_required_ip,
  validator_required,
  validator_required_number_rang
} from "../../modes/tool";
import { http_request, create_device } from "../../modes/api";
import { show_message, msg_enum } from "../../modes/elementUI";
import {mapState} from 'vuex'

export default {
  name: "Create",
  data() {
    return {
      deviceForm: {
        name: "",
        display: "",
        category: undefined,
        ip: "",
        port: 0,
        lip:'',
        lport:0,
        lat: 0.00,
        lng: 0.00,
        alt: 100.00,
        coverR: 1000.00,
        coverS: 0.00,
        coverE: 360.00,
        rectifyAz: 0.00,
        rectifyEl: 0.00,
        probeReportingInterval: 1,
        statusReportingInterval: 1,
        threadAssessmentCount: 3,
        targetTimeOut:10
      },
      deviceRules: {
        name: validator_required_rang(5,25, "name"),
        display: validator_rang(0, 100),
        category: validator_required("category"),
        ip: validator_required_ip("IP"),
        port: validator_required_number_rang(3000, 65530, "Port"),
        lip: validator_required_ip("LIP"),
        lport: validator_required_number_rang(3000, 65530, "LPort"),
        lat: validator_required_number_rang(0, 90, "纬度"),
        lng: validator_required_number_rang(0, 180, "经度"),
        alt: validator_required_number_rang(0, 5000, "海拔"),
        coverR: validator_required_number_rang(0, 15000, "覆盖半径"),
        coverS: validator_required_number_rang(0, 360, "起始角度"),
        coverE: validator_required_number_rang(0, 360, "结束角度"),
        rectifyAz: validator_required_number_rang(0, 360, "方位纠偏"),
        rectifyEl: validator_required_number_rang(0, 360, "俯仰纠偏"),
        probeReportingInterval: validator_required("probeReportingInterval"),
        statusReportingInterval: validator_required("statusReportingInterval"),
        threadAssessmentCount:  validator_required_number_rang(0, 10, "威胁判定点数"),
        targetTimeOut:validator_required_number_rang(1, 20, "目标超时时间")
      }
    };
  },
  computed:{
    ...mapState(['device_categories'])
  },
  methods: {
    doSave() {
      this.$refs["deviceForm"].validate(valid => {
        if(this.deviceForm.category){
          if (valid) {
            console.log("device info ", this.deviceForm);
            http_request(create_device, {
              ...this.deviceForm,
              lng:parseFloat(this.deviceForm.lng),
              lat:parseFloat(this.deviceForm.lat),
              alt:parseFloat(this.deviceForm.alt),
              port:parseInt(this.deviceForm.port),
              lport:parseInt(this.deviceForm.lport),
              coverR:parseInt(this.deviceForm.coverR),
              coverE:parseInt(this.deviceForm.coverE),
              coverS:parseInt(this.deviceForm.coverS),
              rectifyAz:parseInt(this.deviceForm.rectifyAz),
              rectifyEl:parseInt(this.deviceForm.rectifyEl),
              targetTimeOut:parseInt(this.deviceForm.targetTimeOut),
              probeReportingInterval:parseInt(this.deviceForm.probeReportingInterval),
              statusReportingInterval:parseInt(this.deviceForm.statusReportingInterval),
              threadAssessmentCount:parseInt(this.deviceForm.threadAssessmentCount)
            }, () => {
              show_message(msg_enum.success, this.$t('m.addSuccess'));
              this.$emit("success");
            });
          }
        }
        else{
          show_message(msg_enum.error, this.$t('m.pleaseSelectDeviceType'));
          return;
        }
      });
    },
    doReset(){
      this.deviceForm={
        name: "",
        display: "",
        category: undefined,
        ip: "",
        port: 0,
        lip:'',
        lport:0,
        lat: 0.00,
        lng: 0.00,
        alt: 100.00,
        coverR: 1000.00,
        coverS: 0.00,
        coverE: 360.00,
        rectifyAz: 0.00,
        rectifyEl: 0.00,
        probeReportingInterval: 1,
        statusReportingInterval: 1,
        threadAssessmentCount: 3,
        targetTimeOut:10
      };
    }
  }
};
</script>

<style scoped src="../../assets/table/table.css"></style>
