using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct06
{
    /// <summary>
    /// 大项目干扰（tcp)
    /// </summary>
    public class PluginConst
    {
        public const int Category = 30206;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 6001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_17";

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
