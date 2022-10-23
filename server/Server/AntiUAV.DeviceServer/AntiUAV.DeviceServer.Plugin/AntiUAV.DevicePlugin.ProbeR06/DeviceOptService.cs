using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;

namespace AntiUAV.DevicePlugin.ProbeR06
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public override int DeviceCategory => PluginConst.Category;
    }
}
