<template>
  <div class="weui-div" @click.stop="toggle">

    <span class="weui-switch"
          :class="{
          'switch_disabled':!disabled,
          'weui-switch-on' : value}"
          :value="value"></span>

    <div v-if='!value' class="weui-div-2"
         :class="{'weui_div_2_disabled':disabled==false}">×
    </div>

    <div v-if='value' class="weui-div-1">✓</div>

  </div>
</template>

<script>
  import {HiddenDevWarnLine, HiddenDevWarnRound} from "../../map/mapHandle/mapDraw"
  import {mapActions, mapState} from "vuex";
  import {update_person_info, http_request_await} from "../../modes/api";

  export default {
    name: "VSwtich",
    props: {
      value: {
        type: Boolean,
        default: true
      },
      disabled: {
        type: Boolean,
        default: true
      },
      fid: {
        type: String,
        default: '',
      },
      index: {
        type: Number,
        default: 11090,
      },
      handle: {
        type: Boolean,
        default: true
      },
    },
    computed:
      {
        ...mapState(['user_config_info']),
      },
    methods: {
      ...mapActions(['change_info_user_config']),
      ...mapActions(['set_dev_alarm_status']),
      toggle() {

        if (!this.disabled) {
          return;
        }
        // 是否可点击
        if (!this.value) {
          if (this.index == '2') {
            HiddenDevWarnRound(this.fid, true);// 显示告警区域

            //移除个性化配置 只显示隐藏的
            let CoverList = JSON.parse(this.user_config_info.deviceCover)

            // 从列表移除该ID
            CoverList = CoverList.filter(t => t != this.fid.toString());
            CoverList = JSON.stringify(CoverList);

            this.change_info_user_config({type: 'deviceCover', data: CoverList});

            this.set_dev_alarm_status({did: this.fid, type: 'CoverList', data: true});

            http_request_await(update_person_info, this.user_config_info);
            console.log('CoverList', CoverList);
          }
          else if (this.index == '1') // 显示告警等距线
          {
            HiddenDevWarnLine(this.fid, true);// 显示告警等距线

            // 从列表移除该ID 只显示隐藏的
            let LineList = JSON.parse(this.user_config_info.deviceLine)

            // 添加设备id到list
            LineList = LineList.filter(t => t != this.fid.toString());
            LineList = JSON.stringify(LineList);

            this.change_info_user_config({type: 'deviceLine', data: LineList});
            this.set_dev_alarm_status({did: this.fid, type: 'LineList', data: true});

            http_request_await(update_person_info, this.user_config_info);
            console.log('更新提交的LineList', LineList);
          }

          this.$emit('changSwtichOn', this.value, this.fid, this.index);
        }
        else {
          if (this.index == '2') // 隐藏告警区域
          {
            HiddenDevWarnRound(this.fid, false);

            // 获取list 转换成json
            let CoverList = [];
            if ('' != this.user_config_info.deviceCover) {
              CoverList = JSON.parse(this.user_config_info.deviceCover);
            }
            CoverList.push(this.fid);
            CoverList = JSON.stringify(CoverList);

            this.change_info_user_config({type: 'deviceCover', data: CoverList});
            this.set_dev_alarm_status({did: this.fid, type: 'CoverList', data: false});

            http_request_await(update_person_info, this.user_config_info);
            console.log('更新提交的CoverList', CoverList);
          }
          else if (this.index == '1')// 隐藏告警等距线
          {
            HiddenDevWarnLine(this.fid, false);// 隐藏告警等距线

            // 获取list 转换成json
            let LineList = [];
            if ('' != this.user_config_info.deviceLine) {
              LineList = JSON.parse(this.user_config_info.deviceLine);
            }
            LineList.push(this.fid);
            LineList = JSON.stringify(LineList);

            this.change_info_user_config({type: 'deviceLine', data: LineList});
            this.set_dev_alarm_status({did: this.fid, type: 'LineList', data: false});
            http_request_await(update_person_info, this.user_config_info);
            console.log('更新提交的LineList', LineList);
          }
          this.$emit('changSwtichOff', this.value, this.fid, this.index);
        }
      }
    }
  }

