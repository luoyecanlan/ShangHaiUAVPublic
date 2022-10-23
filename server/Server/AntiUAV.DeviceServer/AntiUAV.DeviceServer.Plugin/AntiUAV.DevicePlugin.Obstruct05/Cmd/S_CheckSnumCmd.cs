using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using CSRedis;
using System;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;

        public Task<bool> Invoke(IPeerContent content)
        {

            return Task.FromResult(true);
          
        }
    }
}
