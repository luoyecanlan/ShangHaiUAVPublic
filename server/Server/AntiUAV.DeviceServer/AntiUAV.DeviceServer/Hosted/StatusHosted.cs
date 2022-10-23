using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer.Abstractions;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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
    /// 设备状态服务
    /// </summary>
    public class StatusHosted : BackgroundService
    {
        public StatusHosted(ILogger<StatusHosted> logger, IMemoryCache memory, IDeviceService device, IServiceProvider provider)
        {
            _memory = memory;
            _logger = logger;
            _device = device;
            _provider = provider;
        }

        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly IDeviceService _device;
        private IDeviceHostService _host;
        private readonly IServiceProvider _provider;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var time = 5;
                    try
                    {
                        var dev = _memory.GetDevice();
                        if (dev != null)
                        {
                            time = dev?.StatusReportingInterval ?? 5;
                            var check = StatusCheck(dev.StatusReportingInterval);
                            Task.WaitAll(check, Task.Delay(time * 1000));
                        }
                        else
                            Task.Delay(2000).Wait();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "check device status error.");
                        Task.Delay(2000).Wait();
                    }
                    finally
                    {
                        _memory.CleanErrorMsg(time);
                    }
                }
            });
        }

        /// <summary>
        /// 状态字段存在5个周期
        /// 三个周期之后则为通信故障
        /// 五个周期之后则为离线设备
        /// </summary>
        /// <param name="StatusReportingInterval"></param>
        /// <returns></returns>
        private Task StatusCheck(int StatusReportingInterval)
        {
            //获取设备状态信息
            var status = _memory.GetDeviceStatus();
            status.UpdateTime = DateTime.Now;//模拟器没报，让状态正常
            if (status.UpdateTime.AddSeconds(StatusReportingInterval * 3) < DateTime.Now)
            {
                _memory.UpdateServiceError(ErrorCodeEnum.CommunicationFailure, "与下位机通信故障");//三个上报周期都没有更新过,通信故障
                if (_host == null)
                {
                    var dev = _memory.GetDevice();
                    if (dev != null)
                        _host = _provider.GetServices<IDeviceHostService>().FirstOrDefault(x => x.DeviceCategory == dev.Category);
                }
                else
                {
                    if (_host.OutRefRunCodeTime > 0 && status.UpdateTime.AddSeconds(_host.OutRefRunCodeTime) < DateTime.Now)
                    {
                        //指定时间没有上报内容，那就是断开链接了，刷新运行码
                        _host.RunCode = ConvertExtension.GetRandomString();
                    }
                }
            }
            if (status.UpdateTime.AddSeconds(60) < DateTime.Now)
            {
                //60s没有上报内容，那就是断开链接了
                _host.RunCode = ConvertExtension.GetRandomString();
            }
            //写入redis
            if (!_device.UpdateStatus(status, StatusReportingInterval * 10))//
            {
                _logger.LogWarning("device status write to redis fail.");
            }
            return Task.CompletedTask;
        }
    }
}
