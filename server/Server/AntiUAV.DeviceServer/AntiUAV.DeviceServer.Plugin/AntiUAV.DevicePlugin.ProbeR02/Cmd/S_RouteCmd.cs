using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR02.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (string.IsNullOrEmpty(content.Route))
            {
                content.Route = $"{PluginConst.ProtocolNum}_{BitConverter.ToInt32(content.Source, 12)}";
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
