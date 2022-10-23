import * as SignalR from '@aspnet/signalr'
import Vue from 'vue'
import {get_app_config} from "./http";

let _signalR_client;


async function initSignalRClient(channel) {
  if (!Vue.prototype.$baseUrl) {
    let config = await get_app_config()
    const {api_url} = config;
    Vue.prototype.$baseUrl = api_url;
  }
  return new Promise((s, t) => {
    let _url = `${Vue.prototype.$baseUrl}/${channel}`;
    return s({
      client: new SignalR.HubConnectionBuilder().withUrl(_url).build(),
      url: _url
    });
  });
}

/**
 * 订阅事件
 * @param options
 *  {
 *      channel:string  //频道
 *      events：[
 *      {
 *          name:string //关键字
 *          func：function //方法
 *      }...
 *      ]
 *  }
 * @returns {Promise<void>}
 */
export function subscribeSignalR(options) {
  const {channel, events} = options;
  let _promise = initSignalRClient(channel);
  _promise.then(data => {
    const {client, url} = data;
    _signalR_client = client;
    if (events && events.length > 0) {
      events.forEach(f => {
        const {name, func} = f;
        _signalR_client.off(name);
        _signalR_client.on(name, data => {
          func(data);
        });
      });
    }
    _signalR_client.start().then(() => {
      console.log(`signalR client connect to ${url} success`);
    }).catch(err => {
      console.log(err);
    });
  });
}

/**
 * 注销signalR
 */
export function distorySignalR() {
  if(_signalR_client){
    _signalR_client.stop().then(()=>{
      console.log('stop signal R....')
    }).catch(err=>{
      console.log(err);
    });
  }
}
