<template>
  <!--        格式转换-->
  <div class="correct-box" style="height: 160px">
    <div class="correct-header">
      <span class="correct-title">{{$t('m.formatConversion')}}</span>
      <span class="correct-reset" @click.prevent="reset"><img class="link-button" src="../../assets/gis/reset.png" alt=""></span>
      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span lightblue>Error：</span>{{error}}</span>
      </transition>
    </div>
    <div class="correct-body">
      <div class="correct-left">
        <div class="condition-item" style="margin-bottom: 30px">
          <div class="item-title" style="width: 230px;">{{$t('m.DMSToD')}}：</div>
          <el-input style="width: 80px;" v-model="data_deg" :placeholder="$t('m.degrees')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <el-input style="width: 80px;margin: 0 20px;" v-model="data_min" :placeholder="$t('m.min')">
            <span slot="suffix" class="input-suffix__ light-color">'</span>
          </el-input>
          <el-input style="width: 80px;" v-model="data_sed" :placeholder="$t('m.second')">
            <span slot="suffix" class="input-suffix__ light-color">"</span>
          </el-input>
        </div>
        <div class="condition-item">
          <div class="item-title" style="width: 230px;">{{$t('m.DToDMS')}}：</div>
          <el-input style="width: 180px;" :placeholder="$t('m.degrees')" v-model="data_degree">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
        </div>
      </div>
      <div class="correct-option">
        <div class="correct-button" @click.prevent="calc_degree" style="margin-top: -36px;">
          <img class="link-button" src="../../assets/gis/calc.png" alt="">
          <img class="img-right" style="vertical-align: 28%" src="../../assets/gis/right.png" alt="">
        </div>
        <div class="correct-button" @click.prevent="calc_d_m_s" style="margin-top: 20px;">
          <img class="link-button" src="../../assets/gis/calc.png" alt="">
          <img class="img-right" style="vertical-align: 28%" src="../../assets/gis/right.png" alt="">
        </div>
      </div>
      <div class="resule-box">
        <el-input style="width: 200px;height: 26px;" v-model="result_degree.toFixed(8)" :placeholder="$t('m.degrees')">
          <span slot="suffix" class="input-suffix__ light-color">°</span>
        </el-input>
        <div style="margin-top: 30px;">
          <el-input style="width: 60px;height: 26px;" :placeholder="$t('m.degrees')" v-model="result_deg.toFixed(2)">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <el-input style="width: 60px;height: 26px;margin: 0 5px" :placeholder="$t('m.min')" v-model="result_min.toFixed(2)">
            <span slot="suffix" class="input-suffix__ light-color">'</span>
          </el-input>
          <el-input style="width: 60px;height: 26px;" :placeholder="$t('m.second')" v-model="result_sed.toFixed(2)">
            <span slot="suffix" class="input-suffix__ light-color">"</span>
          </el-input>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import {http_request,convert_degree,convert_d_m_s} from "../../modes/api";
  import {exceOnce, validationNumber} from "../../modes/tool";

  export default {
    name: "FormatConversion",
    data() {
      return {
        data_deg: undefined,
        data_min: undefined,
        data_sed: undefined,
        data_degree: undefined,
        result_degree: 0,
        result_deg: 0,
        result_min: 0,
        result_sed: 0,
        error:undefined
      }
    },
    methods: {
      calc_degree() {
        if (!validationNumber([{
          value: this.data_deg
        }, {
          value: this.data_sed
        }, {
          value: this.data_min
        }])) {
          this.error = this.$t('m.inputParameterError');
          exceOnce(() => {
            this.error = undefined;
          })
          return;
        }
        http_request(convert_degree, {
          deg: parseFloat(this.data_deg),
          min: parseFloat(this.data_min),
          sed: parseFloat(this.data_sed)
        }, data => {
          this.result_degree = data;
        })
      },
      calc_d_m_s() {
        if (!validationNumber([{
          value: this.data_degree
        }])) {
          this.error = this.$t('m.inputParameterError');
          exceOnce(() => {
            this.error = undefined;
          })
          return;
        }
        http_request(convert_d_m_s, parseFloat(this.data_degree), data => {
          this.result_deg = data.item1;
          this.result_min = data.item2;
          this.result_sed = data.item3;
        });
      },
      reset() {
        this.data_deg = undefined;
        this.data_min = undefined;
        this.data_sed = undefined;
        this.data_degree = undefined;
        this.result_degree = 0;
        this.result_deg = 0;
        this.result_min = 0;
        this.result_sed = 0
      }
    }
  }
</script>

<style scoped>

</style>
