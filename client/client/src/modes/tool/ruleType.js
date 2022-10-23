/**
 * 类型限制枚举
 * @type {{date: string, number: string, boolean: string, string: string, method: string, array: string, hex: string, integer: string, float: string, enum: string, url: string, email: string}}
 */
export const type_enum={
  string:'string',
  number:'number',
  boolean:'boolean',
  method:'method',
  integer:'integer',
  float:'float',
  enum:'enum',
  date:'date',
  array:'array',
  url:'url',
  hex:'hex',
  email:'email'
}

/**
 * 常用的验证正则表达式
 * @type {{phone: RegExp}}
 */
export const regType= {
  phone: {
    pattern: /^1[34578]\d{9}$/,
    message: '目前只支持中国大陆的手机号码'
  },
  ip:{
    pattern:/^((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})(\.((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})){3}$/,
    message:'IP地址格式错误'
  },
  id:{
    pattern:/(^[1-9]\d*$)/,
    message:'ID必须为正整数'
  }
}

/**
 * 必填项规则
 * @param name
 * @returns {{message: string, required: boolean}}
 */
function rule_required(name) {
  return {
    required: true,
    message: `input ${name},please!`,
    trigger:'change'
  }
}

/**
 * 类型规则
 * @param type (one of type_enum)
 * @returns {{type: *, message: string}}
 */
function rule_type(type) {
  return{
    type,
    message: `type is not ${type}!`,
    trigger:'blur'
  }
}

/**
 * 范围规则
 * A range is defined using the min and max properties.
 * For string and array types comparison is performed against the length,
 * for number types the number must not be less than min nor greater than max.
 * @param min
 * @param max
 * @returns {{message: string, rang: {min: *, max: *}}}
 */
function rule_rang(min,max) {
  return{
    min: min,
    max: max,
    message: `length must between [${min},${max}]!`,
    trigger:'blur'
  }
}

/**
 * 长度验证规则
 * To validate an exact length of a field specify the len property.
 * For string and array types comparison is performed on the length property,
 * for the number type this property indicates an exact match for the number,
 * ie, it may only be strictly equal to len.
 If the len property is combined with the min and max range properties, len takes precedence.
 * @param len
 * @returns {{length: *, message: string}}
 */
function rule_length(len) {
  return{
    len,
    message:`more than length ${len}!`,
    trigger:'blur'
  }
}

/**
 * 枚举验证规则
 * @param enumValue
 * @returns {{type: string, message: string, enum: *}}
 */
function rule_enumerable(enumValue) {
  return {
    type:type_enum.enum,
    enum:enumValue,
    message:`value must one of ${JSON.stringify(enumValue)}!`,
    trigger:'blur'
  }
}

/**
 * 这个则表达式验证
 * @param pattern
 * @param message
 * @returns {{pattern: *, message: *}}
 */
function rule_pattern(pattern,message) {
  return {
    pattern,
    message,
    trigger:'blur'
  }
}

/**
 * 自定义验证规则
 * @param validator
 * @param params  （rule，value）
 * @param message
 * @returns {{validator: (function(): *), message: string}}
 */
function rule_validator(validator,message='') {
  return {
    validator,
    message,
    trigger:'change'
  }
}

/**
 * 必填项
 * @param prop
 * @returns {{message: string, required: boolean}[]}
 */
export function validator_required(prop) {
  return [
    rule_required(prop)
  ]
}

/**
 * 字符串必填
 * @param min
 * @param max
 * @param prop
 * @returns {*[]}
 */
export function validator_required_rang(min,max,prop) {
  return[
    rule_required(prop),
    rule_rang(min,max),
  ]
}

/**
 * 字符串范围验证
 * @param min
 * @param max
 * @param prop
 * @returns {*[]}
 */
export function validator_rang(min,max) {
  return[
    rule_rang(min,max),
  ]
}
/**
 * 电话号码验证
 * @param prop
 * @returns {*[]}
 */
export function validator_phone() {
  return [
    rule_pattern(regType.phone.pattern, regType.phone.message)
  ]
}
/**
 * 电话号码验证
 * @param prop
 * @returns {*[]}
 */
export function validator_required_phone() {
  return [
    rule_required('phone'),
    rule_pattern(regType.phone.pattern, regType.phone.message)
  ]
}

/**
 * IP地址必填验证
 * @param prop
 * @returns {*[]}
 */
export function validator_required_ip(prop) {
  return[
    rule_required(prop),
    rule_pattern(regType.ip.pattern,regType.ip.message)
  ]
}
/**
 * IP地址验证
 * @param prop
 * @returns {*[]}
 */
export function validator_ip(prop) {
  return[
    rule_pattern(regType.ip.pattern,regType.ip.message)
  ]
}
/**
 * 类型验证
 * @param type
 * @returns {{type: *, message: string}[]}
 */
export function validator_type(type) {
  return[
    rule_type(type)
  ]
}
/**
 * 必填项按照类型验证
 * @param prop
 * @param type
 * @returns {*[]}
 */
export function validator_required_type(prop,type) {
  return[
    rule_required(prop),
    rule_type(type)
  ]
}

/**
 * 验证数字范围且必填
 * @param validator
 * @param prop
 * @param message
 * @returns {*[]}
 */
export function validator_required_number_rang(min,max,prop) {
  return [
    rule_required(prop),
    {
      validator: (rule, value, callback) => {
        console.log('rule_required', value)
        let _is = value <= max && value >= min;
        if (_is) callback();
        else
          return callback(new Error(`value must between [${min},${max}]`))
      },
      trigger: 'blur'
    }
  ]
}

/**
 * 数字范围验证
 * @param min
 * @param max
 * @returns {{validator: (function(*): boolean), message: string}[]}
 */
export function validator_number_rang(min,max) {
  return [
    {
      validator: (rule, value, callback) => {
        console.log('rule_required', value)
        let _is = value <= max && value >= min;
        if (_is) callback();
        else
          return callback(new Error(`value must between [${min},${max}]`))
      },
      trigger: 'blur'
    }
  ]
}

/**
 * Id必填验证
 * @param prop
 * @returns {*[]}
 */
export function validator_required_id(prop) {
  return[
    rule_required(prop),
    rule_pattern(regType.id.pattern,regType.id.message)
  ]
}

/**
 * Id验证
 * @returns {{pattern: *, message: *}[]}
 */
export function validator_id() {
  return[
    rule_pattern(regType.id.pattern,regType.id.message)
  ]
}

