using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using Microsoft.AspNetCore.SignalR;
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
    public class TargetPushService : BackgroundService
    {
        private DateTime LastPushTime = DateTime.Now;
        private readonly ITargetService _service;
        private readonly IDeviceService _devService;
        private readonly ILogger _logger;
        private readonly int _interval;
        private readonly IHubContext<SignalRHub> _hub;
        private readonly IWhiteListService _white;
        public TargetPushService(ILogger<TargetPushService> logger, IDeviceService device, ITargetService service, IWhiteListService white, PushTaskConfig config, IHubContext<SignalRHub> hub)
        {
            _service = service;
            _devService = device;
            _logger = logger;
            _interval = config.TargetInterval;
            _hub = hub;
            _white = white;
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
                            // 0:获取全部
                            var allShips = await _devService?.GetRelationships();
                            var _probeIds = (await _devService.GetStatus()).Where(f => f.DeviceCategory.IsProbeDevice()).Select(f => f.DeviceId).ToList();
                            var rtTags = new RealTimeTarget();
                            //判断当前是否存在在线设备服务
                            var _updatas = new List<TargetPosition>();
                            var _delDatas = new List<TargetDisappear>();
                            for (int i = 0; i < _probeIds.Count; i++)
                            {
                                var devId = _probeIds[i];
                                _updatas = _updatas.Concat(await _service.GetLastUpdateTargetsPositionAsync(devId)).ToList();
                                _delDatas = _delDatas.Concat(await _service.GetLastDisappearTargets(devId)).ToList();
                            }
                            LastPushTime = DateTime.Now;
                            rtTags.LastTime = LastPushTime;
                            //Console.WriteLine($"uc={_updatas.Count()}");
                            var white = await _white.GetAnyAsync();
                            //整理目标数据
                            var _upTargets = _updatas?.Select(up =>
                            {
                                var threat = up.Threat;
                                if (white.Any(x => x.Sn.ToUpper() == up.UAVSn.ToUpper() && DateTime.Now >= x.StarTime && DateTime.Now <= x.EndTime))
                                {
                                    threat = 0;
                                }
                                var mode = new TargetModel()
                                {
                                    Id = up.Id,
                                    DeviceId = up.DeviceId,
                                    CoordinateType = up.CoordinateType,
                                    Lat = up.Lat,
                                    Lng = up.Lng,
                                    Alt = up.Alt,
                                    Vt = up.Vt,
                                    Vr = up.Vr,
                                    Category = up.Category,
                                    Mode = up.Mode,
                                    Threat = threat,
                                    TrackTime = up.TrackTime,
                                    AppLat = up.AppLat,
                                    AppLng = up.AppLng,
                                    HomeLat = up.HomeLat,
                                    HomeLng = up.HomeLng,
                                    UAVSn = up.UAVSn,
                                    UAVType = up.UAVType,
                                    BeginAt = up.BeginAt,
                                    AppAddr = up.AppAddr,
                                    MaxHeight = up.MaxHeight
                                };
                                if (allShips?.Count() > 0)
                                {
                                    mode.MonitorRelationShip = allShips?.FirstOrDefault(
                                        ship => ship?.RType == RelationshipsType.MonitorGd && ship?.TargetId == up.Id)?.Id;
                                    mode.TranspondRelationShip = allShips?.FirstOrDefault(
                                        ship => ship?.RType == RelationshipsType.PositionTurn && ship?.TargetId == up.Id)?.Id;
                                    mode.HitRelationShip = allShips?.FirstOrDefault(
                                        ship => ship?.RType == RelationshipsType.AttackGd && ship?.TargetId == up.Id)?.Id;
                                    mode.TickRelationShip = allShips?.FirstOrDefault(
                                        ship => ship?.RType == RelationshipsType.DecoyGd && ship?.TargetId == up.Id)?.Id;
                                }
                                return mode;
                            });
                            //更新数据刷新时间
                            rtTags.Targets = _upTargets;
                            rtTags.DeleteTargets = _delDatas;
                            //Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%tc=" + rtTags.Targets?.Count());
                            _ = _hub.Clients.All?.SendAsync(SignalRHub.target_channel, rtTags);

                        });

                        Task.WaitAll(work, Task.Delay(_interval));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"push target error:{ex.Message}");
                    }
                }
            });
        }
    }
}
