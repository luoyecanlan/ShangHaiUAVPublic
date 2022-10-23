using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using DbOrm.AntiUAV.Entity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Hosted
{
    /// <summary>
    /// 威胁度判定
    /// </summary>
    public class ThreatHosted : BackgroundService
    {
        public ThreatHosted(ILogger<ThreatHosted> logger, IMemoryCache memory, IThreatWeight threatWeight, GisTool gis)
        {
            _memory = memory;
            _logger = logger;
            _threatWeight = threatWeight;
            _gis = gis;
        }

        private readonly ILogger<ThreatHosted> _logger;
        private readonly IMemoryCache _memory;
        private readonly IThreatWeight _threatWeight;
        private readonly GisTool _gis;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => {
                var time = 5;
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        Task.WaitAll(MapToTrackList(), Task.Delay(time * 1000));
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "update threat service error.");
                    }
                }
            });
        }
        Task MapToTrackList()
        {
            var targetList =  _memory.GetAllTargets();
            ConcurrentBag<Task> taskList = new ConcurrentBag<Task>();
            foreach (var item in targetList)
            {
                try
                {
                    var task = _memory.Assessment(item.Points, _threatWeight, _gis);
                    taskList.Add(task);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Target {0} Failure to calculate threat level.", item);
                }
            }
            Task.WaitAll(taskList.ToArray());
            return Task.CompletedTask;
        }
    }
}
