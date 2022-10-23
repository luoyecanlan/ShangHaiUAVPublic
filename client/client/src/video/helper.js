import axios from 'axios'

/**
 * 封装rtsp登录方法
 * @param url
 * @param data
 * @returns {Promise<any>}
 */
export function rtsp_login(url,data) {
  return new Promise((res, rej) => {
    let dataStr = ''
    Object.keys(data).forEach(key => {
      dataStr += `${key}=${data[key]}&`
    })
    if (dataStr) {
      dataStr = dataStr.substring(0, dataStr.lastIndexOf('&'))
      url = `${url}?${dataStr}`
    }
    let promise = axios.get(url)
    promise.then(response => {
      res(response.data)
    }).catch(error => {
      rej(error)
    })
  });
}
