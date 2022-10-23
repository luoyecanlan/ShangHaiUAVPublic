using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 转发目标信息
    /// </summary>
    public class TurnTargetInfo
    {
        /// <summary>
        /// 转发数据设备
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 目标ID
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
