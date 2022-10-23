using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeR05
{
    /// <summary>
    /// 雷可达边境雷达插件（采用小端在前模式）
    /// </summary>
    public class PluginConst
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public const int Category = 10105;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 9001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_3";

        /// <summary>
        /// 航迹命令字
        /// </summary>
        public static string TrackCmdKey = $"{ProtocolNum}_1";
    }
}
