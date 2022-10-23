using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    public class GuidanceTargetModel
    {
        /// <summary>
        /// 引导设备Id
        /// </summary>
        public int FromDeviceId { get; set; }
        /// <summary>
        /// 目标Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 被引导设备Id
        /// </summary>
        public int ToDeviceId { get; set; }
        /// <summary>
        /// 引导信息是否被验证
        /// 需要与被引导设备上的被引导信息进行比较
        /// </summary>
        public bool IsValidation { get; set; }

    }
}
