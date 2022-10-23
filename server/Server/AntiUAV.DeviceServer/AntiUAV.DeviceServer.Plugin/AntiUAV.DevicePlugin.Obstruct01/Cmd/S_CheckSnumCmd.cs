using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using CSRedis;
using System;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct01.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;

        public Task<bool> Invoke(IPeerContent content)
        {
            var checkbyte = content.Source[content.Source.Length-1];
            //uint check = 0;
            byte checksum = 0;
            int i = 0;
            foreach (byte b in content.Source)
            {
                if (i == content.Source.Length-1)
                {
                    break;
                }
                checksum += b;
                i++;
                
            }
            //BitConverter.ToUInt16(checksum);
            //return Task.FromResult(crc == cacl);
            return Task.FromResult(checkbyte == checksum);
        }
    }
}
