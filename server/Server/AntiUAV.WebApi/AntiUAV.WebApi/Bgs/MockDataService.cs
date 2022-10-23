using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Bgs
{
    public class MockDataService : BackgroundService
    {
        //private IDeviceService deviceService;
        //private ITargetService targetService;
        //private Random random = new Random();
        //const double lon = 116.570607;
        //const double lat = 39.777473;
        //private int deviceId;
        //private int index = 0;
        //const int maxCount= 10;
        //private List<TargetInfo> _targets = new List<TargetInfo>();
        //public MockDataService(IDeviceService device, ITargetService target)
        //{
        //    deviceService = device;
        //    targetService = target;
        //}

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //    return Task.Run(async () =>
            //    {
            //        //获取探测设备ID
            //        var ids = await deviceService.GetAnyAsync<DeviceKey>();
            //        deviceId = ids?.First().Id ?? 1;
            //        while (!stoppingToken.IsCancellationRequested)
            //        {
            //            var work = Task.Run(() =>
            //            {
            //                _ = MockTargetAsync(index >= maxCount);
            //                _ = MockDeviceStatus();
            //            });
            //            Task.WaitAll(work, Task.Delay(1000));
            //        }
            //    });

            return Task.CompletedTask;
        }

        ///// <summary>
        ///// 模拟目标数据
        ///// </summary>
        ///// <param name="isRemove"></param>
        ///// <returns></returns>
        //private async Task MockTargetAsync(bool isRemove = false)
        //{
        //    if (_targets.Count > 0)
        //    {
        //        //修改数据
        //        _targets.ForEach(f =>
        //        {
        //            f.Threat = random.NextDouble() * 80 + 20;
        //            f.Alt += random.NextDouble() * 10;
        //            f.Lat += random.NextDouble() * 0.001;
        //            f.Lng += random.NextDouble() * 0.001;
        //            f.TrackTime = DateTime.Now;
        //        });
        //    }           
        //    //新增
        //    int _maxIndex = _targets.Count;
        //    //数量不满足10时新增
        //    if (_maxIndex < maxCount)
        //    {
        //        int _num = maxCount - _maxIndex;
        //        while (_num-- > 0)
        //        {
        //            _targets.Add(new TargetInfo()
        //            {
        //                Id = $"Target_{index++}",
        //                DeviceId = deviceId,
        //                Alt = random.NextDouble() * 20 + 50,
        //                Category = 0,
        //                Lat = random.NextDouble() * 0.001 + lat,
        //                Lng = random.NextDouble() * 0.001 + lon,
        //                Mode = 1,
        //                Threat = random.NextDouble() * 80 + 20,
        //                TrackTime = DateTime.Now
        //            });
        //        }
        //    }

        //    //删除
        //    if (isRemove)
        //    {
        //        var rtid = _targets.First().Id;
        //        _ = targetService.TargetDisappear(new TargetDisappear[] {
        //            new TargetDisappear() {
        //                TargetId=rtid,
        //                DisappearTime=DateTime.Now,
        //                CauseOfDisappear=DisappearType.TimeOut
        //            }
        //       });
        //        _targets.RemoveAt(0);
        //    }
        //    //提交数据
        //    _ = targetService.UpdateTargets(deviceId, _targets.ToArray());
        //}

        ///// <summary>
        ///// 模拟设备状态信息
        ///// </summary>
        ///// <returns></returns>
        //private async Task MockDeviceStatus()
        //{
        //    var ids = await deviceService.GetAnyAsync<DeviceKey>();

        //    ids.ToList().ForEach(f =>
        //    {
        //        deviceService.Update(
        //            new DeviceStatus()
        //            {
        //                DeviceId = f.Id,
        //                ServiceRunState = random.Next(0, 99) % 2,
        //                ServiceState = random.Next(0, 99) % 2,
        //                DeviceWorkState = random.Next(0, 99) % 2,
        //                DeviceNetState = random.Next(0, 99) % 2,
        //                DeviceBit = random.Next(0, int.MaxValue),
        //                DeviceBitMsg = $"last update time:{DateTime.Now.ToString()}"
        //            });
        //    });

    }
}
