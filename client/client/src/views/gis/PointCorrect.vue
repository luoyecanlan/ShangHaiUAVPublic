<template>
      <!--      内容-->
      <div class="correct-box">
        <div class="correct-header">
          <span class="correct-title">  {{$t('m.twoPointCalibrationNorth')}}</span>
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
              <div class="item-title" >{{$t('m.devicePoint')}}：</div>
              <div class="item-data ellipsis" :class="[!device?'light-color':'']" style="width: 270px;">
                <p v-if="device">
                  <span>E</span>{{device.lng.toFixed(5)}}<span class="light-color">°</span>,
                  <span>W</span>{{device.lat.toFixed(5)}}<span class="light-color">°</span>,
                  <span>H</span>{{device.alt.toFixed(2)}}<span class="light-color">m</span>
                </p>
                <p v-else>{{$t('m.lng')}}、{{$t('m.lat')}}、{{$t('m.alt')}}</p>
              </div>
              <el-select style="width: 170px;" v-model="deviceId" :placeholder="$t('m.selectDevice')">
                <el-option
                  v-for="item in devices"
                  :key="item.id"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            <div class="condition-item">
              <div class="item-title" style="width: 110px">{{$t('m.targetPoint')}}：</div>
              <el-input style="width: 110px;" v-model="targetLng" :placeholder="$t('m.lng')">
                <span slot="suffix" class="input-suffix__ light-color">°</span>
              </el-input>
              <el-input style="width: 110px; margin: 0 10px;" v-model="targetLat" :placeholder="$t('m.lat')">
                <span slot="suffix" class="input-suffix__ light-color">°</span>
              </el-input>
              <el-input style="width: 110px;" v-model="targetAlt" :placeholder="$t('m.alt')">
                <span slot="suffix" class="input-suffix__ light-color">m</span>
              </el-input>
            </div>
            <div class="condition-item">
              <div class="item-title" style="width: 110px">{{$t('m.direction')}}：</div>
              <el-input style="width: 120px;"  v-model="currentAz" :placeholder="$t('m.position')">
                <span slot="suffix" class="input-suffix__ light-color">°</span>
              </el-input>
              <el-input style="width: 120px;margin-left: 10px;" v-model="currentEl" :placeholder="$t('m.pitch')">
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
          <div class="correct-result" style="padding-left: 10px;width: 150px;height: 160px;">
            <p class="result-item" style="width: 150px">
              <span class="result-title">{{$t('m.horizontalCorr')}}：</span>
            </p>
            <p class="result-item">
              <span class="result-value">{{result.az.toFixed(2)}}<span lightblue> °</span></span>
            </p>
            <p class="result-item">
              <span class="result-title">{{$t('m.PitchCorr')}}：</span>
            </p>
            <p class="result-item">
              <span class="result-value">{{result.el.toFixed(2)}}<span lightblue> °</span></span>
            </p>
          </div>
        </div>
      </div>
</template>

<script>
  import {http_request,request_base_devices,convert_correct_by_points} from "../../modes/api";
  import {msg_enum, show_message} from "../../modes/elementUI";
  import {exceOnce, isAz, isEl, isLat, isLng, validationNumber} from "../../modes/tool";

  export default {
      name: "PointCorrect",
      data() {
        return {
          devices: [],
          deviceId:undefined,
          targetLng:undefined,
          targetLat:undefined,
          targetAlt:undefined,
          currentAz:undefined,
          currentEl:undefined,
          result: {
            az:0,
            el:0
          },
          error:undefined
        }
      },
    computed:{
      device(){
        if(this.deviceId){
          return this.devices.filter(f=>f.id===this.deviceId)[0];
        }
      }
    },
    methods:{
        calc() {
          const {device, targetLng, targetLat, targetAlt, currentAz, currentEl} = this;
          if(!device){
            this.error = this.$t('m.needToSelectDeviceFirst');
            exceOnce(() => {
              this.error = undefined;
            });
            return;
          }
          if(!validationNumber([
            {
              value: targetLng,
              func: f => isLng(f)
            },
            {
              value: targetLat,
              func: f =>isLat(f)
            },
            {
              value: targetAlt
            },
            {
              value: currentAz, func: f => isAz(f)
            },
            {
              value: currentEl, func: f => isEl(f)
            },
          ])){
            this.error = this.$t('m.wrongValueType');
            exceOnce(() => {
              this.error = undefined;
            });
            return;
          }
          http_request(convert_correct_by_points, {
            target: {
              lat: parseFloat(targetLat),
              lng: parseFloat(targetLng),
              altitude: parseFloat(targetAlt)
            },
            center: {
              lat: device.lat,
              lng: device.lng,
              altitude: device.alt
            },
            currentAz: parseFloat(currentAz),
            currentEl: parseFloat(currentEl)
          }, data => {
            this.result = data;
          })
        },
      reset() {
        this.targetLng = undefined;
        this.targetLat = undefined;
        this.targetAlt = undefined;
        this.currentAz = undefined;
        this.currentEl = undefined;
        this.deviceId=undefined;
        this.result = {
          az: 0,
          el: 0
        }
      }
    },
    mounted() {
      http_request(request_base_devices,null,data=>{
        this.devices=data;
      });
    }
  }
</script>
