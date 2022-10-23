using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    /// <summary>
    /// 接收元数据输出方式
    /// </summary>
    public enum SourceOutMode
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// UTF8编码
        /// </summary>
        Utf8,
        /// <summary>
        /// Default编码
        /// </summary>
        Default,
        /// <summary>
        /// 十六进制形式
        /// </summary>
        Hex
    }
}
