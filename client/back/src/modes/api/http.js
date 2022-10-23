/**
 *  ajax请求模块
 **/

import axios from 'axios'
import router from '../../router'
import * as URI from 'uri-js'
import bus,{event_type} from '../tool/bus'
import {show_message,msg_enum} from "../elementUI";
import {remove_storage,get_storage,set_storage} from "./stroage";
import Vue from 'vue'
import {get_his_list,get_his_user_list} from "../api/api.js";

const access_key='ACCESS_KEY'
const refresh_token_key='REFRESH_TOKEN_KEY'
const access_time='ACCESS_TIME'
const access_duration='ACCESS_DURATION'
const config_path='/static/config.json'

window.isRefreshing=false;
let refreshSubscribers=[];

function subscribeTokenRefresh(cb) {
  refreshSubscribers.push(cb);
}

function onRefreshed(token) {
  refreshSubscribers.map(cb=>cb(token));
}

export function get_app_config() {
  return new Promise((res, rej) => {
    let promise = axios.get(config_path);
    promise.then(_data => {
      res(_data.data);
    }).catch(err => {
      rej(err)
    })
  })
}

/**
 * 保存api token
 * @param __token
 */
export function save_token(data) {
  const {access_token,refresh_token,nbf,expire_in} =data;
  set_storage(access_key,access_token)
  set_storage(refresh_token_key,refresh_token)
  set_storage(access_duration,expire_in)
  set_storage(access_time,nbf)
}

/**
 * 清理api token
 */
export function clear_token() {
  remove_storage(access_key)
  remove_storage(refresh_token_key)
  remove_storage(access_time)
  remove_storage(access_duration)
}

/**
 * 是否存在token
 */
export function has_token() {
  return !!localStorage[access_key]
}

/**
 * 跳转登录
 */
export function directLogin() {
  clear_token();
  router.replace('/login')
}

/**
 * 权限不足
 */
export function directNotLimit() {
  router.replace('/nolimit')
}

/**
 * 判断token是否过期
 */
function isTokenExpired() {
  let _time = get_storage(access_time)
  let _duration = get_storage(access_duration)
  let now_time = new Date().getTime() / 1000;
  let temp_time = new Date(_time.replace(/-/g, '/')) / 1000;
  let expires_time = temp_time + parseInt(_duration);
  let isExpired = expires_time < now_time;
  return isExpired;
}


/**
 * 判断是否需要刷新token （距离过期时间小区5分钟）
 */
function isNeedRefreshToken() {
  let _time = get_storage(access_time)
  let _duration = get_storage(access_duration)
  let now_time = new Date().getTime() / 1000;
  let temp_time = new Date(_time.replace(/-/g, '/')) / 1000;
  let expires_time = temp_time + parseInt(_duration);
  //判断是否需要刷新token （距离过期时间小区5分钟）
  return expires_time - now_time <= 300;
}

/**
 * 判断token是否生效
 * @returns {boolean}
 */
export function isTokenOK(access_time,access_duration) {
  let now_time=new Date().getTime()/1000;
  let temp_time=new Date(access_time.replace(/-/g,'/'))/1000;
  let expires_time=temp_time+parseInt(access_duration);
  let isExpired=expires_time<now_time;
  return isExpired;
}

/**
 * 刷新token
 */
function refreshToken() {
  //刷新token
  return ajax(`/api/rf?rftoken=${get_storage(refresh_token_key)}`, null, 'POST');
}

/**
 * 请求拦截器
 */
