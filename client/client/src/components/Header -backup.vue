<template>
  <div v-if="isShow">
    <el-row style="height: 25px">
      <el-col :span="24" style="padding-right: 220px">
        <w-login-info style="float: right;margin-top: 2px"/>
      </el-col>
    </el-row>

    <el-row>
      <el-col :span="14" style="margin-left: 605px;margin-top: 6px">
        <!--设备横向列表-->
        <div class="HeaderDevListClass">
          <router-link to="" v-for="(item,key) in devices" :key="key">
            <el-dropdown style="overflow: hidden" @click="doSelect(item)">
              <div class="menu-item">
                <span style="color: #45DAFE;font-size: 18px;" @click="doSelect(item)">{{item.name}}</span>
                <span style="color: red;font-size: 18px;">⬤</span>
              </div>
              <el-dropdown-menu slot="dropdown" class="DevInfoWndClass">
                <!--                  新的窗口，抽成单独的控件-->
                <div style="height: 540px;width: 298px">
                  <div class="dialog-lt"></div>
                  <div class="dialog-lb"></div>
                  <div class="dialog-rt"></div>
                  <div class="dialog-rb"></div>

                  <div class="DevInfoHeaderWndClass">
                    <el-row>
                      <el-col :span="4" style="margin-top: 6px">
                        <img src="../assets/button/InfoImg.png" style="margin: 10px;margin-left: 15px">
                      </el-col>
                      <el-col :span="8" style="margin-top: 18px">
                        <label style="
                                 font-size: 18px;color: #45DAFE;font-weight: 800;
                                 font-family: 'Microsoft YaHei'">详情显示</label>
                      </el-col>
                      <el-col :span="12" style="margin-top: 24px;">
                        <button class="BackDevInfoCloseImg"/>
                      </el-col>
                    </el-row>
                  </div>


                  <el-row style="height: 460px;margin: 15px;margin-top: 5px">
                    <el-col :span="12" style="text-align: right;padding-right: 10px">
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">设备名称:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">设备类型:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">IP地址:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">IP端口:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">监听IP地址:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">监听IP端口:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">覆盖范围:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">覆盖半径:</label>
                      </div>
                      <!--                      <div class="ListInfoMargin">-->
                      <!--                        <label class="TextNormallight">告警区域:</label>-->
                      <!--                      </div>-->
                      <!--                      <div class="ListInfoMargin">-->
                      <!--                        <label class="TextNormallight">海拔高度:</label>-->
                      <!--                      </div>-->
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">设备经度:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">设备纬度:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">纠偏信息:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">威胁判断点数:</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextNormallight">目标超时时间:</label>
                      </div>
                    </el-col>

                    <el-col :span="12">
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.name==null?'空值':item.name}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">DevType</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.ip.length==0?'空值':item.ip}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.port.length==0?'空值':item.port}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.lip.length==0?'空值':item.lip}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.lport.length==0?'空值':item.lport}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.coverS}}~{{item.coverE}}°</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.coverR}}m</label>
                      </div>
                      <!--                      <div class="ListInfoMargin">-->
                      <!--                        <label class="TextHighlight">WarningArea</label>-->
                      <!--                      </div>-->
                      <!--                      <div class="ListInfoMargin">-->
                      <!--                        <label class="TextHighlight">Altitude</label>-->
                      <!--                      </div>-->
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.lng.length==0?'空值':item.lng}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.alt.length==0?'空值':item.lat}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">RectifyInformat</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label
                          class="TextHighlight">{{item.threadAssessmentCount==''?'空值':item.threadAssessmentCount}}</label>
                      </div>
                      <div class="ListInfoMargin">
                        <label class="TextHighlight">{{item.targetTimeOut==''?'空值':item.targetTimeOut}}</label>
                      </div>
                    </el-col>
                  </el-row>

                </div>
              </el-dropdown-menu>

            </el-dropdown>

          </router-link>

        </div>
      </el-col>
      <el-col :span="1" style="margin-top: 12px;margin-left: 20px">

        <el-dropdown style="overflow: hidden;" trigger="click">

          <button class="SettingButtonImg" @click="LogDevInfo()"/>

          <el-dropdown-menu slot="dropdown" style="
             margin-right: -140px;
             box-shadow: 0px 0px 35px #43DCFF inset;
             background-color: rgba(15, 21, 47, 0.7);
             border: 1px solid #43DCFF;
             overflow: hidden;">

            <!--            窗口 做成控件-->
            <div style="height: 100%;width: 320px">
              <div class="dialog-lt" style="margin-top: -22px"/>
              <div class="dialog-lb"/>
              <div class="dialog-rt" style="margin-top: -21px"/>
              <div class="dialog-rb"/>

              <div style="height: 30px;width: 100%;margin-top: 15px">
                <label style="padding-left: 165px;
                font-size: 18px;font-weight: 700;color: #FDF419">等距线</label>
                <label style="padding-left: 10px;margin-top: 15px;
                font-size: 18px;font-weight: 700;color: #FDF419">覆盖范围</label>
              </div>

              <router-link to="" v-for="(item,key) in devices" :key="key">
                <div class="SwtichMapClass">
                  <label class="TextHighlight" style="width: 150px;padding-left: 20px">{{item.name}} :</label>
                  <v-swtich style="margin-left: 20px" :value="value" :FId="item.name" :index="index" class="vswitch"
                            :handle="false" @changeSwitch="changeSwitch"/>
                  <v-swtich style="margin-left: 20px" :value="value" :FId="item.name" :index="index" class="vswitch"
                            :handle="false" @changeSwitch="changeSwitch"/>
                </div>
              </router-link>
            </div>
          </el-dropdown-menu>
        </el-dropdown>

      </el-col>
    </el-row>

  </div>
