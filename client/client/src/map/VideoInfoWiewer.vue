<template>
  <div class="TargetInfoViewer">

    <video style="width: 100%; height: 100%" class="h5video" :id="videoId"></video>

    <img src="../assets/sign/zuidahua.png" :title="$t('m.windowingVideo')"
         @click="doWndVideo()"
         style="position:absolute;bottom: 10px;right: 10px;cursor: pointer" alt="">

    <img src="../assets/sign/guanbi.png" :title="$t('m.closeVideo')"
         @click="doClose()"
         style="position:absolute;top: 10px;right: 10px;cursor: pointer" alt="">

  </div>
</template>

<script>
  import {rtsp_login} from "../video/helper";
  import {H5sPlayerWS} from '../video/h5splayer'
  import Md5 from 'md5'
  import bus from "../modes/tool/bus";
  import {http_request, request_system_config} from "../modes/api";
  import Vue from 'vue'

  export default {
    name: "VideoInfoWiewer",
    data() {
      return {
        h5handler: undefined,
        session: '',
        proto: 'WS',
        token: 'token2',
        videoId: 'token2', //d479
        RTSP_CONF: {
          username: 'admin',
          password: 'JZGPyyds2022@', //'12345',
          url: 'http://120.48.147.194:7070/', //'http://47.92.98.138:8080/',
          wshost: '120.48.147.194:7070' //"47.92.98.138:8080"
        }
      }
    },
    methods: {
      async playVideo() {
        //登录
        const {url, username, password} = this.RTSP_CONF;
        let api = `${url}api/v1/Login`;

        console.log('VideoResult77777777777777777777777777777777777777777777777777777777777777777777777');
        let result = await rtsp_login(api, {user: username, password: Md5(password)});

        console.log('VideoResult77777777777777777777777777777777777777777777777777777777777777777777777', result);

        if (result) {
          //登录成功
          this.session = result.strSession;
          this.doPlay();
        } else {
          //登录失败
          alert("视频服务异常7777777777777777777777777777777777777777777777，Code:LOGIN_001");
        }
      },
      doPlay() {
        let {token, session, videoId, h5handler} = this;
        if (h5handler) {
          h5handler.disconnect();
          // delete h5handler;
          h5handler = undefined;
        }
        let conf = {
          videoid: videoId,
          protocol: window.location.protocol,
          host: this.RTSP_CONF.wshost,
          rootpath: '/',
          token: token,
          hlsver: 'v1',//v1',
          session: session
        };
        this.h5handler = new H5sPlayerWS(conf);
        this.h5handler.connect();
      },
      doClose() {
        try {
          this.h5handler.disconnect();
          this.h5handler = undefined;
          document.getElementById('VideoViewerDisplay').style.display = 'none';
        } catch (e) {
          document.getElementById('VideoViewerDisplay').style.display = 'none';
        }
      },
      doWndVideo() {
        const {href} = this.$router.resolve({
          name: "h5video"
        });

        window.open(href, '', 'width=1920,height=1030,');

      }
    },
    mounted() {
      bus.$off('OpenVideo');
      bus.$off('DelVideo');

      http_request(request_system_config, null, _data =>
      {
        this.RTSP_CONF.url = _data[0].info + '/';
        this.RTSP_CONF.wshost = _data[0].info.slice(7);

        this.videoId = Vue.prototype.$redLLight;
        this.token = Vue.prototype.$redLLight;
      });

      bus.$on('OpenVideo', () =>
      {
        this.playVideo();
      });

      bus.$on('DelVideo', () => {
        try {
          console.log('this.h5handler', this.h5handler);
          if (this.h5handler) {
            this.h5handler.disconnect();
            this.h5handler = undefined;
          }
        } catch (e) {
        }
      });
    }
  }
</script>

<style scoped>
  .TargetInfoViewer {
    background-color: rgba(15, 21, 47, 0.4);
    box-shadow: 0px 0px 35px #43DCFF inset;
    border: 1px solid #43DCFF;
    width: 402px;
    height: 320px;
  }
</style>
