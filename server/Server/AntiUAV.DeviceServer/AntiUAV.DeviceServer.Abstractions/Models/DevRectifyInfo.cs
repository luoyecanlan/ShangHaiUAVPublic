using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 纠偏信息
    /// </summary>
    public class DevRectifyInfo
    {
        public double Az { get; set; }

        public double El { get; set; }

        public bool Equals(DeviceInfo dev)
        {
            if (dev != null)
            {
                return dev.RectifyAz == Az && dev.RectifyEl == El;
            }
            return default;
        }
    }
}
