using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR06.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;

        public Task<bool> Invoke(IPeerContent content)
        {
            //var crc = BitConverter.ToUInt32(content.Source, content.Source.Length - 4);
            //var cacl = content.Source.CRC32(4u, (uint)content.Source.Length - 4);

            //return Task.FromResult(crc == cacl);
            return Task.FromResult(true);
        }
    }
}
