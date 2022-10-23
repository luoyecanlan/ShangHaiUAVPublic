using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 被引导目标信息
    /// </summary>
    public class BeGuidanceTargetInfo
    {
        public BeGuidanceTargetInfo(string tgId, int fromId)
        {
            TargetId = tgId;
            FromDeviceId = fromId;
            GuidanceInfo = new GuidancePositionInfo(TargetId);
        }
        /// <summary>
        /// 目标ID
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 引导设备ID
        /// </summary>
        public int FromDeviceId { get; set; }

        /// <summary>
        /// 被引导信息无效标记
        /// </summary>
        //public bool Invalid { get; set; }

        /// <summary>
        /// 引导位置信息
        /// </summary>
        public GuidancePositionInfo GuidanceInfo { get; private set; }
    }
}
