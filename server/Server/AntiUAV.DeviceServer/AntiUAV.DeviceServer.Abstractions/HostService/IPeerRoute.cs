using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService
{
    /// <summary>
    /// 管道路由
    /// </summary>
    public interface IPeerRoute
    {
        /// <summary>
        /// 获取系统命令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        IPeerSysCmd GetSysCmd(PeerSysCmdType cmdType);

        /// <summary>
        /// 获取一般命令
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPeerCmd> GetCmd(PeerCmdType cmdType);


        /// <summary>
        /// 获取事件命令
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IPeerCmd GetCmd(string key);

        /// <summary>
        /// 管道路由执行
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task ExcutePipeLineAsync(IPeerContent content);

        /// <summary>
        /// 初始化路由命令
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        void LoadCmds(IServiceProvider provider);
    }
}
