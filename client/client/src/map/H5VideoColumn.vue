<!--<template>-->
<!--    <div>-->
<!--      <div id="" v-if="devices.length">-->
<!--        <div style="display:inline-block"-->
<!--             v-for="(dev,key) in devices"-->
<!--             v-show="dev.category<30000 && dev.category>20000 && dev.status >= 1"-->
<!--             :key="key">-->
<!--          <img :title="dev.name" src="../assets/sign/openH5Video.png"-->
<!--               style="cursor: pointer;margin-left: 10px" @click="OpenH5Video(dev)">-->
<!--        </div>-->
<!--      </div>-->
<!--    </div>-->
<!--</template>-->

<!--<script>-->
<!--  import {mapState, mapActions} from 'vuex'-->
<!--  import VideoInfoWiewer from "../map/VideoInfoWiewer";-->
<!--  import {show_message, msg_enum} from "./../modes/elementUI";-->
<!--  import bus from "../modes/tool/bus";-->

<!--  export default {-->
<!--    name: "DeviceList",-->
<!--    components: {-->
<!--      VideoInfoWiewer: VideoInfoWiewer,-->
<!--    },-->
<!--    data() {-->
<!--      return {-->
<!--      }-->
<!--    },-->
<!--    methods:{-->
<!--      OpenH5Video(item){-->
<!--        bus.$emit('OpenVideo');-->
<!--        document.getElementById('VideoViewerDisplay').style.display = '';-->
<!--      }-->
<!--    },-->
<!--    computed: {-->
<!--      ...mapState(['devices']),-->
<!--    },-->
<!--    mounted() {-->
<!--    }-->
<!--  }-->
<!--</script>-->

<!--<style scoped>-->

<!--</style>-->


<template>
  <div>
    <div id="" v-if="devices.length">
      <div style="display:inline-block"
           v-for="(dev,key) in devices"
           v-show="dev.category<30000 && dev.category>20000 && dev.status == 0"
           :key="key">
        <img :title="dev.name" src="../assets/sign/openH5Video.png"
             style="cursor: pointer;margin-left: 10px" @click="OpenH5Video(dev)">
      </div>
    </div>
  </div>
</template>

<script>
import {mapState, mapActions} from 'vuex'
import VideoInfoWiewer from "../map/VideoInfoWiewer";
import {show_message, msg_enum} from "./../modes/elementUI";

export default {
  name: "DeviceList",
  components: {
    VideoInfoWiewer: VideoInfoWiewer,
  },
  data() {
    return {
    }
  },
  methods:{
    OpenH5Video(item){
      document.getElementById('VideoViewerDisplay').style.display = '';
      // show_message(msg_enum.success,item.name);
      //
      // const { href } = this.$router.resolve({
      //   name: "h5video"
      // });
      //
      // window.open(href, '','width=800,height=800');
    }
  },
  computed: {
    ...mapState(['devices']),
  },
  mounted() {
  }
}
</script>

<style scoped>

</style>
