using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Hosted
{
    /// <summary>
    /// 历史目标保存服务
    /// </summary>
    public class HistoryTrackHosted : BackgroundService
    {
        public HistoryTrackHosted(ILogger<HistoryTrackHosted> logger, IMemoryCache memory, ITargetService target)
        {
            _memory = memory;
            _logger = logger;
            _target = target;
        }
        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly ITargetService _target;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        Task.WaitAll(_target.DisappearTgsSave(_memory.GetDevice()?.Id ?? 0), Task.Delay(5 * 1000));//5秒一周期
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "device server save history track error.");
                        await Task.Delay(2000);
                    }
                }
            });
        }
    }
}
