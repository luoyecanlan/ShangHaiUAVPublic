using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP11.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;

        public Task<bool> Invoke(IPeerContent content)
        {
            //var crc = BitConverter.ToUInt32(content.Source, content.Source.Length - 8);
            //var cacl = content.Source.CRC32(8u, (uint)content.Source.Length - 8);
            byte[] buff = new byte[4];
            Array.Copy(content.Source, content.Source.Length - 8, buff,0,4);
            Array.Reverse(buff);
            var crc = BitConverter.ToUInt32(buff);

            int cacl = 0;
            for(int i=8;i< content.Source.Length - 8; i++)
            {
                cacl += content.Source[i];
            }
            
           
            
            return Task.FromResult(crc == cacl);
        }
    }
}
