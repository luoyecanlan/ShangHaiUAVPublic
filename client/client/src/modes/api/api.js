 /**
 * 接口封装模块
 */

import {ajax,ajax_await} from './http'

const api_url='/api'

const http_type={
  get:'GET',
  post:'POST',
  put:'PUT',
  delete:'DELETE'
}

//登录登出管理
/**
 * 登录
 * @param username
 * @param password
 */
export const request_access=({username,password})=>ajax(`${api_url}/login`,{userName:username,password},http_type.post);
/**
 * 登出
 * @returns {Promise<any>}
 */
export const distory_access=()=>ajax(`${api_url}/logout`,null,http_type.post);
/**
 * 刷新
 * @param refreshToken
 * @returns {Promise<any>}
 */
export const refresh_access=(refreshToken)=>ajax(`${api_url}/access?reftoken=${refreshToken}`,null,http_type.put);
//当前用户操作
/**
 * 请求当前用户信息
 * @returns {Promise<any>}
 */
export const request_current=()=>ajax_await(`${api_url}`);
/**
 * 当前用户基本信息修改
 * @returns {Promise<any>}
 */
export const update_current=(data)=>ajax(`${api_url}/user`,data,http_type.put);
/**
 * 修改当前用户密码
 * @returns {Promise<any>}
 */
export const update_current_pwd=(data)=>ajax(`${api_url}/user`,data,http_type.post);

/**
 * 获取设备相关的预警区信息
 * @param did 设备编号
 * @returns {Promise<any>}
 */
export const request_warning_zones=(did)=>ajax(`${api_url}/client/metadata/${did}`);
/**
 * 获取所有设备简要状态信息
 * @param key
 * @returns {Promise<any>}
 */
export const request_simple_device_status=()=>ajax(`${api_url}/client/RealTime/status`);
/**
 * 获取单设备状态
 * @param did
 * @returns {Promise<any>}
 */
export const request_device_status=(did)=>ajax(`${api_url}/client/RealTime/status/${did}`);
/**
 * 获取单设备所有目标
 * @param did
 * @param src
 * @returns {Promise<any>}
 */
export const request_device_targets=({did,src})=>ajax(`${api_url}/client/RealTime/targets/${did}?sec=${src}`);
/**
 * 获取单个目标详情
 * @param did
 * @returns {Promise<any>}
 */
export const request_target_info=({tgid})=>ajax(`${api_url}/RealTime/tg/${tgid}`);

/**
 * 获取转发地址
 * @param data get
 * @returns {Promise<any>}
 */
export const request_tgturn_list=(data)=>ajax(`${api_url}/tgturn`,data);

/**
 * 获取地图配置信息
 * @param data
 * @returns {Promise<any>}
 */
export const request_map_info_list=(data)=>ajax_await(`${api_url}/map`,data);
export const request_map_info=(id)=>ajax_await(`${api_url}/map/${id}`);
export const create_map_info=(data)=>ajax_await(`${api_url}/map`,data,http_type.put);
export const update_map_info=(data)=>ajax_await(`${api_url}/map`,data,http_type.post);
export const del_map_info=({tgid})=>ajax_await(`${api_url}/map/${tgid}`,null,http_type.delete);
/**
 * 获取个性化配置信息
 * @param data
 * @returns {Promise<any>}
 */
export const request_person_info_list=(data)=>ajax_await(`${api_url}/person`,data);
export const request_person_info=({id})=>ajax_await(`${api_url}/person/${id}`);
export const create_person_info=(data)=>ajax_await(`${api_url}/person`,data,http_type.put);
export const update_person_info=(data)=>ajax_await(`${api_url}/person`,data,http_type.post);
//目标打击
export const request_target_hit=({id,devId,hitreq})=>ajax(`${api_url}/hit/${id}/${devId}/${hitreq}`,null,http_type.post);
export const request_target_hit_del=({rid,devId})=>ajax(`${api_url}/hit/${rid}/${devId}`,null,http_type.delete);
//目标监视
export const request_target_monitor=(id)=>ajax(`${api_url}/monitor/${id}`,null,http_type.post);
export const request_target_monitor_del=(id)=>ajax(`${api_url}/monitor/${id}`,null,http_type.delete);
//目标转发
export const request_dev_target_transmit=(data)=>ajax(`${api_url}/transmit`,data,http_type.post);
export const request_dev_target_del_transmit=(id)=>ajax(`${api_url}/transmit/${id}`,null,http_type.delete);
 /**
  * 获取预警区
  * @param key
  * @returns {Promise<any>}
  */
 export const request_warn_zones = (data) => ajax(`${api_url}/zone`, data);
 /**
  * 获取全部预警区信息
  * @param key
  * @returns {Promise<any>}
  */
 export const request_all_warn_zones = () => ajax(`${api_url}/zone/all`);
