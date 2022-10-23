<template>
      <!--      内容-->
      <div class="correct-box">
        <div class="correct-header">
          <span class="correct-title">  {{$t('m.relativeCalibraNorth')}}</span>
          <span class="correct-reset" @click.prevent="reset">
            <img class="link-button" src="../../assets/gis/reset.png" alt="">
          </span>
          <transition name="fade">
            <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
          </transition>
        </div>
        <div class="correct-body">
          <div class="correct-left">
            <div class="condition-item">
              <div class="item-title">{{$t('m.devicePointA')}}：</div>
              <div class="item-data ellipsis" :class="[!deviceA?'light-color':'']" style="width: 300px">
                <p v-if="deviceA">
                  <span>E</span>{{deviceA.lng.toFixed(6)}}<span class="light-color">°</span>,
                  <span>W</span>{{deviceA.lat.toFixed(6)}}<span class="light-color">°</span>,
                  <span>H</span>{{deviceA.alt.toFixed(2)}}<span class="light-color">m</span>
                </p>
                <p v-else>{{$t('m.lng')}}、{{$t('m.lat')}}、{{$t('m.alt')}}</p>
              </div>
              <el-select class="item-devs" v-model="deviceAId" :placeholder="$t('m.select')">
                <el-option
                  v-for="item in devices"
                  :key="item.id"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            <div class="condition-item">
              <div class="item-title">{{$t('m.devicePointB')}}：</div>
              <div class="item-data ellipsis" :class="[!deviceB?'light-color':'']" style="width: 300px">
                <p v-if="deviceB">
                  <span>E</span>{{deviceB.lng.toFixed(6)}}<span class="light-color">°</span>,
                  <span>W</span>{{deviceB.lat.toFixed(6)}}<span class="light-color">°</span>,
                  <span>H</span>{{deviceB.alt.toFixed(2)}}<span class="light-color">m</span>
                </p>
                <p v-else>{{$t('m.lng')}}、{{$t('m.lat')}}、{{$t('m.alt')}}</p>
              </div>
              <el-select class="item-devs" v-model="deviceBId" :placeholder="$t('m.select')">
                <el-option
                  v-for="item in devices"
                  :key="item.id"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            <div class="condition-item">
              <div class="item-title">{{$t('m.distance')}}：</div>
              <el-input style="width: 120px;" v-model="dis" :placeholder="$t('m.distance')">
                <span slot="suffix" class="input-suffix__ light-color">m</span>
              </el-input>
              <div class="item-title" style="margin-left: 30px;">{{$t('m.position')}}：</div>
              <el-input style="width: 120px;" v-model="az" :placeholder="$t('m.distance')">
                <span slot="suffix" class="input-suffix__ light-color">°</span>
              </el-input>
            </div>
          </div>
          <div class="correct-option" style="margin-left: -20px;">
            <div class="correct-button" @click.prevent="calc">
              <img class="link-button" src="../../assets/gis/calc.png" alt="">
              <img class="img-right" style="vertical-align: 28%" src="../../assets/gis/right.png" alt="">
            </div>
          </div>
          <div class="correct-result" style="padding-left: 10px;padding-top: 14px;;height: 140px;width: 200px">
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
                {{(result.dis>10000)?(result.dis/1000).toFixed(2):(result.dis).toFixed(2)}}
                <span lightblue>{{(result.dis>10000)?" km":" m"}}</span>
              </span>
            </p>
            <p class="result-item">
              <span class="result-title">{{$t('m.height')}}：</span>
              <span class="result-value">{{result.ad.toFixed(2)}}<span lightblue> m</span></span>
            </p>
          </div>
        </div>
      </div>
</template>

<script>
  import {http_request,request_base_devices,convert_correct_by_relative} from "../../modes/api";
  import {exceOnce, isAz, validationNumber} from "../../modes/tool";
  export default {
    name: "RelativeCorrect",
    data() {
      return {
        devices: [],
        deviceAId: undefined,
        deviceBId: undefined,
        dis: undefined,
        az: undefined,
        result: {
          az: 0,
          el: 0,
          dis: 0,
          ad: 0
        },
        error: undefined
      }
    },
    computed: {
      deviceA() {
        if (this.deviceAId) {
          return this.devices.filter(f => f.id === this.deviceAId)[0];
        }
      },
      deviceB() {
        if (this.deviceBId) {
          return this.devices.filter(f => f.id === this.deviceBId)[0];
        }
      }
    },
    methods: {
      calc() {
        const {deviceA, deviceB, dis, az} = this;
        if (!deviceA || !deviceB) {
          this.error = this.$t('m.needToSelectDeviceFirst');
          exceOnce(() => {
            this.error = undefined;
          });
          return;
        }
        if (!validationNumber([
          {value: dis},
          {value: az, func: f => isAz(f)}
        ])) {
          this.error = this.$t('m.inputParameterError');
          exceOnce(() => {
            this.error = undefined;
          });
          return;
        }
        http_request(convert_correct_by_relative, {
          center: {
            lat: deviceA.lat,
            lng: deviceA.lng,
            altitude: deviceA.alt
          },
          target: {
            lat: deviceB.lat,
            lng: deviceB.lng,
            altitude: deviceB.alt
          },
          dis: parseFloat(dis),
          az: parseFloat(az)
        }, data => {
          this.result = data;
        })
      },
      reset() {
        this.deviceAId = undefined;
        this.deviceBId = undefined;
        this.dis = 0;
        this.az = 0;
        this.result = {
          az: 0,
          el: 0,
          dis: 0,
          ad: 0
        }
      }
    },
    mounted() {
      http_request(request_base_devices, null, data => {
        this.devices = data;
      });
    }
  }
</script>
