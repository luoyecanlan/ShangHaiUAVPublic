using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05.Cmd
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
                var key = Encoding.UTF8.GetString(content.Source, 0, 2);
                content.Route = key switch
                {
                    "T_" => PluginConst.StatusCmdKey,
                    "S_" => PluginConst.TrackCmdKey,
                    _ => "UnKnow",
                };
                return Task.FromResult(true);
            }
            else
            {
                content.ForcedOver = true;
                return Task.FromResult(false);
            }
        }
    }
}
