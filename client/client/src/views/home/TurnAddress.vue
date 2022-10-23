<template>
  <div class="correct-box">

    <div class="correct-header">
      <span class="correct-title" style="font-weight: normal;">{{$t('m.guideAddress')}}</span>

      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
      </transition>
    </div>

    <div class="correct-body" style="flex-direction: column;margin-top: -16px;">
      <div class="turn-address-add">
        <el-input class="input" v-model="t_name" :placeholder="$t('m.remark')"></el-input>
        <el-input class="input" v-model="t_ip" :placeholder="$t('m.IPAdress')"></el-input>
        <el-input style="flex: 0.5" class="input" v-model="t_port" :placeholder="$t('m.port')"></el-input>
        <div class="add-save" @click.prevent="doSave">{{$t('m.confirm')}}</div>
      </div>
      <div class="list-turn-address" v-if="turnList.length">
        <div class="list-turn-scroll-box" id="container">
          <vue-scroll :ops="options">
            <div class="turn-address-item" v-for="(turn,key) in turnList" :key="key">
              <span style="width: 140px;">{{turn.remark}}</span>
              <span style="flex: 1">{{turn.ip}}:{{turn.port}}</span>
              <span>
                <i class="el-icon-close link-button" @click.prevent="doDelete(turn)"></i>
              </span>
            </div>
          </vue-scroll>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import {exceOnce, isIP, isPort, validation} from "../../modes/tool";
  import {add_target_turn, remove_target_turn,request_target_turn,http_request} from "../../modes/api";
  import {msg_enum, show_confirm, show_message} from "../../modes/elementUI";
  export default {
    name: "TurnAddress",
    data() {
      return {
        turnList: [],
        t_name: undefined,
        t_ip: undefined,
        t_port: undefined,
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
      clearInputData(){
        this.t_name=undefined;
        this.t_ip=undefined;
        this.t_port=undefined;
      },
      initData() {
        http_request(request_target_turn,null, data => {
          this.turnList = data;
        });
      },
      doSave() {
        const {t_name, t_ip, t_port} = this;
        if (!validation([
          {value: t_name},
          {value: t_ip, func: f => isIP(f)},
          {value: t_port, func: f => isPort(f)}
        ])) {
          this.error = this.$t('m.wrongValueType');
          exceOnce(() => {
            this.error = undefined;
          });
          return;
        }
        http_request(add_target_turn, {
          ip: t_ip,
          port: parseInt(t_port),
          remark: t_name
        }, () => {
          //新增成功
          show_message(msg_enum.success, this.$t('m.addSuccess'));
          this.clearInputData();
          this.initData();
        });
      },
      doDelete(info){
        show_confirm(this.$t('m.tips'),this.$t('m.sureDel'),()=>{
          http_request(remove_target_turn,info.id,()=>{
            show_message(msg_enum.success, this.$t('m.delSuccess'));
            this.initData();
          });
        });
      }
    },
    mounted() {
      this.initData();
    }
  }
</script>

<style scoped>
  .turn-address-add{
    display: flex;
    flex-direction: row;
    height: 30px;
  }
  .turn-address-add .input{
    flex: 1;
    margin-right: -1px;
  }
  .turn-address-add .input:first-child{
    flex: 0.5;
  }
  .add-save{
    font-size: 14px;
    height: 26px;
    width: 66px;
    border: 1px solid rgba(64,204,249,0.5);
    line-height: 24px;
    text-align: center;
    cursor: pointer;
  }
  .add-save:hover{
    background-color: #40cef9;
    color: #020e2c;
    transition: all 0.2s;
  }
  .list-turn-address{
    flex: 1;
    width: 100%;
    overflow:hidden;
    position: relative;
    margin-top: 5px;
    /*overflow-x: hidden;*/
    /*background-color: #40cef9;*/
    /*border:1px solid #40cef9;*/
  }
  .list-turn-address:before{
    content: "";
    width: 1px;
    height: 80%;
    background: linear-gradient(transparent,rgba(64,204,249,0.6),transparent);
    position: absolute;
    top: 10%;
    left: 120px;
  }
  #container{
    position: absolute;
    height: 100%;
    width: 100%;
    overflow-y: auto;
  }
  .turn-address-item{
    display: flex;
    flex-direction: row;
    line-height: 34px;
    padding-right: 34px;
  }
  .turn-address-item span{
    padding-left: 10px;
  }
  .el-icon-close{
    font-size: 12px;
    color: rgba(64,204,249,0.5);
  }
  .el-icon-close:hover{
    color: rgba(255,255,0,0.5);
    transition: all 0.2s;
  }
</style>
