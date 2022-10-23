using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 设备位置信息
    /// </summary>
    public class DevPositionInfo
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        public double Alt { get; set; }

        public bool Equals(DeviceInfo dev)
        {
            if (dev != null)
            {
                return dev.Lat == Lat && dev.Lng == Lng && dev.Alt == Alt;
            }
            return default;
        }
    }
}
