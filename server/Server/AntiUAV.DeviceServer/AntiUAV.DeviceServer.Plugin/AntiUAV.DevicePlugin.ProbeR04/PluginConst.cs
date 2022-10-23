namespace AntiUAV.DevicePlugin.ProbeR04
{
    /// <summary>
    /// 安徽耀峰雷达插件
    /// </summary>
    public class PluginConst
    {
        public const int Category = 10104;

        public const string TrackCmdKey = "TRACK";

        public const string StatusCmdKey = "STATUS";

        public const string CrossCmdKey = "CROSS";

        public static readonly byte[] TrackCheckHead = { 0xCC, 0x55, 0x55, 0xCC };

        public static readonly byte[] StatusCheckHead = { 0x55, 0xDD, 0xDD, 0x55 };

        public static readonly byte[] CrossCheckHead = { 0x55, 0xCC, 0xCC, 0x55 };
    }
}
