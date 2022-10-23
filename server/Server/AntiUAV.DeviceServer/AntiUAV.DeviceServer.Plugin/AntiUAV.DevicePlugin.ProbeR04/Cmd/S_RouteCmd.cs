using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR04.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (content.Source.Length >= 4)
            {
                var src = content.Source;
                var Arr1 = PluginConst.StatusCheckHead;
                var Arr2 = PluginConst.CrossCheckHead;
                var Arr3 = PluginConst.TrackCheckHead;
                if (Arr1[0] == src[0] && Arr1[1] == src[1] && Arr1[2] == src[2] && Arr1[3] == src[3])
                    content.Route = PluginConst.StatusCmdKey;
                else if (Arr2[0] == src[0] && Arr2[1] == src[1] && Arr2[2] == src[2] && Arr2[3] == src[3])
                    content.Route = PluginConst.CrossCmdKey;
                else if (Arr3[0] == src[0] && Arr3[1] == src[1] && Arr3[2] == src[2] && Arr3[3] == src[3])
                    content.Route = PluginConst.TrackCmdKey;
                else
                {
                    content.ForcedOver = true;
                    return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
            else
            {
                content.ForcedOver = true;
                return Task.FromResult(false);
            }
        }
    }
}
