using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR05.Cmd
{
    public class S_CheckSnumCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Checksnum;
        /// <summary>
        /// 小雷达协议校验位不在结尾 不固定 无法在此处校验
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task<bool> Invoke(IPeerContent content)
        {
            //var crc = BitConverter.ToUInt32(content.Source, content.Source.Length - 8);
            //var cacl = content.Source.CRC32(8u, (uint)content.Source.Length - 8);
            //return Task.FromResult(crc == cacl);
            return Task.FromResult(true);
        }
    }
}
