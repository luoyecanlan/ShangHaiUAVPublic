using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeR01
{
    /// <summary>
    /// 无人机接入APP插件
    /// </summary>
    public class PluginConst
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public const int Category = 10101;

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
