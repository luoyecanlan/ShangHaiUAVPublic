using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public class DeviceFollow
    {
        /// <summary>
        /// 操作码
        /// </summary>
        public FollowOperateCode operateCode { get; set; }
        
    }

    /// <summary>
    /// 设备状态码
    /// </summary>
    public enum FollowOperateCode
    {
        Search=1,
        Stop=2
    }
}
