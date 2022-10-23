using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct02
{
    /// <summary>
    /// 数智元五频定向干扰器插件（udp）
    /// </summary>
    public class PluginConst
    {
        public const int Category = 30201;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 6001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_8d";

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