</script>
<style scoped>
  .weui-div {
    position: relative;
    font-weight: bold;
    cursor: pointer;
    width: 50px;
    height: 16px;
    padding-top: 2px;
  }

  .weui-div-1 {
    padding-top: 2px;
    position: absolute;
    left: 10px;
    top: 0;
    line-height: 16px;
    font-size: 16px;
    color: #43DCFE;
  }

  .weui-div-2 {
    padding-top: 2px;
    position: absolute;
    right: 11px;
    top: 0;
    line-height: 16px;
    font-size: 22px;
    color: #43DCFE;
  }

  .weui_div_2_disabled {
    cursor: not-allowed;
    padding-top: 2px;
    position: absolute;
    right: 11px;
    top: 0;
    line-height: 16px;
    font-size: 22px;
    color: #cccccc !important;
  }

  .weui-switch {
    box-shadow: 0 0 3px 4px rgba(64, 204, 249, 0.4) inset;
    display: block;
    position: relative;
    width: 45px;
    height: 18px;
    border: 1px solid #43DCFF;
    outline: 0;
    border-radius: 16px;
    box-sizing: border-box;
    background-color: #43DCFF;
    transition: background-color 0.1s, border 0.1s;
    cursor: pointer;
  }

  .weui-switch:before {
    content: " ";
    position: absolute;
    top: -2px;
    left: -2px;
    width: 45px;
    height: 18px;
    border-radius: 15px;
    border: solid 1px #43DCFF;
    background-color: rgba(15, 21, 47, 0.9);
    box-shadow: 0 0 3px 1px rgba(64, 204, 249, 0.4) inset;
    transition: transform 0.35s cubic-bezier(0.45, 1, 0.4, 1);
  }

  .weui-switch:after {
    content: " ";
    position: absolute;
    top: 0px;
    left: 0px;
    width: 18px;
    height: 16px;
    background-color: #FDF419;
    border-radius: 15px;
    transition: transform 0.35s cubic-bezier(0.4, 0.4, 0.25, 1.35);
  }

  .switch_disabled {
    cursor: not-allowed;
    box-shadow: 0 0 3px 2px rgba(220, 220, 220, 0.6) inset;
    display: block;
    position: relative;
    width: 45px;
    height: 18px;
    border: 1px solid #DCDCDC;
    outline: 0;
    border-radius: 16px;
    box-sizing: border-box;
    background-color: rgba(15, 21, 47, 0.6);
    transition: background-color 0.1s, border 0.1s;
  }

  .switch_disabled:before {
    content: " ";
    position: absolute;
    top: -2px;
    left: -2px;
    width: 45px;
    height: 18px;
    border-radius: 15px;
    border: solid 1px rgba(220, 220, 220, 0.6);
    box-shadow: 0 0 3px 2px rgba(220, 220, 220, 0.4) inset;
    background-color: rgba(15, 21, 47, 0.3);
    transition: transform 0.35s cubic-bezier(0.45, 1, 0.4, 1);
  }

  .switch_disabled:after {
    content: " ";
    position: absolute;
    top: 0px;
    left: 0px;
    width: 18px;
    height: 16px;
    background-color: #c8c9cc;
    border-radius: 15px;
    transition: transform 0.35s cubic-bezier(0.4, 0.4, 0.25, 1.35);
  }

  .weui-switch-on {
    border-color: #43DCFF;
    background-color: rgba(15, 21, 47, 0.3);
  }

  .weui-switch-on:before {
    border-color: #43DCFF;
    background-color: rgba(15, 21, 47, 0.3);
  }

  .weui-switch-on:after {
    background-color: #43DCFF;
    transform: translateX(26px);
  }
</style>
