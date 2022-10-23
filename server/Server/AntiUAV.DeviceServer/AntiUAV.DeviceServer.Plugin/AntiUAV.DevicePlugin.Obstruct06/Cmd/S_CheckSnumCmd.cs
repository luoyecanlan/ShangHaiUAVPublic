using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using CSRedis;
using System;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct06.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;

        public Task<bool> Invoke(IPeerContent content)
        {
            var crc = BitConverter.ToUInt16(content.Source, content.Source.Length - 3);

            byte[] data=new byte[content.Source.Length-4];
            Array.ConstrainedCopy(content.Source, 2, data, 0, data.Length);
            var cacl = data.CRC16_CCITT(data.Length);
            return Task.FromResult(crc == cacl);
        }
    }
}
