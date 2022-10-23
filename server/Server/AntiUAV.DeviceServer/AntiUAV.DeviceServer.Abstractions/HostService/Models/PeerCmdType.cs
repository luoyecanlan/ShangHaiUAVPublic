using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    public enum PeerCmdType
    {
        /// <summary>
        /// 中间件（在解析后执行）
        /// </summary>
        Middleware,
        /// <summary>
        /// 过滤器（在解析前执行）
        /// </summary>
        Filter,
        /// <summary>
        /// 行为命令（解析操作）
        /// </summary>
        Action
    }
}
