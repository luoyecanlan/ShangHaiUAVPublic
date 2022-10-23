using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeR01
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
        }

        public override int DeviceCategory => PluginConst.Category;
    }
}
