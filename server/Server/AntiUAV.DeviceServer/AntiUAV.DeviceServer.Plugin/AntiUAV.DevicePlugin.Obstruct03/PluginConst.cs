using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct03
{
    /// <summary>
    /// 特信干扰器插件（udp）
    /// </summary>
    public class PluginConst
    {
        public const int Category = 30202;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 6001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_823";

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
