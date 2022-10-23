using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{

    [PeerCmd(only: true)]
    public enum PeerSysCmdType
    {
        /// <summary>
        /// 头尾校验命令
        /// </summary>
        CheckHeadAndTail,
        /// <summary>
        /// 校验和校验命令
        /// </summary>
        Checksnum,
        /// <summary>
        /// 路由命令（寻址操作）
        /// </summary>
        [PeerCmd(true, true)]
        Route,
        /// <summary>
        /// 警告命令
        /// </summary>
        Warn,
        /// <summary>
        /// 异常命令
        /// </summary>
        Error,
        /// <summary>
        /// 透传命令
        /// </summary>
        Out,
        /// <summary>
        /// 消息命令
        /// </summary>
        Msg,
    }
}
