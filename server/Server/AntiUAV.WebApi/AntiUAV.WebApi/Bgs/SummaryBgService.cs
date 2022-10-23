using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Bgs
{
    public class SummaryBgService : BackgroundService
    {
        private readonly ILogger<SummaryBgService> _logger;
        private readonly int _interval;
        private readonly ISummaryService _summary;
        public SummaryBgService(ILogger<SummaryBgService> logger,PushTaskConfig config,ISummaryService summary)
        {
            _logger = logger;
            _interval = config.SummaryInterval;
            _summary = summary;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var work = Task.Run(() => {
                            _summary.DoSummary();
                        });
                        Task.WaitAll(work, Task.Delay(_interval));
                    }
                    catch (Exception es)
                    {
                        _logger.LogError($"summary target error:{es.Message}");
                    }
                }
            });
        }
    }
}
