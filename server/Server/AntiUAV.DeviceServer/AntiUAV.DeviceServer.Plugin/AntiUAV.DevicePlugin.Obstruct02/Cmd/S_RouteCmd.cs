using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct02.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (string.IsNullOrEmpty(content.Route))
            {
                //按协议修改
                if (content.Source[2] == 0x89 || content.Source[2] == 0x8A || content.Source[2] == 0x8d)
                {
                    content.Route = $"{PluginConst.ProtocolNum}_8d";
                }
                else
                    content.Route = $"{PluginConst.ProtocolNum}_{Convert.ToString(content.Source[2], 16)}";
            }
            else
            {
                content.Route = PluginConst.GuidanceCmdKey;
            }
            return Task.FromResult(true);
        }
    }
}
