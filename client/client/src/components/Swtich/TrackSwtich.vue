<template>
  <div class="weui-div" @click.stop="toggle">

    <span class="weui-switch"
          :class="{
          'switch_disabled':!disabled,
          'weui-switch-on' : value}"
          :value="value"></span>

    <div v-if='!value' class="weui-div-2">
      <img src="../../assets/sign/x.png" alt="" style="vertical-align: 20%;">
    </div>

    <div v-if='value' class="weui-div-1">
      <img src="../../assets/sign/v.png" alt="" style="vertical-align: 14%;">
    </div>

    <!--<div v-if='!value' class="weui-div-2"-->
    <!--:class="{'weui_div_2_disabled':disabled==false}">×</div>-->

    <!--<div v-if='value' class="weui-div-1">✓</div>-->

  </div>
</template>

<script>
  import {mapActions, mapState} from "vuex";

  export default {
    name: "AttackSwtich",
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
      }
    },
    data() {
      return {
        //me_checked: this.value
      }
    },
    computed:
      {
        ...mapState(['select_target_id']),
      },
    methods: {
      toggle() {

        if (!this.disabled) {
          return;
        }
        // 是否可点击

        if (!this.value)
        {
          this.$emit('changSwtichOn', this.checked, this.fid, this.index);
        }
        else
        {
          this.$emit('changSwtichOff', this.checked, this.fid, this.index);
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
    width: 58px;
    height: 14px;
    padding-top: 1px;
  }

  .weui-div-1 {
    padding-top: 2px;
    position: absolute;
    left: 4px;
    top: 1px;
    line-height: 14px;
    font-size: 16px;
    color: #43DCFE;
  }

  .weui-div-2 {
    padding-top: 2px;
    position: absolute;
    right: 9px;
    top: 0;
    line-height: 14px;
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
    color: rgba(67, 220, 255,0.5) !important;
  }

  .weui-switch {
    box-shadow: 0 0 3px 4px rgba(64, 204, 249, 0.4) inset;
    display: block;
    position: relative;
    width: 53px;
    height: 16px;
    border: 1px solid #43DCFF;
    outline: 0;
    border-radius: 6px;
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
    width: 53px;
    height: 16px;
    border-radius: 6px;
    border: solid 1px #43DCFF;
    background-color: rgba(15, 21, 47, 1);
    box-shadow: 0 0 3px 2px rgba(64, 204, 249, 0.4) inset;
    transition: transform 0.35s cubic-bezier(0.45, 1, 0.4, 1);
  }

  .weui-switch:after {
    content: " ";
    position: absolute;
    top: -6px;
    left: -10px;
    width: 28px;
    height: 28px;
    background-image: url("../../assets/sign/operTrack.png");
    transition: transform 0.35s cubic-bezier(0.4, 0.4, 0.25, 1.35);
  }

  .switch_disabled {
    cursor: not-allowed;
    box-shadow: 0 0 3px 4px rgba(64, 204, 249, 0.4) inset;
    display: block;
    position: relative;
    width: 53px;
    height: 16px;
    border: 1px solid #43DCFF;
    outline: 0;
    border-radius: 6px;
    box-sizing: border-box;
    background-color: #43DCFF;
    transition: background-color 0.1s, border 0.1s;
  }

  .switch_disabled:before {
    content: " ";
    position: absolute;
    top: -2px;
    left: -2px;
    width: 53px;
    height: 16px;
    border-radius: 6px;
    border: solid 1px rgba(67, 220, 255,0.6);
    background-color: rgba(15, 21, 47, 0.9);
    box-shadow: 0 0 3px 1px rgba(64, 204, 249, 0.4) inset;
    transition: transform 0.35s cubic-bezier(0.45, 1, 0.4, 1);
  }

  .switch_disabled:after {
    content: " ";
    position: absolute;
    top: -6px;
    left: -10px;
    width: 28px;
    height: 28px;
    background-image: url("../../assets/sign/operTrack_gray.png");
    transition: transform 0.35s cubic-bezier(0.4, 0.4, 0.25, 1.35);
  }

  .weui-switch-on {
    border-color: #43DCFF;
    background-color: rgba(15, 21, 47, 0.3);
  }

  .weui-switch-on:before {
    border-color: #43DCFF;
    background-color: rgba(15, 21, 47, 1);
  }

  .weui-switch-on:after {
    background-image: url("../../assets/sign/operTrack_select.png");
    transform: translateX(41px);
  }
</style>

