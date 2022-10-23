using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct01
{
    /// <summary>
    /// 河南数智元全向干扰器插件udp
    /// </summary>
    public class PluginConst
    {
        public const int Category = 30101;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 6001;

        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_0x8d";

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
