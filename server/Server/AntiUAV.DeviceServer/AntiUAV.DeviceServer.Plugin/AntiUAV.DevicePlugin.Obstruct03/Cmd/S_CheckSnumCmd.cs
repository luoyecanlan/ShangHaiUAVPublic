using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using CSRedis;
using System;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct03.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;

        public Task<bool> Invoke(IPeerContent content)
        {

            return Task.FromResult(true);
            byte checksum = 0x00;
            for (int i = 3; i < content.Source.Length - 2; i++)
            {
                checksum += content.Source[i];
            }
            var checkbyte = content.Source[content.Source.Length-1];
           
            return Task.FromResult(checkbyte == checksum);
        }
    }
}
