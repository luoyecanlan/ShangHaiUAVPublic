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
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR03.Cmd
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
        private readonly int _monitorCount = 3;

        public async Task Invoke(IPeerContent content)
        {
            var dev = _memory.GetDevice();
            var track = content.Source.ToStuct<TRACK>();
            var tgs = new List<TargetInfo>();
            if (track.TrackCount > 0)
            {
                if (track.No0.Snum > 0 && track.No0.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No0, dev));
                }
                if (track.No1.Snum > 0 && track.No1.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No1, dev));
                }
                if (track.No2.Snum > 0 && track.No2.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No2, dev));
                }
                if (track.No3.Snum > 0 && track.No3.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No3, dev));
                }
                if (track.No4.Snum > 0 && track.No4.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No4, dev));
                }
                if (track.No5.Snum > 0 && track.No5.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No5, dev));
                }
                if (track.No6.Snum > 0 && track.No6.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No6, dev));
                }
                if (track.No7.Snum > 0 && track.No7.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No7, dev));
                }
                if (track.No8.Snum > 0 && track.No8.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No8, dev));
                }
                if (track.No9.Snum > 0 && track.No9.MonitorCount > _monitorCount)
                {
                    tgs.Add(GetTargetInfo(track.No9, dev));
                }
            }
            await _memory.UpdateTarget(tgs.ToArray());
            content.SourceAys = tgs;
            //_logger.LogInformation($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()},source track count:{track.TrackCount}.");
        }

        private TargetInfo GetTargetInfo(TRACK_INFO info, DeviceInfo dev)
        {
            //_logger.LogWarning($"航迹编号：{info.Snum}");
            var az = Math.Round(info.Az, 3) /*+ dev.RectifyAz*/;
            //while (az < 0 || az > 360)
            //{
            //    if (az < 0)
            //        az += 360;
            //    else
            //        az -= 360;
            //}
            var el = Math.Round(info.El, 3) /*+ dev.RectifyEl*/;
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{info.Snum}";
            var tg = new TargetInfo()
            {
                DeviceId = dev.Id,
                Id = id,
                CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                Freq = -1,
                ProbeAz = az,
                PeobeEl = el,
                ProbeDis = Math.Round(info.Dis, 1),
                ProbeHigh = Math.Round(info.Alt, 1),
                Category = 1,
                Mode = info.Relation,
                Vr = Math.Abs(Math.Round(info.Vr, 1)),
                Vt = Math.Abs(Math.Round(info.Vz, 1)),
                TrackTime = DateTime.Now,
                Threat = _memory.GetThreat(id)
            };

            var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };
            var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔
            tg.Lat = Math.Round(position.Lat, 6);
            tg.Lng = Math.Round(position.Lng, 6);
            tg.Alt = Math.Round(position.Altitude, 1);
            return tg;
        }
    }
    /**
    * 开关机控制
    **/
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct TRACK
    {
        /// <summary>
        /// 目的地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] DestinationAdd;

        /// <summary>
        /// 源地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] SourceAdd;

        /// <summary>
        /// 扇区编号
        /// </summary>
        public int AzSectorNum;

        /// <summary>
        /// 航迹数
        /// </summary>
        public int TrackCount;

        /// <summary>
        /// 航迹0
        /// </summary>
        public TRACK_INFO No0;

        /// <summary>
        /// 航迹1
        /// </summary>
        public TRACK_INFO No1;

        /// <summary>
        /// 航迹2
        /// </summary>
        public TRACK_INFO No2;

        /// <summary>
        /// 航迹3
        /// </summary>
        public TRACK_INFO No3;

        /// <summary>
        /// 航迹4
        /// </summary>
        public TRACK_INFO No4;

        /// <summary>
        /// 航迹5
        /// </summary>
        public TRACK_INFO No5;

        /// <summary>
        /// 航迹6
        /// </summary>
        public TRACK_INFO No6;

        /// <summary>
        /// 航迹7
        /// </summary>
        public TRACK_INFO No7;

        /// <summary>
        /// 航迹8
        /// </summary>
        public TRACK_INFO No8;

        /// <summary>
        /// 航迹9
        /// </summary>
        public TRACK_INFO No9;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct TRACK_INFO
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Snum;

        /// <summary>
        /// 时间
        /// </summary>
        public long Time;

        /// <summary>
        /// 距离 m
        /// </summary>
        public float Dis;

        /// <summary>
        /// 方位 °
        /// </summary>
        public float Az;

        /// <summary>
        /// 俯仰 °
        /// </summary>
        public float El;

        /// <summary>
        /// 径向速度 m/s
        /// </summary>
        public float Vr;

        /// <summary>
        /// 强度 无量纲
        /// </summary>
        public int Intensity;

        /// <summary>
        /// 经度 ° 5位小数需要除100000
        /// </summary>
        public int Lng;

        /// <summary>
        /// 纬度 ° 5位小数需要除100000
        /// </summary>
        public int Lat;

        /// <summary>
        /// 海拔 m
        /// </summary>
        public float Alt;

        /// <summary>
        /// 东速度
        /// </summary>
        public float Ve;

        /// <summary>
        /// 北向速度
        /// </summary>
        public float Vn;

        /// <summary>
        /// 纵向速度
        /// </summary>
        public float Vz;

        /// <summary>
        /// 斜距在东向的分量 m
        /// </summary>
        public float X;

        /// <summary>
        /// 斜距在北向的分量 m
        /// </summary>
        public float Y;

        /// <summary>
        /// 斜距在竖直方向的分量（即目标与雷达的相对高度） m
        /// </summary>
        public float Z;

        /// <summary>
        /// 相关（真实0/外推1）
        /// </summary>
        public int Relation;

        /// <summary>
        /// 跟踪次数
        /// </summary>
        public int MonitorCount;

        /// <summary>
        /// 丢失次数
        /// </summary>
        public int LostCount;

        /// <summary>
        /// 设备编号
        /// </summary>
        public byte DevSnum;

        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] Reserve;
    };
}
