
import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store'
import ElementUI from 'element-ui'
import Vuei18n from 'vue-i18n'
import vuescroll from 'vuescroll'

Vue.use(Vuei18n)
Vue.use(ElementUI)
Vue.component('vue-scroll', vuescroll);

const i18n =new Vuei18n({
  locale:'zh-CN',
  messages:{
    'zh-CN':require('./lang/zh'),
    'en-US':require('./lang/en'),
    'ar-SA':require('./lang/ar')
  }
})

Vue.filter('formateNumber',value=>{
  return Number(value).toFixed(4);
})

// Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  i18n,
  components: { App },
  template: '<App/>'
})
