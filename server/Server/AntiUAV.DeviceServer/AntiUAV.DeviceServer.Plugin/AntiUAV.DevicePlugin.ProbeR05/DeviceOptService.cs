using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR05
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public override int DeviceCategory => PluginConst.Category;
    }
}
