using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeR02
{
    /// <summary>
    /// 中电科27所 机相扫雷达插件
    /// </summary>
    public class PluginConst
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public const int Category = 10102;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 9001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_5";

        /// <summary>
        /// 航迹命令字
        /// </summary>
        public static string TrackCmdKey = $"{ProtocolNum}_4";
    }
}
