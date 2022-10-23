<template>
  <!--        根据绝对位置计算相对位置-->
  <div class="correct-box" style="height: 140px;">
    <div class="correct-header">
      <span class="correct-title">{{$t('m.relativePositionCalculatedAbsolutePosition')}}</span>
      <span class="correct-reset" @click.prevent="reset"><img class="link-button" src="../../assets/gis/reset.png" alt=""></span>
      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
      </transition>
    </div>
    <div class="correct-body">
      <div class="correct-left">
        <div class="condition-item">
          <div class="item-title" style="width: 100px">{{$t('m.startPoint')}}：</div>
          <el-input style="width: 116px;" v-model="center.lng" :placeholder="$t('m.lng')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <el-input style="width: 116px; margin: 0 10px;" v-model="center.lat" :placeholder="$t('m.lat')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <el-input style="width: 116px;" v-model="center.altitude" :placeholder="$t('m.alt')">
            <span slot="suffix" class="input-suffix__ light-color">m</span>
          </el-input>
        </div>
        <div class="condition-item">
          <div class="item-title" style="width: 100px">{{$t('m.targetPoint')}}：</div>
          <el-input style="width: 116px;" v-model="target.lng" :placeholder="$t('m.lng')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <el-input style="width: 116px; margin: 0 10px;" v-model="target.lat" :placeholder="$t('m.lat')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <el-input style="width: 116px;" v-model="target.altitude" :placeholder="$t('m.alt')">
            <span slot="suffix" class="input-suffix__ light-color">m</span>
          </el-input>
        </div>
      </div>
      <div class="correct-option">
        <div class="correct-button" style="margin-top: -10px;" @click.prevent="calc">
          <img class="link-button" src="../../assets/gis/calc.png" alt="">
          <img class="img-right" style="vertical-align: 28%" src="../../assets/gis/right.png" alt="">
        </div>
      </div>
      <div class="correct-result" style="width: 200px;margin-top: -30px;padding: 5px 10px;">
        <p class="result-item">
          <span class="result-title">{{$t('m.position')}}：</span>
          <span class="result-value">{{result.az.toFixed(2)}}<span lightblue> °</span></span>
        </p>
        <p class="result-item">
          <span class="result-title">{{$t('m.pitch')}}：</span>
          <span class="result-value">{{result.el.toFixed(2)}}<span lightblue> °</span></span>
        </p>
        <p class="result-item">
          <span class="result-title">{{$t('m.distance')}}：</span>
          <span class="result-value">
            {{(result.dis>10000)?`${(result.dis/1000).toFixed(2)}`:`${(result.dis).toFixed(2)}`}}
            <span lightblue>{{(result.dis>10000)?' km':' m'}}</span>
          </span>
        </p>
        <p class="result-item">
          <span class="result-title">{{$t('m.alt')}}：</span>
          <span class="result-value">{{result.ad.toFixed(2)}}<span lightblue> m</span></span>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
  import {convert_point_info,http_request} from '../../modes/api'
  import {exceOnce, isLat, isLng, validationNumber} from "../../modes/tool";

  export default {
    name: "AbsToRel",
    data() {
      return {
        error: undefined,
        target: {},
        center: {},
        result: {
          az: 0,
          el: 0,
          dis: 0,
          ad: 0
        }
      }
    },
    methods: {
      calc() {
        const {target, center} = this;
        //整体数据验证
        if (!validationNumber([
          {
            value: target.lat,
            func: f => isLat(f)
          },
          {
            value: target.lng,
            func: f => isLng(f)
          },
          {
            value: center.lat,
            func: f => isLat(f)
          },
          {
            value: center.lng,
            func: f =>isLng(f)
          },
          {value: target.altitude},
          {value: center.altitude}
        ])) {
          this.error = this.$t('m.wrongValueType');
          exceOnce(()=>{
            this.error = undefined;
          });
          return;
        }
        let info = {
          target: {
            lat: parseFloat(target.lat),
            lng: parseFloat(target.lng),
            altitude: parseFloat(target.altitude)
          },
          center: {
            lat: parseFloat(center.lat),
            lng: parseFloat(center.lng),
            altitude: parseFloat(center.altitude)
          }
        };
        http_request(convert_point_info, info, data => {
          this.result = data;
        });
      },
      reset() {
        this.target = {};
        this.center = {};
        this.result = {
          az: 0,
          el: 0,
          dis: 0,
          ad: 0
        };
      }
    }
  }
</script>

<style scoped>

</style>
