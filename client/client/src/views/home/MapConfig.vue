<template>
  <div class="correct-box">
    <div class="correct-header">
      <span class="correct-title">
        <span class="button-map-switch" @click.prevent="offline" :class="!isOnline?'button-checked':''">{{$t('m.offLineMap')}} </span>
        <span>|</span>
        <span class="button-map-switch" @click.prevent="online" :class="isOnline?'button-checked':''">{{$t('m.onlineMap')}} </span>
      </span>
      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
      </transition>
    </div>
    <div class="correct-body" style="margin-top: -16px;flex-direction: column" v-if="mapConfig">
      <div>
        <el-input v-model="mapConfig.url"
                  :placeholder="$t('m.onlineMap')"
                  @keyup.enter.native="saveOnChange"
                  @change="saveOnChange"
                  style="width: 360px;">
          <i slot="suffix" class="el-icon-close link-button light-color"
             @click.prevent="clearOnUrl" style="line-height: 26px;"></i>
        </el-input>
      </div>
      <div style="display: flex;flex-direction: row;margin-top: 16px;">
        <el-input class="offset-right" v-model="mapConfig.zoomMax"
                  @keyup.enter.native="saveOnChange" @change="saveOnChange" :title="$t('m.ZoomValueUpperLimit')"/>
        <el-input class="offset-right" v-model="mapConfig.zoomMin"
                  @keyup.enter.native="saveOnChange"  @change="saveOnChange" :title="$t('m.ZoomValueLowerLimit')"/>
        <el-input class="offset-right" v-model="mapConfig.zoomDefault"
                  @keyup.enter.native="saveOnChange"  @change="saveOnChange" :title="$t('m.ZoomValueUpperDefault')"/>
      </div>
      <div style="display: flex;flex-direction: row;margin-top: 16px;">
        <el-input class="offset-right" v-model="mapConfig.boundaryMaxLng"
                  @keyup.enter.native="saveOnChange"  @change="saveOnChange" :title="$t('m.longitudeUpperLimit')"/>
        <el-input class="offset-right" v-model="mapConfig.boundaryMinLng"
                  @keyup.enter.native="saveOnChange"  @change="saveOnChange" :title="$t('m.longitudeLowerLimit')"/>
        <el-input class="offset-right" v-model="mapConfig.boundaryMaxLat"
                  @keyup.enter.native="saveOnChange"  @change="saveOnChange" :title="$t('m.latitudeUpperLimit')"/>
        <el-input class="offset-right" v-model="mapConfig.boundaryMinLat"
                  @keyup.enter.native="saveOnChange"  @change="saveOnChange" :title="$t('m.latitudeLowerLimit')"/>
      </div>
    </div>
  </div>
</template>

<script>
  import {http_fast_request, http_request, request_map_config, update_map_config} from "../../modes/api";
  import {exceOnce, isLat, isLng, validation, validationNumber} from "../../modes/tool";
  import {msg_enum, show_message} from "../../modes/elementUI";

    export default {
      name: "MapConfig",
      data() {
        return {
          onlineKey: 'onlinemapconfig',
          offlineKey: 'offlineconfig',
          configs:[],
          mapConfig: undefined,
          // onlineMapConfig: {
          //   url: undefined,
          //   zoomMax: undefined,
          //   zoomMin: undefined,
          //   zoomDefault: undefined,
          //   maxLng: undefined,
          //   maxLat: undefined,
          //   minLng: undefined,
          //   minLat: undefined
          // },
          // offlineMapConfig: {
          //   url: undefined,
          //   zoomMax: undefined,
          //   zoomMin: undefined,
          //   zoomDefault: undefined,
          //   maxLng: undefined,
          //   maxLat: undefined,
          //   minLng: undefined,
          //   minLat: undefined
          // },
          isOnline: false,
          error: undefined
        }
      },
      methods: {
        clearOnUrl(){
          this.mapConfig.url=undefined;
        },
        validationConfig(config,msg,callback){
          const {
            url, zoomMax, zoomMin, zoomDefault, boundaryMaxLng, boundaryMaxLat, boundaryMinLng, boundaryMinLat
          }=config;
          if(!url||url=='') {
            this.error=msg;
            exceOnce(() => {
              this.error = undefined
            });
            return;
          }
          if(!validationNumber([
            {value:parseInt(zoomMax),func:f=>{return f<24&&f>1}},
            {value:parseInt(zoomMin),func:f=>{return f<24&&f>1}},
            {value:parseInt(zoomDefault),func:f=>{return f<24&&f>1}},
            {value:parseFloat(boundaryMaxLat),func:f=>isLat(f)},
            {value:parseFloat(boundaryMaxLng),func:f=>isLng(f)},
            {value:parseFloat(boundaryMinLat),func:f=>isLat(f)},
            {value:parseFloat(boundaryMinLng),func:f=>isLng(f)}
          ])) {
            this.error = msg;
            exceOnce(() => {
              this.error = undefined
            });
            return;
          }
          callback&&callback();
        },
        saveOnChange(){
          this.validationConfig(this.mapConfig,this.$t('m.wrongValueType') ,()=>{
            const {
              id,url,name,zoomMax, zoomMin, zoomDefault, boundaryMaxLng, boundaryMaxLat, boundaryMinLng, boundaryMinLat
            }=this.mapConfig;
            http_request(update_map_config,{
              id,url,name,zoomMax:parseInt(zoomMax),zoomDefault:parseInt(zoomDefault),zoomMin:parseFloat(zoomMin),
              boundaryMinLng:parseFloat(boundaryMinLng),
              boundaryMinLat:parseFloat(boundaryMinLat),
              boundaryMaxLng:parseFloat(boundaryMaxLng),
              boundaryMaxLat:parseFloat(boundaryMaxLat)
            },()=>{
              show_message(msg_enum.success, this.$t('m.modifySuccess'));
              this.initData();
            });
          });
        },
        offline() {
          this.isOnline = false;
          this.mapConfig=this.filterConfig(this.offlineKey);
        },
        online() {
          this.isOnline = true;
          this.mapConfig=this.filterConfig(this.onlineKey);
        },
        filterConfig(key) {
          const {configs} = this;
          if (configs && configs.length > 0) {
            return configs.filter(f => f.name === key)[0];
          }
        },
        initData(callback) {
          const {onlineKey, offlineKey} = this;
          http_fast_request(request_map_config, null, data => {
            this.configs = data;
            callback&&callback();
          });
        }
      },
      mounted() {
        this.initData(()=>{
          this.online();
        });

      }
    }
</script>

<style scoped>
.button-map-switch{
  cursor: pointer;
  color: rgba(64,204,249,0.4);
}
  .button-map-switch:hover{
    color: rgba(64,204,249,0.6);
  }
.button-checked:hover,.button-checked {
  color: #40cef9;
  transition: all 0.2s;
}
 .offset-right{
    margin-right: 10px;
  }
</style>
