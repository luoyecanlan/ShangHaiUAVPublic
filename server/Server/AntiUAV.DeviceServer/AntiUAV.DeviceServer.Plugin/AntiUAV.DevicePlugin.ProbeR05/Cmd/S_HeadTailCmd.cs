using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR05.Cmd
{
    public class S_HeadTailCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.CheckHeadAndTail;

        /// <summary>
        /// 头
        /// </summary>
        public static readonly byte[] Head = { 0xaa, 0xaa, 0xaa, 0xaa };

        public Task<bool> Invoke(IPeerContent content)
        {
            var head = new byte[Head.Length];
            Buffer.BlockCopy(content.Source, 0, head, 0, head.Length);
            return Task.FromResult(Enumerable.SequenceEqual(Head, head));
        }
    }
}
