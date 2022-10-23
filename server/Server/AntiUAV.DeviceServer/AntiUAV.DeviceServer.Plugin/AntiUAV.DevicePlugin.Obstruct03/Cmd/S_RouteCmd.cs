using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct03.Cmd
{
    public class S_RouteCmd : IPeerSysCmd
    {
        public S_RouteCmd(ILogger<S_RouteCmd> logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.Route;

        public Task<bool> Invoke(IPeerContent content)
        {
            if (string.IsNullOrEmpty(content.Route))
            {
                //按协议修改
                content.Route = $"{PluginConst.ProtocolNum}_{Convert.ToString(content.Source[5], 16)}{Convert.ToString(content.Source[6], 16)}";
            }
            else
            {
                content.Route = PluginConst.GuidanceCmdKey;
            }
            _logger.LogInformation($"收到路由：{content.Route}");
            return Task.FromResult(true);
        }
    }
}
