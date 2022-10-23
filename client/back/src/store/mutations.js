import {
  COMMIT_LOGIN_INFO,
  COMMIT_DEVICE_CATEGORIES,
  REQUEST_DEVICES,
  COMMIT_DEVICE_STATUS,
  COMMIT_DEVICE_RELATION_SHIPS, UPDATE_TGID
} from "./mutation-types";

export default {
  [COMMIT_LOGIN_INFO](state, {data}) {
    state.login_info = data
  },
  [COMMIT_DEVICE_CATEGORIES](state, {data}) {
    state.device_categories = data
  },
  [REQUEST_DEVICES](state,{data}){
    state.device_infos=data;
  },
  [COMMIT_DEVICE_STATUS](state,{data}) {
    //console.log(data,Object.keys(data));
    if (data && data.length > 0) {
      data.map(f => {
        if(f){
          state.device_status[f.deviceId] = f;
        }

      });
    }
  },
  [COMMIT_DEVICE_RELATION_SHIPS](state,{data}) {
    state.device_relation_ships=[];
    if(data){
      state.device_relation_ships=data;
    }
  },
  [UPDATE_TGID](state,{data}) {
    state.tgId=data;

  }
}
