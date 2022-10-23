using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.PluginService
{
    /// <summary>
    /// 设备本地服务
    /// </summary>
    public interface IDeviceHostService
    {
        int DeviceCategory { get; }
        
        bool IsSysUdp { get; }

        void Start();

        void Stop();

        void HostState();

        /// <summary>
        /// 设备运行周期码（4位随机数）
        /// </summary>
        string RunCode { get; set; }

        /// <summary>
        /// 使用外部刷新运行周期码时间 为0则不使用 单位秒
        /// </summary>
        int OutRefRunCodeTime { get; }
    }
}
