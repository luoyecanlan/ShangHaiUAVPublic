using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 消失目标
    /// </summary>
    public class TargetDisappear
    {
        /// <summary>
        /// 目标ID
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 消失时间
        /// </summary>
        public DateTime DisappearTime { get; set; }

        /// <summary>
        /// 消失原因
        /// </summary>
        public DisappearType CauseOfDisappear { get; set; }
    }

    /// <summary>
    /// 消失类型
    /// </summary>
    public enum DisappearType
    {
        /// <summary>
        /// 设备服务超时
        /// </summary>
        TimeOut,
        /// <summary>
        /// 中心服务超时
        /// </summary>
        ServerTimeOut,
        /// <summary>
        /// 销批
        /// </summary>
        ClearBatch
    }
}
