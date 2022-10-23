using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeP02
{
    /// <summary>
    /// 历正频谱探测插件
    /// </summary>
    public class PluginConst
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public const int Category = 10201;

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
