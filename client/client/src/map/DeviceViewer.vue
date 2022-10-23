<template>
  <transition v-if="device_info">
    <div class="viewer">
      {{device_info}}
    </div>
  </transition>
</template>

<script>
    import {mapState} from 'vuex'

    export default {
        name: "DeviceViewer",
        data() {
            return {}
        },
        computed: {
            ...mapState(['device_info'])
        }
    }
</script>

<style scoped>
  .viewer {
    background-color: #ff4d51;
    width: 300px;
    /*background-color: rgba(15, 21, 47, 0.7);*/
    /*box-shadow: 0px 0px 35px #43DCFF inset;*/
    /*border:1px solid #43DCFF;*/
  }
</style>