axios.interceptors.request.use(config=>{
  config.headers['Content-Type'] = 'application/json;charset=UTF-8'
  let _path=URI.parse(config.url).path
  //配置文件读取不设置
  if(/\/static\/config.json/.test(_path)){
    return config
  }
  //登录接口不需要进行token验证
  if(/\/api\/login/.test(_path)){
    //登录
    return config;
  }
  const access_token=localStorage[access_key];
  if(access_token) {
    config.headers.Authorization = `Bearer ${get_storage(access_key)}`
    //验证是否过期
    if (isTokenExpired()) {
      directLogin()
      return
    }
    //判断并刷新token
    if (isNeedRefreshToken()&&config.url.indexOf('api/rf')===-1) {
      console.log("...")
      if(!window.isRefreshing){
        window.isRefreshing=true;
        refreshToken().then(data => {
          if(data.code===0){
            window.isRefreshing=false;
            const {access_token, refresh_token, expire_in, nbf} = data.data;
            save_token({access_token, refresh_token, expire_in, nbf});
            onRefreshed(access_token);
          }else{
            directLogin();
          }
        }).catch(err => {
          show_message(msg_enum.error,err.response.data.message);
          directLogin();
        });
      }

      let retry=new Promise((resolve,reject)=>{
        subscribeTokenRefresh((token)=>{
          config.headers.Authorization = `Bearer ${token}`
          resolve(config);
        })
      })
      return retry;
    }
    return config;
  }else{
    directLogin();
  }
},error => {
  console.log(error)
  show_message(msg_enum.error,`请求拦截器异常:${error}`)
  return Promise.reject(error)
});


axios.interceptors.response.use(rep=>{
  if(rep.status===401){
    //刷新token
    directLogin();
  }else if(rep.status===403){
    //show_message(msg_enum.warning, '无权限')
    directNotLimit();
  }
  else if(rep.status===500||rep.status===505||rep.status===507) {
    show_message(msg_enum.error, 'service error')
  }
  return rep
},error=>{
  if(error.response&&error.response.status===401){
    console.log("axios.interceptors.response.error.401",error);
    directLogin()
  }else if(error.response&&error.response.status===403){
    directNotLimit()
  }
  return error.response
})

/**
 * 封装ajax请求
 * @param url
 * @param data
 * @param type
 * @returns {Promise<any>}
 */
export async function ajax(url='',data={},type='GET') {
  if (!Vue.prototype.$baseUrl) {
    let data = await get_app_config();
    const {api_url} = data;
    Vue.prototype.$baseUrl = api_url;
    //console.log(api_url);
  }
  return new Promise(function (resolve, reject) {
    //debugger
    let promise;
    url = `${Vue.prototype.$baseUrl}${url}`
    if (type === 'GET') {
      let dataStr = ''

      Object.keys(data).forEach(key => {
        dataStr += `${key}=${data[key]}&`
      })

      if (dataStr)
      {
        dataStr = dataStr.substring(0, dataStr.lastIndexOf('&'))
        url = `${url}?${dataStr}`
      }
      promise = axios.get(url)
    } else if (type === 'POST') {
      promise = axios.post(url, data)
    } else if (type === 'DELETE') {
      promise = axios.delete(url, {data: data})
    } else if (type === 'PUT') {
      promise = axios.put(url, data)
    } else {
      promise = axios.post(url, data)
    }

    promise.then(response =>
    {
      resolve(response.data ? response.data : response)
    }).catch(error =>
    {
      reject(error)
    })
  })
}
/**
 * 请求接口统一入口方法
 * @param api 接口方法
 * @param param 参数
 * @param callback  请求成功后执行的回调方法
 */
export function http_fast_request(api,param,callback) {
  // bus.$emit(event_type.loading,true);
  let _result = api(param)
  _result.then(_data => {
    //console.log(_data,api)
    // bus.$emit(event_type.loading,false);
    if (_data&&_data.code === 0) {
      callback && callback(_data.data)
    } else {
      show_message(msg_enum.error, 'Error')//_data.message || '出错啦')
    }
  }).catch(error => {
    console.log("http_request", error);
    directLogin();
  })
}
/**
 * 请求接口统一入口方法
 * @param api 接口方法
 * @param param 参数
 * @param callback  请求成功后执行的回调方法
 */
export function http_request(api,param,callback) {
  bus.$emit(event_type.loading,true);

  let _result = api(param)
  _result.then(_data => {
    bus.$emit(event_type.loading,false);
    //console.log(_data,api)
    if (_data&&_data.code === 0)
    {
      callback&&callback(_data.data)
    }
    else
    {
      if(api == get_his_user_list ||api == get_his_list)
      {
        callback&&callback(_data.message)
      }

      //show_message(msg_enum.error, 'Error') //_data.message||'接口异常')
    }
  }).catch(error => {
    bus.$emit(event_type.loading, false);
    directLogin();
  })
}
