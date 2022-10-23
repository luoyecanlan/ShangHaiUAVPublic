using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    /// <summary>
    /// 管道服务状态
    /// </summary>
    public enum PeerServerState
    {
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 运行
        /// </summary>
        Run,
    }
}
