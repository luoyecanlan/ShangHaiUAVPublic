/**
 * 用户角色
 * @type {*[]}
 */
export const roleType=[
  { label:'客户端', value:'client' },
  { label:'超级管理员', value:'super' },
  { label:'管理员', value:'admin' },
  { label:'设备端', value:'device' },
  { label:'设备操作', value:'devopt' }
]

/**
 * 设备类型
 * @type {*[]}
 */
export const deviceType=[
  { label:'Device_A', value:0 },
  { label:'Device_B', value:1 },
  { label:'Device_C', value:2 },
  { label:'Device_D', value:3 }
]

/**
 * 区域操作初始化类型
 * @type {{removed: string, created: string, updated: string}}
 */
export const zoneInitType= {
  created: 'CREATED',
  updated: 'UPDATED',
  removed: 'REMOVED',
  initData: 'initdata'
}
