using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR06.Cmd
{
    public class A_TrackCmd : IPeerCmd
    {
        public A_TrackCmd(ILogger<A_TrackCmd> logger, IMemoryCache memory, GisTool tool, IServiceProvider provider)
        {
            _logger = logger;
            _memory = memory;
            _tool = tool;
            var dev = _memory.GetDevice();
            _host = provider.GetServices<IDeviceHostService>().FirstOrDefault(x => x.DeviceCategory == dev.Category);
        }

        public int Category => PluginConst.Category;
        public string Key => PluginConst.TrackCmdKey;
        public PeerCmdType Order => PeerCmdType.Action;

        private readonly ILogger<A_TrackCmd> _logger;
        private readonly IMemoryCache _memory;
        private readonly IDeviceHostService _host;
        private readonly GisTool _tool;

        public async Task Invoke(IPeerContent content)
        {
           
            if (content.Source!=null)
            {
                var dev = _memory.GetDevice();
                var tgs = new List<TargetInfo>();
                var track = content.Source.ToStuct<R_ProbeR06_Track>();
                
                var tg = MapToTargetInfo(track, dev);
                if (track.TrackDispear == 0)
                {
                    tgs.Add(tg);
                }
                
                await _memory.UpdateTarget(tgs.ToArray());
                content.SourceAys = tgs;
                _logger.LogDebug($"收到目标{tg.Id}信息");
                _logger.LogDebug($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()},source track count:{"TrackCount"}.");
            }
            else
            {
                _logger.LogWarning("The A_TrackCmd Command failed detection");
                await Task.FromCanceled(new System.Threading.CancellationToken());
            }
        }
        TargetInfo MapToTargetInfo(R_ProbeR06_Track track, DeviceInfo dev)
        {
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{track.TargetId}";
            var tg = new TargetInfo
            {
                Id = id,
                Alt = track.TargetAlt,
                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.Perception,
                Freq = -1,
                ProbeAz = track.TargetAz,
                PeobeEl = track.TargetEl,
                ProbeDis = track.TargetDistance,
                ProbeHigh = track.TargetAlt,
                Category = 1,
                Mode = 0,
                Vr = track.TargetRSpeed,
                Vt = track.TargetSpeed,
                Lat=track.TargetLat,
                Lng=track.TargetLng,
                TrackTime = DateTime.Now,
                Threat = _memory.GetThreat(id)
            };
            _logger.LogError($"收到引导目标信息：{tg.ToJson()}");
            

            //var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };
            //var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔 
            //tg.Lat = Math.Round(position.Lat, 6);
            //tg.Lng = Math.Round(position.Lng, 6);
            //tg.Alt = Math.Round(position.Altitude, 1);
            return tg;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_ProbeR06_Track
    {
        public UInt32 RadarId;// 雷达编号  区分不同的雷达

        public UInt32 TargetId;//  从1开始递增

        public float TargetDistance;  //单位：米
        public float TargetAz;  //单位：度
        public float TargetEl;  //单位：度

        public float TargetIntensity;
        public float TargetSpeed;   // 米/秒  

        public float TargetLng;  //单位：度  正为东经，负为西经；
        public float TargetLat;  //单位：度  正为北纬，负为南纬；
        public float TargetAlt;//单位：米

        public float TargetDirection;   // 单位：度
        public float TargetRSpeed;   // 单位：米/秒    

        public UInt32 TrackDispear;//  当不为0时，目标消失。
        public UInt32 Reverse;//  
        public Int64 TimeStamp;   //自 0001 年 1 月 1 日午夜 12:00:00 以来已经过的时间的以 100 毫微秒为间隔的间隔数，


    }
}
