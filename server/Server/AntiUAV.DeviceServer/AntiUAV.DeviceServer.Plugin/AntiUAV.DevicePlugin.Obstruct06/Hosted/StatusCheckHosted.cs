using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct06.Hosted
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
        byte[] queryDeviceStatus = new byte[9] { 0x7E, 0x01, 0x00, 0x11, 0xFF, 0x00, 0xD7, 0x6E, 0x7F };
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
                            GlobleObj.tcpClient.Send(queryDeviceStatus);
                            //_logger.LogInformation($"下发查询状态指令[{dev.Ip}:{dev.Port}].{string.Join(",", buff.Select(x => x.ToString("X2")))}");

                            _peer.Send(queryDeviceStatus, dev.Lip, dev.Lport);
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
