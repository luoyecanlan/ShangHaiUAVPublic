using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer.Abstractions;
using AntiUAV.DeviceServer.Abstractions.Models;
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
    /// 航迹信息服务
    /// </summary>
    public class TrackHosted : BackgroundService
    {
        public TrackHosted(ILogger<TrackHosted> logger, IMemoryCache memory, ITargetService target)
        {
            _memory = memory;
            _logger = logger;
            _target = target;
        }
        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly ITargetService _target;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var dev = _memory.GetDevice();
                    if (dev != null)
                    {

                        Task.WaitAll(TrackCheck(), Task.Delay(dev.ProbeReportingInterval * 1000));
                    }
                    else
                        await Task.Delay(3000);
                    Task TrackCheck()
                    {
                        if (dev.IsMonitor())//若是可追踪设备
                        {
                            var begd = _memory.GetDeviceStatus()?.BeGuidanceInfo;
                            if (!string.IsNullOrEmpty(begd?.TargetId))//判定一下当前是否在追踪目标
                            {
                                //========================================
                                //将追踪目标更新至REDIS后直接返回,但是现在追踪目标从设备反馈信息还没存储，没写到那，写到了再加上。
                                //基本思路是在begd里面加个当前位置信息做保存，这里从begd里面提取出来
                                //begd里面直接存储一个TargetInfo，提取出来就直接是info，执行后面的操作就可以了
                                //========================================
                                var info = new Bussiness.Models.TargetInfo() { DeviceId = dev.Id };
                                return _target.UpdateTargetInfo(begd.FromDeviceId, info);
                            }
                        }
                        //清理消失（超时）目标
                        var disapper = _memory.CleanTarget(dev.TargetTimeOut).Select(x => x.Last.Id).ToList();
                        //_logger.LogDebug($"被清理{disapper.Count}.");
                        //所有当前可用目标集合
                        var alltgs = _memory.GetAllTargets().Select(x => x.Last).ToList();
                        //_logger.LogInformation($"disapper target {disapper.Count()},update target {alltgs.Count()}");
                        return _target.UpdateTargetsInfo(dev.Id, alltgs, disapper);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "check device server track error.");
                    await Task.Delay(2000);
                }
            }
        }
    }
}