</template>

<script>
    import Menu from "./Menu";
    import LoginInfo from "./LoginInfo";
    import Clock from "./Clock";
    import {mapState, mapActions} from 'vuex'
    import {show_message, msg_enum} from "./../modes/elementUI";
    import VSwtich from './Swtich/VSwtich.vue'
    import {ChangeMapCenterFromLngLat} from "../map/js";

    export default {
        name: "w_Header",

        data() {
            return {
                value: true,
                FId: '123',
                index: 0,
            }
        },
        methods: {
            doSelect(_device) {
                show_message(msg_enum.success, "跳转到设备位置");
                // //修改地图中心点
                ChangeMapCenterFromLngLat({lng: _device.lng, lat: _device.lat});
            },
            LogDevInfo() {
                // show_message(msg_enum.success, devices[0].toString());
            },
            HiidenDevInfoDrop() {
                // this.$refs.devInfoDrop.visible = hidden;
                //this.$refs.dropdown.visible = false;
            },
            changeSwitch(checked, id, index) {
                show_message(msg_enum.success, "开启 [" + id + '] 设备');
            }
        },

        components: {
            wMenu: Menu,
            wLoginInfo: LoginInfo,
            wClock: Clock,
            VSwtich: VSwtich,
        },
        computed: {
            ...mapState(['devices']),
            isShow() {
                if (this.$route.meta) {
                    return !!!this.$route.meta.hideMenu;
                }
                return true;
            }
        }
    };
</script>

<style scoped>
  .ListInfoMargin {
    margin-top: 18px
  }

  .TextNormallight {
    font-size: 18px;
    color: #1DAECE;
  }

  .TextHighlight {
    font-size: 18px;
    color: #45DAFE;
    font-weight: 700;
  }

  .BackDevInfoCloseImg {
    outline: none;
    background: #43DCFE;
    float: right;
    margin-right: 20px;
    height: 3px;
    width: 21px;
    color: #43DCFE;
    cursor: pointer;
  }

  .SettingButtonImg {
    background: url('../assets/button/SettingButtonImg.png');
    width: 23px;
    height: 23px;
    cursor: pointer;
  }

  .HeaderDevListClass {
    width: 1100px;
    overflow-x: hidden;
    overflow-y: hidden;
    white-space: nowrap;
  }

  .DevInfoWndClass {
    background-color: rgba(15, 21, 47, 0.75);
    box-shadow: 0px 0px 25px #4DDCFE inset;
    border: 1px solid #4DDCFE;
    overflow: hidden;
    margin-right: -70px
  }

  .DevInfoHeaderWndClass {
    height: 55px;
    margin-top: -10px;
    box-shadow: 0px 0px 25px #4DDCFE inset;
    border: 1px solid #43DCFF;
  }

  .SwtichMapClass {
    color: #40cef9;
    display: flex;
    flex-direction: row;
    margin-top: 10px;
    margin-bottom: 20px;
    text-align: right;
  }

  .menu-item {
    display: inline-block;
    height: 36px;
    line-height: 36px;
    text-align: center;
    box-sizing: border-box;
    font-family: "Microsoft YaHei";
    transition: all 0.2s;
    margin-right: 30px;
  }

  .dialog-lt, .dialog-lb, .dialog-rb, .dialog-rt {
    width: 90px;
    height: 98px;
    margin: -2px;
    margin-top: -8px;
    position: absolute;
  }

  .dialog-lt {
    border-left: 5px solid #43DCFF;
    border-top: 5px solid #43DCFF;
  }

  .dialog-lb {
    border-left: 5px solid #43DCFF;
    border-bottom: 5px solid #43DCFF;
    bottom: 0px;
  }

  .dialog-rt {
    border-right: 4px solid #43DCFF;
    border-top: 4px solid #43DCFF;
    right: 0px;
  }

  .dialog-rb {
    border-right: 4px solid #43DCFF;
    border-bottom: 4px solid #43DCFF;
    right: 0px;
    bottom: 0px;
  }
</style>
