using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.MonitorP11
{
    /// <summary>
    /// 神戎光电
    /// </summary>
    public class PluginConst
    {
        public const int Category = 20111;

        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 5003;
        /// <summary>
        /// 状态命令字
        /// </summary>
        public static string StatusCmdKey = $"{ProtocolNum}_1010000";

        /// <summary>
        /// 跟踪目标信息命令字
        /// </summary>
        public static string MonitorTgCmdKey = $"{ProtocolNum}_1020000";
        /// <summary>
        /// 位置修正纠偏设置
        /// </summary>
        public static string RectifyCmdKey = $"{ProtocolNum}_2010000";
        /// <summary>
        /// 跟踪操作指令
        /// </summary>
        public static string FollowOptCmdKey = $"{ProtocolNum}_2020000";
        /// <summary>
        /// 设备操作指令
        /// </summary>
        public static string DeviceOptCmdKey = $"{ProtocolNum}_2030000";
        /// <summary>
        /// 引导指令命令字
        /// </summary>
        public static string GuidanceCmdKey = $"{ProtocolNum}_2040000";
       

    }
}
