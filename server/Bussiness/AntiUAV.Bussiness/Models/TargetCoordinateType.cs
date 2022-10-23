using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 目标坐标类型
    /// </summary>
    public enum TargetCoordinateType
    {
        /// <summary>
        /// 无效
        /// </summary>
        None,
        /// <summary>
        /// 经纬度
        /// </summary>
        LongitudeAndLatitude,
        /// <summary>
        /// 感知
        /// </summary>
        Perception
    }
}
