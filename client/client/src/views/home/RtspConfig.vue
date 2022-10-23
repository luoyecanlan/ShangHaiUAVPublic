<template>
  <div class="correct-box">
    <div class="correct-header">
      <span class="correct-title" style="font-weight: normal">RTSP</span>
      <transition name="fade">
        <span class="err-tooltip" v-if="error"><span yellow>Error：</span>{{error}}</span>
      </transition>
    </div>
    <div class="correct-body" style="margin-top: -16px;">
      <el-input v-model="rtspAddress"
                placeholder="URL"
                @keyup.enter.native="saveRtsp"
                @change="saveRtsp"
                style="width: 360px;">
        <i slot="suffix" class="el-icon-close link-button light-color" style="line-height: 26px;"
           @click="rtspConfig.info=undefined"></i>
      </el-input>
    </div>
  </div>
</template>

<script>
    import {exceOnce} from "../../modes/tool";
    import {
      add_system_config,
      http_fast_request,
      request_by_key_system_config,
      update_system_config
    } from "../../modes/api";
    import {msg_enum, show_message} from "../../modes/elementUI";

    export default {
      name: "RtspConfig",
      data() {
        return {
          rtspKey: 'rtspAddress',
          rtspAddress: undefined,
          rtspConfig:undefined,
          isUpdate: false,
          error: undefined
        }
      },
      methods: {
        initData() {
          http_fast_request(request_by_key_system_config, this.rtspKey, data => {
            this.isUpdate = !!data;
            if (data) {
              this.rtspConfig = data;
              this.rtspAddress=this.rtspConfig.info;
            }
          });
        },
        saveRtsp() {
          if (!this.rtspAddress || this.rtspAddress === '') {
            this.error = this.$t('m.wrongValueType');
            exceOnce(() => {
              this.error = undefined;
            });
            return;
          }
          if (this.isUpdate) {
            this.rtspConfig.info=this.rtspAddress;
            http_fast_request(update_system_config, {...this.rtspConfig}, () => {
              show_message(msg_enum.success, this.$t('m.setSuc'));
              this.initData();
            });
          } else {
            http_fast_request(add_system_config, {name: this.rtspKey, info: this.rtspAddress}, () => {
              show_message(msg_enum.success, this.$t('m.setSuc'));
              this.initData();
            });
          }
        }
      },
      mounted() {
        this.initData();
      }
    }
</script>

<style scoped>

</style>
