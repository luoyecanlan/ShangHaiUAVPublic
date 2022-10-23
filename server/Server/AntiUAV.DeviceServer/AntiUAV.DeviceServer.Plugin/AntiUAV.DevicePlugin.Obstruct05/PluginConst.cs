using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct05
{
    /// <summary>
    /// 干扰枪插件（http）
    /// </summary>
    public class PluginConst
    {
        public const int Category = 30204;

        /// <summary>
        /// 航迹命令
        /// </summary>
        public const string TrackCmdKey = "TRACK";
        /// <summary>
        /// 状态命令
        /// </summary>
        public const string StatusCmdKey = "STATUS";
        /// <summary>
        /// 引导指令命令字
        /// </summary>
        public static string GuidanceCmdKey = "GD";
    }
}
