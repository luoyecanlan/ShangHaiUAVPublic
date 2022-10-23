using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct02.Hosted
{
    public class StatusCheckHosted : BackgroundService

    {
        public StatusCheckHosted(ILogger<StatusCheckHosted> logger, IMemoryCache memory, IPeerServer peer)
        {
            _memory = memory;
            _logger = logger;
            _peer = peer;
        }
        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly IPeerServer _peer;
        private readonly int _delayTime = 1;//s

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var work = Task.Run(() =>
                        {
                            var dev = _memory.GetDevice();
                            var buff = new byte[7] { 0x53, 0x5A, 0x0D, 0x06, 0x00, 0x01, 0xC1 };

                            _peer.Send(buff, dev.Ip, dev.Port);
                        });
                        Task.WaitAll(work, Task.Delay(_delayTime * 1000));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"query device status error:{ex.Message}");
                    }
                }
            });
        }
    }
}
