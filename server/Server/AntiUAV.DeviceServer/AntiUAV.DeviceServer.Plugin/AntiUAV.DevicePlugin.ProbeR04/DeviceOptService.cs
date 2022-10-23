using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;

namespace AntiUAV.DevicePlugin.ProbeR04
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public override int DeviceCategory => PluginConst.Category;
    }
}
