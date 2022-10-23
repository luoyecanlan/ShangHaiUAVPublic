namespace AntiUAV.DevicePlugin.ProbeR06
{
    /// <summary>
    /// 西安易泽丰雷达插件
    /// </summary>
    public class PluginConst
    {
        public const int Category = 10107;
        /// <summary>
        /// 协议号
        /// </summary>
        public const int ProtocolNum = 9001;

        public static string TrackCmdKey = $"{ProtocolNum}_64";

        public static string StatusCmdKey = $"{ProtocolNum}_88";



    }
}
