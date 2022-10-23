using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR06.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (string.IsNullOrEmpty(content.Route))
            {
                if (content.Source.Length != 88)
                {

                }
                content.Route = $"{PluginConst.ProtocolNum}_{(content.Source.Length)}";
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
