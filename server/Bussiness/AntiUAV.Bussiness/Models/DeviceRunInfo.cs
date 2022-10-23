using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 设备运行参数
    /// </summary>
    public class DeviceRunInfo
    {
        /// <summary>
        /// 实时运行码
        /// 此字段根据设备类型或设备大类不同含义不同
        /// </summary>
        public int RunCode { get; set; }
        /// <summary>
        /// 当前方位
        /// </summary>
        public double CurrentAz { get; set; }
        /// <summary>
        /// 当前俯仰
        /// </summary>
        public double CurrentEl { get; set; }
    }
}
