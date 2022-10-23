using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.AirSwitch02
{
    /// <summary>
    /// 网络继电器（tcp）
    /// </summary>   
    public class PluginConst
    {
        public const int Category = 30303;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 6001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_2";

        /// <summary>
        /// 跟踪目标信息命令字
        /// </summary>
        public static string MonitorTgCmdKey = $"{ProtocolNum}_0x102";

        /// <summary>
        /// 引导指令命令字
        /// </summary>
        public static string GuidanceCmdKey = "GD";
    }
}
