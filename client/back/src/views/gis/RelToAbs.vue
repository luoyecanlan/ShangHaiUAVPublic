<template>
  <!--        根据相对位置计算绝对位置-->
  <div class="correct-box" style="height: 140px;">
    <div class="correct-header">
      <span class="correct-title">{{$t('m.absolutePositionCalculatedRelativePosition')}}</span>
      <span class="correct-reset" @click.prevent="reset"><img class="link-button" src="../../assets/gis/reset.png" alt=""></span>
      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
      </transition>
    </div>
    <div class="correct-body">
      <div class="correct-left">
        <div class="condition-item">
          <div class="item-title" style="width: 100px">{{$t('m.lng')}}：</div>
          <el-input style="width: 120px;" v-model="center.lng" :placeholder="$t('m.lng')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
          <div class="item-title">{{$t('m.lat')}}：</div>
          <el-input style="width: 120px;" v-model="center.lat" :placeholder="$t('m.lat')">
            <span slot="suffix" class="input-suffix__ light-color">°</span>
          </el-input>
        </div>
        <div class="condition-item">
          <div class="item-title" style="width: 100px">{{$t('m.distance')}}：</div>
          <el-input style="width: 120px;" v-model="dis" :placeholder="$t('m.alt')">
            <span slot="suffix" class="input-suffix__ light-color">m</span>
          </el-input>
          <div class="item-title">{{$t('m.position')}}：</div>
          <el-input style="width: 120px;" v-model="az" :placeholder="$t('m.alt')">
            <span slot="suffix" class="input-suffix__ light-color">m</span>
          </el-input>
        </div>
      </div>
      <div class="correct-option">
        <div class="correct-button" @click.prevent="calc" style="margin-top: -10px;">
          <img class="link-button" src="../../assets/gis/calc.png" alt="">
          <img class="img-right" style="vertical-align: 28%" src="../../assets/gis/right.png" alt="">
        </div>
      </div>
      <div class="correct-result" style="width: 230px;height: 76px;padding: 10px">
        <p class="result-item">
          <span class="result-title">{{$t('m.lng')}}：</span>
          <span class="result-value">{{result.lng.toFixed(5)}}<span lightblue> °</span></span>
        </p>
        <p class="result-item">
          <span class="result-title">{{$t('m.lat')}}：</span>
          <span class="result-value">{{result.lat.toFixed(5)}}<span lightblue> °</span></span>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
  import {http_request,convert_lat_lng} from "../../modes/api";
  import {exceOnce, isAz, isLat, isLng, validationNumber} from "../../modes/tool";
  export default {
    name: "RelToAbs",
    data() {
      return {
        center: {},
        az: undefined,
        dis: undefined,
        result: {
          lat: 0, lng: 0
        },
        error:undefined
      }
    },
    methods: {
      calc() {
        const {center, az, dis} = this;
        if(!validationNumber([
          {
            value: center.lat,
            func: f => isLat(f)
          },
          {
            value: center.lng,
            func: f => isLng(f)
          },
          {
            value: dis
          },
          {
            value: az,
            func: f => isAz(f)
          }
        ])){
          this.error = this.$t('m.wrongValueType');
          exceOnce(() => {
            this.error = undefined;
          })
          return;
        }
        http_request(convert_lat_lng, {
          center: {
            lat: parseFloat(center.lat),
            lng: parseFloat(center.lng)
          },
          az: parseFloat(az),
          dis: parseFloat(dis)
        }, data => {
          this.result = data;
        })
      },
      reset() {
        this.center = {lat: undefined, lng: undefined};
        this.az = undefined;
        this.dis = undefined;
      }
    }
  }
</script>

<style scoped>

</style>
