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
    public class RelationShipService : BackgroundService
    {
        private readonly IDeviceService _device;
        private readonly ITargetService _target;
        private readonly ILogger _logger;
        private readonly int _interval;
        private readonly IHubContext<SignalRHub> _hub;
        public RelationShipService(IDeviceService device, ILogger<RelationShipService> logger, PushTaskConfig config, IHubContext<SignalRHub> hub, ITargetService target)
        {
            _device = device;
            _logger = logger;
            _interval = config.RelationShipInterval;
            _target = target;
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
                            var relationships = await _device.GetRelationships();
                            _ = _hub.Clients?.All?.SendAsync(SignalRHub.device_relation_ship_channel, relationships);
                            foreach (var rs in relationships)
                            {
                                if (!_target.TargetExistence(rs.TargetId, rs.FromDeviceId))
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

                                if (_device.IsOnline(rs.FromDeviceId) == false)
                                {
                                    await _device.RemoveRelationships(d => d.FromDeviceId == rs.FromDeviceId).ContinueWith(res =>
                                    {
                                        _logger.LogInformation($"from device {rs.ToDeviceId}) under line , relevant guidance cleared.");
                                    });
                                }

                                if (_device.IsOnline(rs.ToDeviceId) == false)
                                {
                                    await _device.RemoveRelationships(d => d.ToDeviceId == rs.ToDeviceId).ContinueWith(res =>
                                    {
                                        _logger.LogInformation($"to device {rs.ToDeviceId}) under line , relevant guidance cleared.");
                                    });
                                }
                            }
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
