using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    public class DeviceStatusModel : DeviceStatus
    {
        /// <summary>
        /// 是否正在引导数据
        /// </summary>
        public bool IsGuidance { get; set; }
        /// <summary>
        /// 是否正在被引导
        /// </summary>
        public bool IsBeGuidance { get; set; }
        /// <summary>
        /// 是否正在转发数据
        /// </summary>
        public bool IsTurnTarget { get; set; }
    }
}
