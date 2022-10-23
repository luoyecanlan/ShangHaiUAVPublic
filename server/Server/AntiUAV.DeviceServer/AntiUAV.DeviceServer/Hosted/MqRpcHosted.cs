using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.NoticeModels;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.Service;
using EasyNetQ;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Hosted
{
    public class MqRpcHosted : BackgroundService
    {
        public MqRpcHosted(ILogger<MqRpcHosted> logger, IBus bus, IMemoryCache memory, IServiceOpt opt, IDeviceService device)
        {
            _bus = bus;
            _bus.Advanced.ExchangeDeclare("1", "fanout");
            _memory = memory;
            _opt = opt;
            _logger = logger;
            _device = device;
            _subId = $"device_server_{_memory.GetDevice().Id}";
        }

        private readonly ILogger _logger;
        private readonly IBus _bus;
        private readonly string _subId;
        private readonly IMemoryCache _memory;
        private readonly IServiceOpt _opt;
        private readonly IDeviceService _device;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bus.SubscribeAsync<RelationNotice>(_subId, ConsumerMessage,
                configure => configure.WithAutoDelete(true));
            _bus.SubscribeAsync<DeviceInfoNotice>(_subId, ConsumerMessage,
                configure => configure.WithAutoDelete(true));
            _bus.SubscribeAsync<PerZonesNotice>(_subId, ConsumerMessage,
                configure => configure.WithAutoDelete(true));
            //_bus.RespondAsync<RpcRequestModel, RpcResponseModel>(ConsumerMessage);
            return Task.CompletedTask;
        }

        public async Task ConsumerMessage(DeviceInfoNotice message)
        {
            try
            {
                _logger.LogDebug($"rabbitmq notice device info change for {message.Code}.");
                var id = _memory.GetDevice()?.Id;
                if (id == message.DeviceId)
                {
                    if (await _opt.ReloadDeviceInfoAsync(message.DeviceId))
                    {
                        _logger.LogInformation($"rabbitmq notice device {message.DeviceId} info change , device {id} info refresh finished .");
                    }
                    else
                    {
                        _logger.LogWarning($"rabbitmq notice device {message.DeviceId} info change , device {id} info refresh failed !!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"rabbitmq notice device {message.DeviceId} info change  , but an exception occurred while processing.\r\n error:{ex}");
            }
        }

        public async Task ConsumerMessage(PerZonesNotice message)
        {
            try
            {
                _logger.LogDebug($"rabbitmq notice device per-zones change for {message.Code}.");
                var dev = _memory.GetDevice();
                if (dev != null)
                {
                    if (await _opt.ReloadDeviceZoneAsync(dev.Id))
                    {
                        _logger.LogInformation($"rabbitmq notice per-zones change , device {dev.Id} per-zones refresh finished .");
                    }
                    else
                    {
                        _logger.LogWarning($"rabbitmq notice per-zones change , device {dev.Id} per-zones refresh failed !!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"rabbitmq notice device per-zones change request , but an exception occurred while processing.\r\n error:{ex}");
            }
        }

        public async Task ConsumerMessage(RelationNotice message)
        {
            try
            {
                var dev = _memory.GetDevice();
                if (dev == null) return;
                if (message?.Relationships?.ToDeviceId != dev.Id) return;
                if (message.Relationships?.RType == RelationshipsType.AttackGd)
                {
                    await _opt.SetAttack(JsonConvert.SerializeObject(message.Relationships), true);
                }
                if (message.Relationships == null)
                {
                    await _opt.SetAttack(JsonConvert.SerializeObject(message.Relationships), false);
                }
                _logger.LogDebug($"rabbitmq notice device relation change for {message.Code}.");
                _memory.ReloadRelationships(await _device.GetRelationships());
                _logger.LogInformation($"rabbitmq notice relation change , relation refresh finished .");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"rabbitmq notice device relation change request , but an exception occurred while processing.\r\n error:{ex}");
            }
        }

        public async Task<RpcResponseModel> ConsumerMessage(RpcRequestModel message)
        {
            try
            {
                _logger.LogDebug($"rpc recive for {message.ReqCode}.");
                object resObj = null;
                switch (message.ReqCode)
                {
                    case RpcRequestEnum.RectifySetting:
                        var rectify = message.Data.ToObject<DeviceRectify>();
                        var devRectify = new DevRectifyInfo()
                        {
                            Az = rectify.RectifyAz,
                            El = rectify.RectifyEl
                        };
                        _memory.UpdateDevRectify(devRectify);
                        resObj = await _opt.SetRectify(message.TimeOut ?? 3000);                       
                        break;
                    case RpcRequestEnum.PositionSetting:
                        resObj = await _opt.SetPosition(message.TimeOut ?? 3000);
                        break;
                    case RpcRequestEnum.MonitorOpen:
                        resObj = await _opt.SetMointor(message.Data, true, message.TimeOut ?? 3000);
                        break;
                    case RpcRequestEnum.MonitorClose:
                        resObj = await _opt.SetMointor(message.Data, false, message.TimeOut ?? 3000);
                        break;
                    case RpcRequestEnum.AttackOpen:
                        resObj = await _opt.SetAttack(message.Data, true, message.TimeOut ?? 3000);
                        break;
                    case RpcRequestEnum.AttackClose:
                        resObj = await _opt.SetAttack(message.Data, false, message.TimeOut ?? 3000);
                        break;
                    case RpcRequestEnum.PTZ:
                        resObj = await _opt.SetOperationDevice(message.Data, message.TimeOut ?? 3000);
                        break;
                    default:
                        _logger.LogWarning($"rpc for {message.ReqCode} finished , but unknow request code.");
                        return new RpcResponseModel() { Code = RpcResponseEnum.UnKnow };
                }
                _memory.ReloadRelationships(await _device.GetRelationships());
                _logger.LogInformation($"rpc for {message.ReqCode} finished .");
                return new RpcResponseModel() { Data = resObj };
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"rabbitmq notice device relation change request , but an exception occurred while processing.\r\n error:{ex}");
                return new RpcResponseModel() { Code = RpcResponseEnum.Fail, Data = false };
            }
        }
    }
}
