using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl
{
    public abstract class DeviceHostUdpServerBase : IDeviceHostService
    {
        public DeviceHostUdpServerBase(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory)
        {
            _peer = peer;
            _memory = memory;
            _logger = logger;
        }
        public abstract int DeviceCategory { get; }

        public bool IsSysUdp => true;

        public string RunCode { get; set; }

        public virtual int OutRefRunCodeTime => 30;

        protected readonly IPeerServer _peer;
        protected readonly IMemoryCache _memory;
        protected readonly ILogger _logger;

        public virtual void HostState()
        {

        }

        public virtual void Start()
        {
            var dev = _memory.GetDevice();
            if (dev == null || _peer == null)
                _logger.LogError("devServ udp listion start fail.(no dev info or no serv)");
            else
            {
                _peer.UseCustomServerInfo(new PeerServerInfo($"deviceServer:{dev.Id}", dev.Lip, dev.Lport));
                _peer?.Star();
                _logger.LogInformation($"devServ udp listion start.(devId:{dev.Id},ip:{dev.Lip},port:{dev.Lport})");
            }
        }

        public virtual void Stop()
        {
            _peer?.Stop();
        }
    }
}
