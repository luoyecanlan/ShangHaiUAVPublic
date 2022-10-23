using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.AirSwitch02
{
    public class DeviceHostService : DeviceHostTcpServerBase
    {
        public DeviceHostService(ILogger<DeviceHostService> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {

        }
        public override int DeviceCategory => PluginConst.Category;
    }


}
