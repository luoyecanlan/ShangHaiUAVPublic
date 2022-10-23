using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer.Abstractions;
using AntiUAV.DeviceServer.Serrvice;
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
    /// 自检维护服务
    /// </summary>
    public class ServerCheckHosted : BackgroundService
    {
        public ServerCheckHosted(ILogger<ServerCheckHosted> logger, IMemoryCache memory, ITargetService target, IDeviceService device,IPreWarningZoneService zone)
        {
            _memory = memory;
            _logger = logger;
            _target = target;
            _device = device;
            _zone = zone;
        }
        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly ITargetService _target;
        private readonly IDeviceService _device;
        private readonly IPreWarningZoneService _zone;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        //_logger.LogInformation("server check run.");
                        var check = StatusCheck();
                        Task.WaitAll(check, Task.Delay(5000));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "check device server status error.");
                        Task.Delay(2000).Wait();
                    }
                }
            });
        }

        private async Task StatusCheck()
        {
            var dev = _memory.GetDevice();
            if (dev != null)
            {
                //检测与当前设备相关的关联关系是否有失效
                var relationships = _memory.GetRelationships();
                foreach (var rs in relationships)
                {
                    if (dev.Id == rs.FromDeviceId)
                    {
                        if (!_memory.TargetExistence(rs.TargetId))
                        {
                            await _device.RemoveRelationships(d => d.TargetId == rs.TargetId).ContinueWith(res =>
                            {
                                if (rs.RType == RelationshipsType.PositionTurn)
                                {
                                    _logger.LogInformation($"target {rs.TargetId}({rs.FromDeviceId}) disappears , forwarding address {rs.ToAddressIp}:{rs.ToAddressPort} is cleared .");
                                }
                                else
                                {
                                    _logger.LogInformation($"target {rs.TargetId}) disappears , relevant guidance cleared.");
                                }
                            });
                        }
                    }

                    //if (_device.IsOnline(rs.ToDeviceId) == false)
                    //{
                    //    await _device.RemoveRelationships(d => d.ToDeviceId == rs.ToDeviceId).ContinueWith(res =>
                    //    {
                    //        _logger.LogInformation($"to device {rs.ToDeviceId}) under line , relevant guidance cleared.");
                    //    });
                    //}
                }

                //检查被引导目标存在情况，并更新
                var begd = _memory.GetDeviceStatus()?.BeGuidanceInfo;
                if (begd != null)
                {
                    var check = _target.TargetExistence(begd.TargetId, begd.FromDeviceId);
                    if (!check)
                    {
                        await _device.RemoveRelationships(d => d.TargetId == begd.TargetId).ContinueWith(res =>
                        {
                            _memory.UpdateBeGuidanceInfo(null);
                            _logger.LogInformation($"tracking target {begd.TargetId} disappears and guided information is cleared.");
                        });
                    }
                }

                //刷新关联关系
                _memory.ReloadRelationships(await _device.GetRelationships());

                //刷新预警区内容
                var zones = await _zone.GetAnyAsync(x => 1 == 1);
                _memory.ReloadZones(ServiceOpt.ConvertToDsServMod(zones));
                //刷新设备参数
            }
        }
    }
}
