/*常量名称模板*/

export const DEVSTATE_OFFLINE = '#A8A8A9'
export const DEVSTATE_ONLINE = '#FFD800'
export const DEVSTATE_WORK = '#00FF24'
export const DEVSTATE_ALARM = '#FE0201'

export const DEVSTATE_OFFLINE_BorderColor = '2px solid #A8A8A9'
export const DEVSTATE_ONLINE_BorderColor = '2px solid #FFD800'
export const DEVSTATE_WORK_BorderColor = '2px solid #00FF24'
export const DEVSTATE_ALARM_BorderColor = '2px solid #FE0201'

export const TG_LEVEL0 = '#40CEF9'
export const TG_LEVEL1 = '#FE0201'
export const TG_LEVEL2 = '#FE00FE'
export const TG_LEVEL3 = '#FFDD00'
export const TG_LEVEL4 = '#A8A8A9'

export const TG_LEVEL0_BorderColor = '2px solid #40CEF9'
export const TG_LEVEL1_BorderColor = '2px solid #FE0201'
export const TG_LEVEL2_BorderColor = '2px solid #FE00FE'
export const TG_LEVEL3_BorderColor = '2px solid #FFDD00'
export const TG_LEVEL4_BorderColor = '2px solid #A8A8A9'

// 获取颜色
export function GetTargetAlarmColor(threat, trans) {
  let color = '';
  // switch (threat) {
  //   case 0:
  //     color = "rgb(67, 220, 255 ,"+trans+")"//蓝色
  //     break;
  //   case 1:
  //     color = "rgb(255, 0, 0 ,"+trans+")"  // 红色
  //     break;
  //   case 2:
  //     color = "rgb(254, 0, 254 ,"+trans+")"  //紫色
  //     break;
  //   case 3:
  //     color = "rgb(255, 216, 0,"+trans+")" //黄色
  //     break;
  //   default:
  //     color = "rgb(67, 220, 255 ,"+trans+")" //蓝色
  //     break;
  // }
  // return color;
  switch (threat) {
    case 0:
      color = "rgba(0,170,24," + trans + ")"//绿色
      break;
    case 1:
      color = "rgba(67, 220, 255 ," + trans + ")"  // 蓝色
      break;
    case 2:
      color = "rgba(67, 220, 255 ," + trans + ")"  //蓝色
      break;
    case 3:
      color = "rgba(255, 0, 0," + trans + ")" //红色
      break;
    default:
      color = "rgba(0,170,24 ," + trans + ")" //红色
      break;
  }
  return color;
}
