import {regType} from "./ruleType";

export function validationNumber(arr) {
  let retArr= arr.filter(f => {
    if (f.func) {
      return  isNumber(f.value) &&f.func(parseFloat(f.value));
    }
    return isNumber(f.value);
  });
  return retArr.length==arr.length;
}

/**
 * 验证数据
 * @param arr
 * @returns {boolean}
 */
export function validation(arr) {
  let retArr= arr.filter(f => {
    if (f.func) {
      return f.func(f.value);
    }else{
      return !!f;
    }
  });
  return retArr.length==arr.length;
}

/**
 * 是否为端口
 * @param f
 * @returns {boolean}
 */
export function isPort(f) {
  return isNumber(f) && f > 3000 && f < 65535;
}

/**
 * 是否为IP地址
 * @param f
 * @returns {boolean}
 */
export function isIP(f) {
  return regType.ip.pattern.test(f);
}
/**
 * 纬度范围[-90,90]
 * @param f
 */
export function isLat(f) {
  return f>=-90&&f<=90;
}

/**
 * 经度范围[-180,180]
 * @param f
 */
export function isLng(f) {
  return f>=-180&&f<=180;
}

/**
 * 方位范围[0，360]
 * @param f
 */
export function isAz(f) {
  return f>=-0&&f<=360;
}
/**
 * 俯仰范围[-90，90]
 * @param f
 */
export function isEl(f) {
  return f > -90 && f < 90;
}

/**
 * 浮点型数字验证
 * @param val
 * @returns {boolean}
 */
function isNumber(val) {
  let regPos = /^\d+(\.\d+)?$/; //非负浮点数
  let regNeg = /^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$/; //负浮点数
  if (regPos.test(val) || regNeg.test(val)) {
    return true;
  } else {
    return false;
  }
}

/**
 * 延迟执行一次
 * @param func
 */
export function exceOnce(func,time=3000) {
  let _handle = setTimeout(() => {
    func && func();
    clearTimeout(_handle);
  }, time);
}
