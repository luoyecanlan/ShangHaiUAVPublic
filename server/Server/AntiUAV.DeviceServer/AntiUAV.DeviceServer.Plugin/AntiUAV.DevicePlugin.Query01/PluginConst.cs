using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Query01
{
    /// <summary>
    /// 783问答机插件(也是发现目标，当作雷达处理）
    /// </summary>
    public class PluginConst
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public const int Category = 10106;

        /// <summary>
        /// 航迹命令(广播）
        /// </summary>
        //public const string TrackCmdKey = "4d";

        /// <summary>
        /// 航迹命令（问答）
        /// </summary>
        public const string TrackCmdKey = "4a";
        /// <summary>
        /// 状态命令
        /// </summary>
        public const string StatusCmdKey = "STATUS";
    }
}
