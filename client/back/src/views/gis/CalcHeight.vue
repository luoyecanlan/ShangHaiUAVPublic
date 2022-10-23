<template>
  <!--        计算高度-->
  <div class="correct-box" style="height: 140px;">
    <div class="correct-header">
      <span class="correct-title">{{$t('m.calculaHeight')}}</span>
      <span class="correct-reset"@click.prevent="reset" ><img class="link-button" src="../../assets/gis/reset.png" alt=""></span>
      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
      </transition>
    </div>
    <div class="correct-body">
      <div class="correct-left">
        <div class="condition-item">
          <div class="item-title" style="width: 100px;">{{$t('m.distance')}}：</div>
          <el-input style="width: 120px;" maxlength="10" v-model="dis" :placeholder="$t('m.distance')">
            <span slot="suffix" class="input-suffix__ light-color">m</span>
          </el-input>
          <div class="item-title">{{$t('m.pitch')}}：</div>
          <el-input style="width: 120px;" v-model="pitch" :placeholder="$t('m.pitch')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
        </div>
      </div>
      <div class="correct-option">
        <div class="correct-button" @click.prevent="calc" style="margin-top: -26px;">
          <img class="link-button" src="../../assets/gis/calc.png" alt="">
          <img class="img-right" style="vertical-align: 28%" src="../../assets/gis/right.png" alt="">
        </div>
      </div>
      <div class="correct-result" style="width: 200px;height: 26px;padding:3px 20px;">
        <span class="result-title">{{$t('m.alt')}}：</span>
        <span class="result-value">
            <span ellipsis>{{(result>10000)?`${(result/1000).toFixed(2)}`:`${result.toFixed(2)}`}}</span>
            <span lightblue>{{(result>10000)?' km':' m'}}</span>
          </span>
      </div>
    </div>
  </div>
</template>

<script>
  import {http_request,convert_height} from "../../modes/api";
  import {exceOnce, validationNumber} from "../../modes/tool";

  export default {
    name: "CalcHeight",
    data() {
      return {
        pitch: undefined,
        dis: undefined,
        result: 0,
        error: undefined
      }
    },
    methods: {
      calc() {
        const {pitch, dis} = this;
        if (!validationNumber([
          {
            value: pitch, func: f => {
              return f < 90 && f > -90
            }
          }, {
            value: dis, func: f => {
              return f < 99999999 && f > 0
            }
          }])) {
          this.error =  this.$t('m.wrongValueType');
          exceOnce(() => {
            this.error = undefined;
          });
          return;
        }
        http_request(convert_height, {
          pitch: parseFloat(pitch),
          dis: parseFloat(dis)
        }, data => {
          this.result = data;
        });
      },
      reset() {
        this.pitch = undefined;
        this.dis = undefined;
        this.result = 0;
      }
    }
  }
</script>

<style scoped>

</style>
