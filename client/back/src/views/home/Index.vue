<template>
  <div id="home" class="page-box">
    <div class="home-top">
      <scroll-viewer/>
    </div>
    <div class="home-bottom">
      <div class="home-bottom-left">
        <relationship/>
      </div>
      <div class="home-bottom-right">
        <system-config/>
      </div>
    </div>
  </div>
</template>

<script>
  import ScrollViewer from './ScrollViewer'
  import Build from '../../components/Building'
  import Relationship from "./Relationship";
  import SystemConfig from "./SystemConfig";
  import {mapActions,mapState} from 'vuex'
  import {request_all_devices, http_fast_request, subscribeSignalR, distorySignalR} from "../../modes/api";

  export default {
    name: "Index",
    data() {
      return {
        _refresh_device_handle__: undefined
      }
    },
    computed:{
      ...mapState(['device_categories'])
    },
    methods: {
      ...mapActions(['commit_realtime_devices',
        'commit_realtime_device_status',
        'commit_relation_ships',
        'request_device_categories'
      ]),
      request_realtime_devices() {
        http_fast_request(request_all_devices, null, (devs) => {
          this.commit_realtime_devices(devs);
        });
      },
      beginRefreshDevices() {
        if (!this._refresh_device_handle__) {
          this._refresh_device_handle__ = setInterval(() => {
            this.request_realtime_devices();
          }, 5000);
        }
      },
      closeRefreshDevice() {
        if (this._refresh_device_handle__) {
          clearInterval(this._refresh_device_handle__);
        }
      },
      subscribeDeviceStatus() {
        subscribeSignalR({
          channel: 'lads_channel',
          events: [
            {
              name: 'device_status_channel',
              func: data => {
                this.commit_realtime_device_status(data);
                console.log('device_status_channel', data);
              }
            },
            {
              name: 'device_relation_ship_channel', func: data => {
                this.commit_relation_ships(data);
                console.log('device_relation_ship_channel', data);
              }
            },
            {
              name: 'target_channel', func: data => {
                //this.commit_relation_ships(data);
                //console.log('target_channel', data);
              }
            }
          ]
        });
      }
    },
    components: {
      SystemConfig,
      Relationship,
      BuildWebsite: Build,
      ScrollViewer
    },
    mounted() {
      //设备类型
      if(!this.device_categories||this.device_categories.length==0){
        this.request_device_categories();
      }
      this.request_realtime_devices();
      this.beginRefreshDevices();
      this.subscribeDeviceStatus();
    },
    beforeDestroy() {
      this.closeRefreshDevice();
      distorySignalR();
    }
  }
</script>

<style scoped>
  .page-box{
    display: flex;
    flex-direction: column;
  }
  .home-top{
    flex: 1;
  }
  .home-bottom{
    flex: 1;
    display: flex;
    flex-direction: row;
    margin: 10px 0 0;
  }
  .home-bottom-left{
    width: 680px;
    height: 100%;
    position: relative;
  }
  .home-bottom-right{
    flex: 1;
    margin-left: 20px;
    height: 100%;
  }
</style>
