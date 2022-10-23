import Vue from 'vue'
import Router from 'vue-router'
import Home from '../pages/Home'
import Login from '../pages/Login'
import NotFound from '../pages/404'
import NoLimit from '../pages/NotLimit'
import H5Video from '../map/VideoWndViewer'

import User from '../views/user/Index'
import Management from '../views/home/Index'
import Device from '../views/device/Index'
// import NotFound from '../components/404'
// import NoLimit from '../components/Limit'
import Token from '../views/token/Index'
import Zone from '../views/warningzone/Index'

import DataAnalysis from '../views/dataAnalysis/Index'
import Gis from '../views/gis/Index'
import History from '../views/history/Index'
import WhiteList from '../views/whiteList/Index'
import ReplayWnd from '../views/history/tgReplay/ReplayTGWnd'


const originalPush = Router.prototype.push
Router.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err)
}

const originalReplace = Router.prototype.replace
Router.prototype.replace=function replace(location){
  return originalReplace.call(this,location).catch(err=>err)
}
Vue.use(Router)
export default new Router({
  routes: [
    {
      path: '/h5video',
      name: 'h5video',
      meta:{
        hideMenu:true
      },
      component: H5Video
    },
    {
      path: '/home',
      name: 'home',
      component: Home
    },
    {
      path:'/login',
      name:'login',
      meta:{
        hideMenu:true
      },
      component:Login
    },
    {
      path: '/home',
      name: 'management',
      component: Management
    },
    {
      path:'/replayWnd',
      name:'replayWnd',
      meta:{
        hideMenu:true
      },
      component:ReplayWnd
    },
    {
      path:'/user',
      name:'user',
      component:User
    },
    {
      path:'/device',
      name:'device',
      component:Device
    },
    {
      path:'/token',
      name:'token',
      component:Token
    },
    {
      path:'/zone',
      name:'zone',
      component:Zone
    },
    {
      path:'/dataAnalysis',
      name:'dataAnalysis',
      component:DataAnalysis
    },
    {
      path:'/gis',
      name:'gis',
      component:Gis
    },
    {
      path:'/history',
      name:'history',
      component:History
    },
    {
      path:'/whitelist',
      name:'whitelist',
      component:WhiteList
    },


    {
      path:'/404',
      name:'404',
      meta:{
        hideMenu:true
      },
      component:NotFound
    },
    {
      path:'/nolimit',
      name:'nolimit',
      meta:{
        hideMenu:true
      },
      component:NoLimit
    },
    {
      path:'/',
      redirect:'/home'
    },
    {
      path:'*',
      redirect: '/404'
    }

  ]
})
