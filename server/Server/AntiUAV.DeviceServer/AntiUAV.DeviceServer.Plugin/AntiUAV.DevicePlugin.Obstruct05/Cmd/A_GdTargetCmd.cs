using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05.Cmd
{
    public class A_GdTargetCmd : IPeerCmd
    {
        public A_GdTargetCmd(ILogger<A_GdTargetCmd> logger, IMemoryCache memory, IServiceOpt opt)
        {
            _logger = logger;
            _memory = memory;
            _opt = opt;
        }

        private readonly ILogger _logger;
        private readonly IMemoryCache _memory;
        private readonly IServiceOpt _opt;

        public int Category => PluginConst.Category;

        public string Key => PluginConst.GuidanceCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        public Task Invoke(IPeerContent content)
        {
            var gpi = content.Source.ToObject<GuidancePositionInfo>(4);
            _logger.LogDebug($"收到引导目标信息：{gpi.ToJson()}");
            return _opt.Guidance(gpi);
        }
    }
}
