using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Hosted
{
    public class PersistenceRunCheckHosted : BackgroundService
    {
        public PersistenceRunCheckHosted(ILogger<PersistenceRunCheckHosted> logger, IMemoryCache memory, IServiceOpt opt)
        {
            _memory = memory;
            _logger = logger;
            _opt = opt;
        }
        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly IServiceOpt _opt;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        //_logger.LogInformation("server check run.");
                        var check = Check();
                        Task.WaitAll(check, Task.Delay(5000));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "check persistence & run data server error.");
                        Task.Delay(2000).Wait();
                    }
                }
            });
        }

        private Task Check()
        {
            var dev = _memory.GetDevice();
            if (dev != null)
            {
                //实际上报数位置据和持久化数据比对
                var position = _memory.GetDevPosition();
                if (position?.Equals(dev) == false)
                {
                    _memory.UpdateServiceError(ErrorCodeEnum.InconsistentSettings, "位置坐标与设定不一致,正在尝试修复...");
                    _opt.SetPosition().ContinueWith(res =>
                    {
                        if (!res.Result)
                            _logger.LogWarning("auto set device position fail.");
                    });
                }

                //实际上报数纠偏据和持久化数据比对
                var rectify = _memory.GetDevRectify();
                if (rectify?.Equals(dev) == false)
                {
                    _memory.UpdateServiceError(ErrorCodeEnum.InconsistentSettings, "纠偏值与设定不一致,正在尝试修复...");
                    _opt.SetRectify().ContinueWith(res =>
                    {
                        if (!res.Result)
                            _logger.LogWarning("auto set device rectify fail.");
                    });
                }
            }
            return Task.CompletedTask;
        }
    }
}
