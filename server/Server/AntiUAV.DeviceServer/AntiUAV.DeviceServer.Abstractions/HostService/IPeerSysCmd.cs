using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService
{
    /// <summary>
    /// 系统管道命令
    /// </summary>
    public interface IPeerSysCmd
    {
        /// <summary>
        /// 命令Key-设备的类型，用于区分不同设备的命令
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 命令类型-用于区分不同系统命令
        /// </summary>
        PeerSysCmdType Order { get; }
        /// <summary>
        /// 执行内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<bool> Invoke(IPeerContent content);
    }
}
