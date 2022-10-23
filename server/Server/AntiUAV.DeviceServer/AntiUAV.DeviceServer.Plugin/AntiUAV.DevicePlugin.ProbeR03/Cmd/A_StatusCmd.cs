using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR03.Cmd
{
    public class A_StatusCmd : IPeerCmd
    {
        public A_StatusCmd(ILogger<A_StatusCmd> logger, IMemoryCache memory)
        {
            _memory = memory;
            _logger = logger;
        }
        public int Category => PluginConst.Category;

        public string Key => PluginConst.StatusCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;

        public Task Invoke(IPeerContent content)
        {
            var dev = _memory.GetDevice();
            //_logger.LogInformation($"recive dev:{dev.Id} status...");
            return Task.CompletedTask;
        }
    }
}
