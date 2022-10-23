using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 云台设备控制
    /// </summary>
    public class DevicePTZInfo
    {
        /// <summary>
        /// 水平角
        /// </summary>
        public int Az { get; set; }
        /// <summary>
        /// 俯仰角
        /// </summary>
        public int El { get; set; }
        /// <summary>
        /// 转台速度 转台速度 视场角增减速度 焦距增减速度（十一所光电）
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// 操作码（1开始2停止 11所光电）
        /// </summary>
        public OperateCode operateCode { get; set; }
        /// <summary>
        /// 操作项（1-水平 2-俯仰 3-归零（转台0位）4-指北（修正后0位）5-视场角 6-焦距 十一所光电）
        /// </summary>
        public OperateItem operateItem { get; set; }
        
    }
    /// <summary>
    /// 操作项
    /// </summary>
    public enum OperateItem
    {
        /// <summary>
        /// 水平
        /// </summary>
        Az=1,
        /// <summary>
        /// 俯仰
        /// </summary>
        El=2,
        /// <summary>
        /// 归零（转台0）
        /// </summary>
        ToZero=3,
        /// <summary>
        /// 指北（纠偏后北）
        /// </summary>
        ToNorth=4,
        /// <summary>
        /// 视场
        /// </summary>
        Field=5,
        /// <summary>
        /// 焦距
        /// </summary>
        Focus=6
    }
    /// <summary>
    /// 操作项
    /// </summary>
    public enum OperateCode
    {
        /// <summary>
        /// 开始
        /// </summary>
        On=1,
        /// <summary>
        /// 停止
        /// </summary>
        Off=2
        
    }

}
