using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfo: DbOrm.AntiUAV.Entity.DeviceInfo
    {
        /// <summary>
        /// 本地数据更新时间
        /// </summary>
        public DateTime LocalUpdateTime { get; set; }
    }
}
