import {
  COMMIT_LOGIN_INFO,
  COMMIT_DEVICE_CATEGORIES,
  REQUEST_DEVICES,
  COMMIT_DEVICE_STATUS,
  COMMIT_DEVICE_RELATION_SHIPS,
  UPDATE_TGID
} from "./mutation-types";
import {http_request,request_device_categories} from "../modes/api";

export default {
  /**
   * 设置登录用户信息
   * @param commit
   * @param _info
   */
  commit_login_info({commit}, _info) {
    commit(COMMIT_LOGIN_INFO, {data: _info})
  },
  request_device_categories({commit}) {
    http_request(request_device_categories, null, (data) => {
      commit(COMMIT_DEVICE_CATEGORIES, {data});
    });
  },
  commit_realtime_devices({commit},data) {
    commit(REQUEST_DEVICES, {data});
  },
  commit_realtime_device_status({commit}, status) {
    commit(COMMIT_DEVICE_STATUS, {data: status});
  },
  commit_relation_ships({commit},data){
    commit(COMMIT_DEVICE_RELATION_SHIPS,{data});
  },
  update_tgId({commit},data){
    commit(UPDATE_TGID,{data});
  }

}