//获取系统配置
export const request_system_config=()=>ajax(`${api_url}/sys`);
//云台控制
export const request_PTZ=(data)=>ajax(`${api_url}/PTZ`,data,http_type.post);
//纠偏设置
export const request_Rectify=(data)=>ajax(`${api_url}/Rectify`,data,http_type.post);
//光电跟踪
export const request_Follow=(data)=>ajax(`${api_url}/Follow`,data,http_type.post);
 //预警区管理
 /**
  * 创建预警区
  * @param data
  * @returns {Promise<any>}
  */
 export const create_warn_zone = (data) => ajax(`${api_url}/zone`, data, http_type.post);
 /**
  * 删除预警区
  * @param id
  * @returns {Promise<any>}
  */
 export const delete_warn_zone = (id) => ajax(`${api_url}/zone/${id}`, null, http_type.delete);
 /**
  * 更新预警区图形
  * @param data
  * @returns {Promise<any>}
  */
 export const update_warn_zone_area = (data) => ajax(`${api_url}/zone/geo`, data, http_type.put);
 /**
  * 更新预警区信息
  * @param data
  * @returns {Promise<any>}
  */
 export const update_warn_zone_info = (data) => ajax(`${api_url}/zone`, data, http_type.put);

 /**
  * 新建用户
  * @param data
  * @returns {Promise<any>}
  */
 export const create_user = (data) => ajax(`${api_url}/user`, data, http_type.post);
 /**
  * 删除用户
  * @param uid
  * @returns {Promise<any>}
  */
 export const delete_user = (uid) => ajax(`${api_url}/user/${uid}`, null, http_type.delete);
 /**
  * 更新用户角色
  * @param data
  * @returns {Promise<any>}
  */
 export const update_user_role = (data) => ajax(`${api_url}/user`, data, http_type.put);
 /**
  * 获取用户
  * @param key
  * @returns {Promise<any>}
  */
 export const request_users = (data) => ajax(`${api_url}/user`, data);
 /**
  * 获取历史目标
  * @param key
  * @returns {Promise<any>}
  */
 export const request_his_targets = ({ index, size, data }) => ajax(`${api_url}/ICONtarget?index=${index}&size=${size}`, data, http_type.post);

 export const request_his_heatMapData = () => ajax(`${api_url}/his/heatMapData`, null, http_type.post);

 export const request_his_riverData = () => ajax(`${api_url}/his/riverData`, null, http_type.post);

 export const request_his_weiguiData = () => ajax(`${api_url}/his/weiguiData`, null, http_type.post);

 export const request_his_targets_by_tgid = (tgId) => ajax(`${api_url}/his/track/${tgId}`);

 /**
  * 重置用户密码
  * @param uid
  * @returns {Promise<any>}
  */
 export const reset_user_pwd = (uid) => ajax(`${api_url}/user/${uid}`, null, http_type.post);


 //计算相关
 /**
  * 根据两点经纬高计算相对的方位、俯仰、距离、高度信息
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_point_info = (data) => ajax(`${api_url}/gis/3d2info`, data, http_type.post);
 /**
  * 已知一给点的经纬度，方位角和距离，计算另一个点的坐标
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_lat_lng = (data) => ajax(`${api_url}/gis/2latlng`, data, http_type.post);
 /**
  * 相对校北
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_correct_by_relative = (data) => ajax(`${api_url}/gis/correct/relative`, data, http_type.post);
 /**
  * 两点校北
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_correct_by_points = (data) => ajax(`${api_url}/gis/correct/point`, data, http_type.post);
 /**
  * 度分秒  ->  度
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_degree = ({ deg, min, sed }) => ajax(`${api_url}/gis/todeg?deg=${deg}&min=${min}&sed=${sed}`, null, http_type.post);
 /**
  *  度 ->  度分秒
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_d_m_s = (deg) => ajax(`${api_url}/gis/totuple?deg=${deg}`, null, http_type.post);
 /**
  *  计算高度
  * @param data
  * @returns {Promise<any>}
  */
 export const convert_height = ({ pitch, dis }) => ajax(`${api_url}/gis/2height?pitch=${pitch}&dis=${dis}`, null, http_type.post);
 export const delete_all_hisdata = (data) => ajax(`${api_url}/his/clear`, data, http_type.delete);

 export const delete_single_hisdata = (id) => ajax(`${api_url}/his/${id}`, null, http_type.delete);

 export const get_his_list = (ids) => ajax(`${api_url}/his/Export`,ids,http_type.post);//,null,http_type.post

 export const get_his_user_list = (date) => ajax(`${api_url}/his/ExportLog/${date}`);
 //白名单相关内容
 export const request_white_list = (data) => ajax(`${api_url}/WhiteList`, data);

 export const delete_white_item = (id) => ajax(`${api_url}/whitelist/${id}`, null, http_type.delete);
 export const create_white_item = (data) => ajax(`${api_url}/whitelist`, data, http_type.post);
 // 获取无人机类型
 export const query_uav_types=()=>ajax(`${api_url}/his/uavmodel`);

 export const query_history_ids=(data)=>ajax(`${api_url}/his/target/ids`,data,http_type.post)

 //设备管理
 /**
  * 创建设备
  * @param data
  * @returns {Promise<any>}
  */
 export const create_device = (data) => ajax(`${api_url}/dev`, data, http_type.post);
 /**
  * 删除设备
  * @param id
  * @returns {Promise<any>}
  */
 export const delete_device = (id) => ajax(`${api_url}/dev/${id}`, null, http_type.delete);
 /**
  * 更新设备
  * @param data
  * @returns {Promise<any>}
  */
 export const update_device = (data) => ajax(`${api_url}/dev`, data, http_type.put);
 /**
  * 获取设备
  * @param key
  * @returns {Promise<any>}
  */
 export const request_devices = (data) => ajax(`${api_url}/dev`, data);
 /**
  * 获取全部设备
  * @param key
  * @returns {Promise<any>}
  */
 export const request_all_devices = () => ajax(`${api_url}/dev/all`);
 /**
  * 获取设备简要信息
  * @param key
  * @returns {Promise<any>}
  */
 export const request_base_devices = () => ajax(`${api_url}/dev/devs`);
 /**
  * 获取设备类型
  * @param key
  * @returns {Promise<any>}
  */
 export const request_device_categories = () => ajax(`${api_url}/dev/ct`);
 //令牌管理
 /**
  * 获取全部令牌
  * @returns {Promise<any>}
  */
 export const request_tokens = (data) => ajax(`${api_url}/token`, data);
 /**
  * 强制令牌失效
  * @param userid
  * @returns {Promise<any>}
  */
 export const distory_token = (userid) => ajax(`${api_url}/token/${userid}`, null, http_type.delete);
 //配置相关接口


 /**
  * 通过Key获取系统配置信息
  * @returns {Promise<any>}
  */
 export const request_by_key_system_config = (key) => ajax(`${api_url}/sysconf/${key}`);
 /**
  * 新增系统配置信息
  * @param data
  * @returns {Promise<any>}
  */
 export const add_system_config = (data) => ajax(`${api_url}/sys`, data, http_type.put);
 /**
  * 修改系统配置信息
  * @param data
  * @returns {Promise<any>}
  */
 export const update_system_config = (data) => ajax(`${api_url}/sys`, data, http_type.post);
 /**
  * 删除系统配置信息
  * @param id
  * @returns {Promise<any>}
  */
 export const remove_system_config = (id) => ajax(`${api_url}/sys/${id}`, null, http_type.delete);

 /**
  * 获取转发地址信息
  * @returns {Promise<any>}
  */
 export const request_target_turn = () => ajax(`${api_url}/tgturn`);
 /**
  * 新增转发地址信息
  * @param data
  * @returns {Promise<any>}
  */
 export const add_target_turn = (data) => ajax(`${api_url}/tgturn`, data, http_type.put);
 /**
  * 删除转发地址信息
  * @param id
  * @returns {Promise<any>}
  */
 export const remove_target_turn = (id) => ajax(`${api_url}/tgturn/${id}`, null, http_type.delete);


 /**
  * 获取地图配置信息
  * @returns {Promise<any>}
  */
 export const request_map_config = () => ajax(`${api_url}/map`);
 /**
  * 新增地图配置
  * @param data
  * @returns {Promise<any>}
  */
 export const add_map_config = (data) => ajax(`${api_url}/map`, data, http_type.put);
 /**
  * 修改地图配置
  * @param data
  * @returns {Promise<any>}
  */
 export const update_map_config = (data) => ajax(`${api_url}/map`, data, http_type.post);
 /**
  * 删除地图配置
  * @param id
  * @returns {Promise<any>}
  */
 export const remove_map_config = (id) => ajax(`${api_url}/map/${id}`, null, http_type.delete);

 /**
  * 设备服务开启关闭
  * @param id
  * @param state
  * @returns {Promise<any>}
  */
 export const device_server_switch = ({ id, state }) => ajax(`${api_url}/ssw?id=${id}&open=${state}`, null, http_type.post);


 export const device_power_switch = ({ id, open }) => ajax(`${api_url}/sw?id=${id}&open=${open}`, null, http_type.post);
 /**
  * 设备开启关闭
  * @param id
  * @param state
  * @returns {Promise<any>}
  */
 export const device_switch = ({ id, state }) => ajax(`${api_url}/sw?id=${id}&open=${state}`, null, http_type.post);

 /**
  * 断开目标转发
  * @param tgid
  * @returns {Promise<any>}
  */
 export const break_target_transmit = (rid) => ajax(`${api_url}/transmit/${rid}`, null, http_type.delete);
 /**
  * 断开目标监视
  * @param tgid
  * @returns {Promise<any>}
  */
 export const break_monitor_target = (rid) => ajax(`${api_url}/monitor/${rid}`, null, http_type.delete);
 /**
  * 断开目标打击
  * @param tgid
  * @returns {Promise<any>}
  */
 export const break_hit_target = (rid) => ajax(`${api_url}/hit/${rid}`, null, http_type.delete);
