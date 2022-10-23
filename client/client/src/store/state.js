export default {
  /**
   * 登录信息
   */
  login_info:{},
  /**
   * 设备类型
   */
  device_categories:[],
  /**
   * 设备列表
   */
  devices:[],
  /**
   * 阵地设备，此处使用云哨
   */
  positionDevices:[],
  /**
   * 设备关联的告警区列表
   */
  device_warning_zones:[],
  /**
   * 设备详细信息
   */
  device_info:undefined,
  /**
   * 设备状态信息
   */
  device_status:undefined,
  /**
   * 设备关联的目标列表
   */
  device_targets:[],

  device_infos:[],
  //device_status:{},
  device_guidance:[],
  device_relation_ships:[],
  tgId:'',
  /**
   * 目标详细信息
   */
  device_target_info:undefined,
  /**
   * 目标过期时间(秒)
   */
  target_expiration_time:15,
  /**
   * 目标列表
   */
  targets:[],
  targets_filter:[],
  targets_0level_num:0,// 0级告警数量
  targets_1level_num:0,
  targets_2level_num:0,
  targets_3level_num:0,
  targets_out_level_num:0,
  select_target_id:"",
  select_dev_id:"",
  select_was_info:{
    id:'',
    type:''
  },
  select_target_info:{},
  /**
   * 目标信息筛选条件
   */
  zero_level_check:true,
  one_level_check:true,
  two_level_check:true,
  three_level_check:true,
  out_level_check:true,
  alt_slider: [0, 3000],
  speed_slider:[-100, 200],
  /**
   * 用户和配置资源
   */
  target_turn_ip:'', // 转发地址
  target_turn_port:'', // 转发地址
  map_type:true, // true 在线地图 false 离线地图
  map_online_address:'', // 在线地图地址
  map_online_id:'', // 在线地图id
  map_offline_address:'', // 离线地图地址
  map_offline_id:'', // 离线地图id

  deviceCover:[],
  deviceLine:[],
  user_config_info:{
    mapId: 0,
    deviceCover: "{}", // 设备覆盖范围是否显示
    deviceLine: "{}", // 设备等距线是否显示
    filterVMin: -100,//
    filterVMax: 200,//
    filterDiscMin: 0,//
    filterDiscMax: 200,//
    filterAltMin: 0,// 最小海拔
    filterAltMax: 3000,// 最大海拔
    filterThreat: '{"zeroLevelCheck":true,"oneLevelCheck":true,"twoLevelCheck":true,"threeLevelCheck":true,"outLevelCheck":true}',// 告警级别
    filterCategory: "",// 无人机类型
    updatetime: "2020-04-17T07:03:54.529Z",
    id: 0
  },

  bandId:0, // 頻段id
  hitDevList:[], // 干扰设备
};
