<template>
  <!--        根据绝对位置计算相对位置-->
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" style="width: 240px" is-title>
        <img src="../../assets/table/title-arrow.png" alt=""> {{$t('m.association')}}
      </span>
    </div>
    <!--v-if="device_ships&&device_ships.length"-->
    <div class="list-container" style="padding-left:30px;padding-right:30px;padding-bottom: 8px;">
      <div class="tb-border-lt"></div>
      <div class="tb-border-rt"></div>
      <div class="tb-border-rb" style="bottom: 30px;"></div>
      <div class="tb-border-lb" style="bottom: 30px;"></div>
      <div class="relationship-box">
        <div class="list-scroll-box" id="container">
          <vue-scroll :ops="options" v-if="!nodata" >
            <div class="scroll-box">
              <div v-if="device_relation_ships&&device_relation_ships.length">
                <div class="relationship-item" v-for="(ship,key) in device_relation_ships" :key="key">
                  <div class="relationship-from-device is-radar">
                    <span>ID:{{ship.fromDeviceId}}</span>
                  </div>
                  <div style="flex: 1" to-right></div>
                  <div class="target-uav ">
                    <span class="ellipsis">{{ship.targetId}}</span>
                  </div>
                  <div style="flex: 2;" to-left></div>
                  <div class="button-break-ship link-button" @click.stop="breakShip(ship)"></div>
                  <div style="flex: 2;" to-right></div>
                  <div class="relationship-to" :class="`ship_${ship.rType}`">
                    <span v-if="ship.rType===0">{{`${ship.toAddressIp}:${ship.toAddressPort}`}}</span>
                    <span v-else>ID:{{ship.toDeviceId}}</span>
                  </div>
                </div>
              </div>
            </div>
          </vue-scroll>
          <div v-if="nodata" class="no-data-box">{{$t('m.unAssociation')}}</div>
        </div>
      </div>

    </div>
  </div>
</template>

<script>
  import {mapState} from 'vuex'
  import {http_request, break_target_transmit, break_hit_target, break_monitor_target} from "../../modes/api";
  import {msg_enum, show_confirm, show_message} from "../../modes/elementUI";

  export default {
      name: "Relationship",
      data() {
        return {
          name: undefined,
          address: undefined,
          error: undefined,
          options:{
            keepShow: false,
            bar: {
              keepShow: true,
              opacity: 0.5,
              onlyShowBarOnScroll: false, //是否只有滚动的时候才显示滚动条
              background: '#43DCFF'
            },
            rail: {
              opacity: 0,
              size: '4px',
            }
          }
        }
      },
      methods: {
        breakShip(ship) {
          const {rType, id} = ship;
          console.log('shipshipshipshipship',ship);
          show_confirm(this.$t('m.tips'),this.$t('m.SureDis'), () => {
            //目标转发 rType=0
            //1：打击引导；2：监视引导；3：诱骗引导
            let func=undefined;
            if (rType === 0) {
              func=break_target_transmit;
            } else if (rType === 1) {
              func=break_hit_target;
            } else if (rType === 2) {
              func=break_monitor_target;
            }
            if(func){
              http_request(func, id, () => {
                show_message(msg_enum.success,this.$t('m.SucRelease'));
              });
            }
          });
        },
      },
      computed: {
        ...mapState(['device_relation_ships']),
        nodata() {
          let hasGuid = this.device_relation_ships && this.device_relation_ships.length;
          return !hasGuid;
        }
      }
    }
</script>

<style scoped>
  #container {
    position: absolute;
    height: 100%;
    overflow-y: auto;
    width: 100%;
  }
.relationship-box{
  position: relative;
  height: 100%;
  margin-top: -10px;
  overflow:hidden;
}
  .relationship-item {
    height: 48px;
    display: flex;
    flex-direction: row;
    margin-bottom:36px;
    padding-right: 42px;
    position: relative;
  }
  .relationship-item:before{
    content: "";
    height: 66px;
    width: 100%;
    position: absolute;
    background: linear-gradient(to right,transparent,rgba(64,204,249,0.1),transparent);
  }
  .relationship-from-device,.target-uav,.button-break-ship,.relationship-to{
    width: 24px;
    height: 24px;
    margin: auto 10px;
    position: relative;
    background-repeat: no-repeat;
    background-position: center center;
    background-size: auto;
  }
.target-uav,.button-break-ship{
  background-size: contain !important;
}
.relationship-from-device{
  background-size: contain;
}
.relationship-from-device span,.target-uav span,.relationship-to span{
  position: absolute;
  bottom: -24px;
  left: 50%;
  font-size: 13px;
  transform: translateX(-50%);
}
div[to-right],div[to-left]{
  position: relative;
  margin: 0 10px;
}
div[to-right]:after,div[to-left]:after{
  position: absolute;
  content: "";
  height: 0px;
  width: 0px;
  border-width: 4px;
  border-style: solid;
  top: 50%;
  margin-top: -4px;
}
div[to-right]:before,div[to-left]:before{
  position: absolute;
  content: "";
  height: 2px;
  width: 100%;
  top: 50%;
  margin-top: -1px;
}
div[to-right]:before{
  background: linear-gradient(to right,transparent,rgba(64,204,249,0.4));
}
div[to-left]:before{
  background: linear-gradient(to left,transparent,rgba(64,204,249,0.4));
}
div[to-right]:after{
  border-color: transparent transparent transparent rgba(64,204,249,0.4) ;
  right: -8px;
}
div[to-left]:after{
  border-color: transparent rgba(64,204,249,0.4) transparent transparent ;
  left: -8px;
}
.target-uav,.relationship-to{
  height: 36px;
  width: 36px;
  background: url("../../assets/home/uav.png");
}
.relationship-to[isValid='false'],relationship-to[isValid='false'] span{
  opacity: 0.6;
}
.target-uav span,.relationship-to span{
  bottom: -18px;
}
.relationship-to{
  background-size: contain;
  background-repeat: no-repeat;
  background-position: center center;
  background-image: url("../../assets/home/link-target.png");
}
.ship_0{
  background-image: url("../../../static/res/ship/ship_0.png");
}
  .ship_1{
    background-image: url("../../../static/res/ship/ship_1.png");
  }
  .ship_2{
    background-image: url("../../../static/res/ship/ship_2.png");
  }
  .ship_3{
    background-image: url("../../../static/res/ship/ship_3.png");
  }
.button-break-ship{
  cursor: pointer;
  height: 30px;
  width: 30px;
  margin: auto;
  background-image: url("../../assets/home/link.png");
}
.button-break-ship:hover{
  background-image: url("../../assets/home/link-break.png");
  transition: all 0.5s;
}
.relationship-from-device.is-radar{
  background-image: url("../../assets/home/leida.png");
}
.relationship-from-device.is-video{
  background-image: url("../../assets/home/jingtou.png");
}
  .no-data-box {
    background-color: rgba(64, 204, 249, 0.05);
    text-align: center;
    line-height: 190px;
    height: 200px;
    font-size: 18px;
    font-weight: bold;
    margin-top: 10px;
    border-radius: 2px;
    position: relative;
  }
</style>
