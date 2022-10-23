using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05.Cmd
{
    public class S_HeadTailCmd : IPeerSysCmd
    {
        public S_HeadTailCmd(ILogger<S_HeadTailCmd> logger)
        {
            _logger = logger;
        }
        private readonly ILogger _logger;
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.CheckHeadAndTail;

        /// <summary>
        /// 头
        /// </summary>
        public static readonly byte[] Head = { 0xff, 0xa5, 0xa5 };

        /// <summary>
        /// 尾
        /// </summary>
        public static readonly byte[] End = { 0xaa };

        public Task<bool> Invoke(IPeerContent content)
        {
         
            return Task.FromResult(true);
        }
    }
}
