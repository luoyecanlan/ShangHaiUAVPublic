using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Bgs
{
    /// <summary>
    /// 目标推送服务
    /// </summary>
    public class DevStatusService : BackgroundService
    {
        private readonly IDeviceService _service;
        private readonly ILogger _logger;
        private readonly int _interval;
        private readonly IHubContext<SignalRHub> _hub;
        public DevStatusService(ILogger<DevStatusService> logger, IDeviceService service, PushTaskConfig config, IHubContext<SignalRHub> hub)
        {
            _service = service;
            _logger = logger;
            _interval = config.DevStatusInterval;
            _hub = hub;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var work = Task.Run(async () =>
                                   {
                                       var _all = await _service.GetAnyAsync();
                                       List<DeviceStatusModel> _allStatus = new List<DeviceStatusModel>();
                                       var _cacheStatus = await _service.GetStatus();
                                       var _ships = await _service.GetRelationships();
                                       _all?.ToList().ForEach(f =>
                                       {
                                           var _devS = new DeviceStatusModel()
                                           {
                                               DeviceId = f.Id,
                                               DeviceCategory = f.Category
                                           };
                                           var _st = _cacheStatus?.FirstOrDefault(s => s?.DeviceId == f.Id);
                                           //Redis中存在状态 则赋值
                                           if (_st != null)
                                           {
                                               _devS.Code = _st.Code;
                                               _devS.ErrorMsg = _st.ErrorMsg;
                                               _devS.RunInfo = _st.RunInfo;
                                               _devS.IsGuidance = _ships?.Count(ss => ss.FromDeviceId == _st.DeviceId && ss.RType > 0) > 0;
                                               _devS.IsBeGuidance = _ships?.Count(ss => ss.ToDeviceId == _st.DeviceId && ss.RType > 0) > 0;
                                               _devS.IsTurnTarget = _ships?.Count(ss => ss.FromDeviceId == _st.DeviceId && ss.RType == 0) > 0;
                                           }
                                           _allStatus.Add(_devS);
                                       });
                                       _ = _hub.Clients?.All?.SendAsync(SignalRHub.device_status_channel, _allStatus);
                                   });
                        Task.WaitAll(work, Task.Delay(_interval));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"push device status error:{ex.Message}");
                    }
                }
            });
        }
    }
}
