<template>
  <div class="set-panel">
    <div class="title" style="margin-top: 12px;padding-bottom: 12px">
      <span style="margin-left: 130px">等距线</span>
      <span style="margin-left: -10px">覆盖范围</span>
    </div>

    <div class="item" style="margin-top: 10px;padding-bottom: 8px;padding-top: 8px" v-if="devices.length">
      <div v-for="(dev,key) in devices" :key="key" v-show="dev.category<20000">
        <div style="margin-top: 0px">
          <span class="item-name">{{dev.name}} :</span>
          <v-swtich :value="dev.line" :fid="dev.id.toString()" id="dev" :index="1" class="item-switch"
                    style="margin-left: 24px;"
                    :handle="false" @changSwtichOn="changSwtichOn" @changSwtichOff="changeSwtichOff"/>
          <v-swtich :value="dev.range" :fid="dev.id.toString()" :index="2" class="item-switch"
                    style="margin-left: 15px;"
                    :handle="false" @changSwtichOn="changSwtichOn" @changSwtichOff="changeSwtichOff"/>
        </div>
      </div>
    </div>

    <div class="item" style="padding-bottom: 8px;padding-top: 8px" v-if="devices.length">
      <div v-for="(dev,key) in devices" :key="key" v-show="dev.category<30000 && dev.category>20000">
        <div style="margin-top: 0px">
          <span class="item-name">{{dev.name}} :</span>
          <v-swtich :value="dev.line" :fid="dev.id.toString()" :index="1" class="item-switch"
                    style="margin-left: 24px;"
                    :handle="false" @changSwtichOn="changSwtichOn" @changSwtichOff="changeSwtichOff"/>
          <v-swtich :value="dev.range" :fid="dev.id.toString()" :index="2" class="item-switch"
                    style="margin-left: 15px;"
                    :handle="false" @changSwtichOn="changSwtichOn" @changSwtichOff="changeSwtichOff"/>
        </div>
      </div>
    </div>

    <div class="item" style="padding-bottom: 8px;padding-top: 8px" v-if="devices.length">
      <div v-for="(dev,key) in devices" :key="key" v-show="dev.category>30000">
        <span class="item-name">{{dev.name}}:</span>
        <v-swtich :value="dev.line" :fid="dev.id.toString()" :index="1" class="item-switch"
                  style="margin-left: 24px;"
                  :handle="false" @changSwtichOn="changSwtichOn" @changSwtichOff="changeSwtichOff"/>
        <v-swtich :value="dev.range" :fid="dev.id.toString()" :index="2" class="item-switch"
                  style="margin-left: 15px;"
                  :handle="false" @changSwtichOn="changSwtichOn" @changSwtichOff="changeSwtichOff"/>
      </div>
    </div>
  </div>
</template>

<script>
  import {mapActions, mapState} from "vuex";
  import VSwtich from './Swtich/VSwtich.vue'
  import {show_message, msg_enum} from "./../modes/elementUI";
  import bus from "../modes/tool/bus";
  import {HiddenDevWarnLine, HiddenDevWarnRound} from "../map/mapHandle/mapDraw"

  export default {
    name: "SettingPanel",
    data() {
      return {
        value: true,
        // counterDevices: [],
        // trackDevices: [],
        // probeDevices: [],
      }
    },
    methods: {
      ...mapActions(['set_dev_alarm_status']),
      changSwtichOn(checked, item, index) {
        console.log('index', index);

        if (index == 1) {
          console.log("开启等距线");
          show_message(msg_enum.success,"设备ID:"+item+" 开启等距线");
        }
        else if (index == 2) {
          console.log("开启告警区域");
          show_message(msg_enum.success,"设备ID:"+item+" 开启告警区域");
        }

      },
      changeSwtichOff(checked, item, index) {
        if (index == 1)
        {
          show_message(msg_enum.success, "设备ID:"+item+" 关闭等距线");
        } else if (index == 2)
        {
          show_message(msg_enum.success, "设备ID:"+item+" 关闭告警区域");
        }
      }
    },
    computed: {
      ...mapState(['device_status']),
      ...mapState(['devices']),
      ...mapState(['device_categories']),
    },
    components:
      {
        VSwtich: VSwtich,
      },
    mounted() {
      // bus.$on('ResetDeviceFilter', (Filter) =>
      // {
      //   console.log("Filter", Filter);
      //
      //   Filter.CoverList.forEach(info => {
      //     //this.set_dev_alarm_status({did: info, type: 'CoverList', data: false});
      //     HiddenDevWarnRound(this.fid, false);
      //   })
      //
      //   Filter.LineList.forEach(info => {
      //     //this.set_dev_alarm_status({did: info, type: 'LineList', data: false});
      //     HiddenDevWarnLine(this.fid, false);// 隐藏告警等距线
      //   })
      // })

    }

  }
</script>

<style scoped>
  .item {
    padding-bottom: 8px;
    padding-top: 8px;
    width: 300px;
    margin-top: 20px;
    margin-left: -10px;
    box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.2);
  }

  .set-panel {
    overflow: hidden;
    padding: 10px 10px;
  }

  .title span:first-child {
    margin-left: 110px;
  }

  .title span {
    font-family: "Microsoft YaHei";
    line-height: 1;
    font-size: 16px;
    font-weight: 800;
    color: #ff0;
    padding: 0 10px;
  }

  .item .item-name {
    font-family: "Microsoft YaHei";
    text-align: right;
    display: inline-block;
    color: #40CEF9;
    line-height: 35px;
    font-size: 16px;
    font-weight: 800;
    width: 130px;
  }

  .item-switch {
    display: inline-block;
  }
</style>
