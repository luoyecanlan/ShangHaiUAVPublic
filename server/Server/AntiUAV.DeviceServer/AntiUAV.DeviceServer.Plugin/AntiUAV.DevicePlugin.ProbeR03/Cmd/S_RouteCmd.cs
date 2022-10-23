using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR03.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (content.Source.Length > 5 && content.Source[0] == 0x77 && content.Source[1] == 0xFF && content.Source[2] == 0xFF && content.Source[3] == 0xFF && content.Source[4] == 0xFF && content.Source[5] == 0xFF)
            {
                content.Route = PluginConst.TrackCmdKey;
            }
            else if (content.Source.Length > 4 && content.Source[0] == 0x5a && content.Source[1] == 0x5a && content.Source[2] == 0x08 && content.Source[3] == 0x02)
            {
                content.Route = PluginConst.StatusCmdKey;
            }
            else
            {
                content.ForcedOver = true;
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
