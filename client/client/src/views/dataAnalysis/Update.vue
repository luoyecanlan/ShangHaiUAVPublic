<template>
  <div class="drawer-box__">
    <div class="list-box">
      <div class="list-title-box">
        <span class="bbox list-title" is-title><img src="../../assets/table/title-arrow.png" alt="">{{$t('m.modify')}} </span>
      </div>
      <el-form
        class="list-container" style="margin-bottom:0px;padding: 45px 0px 20px"
        ref="deviceForm"
        :model="deviceForm"
        :rules="deviceRules"
        label-width="120px">

        <div class="tb-border-lt"></div>
        <div class="tb-border-rt"></div>
        <div class="tb-border-rb" style="bottom: 46px"></div>
        <div class="tb-border-lb" style="bottom: 46px"></div>
        <div class="scroll-box" style="overflow-x: hidden">
            <div class="scroll" style="width: 500px">
              <div style="width: 360px;">
                <div class="group-box__" title="基本信息">
                  <el-form-item :label="$t('m.name')+':'" prop="name">
                    <el-input size="mini" v-model="deviceForm.name"></el-input>
                  </el-form-item>
                  <el-form-item :label="$t('m.type')+':'" prop="role">
                    <span>{{category_name}}</span>
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
                  <el-form-item :label="$t('m.device')+'：'"  style="margin-bottom: 0px">
                    <el-row>
                      <el-col :span="14">
                        <el-form-item prop="ip">
                          <el-input placeholder="IP" size="mini" v-model="deviceForm.ip"></el-input>
                        </el-form-item>
                      </el-col>
                      <el-col :span="2" style="text-align: center">:</el-col>
                      <el-col :span="8">
                        <el-form-item prop="port">
                          <el-input placeholder="Port" size="mini" v-model="deviceForm.port"></el-input>
                        </el-form-item>
                      </el-col>
                    </el-row>
                  </el-form-item>
                  <el-form-item :label="$t('m.sevice')+'：'" style="margin-bottom: 0px">
                    <el-row>
                      <el-col :span="14">
                        <el-form-item prop="lip">
                          <el-input placeholder="IP" size="mini" v-model="deviceForm.lip"></el-input>
                        </el-form-item>
                      </el-col>
                      <el-col :span="2" style="text-align: center">:</el-col>
                      <el-col :span="8">
                        <el-form-item prop="lport">
                          <el-input placeholder="Port" size="mini" v-model="deviceForm.lport"></el-input>
                        </el-form-item>
                      </el-col>
                    </el-row>
                  </el-form-item>
                </div>
                <div class="group-box__" >
                  <el-form-item :label="$t('m.coverRadius')" prop="coverR">
                    <el-input size="mini" v-model="deviceForm.coverR"></el-input>
                  </el-form-item>
                  <el-form-item :label="$t('m.coverZone')" style="margin-bottom: 0px;">
                    <el-row>
                      <el-col :span="11">
                        <el-form-item prop="coverS">
                          <el-input  size="mini" v-model="deviceForm.coverS">
                            <span slot="suffix">°</span>
                          </el-input>
                        </el-form-item>
                      </el-col>
                      <el-col :span="2" style="text-align: center">-</el-col>
                      <el-col :span="11">
                        <el-form-item prop="coverE">
                          <el-input  size="mini" v-model="deviceForm.coverE">
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
                          <el-input title="水平纠偏" size="mini" v-model="deviceForm.rectifyAz"></el-input>
                        </el-form-item>
                      </el-col>
                      <el-col :span="2" style="text-align: center">,</el-col>
                      <el-col :span="11">
                        <el-form-item prop="rectifyEl">
                          <el-input title="俯仰纠偏" size="mini" v-model="deviceForm.rectifyEl"></el-input>
                        </el-form-item>
                      </el-col>
                    </el-row>
                  </el-form-item>
                </div>
                <div class="group-box__" title="探测相关">
                  <el-form-item :label="$t('m.timeout')+'：'" prop="targetTimeOut">
                    <el-input size="mini" title="建议时长为扫描周期*3" v-model="deviceForm.targetTimeOut">
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
  import {mapState} from 'vuex'
  import { http_request, update_device } from "../../modes/api";
  import { show_message, msg_enum } from "../../modes/elementUI";

  export default {
    name: "Update",
    data() {
      return {
        deviceRules: {
          name: validator_required_rang(3, 25, "name"),
          display: validator_rang(0, 100),
          category: validator_required("category"),
          ip: validator_required_ip("IP"),
          port: validator_required_number_rang(3000, 65530, "Port"),
          lip: validator_required_ip("LIP"),
          lport: validator_required_number_rang(3000, 65530, "LPort"),
          coverR: validator_required_number_rang(0, 15000, "覆盖半径"),
          coverS: validator_required_number_rang(0, 360, "起始角度"),
          coverE: validator_required_number_rang(0, 360, "结束角度"),
          probeReportingInterval: validator_required('probeReportingInterval'),
          statusReportingInterval: validator_required('statusReportingInterval'),
          threadAssessmentCount: validator_required_number_rang(0, 10, "威胁判定点数"),
          targetTimeOut: validator_required_number_rang(1, 20, "目标超时时间")
        }
      };
    },
    props: {
      info: {
        required: true
      },
      oldData: {
        required: true
      }
    },
    computed: {
      ...mapState(['device_categories', 'devices']),
      category_name() {
        if (this.device_categories) {
          let _c = this.device_categories.find(f => f.id === this.info.category);
          if (_c)
            return _c.name;
        }
        return this.$t('m.unknown')
      },
      deviceForm() {
        return this.info;
      }
    },
    methods: {
      doSave() {
        this.$refs["deviceForm"].validate(valid => {
          if (valid) {
            console.log("device info ", this.deviceForm);
            http_request(update_device, {
              ...this.deviceForm,
              lng: parseFloat(this.deviceForm.lng),
              lat: parseFloat(this.deviceForm.lat),
              alt: parseFloat(this.deviceForm.alt),
              port: parseInt(this.deviceForm.port),
              lport: parseInt(this.deviceForm.lport),
              coverR: parseInt(this.deviceForm.coverR),
              coverE: parseInt(this.deviceForm.coverE),
              coverS: parseInt(this.deviceForm.coverS),
              rectifyAz: parseInt(this.deviceForm.rectifyAz),
              rectifyEl: parseInt(this.deviceForm.rectifyEl),
              targetTimeOut: parseInt(this.deviceForm.targetTimeOut),
              probeReportingInterval: parseInt(this.deviceForm.probeReportingInterval),
              statusReportingInterval: parseInt(this.deviceForm.statusReportingInterval),
              threadAssessmentCount: parseInt(this.deviceForm.threadAssessmentCount)
            }, () => {
              show_message(msg_enum.success,this.$t('m.modifySuccess'));
              this.$emit("success");
            });
          }
        });
      },
      doReset() {
        this.info = {...this.oldData};
      }
    }
  };
</script>

<style scoped src="../../assets/table/table.css">

</style>
