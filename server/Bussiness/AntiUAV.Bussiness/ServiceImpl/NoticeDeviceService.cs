using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.NoticeModels;
using AntiUAV.Bussiness.Service;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    /// <summary>
    /// 设备服务通知服务
    /// </summary>
    public class NoticeDeviceService : INoticeDeviceService
    {
        public NoticeDeviceService(ILogger<NoticeDeviceService> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
            //_bus.Advanced.ExchangeDeclare("1", "fanout");
        }

        private readonly ILogger _logger;
        private readonly IBus _bus;

        public Task NoticeDeviceChange(int devid, DeviceInfoNoticeCode code)
        {
            _logger.LogDebug($"notice to device server deviceInfo {devid} for {code}.");
            return _bus.PublishAsync(new DeviceInfoNotice(devid, code));
        }

        public Task NoticePerZonesChange(int zoneid, PerZonesNoticeCode code)
        {
            _logger.LogDebug($"notice to device server per-zone {zoneid} for {code}.");
            return _bus.PublishAsync(new PerZonesNotice(zoneid, code));
        }

        public Task NoticeRelationAdd(Relationships relationships)
        {
            _logger.LogDebug($"notice to device server relation add for {relationships.Id}.");
            
            return _bus.PublishAsync(new RelationNotice(relationships));
        }

        public Task NoticeRelationRemove(string rid)
        {
            _logger.LogDebug($"notice to device server relation remove for {rid}.");
            return _bus.PublishAsync(new RelationNotice(rid));
        }
        public Task NoticeRpcRequest(RpcRequestModel message)
        {
            _logger.LogDebug($"notice to device server RPC for {message.Data}.");
            return _bus.RequestAsync<RpcRequestModel, RpcResponseModel>(message);
        }
    }
}
