using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 设备状态信息
    /// </summary>
    public class DeviceStatusInfo : DeviceStatus
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        internal List<ErrorInfo> Errors { get; set; } = new List<ErrorInfo>();

        /// <summary>
        /// 被引导信息
        /// </summary>
        public BeGuidanceTargetInfo BeGuidanceInfo { get; set; }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public class ErrorInfo
    {
        public DateTime ErrorTime { get; set; }

        public ErrorCodeEnum ErrorCode { get; set; }

        public long BitCode { get; set; }

        public string ErrorMsg { get; set; }
    }

    /// <summary>
    /// 错误枚举
    /// </summary>
    public enum ErrorCodeEnum
    {
        /// <summary>
        /// 设备BIT
        /// </summary>
        Bit,
        /// <summary>
        /// 通信故障
        /// </summary>
        CommunicationFailure,
        /// <summary>
        /// 设置不一致
        /// </summary>
        InconsistentSettings
    }
}
