using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct03.Hosted
{
    /// <summary>
    /// 关系维护服务
    /// </summary>
    public class RelationShipHosted : BackgroundService
    {
        public RelationShipHosted(ILogger<RelationShipHosted> logger, IMemoryCache memory, IDeviceService device, IServiceOpt opt)
        {
            _memory = memory;
            _logger = logger;
            _device = device;
            _opt = opt;
        }
        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly IDeviceService _device;
        private readonly IServiceOpt _opt;
        private readonly int _delayTime = 3;//s

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
                            var dev = _memory.GetDevice();
                            var devState = _memory.GetDeviceStatus();
                            var gds = _memory.GetRelationships().Where(x => x.RType == Bussiness.Models.RelationshipsType.AttackGd && x.ToDeviceId == dev.Id);//获取所有与自己有关的打击关联关系
                            if (gds.Count() > 0)
                            {
                                foreach (var gd in gds)
                                {
                                    var sp = DateTime.Now - gd.UpdateTime;
                                    if (sp.TotalMinutes >= 2)
                                    {
                                        //干扰关系已经持续2分钟，执行关闭干扰操作
                                        var res = await _opt.SetAttack("", false);
                                        if (res)
                                        {
                                            _logger.LogInformation($"attack[{dev.Id}({dev.Name})] is close finished for timeout.");
                                        }
                                        else
                                        {
                                            _logger.LogError($"attack[{dev.Id}({dev.Name})] is close fail , no message send to device .");
                                        }
                                        //从redis中移除此条关联关系
                                        await _device.RemoveRelationships(gd.Id);
                                        continue;
                                    }
                                    if (devState.Code == Bussiness.Models.DeviceStatusCode.Free)
                                    {
                                        //有要求开启干扰，但是现在没有开启干扰的情况，需要开启干扰
                                        var res = await _opt.SetAttack(gd.ToJson(), true);
                                        if (res)
                                        {
                                            _logger.LogInformation($"attack[{dev.Id}({dev.Name})] is open finished for auto.");
                                        }
                                        else
                                        {
                                            _logger.LogError($"attack[{dev.Id}({dev.Name})] is open fail , no message send to device .");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (devState.Code == Bussiness.Models.DeviceStatusCode.Running)
                                {
                                    //已经没有干扰关联关系存在了，执行关闭干扰操作
                                    var res = await _opt.SetAttack("", false);
                                    if (res)
                                    {
                                        _logger.LogInformation($"attack[{dev.Id}({dev.Name})] is close finished for auto.");
                                    }
                                    else
                                    {
                                        _logger.LogError($"attack[{dev.Id}({dev.Name})] is close fail , no message send to device .");
                                    }
                                }
                            }
                        });
                        Task.WaitAll(work, Task.Delay(_delayTime * 1000));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"check hit device relationship error:{ex.Message}");
                    }
                }
            });
        }
    }
}
