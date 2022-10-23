<template>
  <transition name="fade">
    <div class="drawer-box__">
      <div class="list-box">
        <div class="list-title-box">
          <span class="bbox list-title" is-title><img src="../../assets/table/title-arrow.png" alt="">{{$t('m.info')}}</span>
        </div>
        <div v-if="info" class="info list-container" style="margin-bottom:0px;padding: 45px 0px 20px">
          <div class="tb-border-lt"></div>
          <div class="tb-border-rt"></div>
          <div class="tb-border-rb" style="bottom: 46px"></div>
          <div class="tb-border-lb" style="bottom: 46px"></div>
          <div class="info-box__">
            <div class="base-info-box__">
              <div class="__group-box__">
                <div class="device-icon-box__">
                  <img src="../../assets/device/device-icon.png" alt="">
                </div>
                <div class="device-info-box__">
                  <div class="device-title__">{{info.name}}</div>
                  <div class="device-info-item__">ID-{{info.id}}</div>
                  <div class="device-info-item__">
                    <img src="../../assets/device/position.png" alt="">
                    <span>[{{info.lng.toFixed(6)}},{{info.lat.toFixed(6)}}]</span>
                  </div>
                  <div class="device-info-item__">
                    <img src="../../assets/device/alt.png" alt="">
                    <span>{{info.alt}}m</span>
                  </div>
                </div>
              </div>
              <div class="__group-box__" style="flex-direction: column">
                <div class="device-info-item__">
                  <span class="bold-font__">{{$t('m.device')}}：</span>
                  <span>{{info.ip}}:{{info.port}}</span>
                </div>
                <div class="device-info-item__">
                  <span class="bold-font__">{{$t('m.sevice')}}：</span>
                  <span>{{info.lip}}:{{info.lport}}</span>
                </div>
              </div>

              <div class="__group-box__">
                <div class="group-item-box__">
                  <div class="device-info-item__">
                    <span class="bold-font__">{{$t('m.timeout')}}：</span>
                    <span>{{info.targetTimeOut}}"</span>
                  </div>
                  <div class="device-info-item__">
                    <span class="bold-font__">{{$t('m.statusReport')}}：</span>
                    <span>{{info.statusReportingInterval}}"</span>
                  </div>
                </div>
                <div class="group-item-box__">
                  <div class="device-info-item__">
                    <span class="bold-font__">{{$t('m.targetReport')}}：</span>
                    <span>{{info.probeReportingInterval}}"</span>
                  </div>
                  <div class="device-info-item__">
                    <span class="bold-font__">{{$t('m.threatDetermin')}}：</span>
                    <span>{{info.threadAssessmentCount}}</span>
                  </div>
                </div>
              </div>

              <div class="__group-box__">
                <div class="group-item-box__" style="height: 80px;display: flex;flex-direction: row">
                  <div class="bold-font__" style="line-height: 80px;">{{$t('m.rectify')}}</div>
                  <div class="group-middle__"></div>
                  <div>
                    <div class="device-info-item__" style="line-height: 40px;">
                      <img src="../../assets/device/el.png" style="vertical-align: -10%" alt="">
                      <span>{{info.rectifyAz}}°</span>
                    </div>
                    <div class="device-info-item__" style="line-height: 40px;">
                      <img src="../../assets/device/az.png" style="vertical-align: -12%" alt="">
                      <span>{{info.rectifyEl}}°</span>
                    </div>
                  </div>
                </div>
                <div class="group-item-box__ cover-bg__" style="position: relative">
                  <div style="position: absolute;top: 10px;right: 0px;">
                    <div class="device-info-item__ bold-font__">{{info.coverS}}°-{{info.coverE}}°</div>
                    <div class="device-info-item__">R:{{info.coverR}}m</div>
                  </div>
                </div>
              </div>

              <div class="__group-box__" style="flex-direction: column">
                <div class="device-info-item__">
                  <span class="bold-font__">{{$t('m.creatTime')}}：</span>
                  <span>{{info.createTime}}</span>
                </div>
                <div class="device-info-item__">
                  <span class="bold-font__">{{$t('m.modifyTime')}}：</span>
                  <span>{{info.updateTime}}</span>
                </div>
              </div>

            </div>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
  import "../../assets/device/info.css"
  import {mapState} from 'vuex'

  export default {
  name: "Viewer",
  data(){
    return {
      users:[],
      serverUser:{},
      zones:[]
    }
  },
  computed:{
    ...mapState(['device_categories']),
    category_name(){
      if(this.device_categories){
        let mode=this.device_categories.find(f=>f.id===this.info.category);
        return mode?mode.name:this.$t('m.unknown');
      }
      return this.$t('m.unknown')
    }
  },
  methods:{
    InitView(){
      const {id} =this.info
      this.users=[];
    }
  },
  props: {
    info: {
      required: true
    }
  }
};
</script>
<style scoped src="../../assets/table/table.css"></style>
