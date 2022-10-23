import {
  COMMIT_LOGIN_INFO,
  COMMIT_DEVICE_CATEGORIES,
  REQUEST_ALL_DEVICES,
  COMMIT_DEVICE_STATUS,
  COMMIT_DEVICE_RELATION_SHIPS,
  UPDATE_TGID,
  QUERY_DEVICES,
  QUERY_DEVICE_INFO,
  QUERY_DEVICE_STATUS,
  QUERY_TARGETS,
  QUERY_TARGET_INFO,
  QUERY_WARNING_ZONES,
  SET_TARGET_0LEVEL_NUM,
  SET_TARGET_1LEVEL_NUM,SELECT_WAS_INFO,
  SET_TARGET_2LEVEL_NUM,DO_SELECT_TARGET_INFO,
  SET_TARGET_3LEVEL_NUM,SET_DEV_COVER_LINE_STATUS,
  SET_DEV_STATUS, SET_MAP_TYPE,SET_DEV_ALARM_STATUS,
  SET_TARGET_OUT_LEVEL_NUM,CHANGE_INFO_USER_CONFIG,
  ADD_TARGET_STATUS_INFO,SET_USER_MAP_INFO,
  SELECT_TARGET_ID, SET_USER_CONFIG_INFO,
  SET_ZERO_LEVEL_CHECK, SET_ONE_LEVEL_CHECK,
  SET_OUT_LEVEL_CHECK, SET_THREE_LEVEL_CHECK, SET_TWO_LEVEL_CHECK,
  CLEAR_TARGETS_FILTER, CLEAR_TARGETS,
  SET_SPEED_SLIDER, SET_ALT_SLIDER, DEL_TARGET_INFO, DEL_TARGET_FILTER_INFO,
  SET_TARGET_TURN, SET_MAP_INFO,SET_BAND_ID

} from "./mutation-types";
import {
  http_request,
  request_device_categories,
  request_all_devices,
  request_device_status,
  request_device_targets,
  request_target_info,
  request_warning_zones
} from "../modes/api";

export default {
  /**
   * 设置登录用户信息
   * @param commit
   * @param _info
   */
  commit_login_info({ commit }, _info) {
    commit(COMMIT_LOGIN_INFO, { data: _info });
  },
  request_device_categories({ commit }) {
    http_request(request_device_categories, null, data => {
          commit(COMMIT_DEVICE_CATEGORIES, { data });
    });
  },
  //获取全部设备
  query_devices({commit}){
    http_request(request_all_devices,null,data=>
    {
      commit(QUERY_DEVICES,{data});
    })
  },
  //选中设备
  select_device({commit},device){
    commit(QUERY_DEVICE_INFO,{data:device})
    //  设备状态查询
    http_request(request_device_status,device.id,data=>{
      commit(QUERY_DEVICE_STATUS,{data});
    })
  },
  //获取设备关联的目标列表
  query_targets({commit},{ids,sec}){
    if(ids.length){
      ids.forEach(id=>{
        http_request(request_device_targets,{did:id,sec},tgs=>{
          console.log(tgs);
          commit(QUERY_TARGETS,{did:id,data:tgs});
        })
      })
    }
  },
  //设置当前选中目标
  select_target({commit},{tgid}){
    http_request(request_target_info,{tgid},info=>{
      //console.log('实时查询的目标信息',info)
      commit(QUERY_TARGET_INFO,{data:info});
    })
  },
  //获取设备关联的告警区列表
  query_wraning_zones({commit},{ids}){
    if(ids.length){
      ids.forEach(id=>{
        http_request(request_warning_zones,id,zones=>{
          commit(QUERY_WARNING_ZONES,{data:zones});
        })
      })
    }
  },
  set_select_target_id({commit},{id,did}){
    commit(SELECT_TARGET_ID,{data:id,did:did});
  },
  set_select_was_info({commit},data){
    commit(SELECT_WAS_INFO,data);
  },
  set_dev_status({commit},data){
    commit(SET_DEV_STATUS,data);
    //console.error(data.did+"---"+data.status+"---"+data.deviceCategory+"uuuuuuuuuuuuuuuuuuuuuuuuuuuuu");
  },
  set_target_0level_num({commit},num){
    commit(SET_TARGET_0LEVEL_NUM,{data:num});
  },
  set_target_1level_num({commit},num) {
    commit(SET_TARGET_1LEVEL_NUM, {data:num});
  },
  set_target_2level_num({commit},num){
    commit(SET_TARGET_2LEVEL_NUM,{data:num});
  },
  set_target_3level_num({commit},num){
    commit(SET_TARGET_3LEVEL_NUM,{data:num});
  },
  set_target_out_level_num({commit},num){
    commit(SET_TARGET_OUT_LEVEL_NUM,{data:num});
  },
  set_zero_level_check({commit},data){
    commit(SET_ZERO_LEVEL_CHECK,{data:data});
  },
  set_one_level_check({commit},data){
    commit(SET_ONE_LEVEL_CHECK,{data:data});
  },
  set_two_level_check({commit},data){
    commit(SET_TWO_LEVEL_CHECK,{data:data});
  },
  set_three_level_check({commit},data){
    commit(SET_THREE_LEVEL_CHECK,{data:data});
  },
  set_out_level_check({commit},data){
    commit(SET_OUT_LEVEL_CHECK,{data:data});
  },
  clear_targets_filter({commit},data){
    commit(CLEAR_TARGETS_FILTER,{data:data});
  },
  clear_targets({commit},data){
    commit(CLEAR_TARGETS,{data:data});
  },
  set_speed_slider({commit},data){
    commit(SET_SPEED_SLIDER,{data:data});
  },
  set_alt_slider({commit},data){
    commit(SET_ALT_SLIDER,{data:data});
  },

  // 删除目标总列表的某个数据
  del_target_info({commit},data){
    commit(DEL_TARGET_INFO,{data:data});
  },
  // 删除过滤列表的某个数据
  del_target_filter_info({commit},data){
    commit(DEL_TARGET_FILTER_INFO,{data:data});
  },
  // 设置状态信息
  add_target_status_info({commit},data){
    commit(ADD_TARGET_STATUS_INFO,{data:data});
  },
  // 设置转发IP端口
  set_target_turn({commit},{ip,port})
  {
    commit(SET_TARGET_TURN,{ip:ip,port:port});
  },
  set_map_type({commit},data)
  {
    commit(SET_MAP_TYPE,data);
  },
  set_map_info({commit},data)
  {
    commit(SET_MAP_INFO,data);
  },
  set_user_config_info({commit},data)
  {
    commit(SET_USER_CONFIG_INFO,data);
  },
  set_user_map_info({commit},data)
  {
    commit(SET_USER_MAP_INFO,data);
  },
  change_info_user_config({commit},{type,data})
  {
    commit(CHANGE_INFO_USER_CONFIG,{type:type,data:data});
  },
  set_dev_alarm_status({commit},{did,type,data})
  {
    commit(SET_DEV_ALARM_STATUS,{did:did,type:type,data:data});
  },
  set_dev_cover_line_status({commit},{type, data})
  {
    commit(SET_DEV_COVER_LINE_STATUS,{type:type,data:data});
  },
  do_select_target_info({commit},data)
  {
    commit(DO_SELECT_TARGET_INFO,data);
  },
  set_band_id({commit},data)
  {
    commit(SET_BAND_ID,data);
  },
  commit_realtime_devices({commit},data) {
    commit(REQUEST_ALL_DEVICES, {data});
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
