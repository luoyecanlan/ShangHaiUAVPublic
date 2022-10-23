using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct04
{
    /// <summary>
    /// 智空干扰协议（http)
    /// </summary>
    public class PluginConst
    {
        public const int Category = 30203;

        /// <summary>
        /// 航迹命令
        /// </summary>
        public const string TrackCmdKey = "TRACK";
        /// <summary>
        /// 状态命令
        /// </summary>
        public const string StatusCmdKey = "STATUS";
    }
}
