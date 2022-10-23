import Vue from 'vue'
import Router from 'vue-router'
import {has_token} from "../modes/api";
import Login from '../views/login/Index'
import User from '../views/user/Index'
import Home from '../views/home/Index'
import Device from '../views/device/Index'
import NotFound from '../components/404'
import NoLimit from '../components/Limit'
import Token from '../views/token/Index'
import Zone from '../views/warningzone/Index'
import Center from '../views/center/Index'
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

let vueRouter= new Router({
  routes: [
    {
      path:'/home',
      name:'home',
      component:Home
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
      path:'/center',
      name:'center',
      component:Center
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
      path:'/',
      redirect:'/home'
    },
    {
      path:'/404',
      name:'notfound',
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
      path:'*',
      redirect: '/404'
    }
  ]
})

vueRouter.beforeEach((to,from,next)=>{
  if(to.name!='login'){
    if(!has_token()) {
      next({
        path:'/login',
        query:{redirect:to.fullPath}
      })
    }else{
      next();
    }
  }else {
    next();
  }
})

export default vueRouter;
