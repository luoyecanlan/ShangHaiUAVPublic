<template>
  <div>
    <router-link to="" v-for="(item,key) in options" :key="key">
      <div class="menu-item" @click.stop="doChecked(item)" v-show="showItem(item)"
           :class="{'checked':item.checked}">
        <span class="item-label">{{item.label}}</span>
      </div>
    </router-link>
  </div>
</template>

<script>
import {mapState} from 'vuex'
export default {
  name: "Menu",
  data() {
    return {
      options: [
        {
          index:'home',
          label: this.$t('m.home'),
          icon: 'iconfont icon-home',
          to: '/',
          checked: true
        },
        // {
        //   index:'management',
        //   label: this.$t('m.management'),
        //   icon: 'iconfont icon-home',
        //   to: '/home',
        //   checked: false
        // },
        {
          index:'device',
          label:this.$t('m.deviceManger'),
          icon: 'iconfont icon-device',
          to: '/device',
          checked: false
        },
        {
          index:'zone',
          label: this.$t('m.alarmZone'),
          icon: 'iconfont icon-zone',
          to: '/zone',
          checked: false
        },
        {
          index:'user',
          label:this.$t('m.userManger'),
          icon: 'iconfont icon-team',
          to: '/user',
          checked: false
        },
        {
          index:'dataAnalysis',
          label: this.$t('m.dataAnalysis'),
          icon: 'iconfont icon-token',
          to: '/dataAnalysis',
          checked: false
        },
        // {
        //   index:'gis',
        //   label: this.$t('m.devCalibrationNorth'),
        //   icon: 'iconfont icon-token',
        //   to: '/gis',
        //   checked: false
        // },
        {
          index:'history',
          label: '历史数据',
          icon: 'iconfont icon-token',
          to: '/history',
          checked: false
        },
        {
          index:'whitelist',
          label: '白名单',
          icon: 'iconfont icon-team',
          to: '/whitelist',
          checked: false
        }
      ]
    }
  },
  computed:{
    ...mapState(['login_info'])
  },
  methods:{
    showItem(item){
      // if(item.index==='token'){
      //   return this.login_info.role==='super';
      // }
      return true;
    },
    doChecked(item){
      const {index,to}=item
      this.options.map(f=>{
        f.checked=f.index===index;
      })
      this.$router.push(to);
    }
  },
  mounted() {
    const _current=this.$route.name;
    //console.log(_current)
    this.options.map(f=>{
      f.checked=f.index===_current;
    })
  }
}
</script>

<style scoped>
.menu-item{
  display: inline-block;
  height: 34px;
  width: 140px;
  line-height: 33px;
  text-align: center;
  box-sizing: border-box;
  transition: all 0.2s;
  margin-right: 14px;
  position: relative;
}
.menu-item:before{
  content: "";
  position: absolute;
  display: block;
  height: 100%;
  width: 100%;
  background-color: rgba(64,204,249,0.1);
  box-sizing: border-box;
  border: 1px solid #43DCFF;
  transform-origin: center center;
  transform: skewX(-48deg);
  box-shadow: 0 0 3px 3px rgba(64,204,249,0.4) inset;
}
.item-label{
  color: #40cef9;
  font-size: 18px;
}
.menu-item:hover .item-label{
  color: #84e3ff;
}
.checked:before{
  background-color: rgba(2,14,44,0.1);
  box-shadow: 0 0 3px 3px rgba(132,227,255,.8) inset;
  transition: all 0.2s;
}
.checked:hover .item-label,.checked .item-label{
  color: #02a5d5;
  transition: all 0.2s;
}
</style>
