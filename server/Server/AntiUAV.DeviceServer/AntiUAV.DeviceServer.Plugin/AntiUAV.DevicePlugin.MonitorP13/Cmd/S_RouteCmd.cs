using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP13.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (string.IsNullOrEmpty(content.Route))
            {
                var tmp = BitConverter.ToInt32(content.Source, 12);
                //byte[] targetAz = BitConverter.GetBytes(tmp);
                //Array.Reverse(targetAz);
                //var order = Convert.ToString(BitConverter.ToInt32(targetAz), 16).PadRight(7,'0');
                content.Route = $"{PluginConst.ProtocolNum}_{tmp}";
            }
            else
            {
                content.Route = PluginConst.GuidanceCmdKey;
            }
            return Task.FromResult(true);
        }
    }
}
