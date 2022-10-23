// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import vuescroll from 'vuescroll/dist/vuescroll-native';
import App from './App'
import router from './router'
import store from './store'
import ElementUI from 'element-ui'
import 'vuescroll/dist/vuescroll.css';
import VueI18n from 'vue-i18n'
import BaiduMap from "vue-baidu-map";
import axios from "axios";
// import echarts from 'echarts';
import  * as  echarts from 'echarts'
Vue.prototype.$echarts = echarts

// 在 webpack 环境下指向 vue-echarts/components/ECharts.vue
import ECharts from 'vue-echarts'

// 手动导入 ECharts 各模块来减小打包体积
import 'echarts/lib/chart/line'
import 'echarts/lib/chart/bar'
import 'echarts/lib/chart/pie'

// title,legend,tooltip 是 echarts 的组件，
// 在图表中使用需要按组件导入，否则图表的标题图例可能不能显示。
import 'echarts/lib/component/tooltip'
import 'echarts/lib/component/legend'
import 'echarts/lib/component/title'
// 导入自定义主题
// ./views/theme/ 下保存的是自定义的主题
import chartsTheme from '@/theme/chartsTheme.json'
// import wonderland from '@/theme/wonderland.json'

// 注册自定义主题
ECharts.registerTheme('chartsTheme', chartsTheme)
//Vue.config.productionTip = false
// 注册组件后即可使用
Vue.component('v-chart', ECharts)
Vue.use(ElementUI);
Vue.use(vuescroll);
Vue.use(VueI18n);
Vue.use(BaiduMap, {
  ak: "bSx3PV1zoTHpkQFsQoUt2LNeG0GqtGwV"
});
Vue.prototype.$ajax = axios;
const i18n=new VueI18n({
  locale:'zh-CN',
  messages:{
    'zh-CN':require('./lang/zh'),
    'en-US':require('./lang/en'),
    //'ar-SA':require('./lang/ar')
  }
})
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  i18n,
  store,
  components: { App },
  template: '<App/>'
})

