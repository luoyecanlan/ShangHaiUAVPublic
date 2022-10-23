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
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR04.Cmd
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
            var head = new byte[PluginConst.TrackCheckHead.Length];
            Buffer.BlockCopy(content.Source, 0, head, 0, head.Length);
            var checkCmd = Enumerable.SequenceEqual(PluginConst.TrackCheckHead, head);
            if (checkCmd)
            {
                var dev = _memory.GetDevice();
                var tgs = new List<TargetInfo>();
                var track = content.Source.ToStuct<R_ProbeR04_Track>();
                var tg = MapToTargetInfo(track, dev);
                tgs.Add(tg);
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
        TargetInfo MapToTargetInfo(R_ProbeR04_Track track, DeviceInfo dev)
        {
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{track.TargetNO}";
            //var tg = new TargetInfo
            //{
            //    Id = id,
            //    Alt = track.TargetHeight,
            //    DeviceId = dev.Id,
            //    CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
            //    Freq = -1,
            //    ProbeAz = track.TargetNorthPoint * 0.01,
            //    PeobeEl = track.TargetElevation * 0.01,
            //    ProbeDis = track.TargetDistance,
            //    ProbeHigh = track.TargetHeight,
            //    Category = 1,
            //    Mode = track.SendModel,
            //    Vr = track.TargetSailSpeed * 0.1,
            //    Vt = track.TargetSpeed * 0.1,
            //    Lat = track.TargetLat,
            //    Lng = track.TargetLng,
            //    TrackTime = DateTime.Now,

            //    Threat = _memory.GetThreat(track.TargetNO.ToString())
            //};
            //////////////
            var tg = new TargetInfo
            {
                Id = id,
                Alt = track.TargetHeight,
                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.Perception,
                Freq = -1,
                ProbeAz = track.TargetNorthPoint > 0 ? track.TargetNorthPoint * 0.01 : (36000+track.TargetNorthPoint) * 0.01,
                PeobeEl = track.TargetElevation * 0.01,
                ProbeDis = track.TargetDistance,
                ProbeHigh = track.TargetHeight,
                Category = 1,
                Mode =Convert.ToInt32(track.SendModel),
                Vr = track.TargetSailSpeed * 0.1,
                Vt = track.TargetSpeed * 0.1,
                Lat = track.TargetLat,
                Lng = track.TargetLng,
                TrackTime = DateTime.Now,

            Threat = _memory.GetThreat(track.TargetNO.ToString())
            };



            //var tg = new TargetInfo
            //{
            //    Id = id,
            //    Alt = 164.7212,
            //    DeviceId = dev.Id,
            //    CoordinateType = TargetCoordinateType.Perception,
            //    Freq = -1,
            //    ProbeAz = 334.85,
            //    PeobeEl = 3.52,
            //    ProbeDis = 2410,
            //    ProbeHigh = 164.7212,
            //    Category = 1,
            //    Mode = track.SendModel,
            //    Vr = track.TargetSailSpeed * 0.1,
            //    Vt = track.TargetSpeed * 0.1,
            //    Lat = 38.0998954772949,
            //    Lng = 115.755020141602,
            //    TrackTime = DateTime.Now,

            //    Threat = _memory.GetThreat(track.TargetNO.ToString())
            //};
            //var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };
            //var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔 
            //tg.Lat = Math.Round(position.Lat, 6);
            //tg.Lng = Math.Round(position.Lng, 6);
            //tg.Alt = Math.Round(position.Altitude, 1);
            return tg;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_ProbeR04_Track
    {
        public uint Head; //帧头	Unsiged Int	4	32bit	0xCC5555CC	-	-
        public ushort Length; //报文 字节数 Unsiged Short	2	16bit	64	-	整个数据包字节数
        public ushort RadarId; //雷达ID	Unsigned Short	2	16bit	-	-	-
        public ulong TimeSpan; //时间戳	Unsiged Int64	8	64bit		1ms	UTC时间
        public ushort Revs; //Revs Unsigned Short	2	16bit	-	-	-
        public ushort StartFlag; //起批标志	Unsiged Short	2	16bit	0～2	-	0为起批未确定1为确定目标2该目标丢弃（还可以根据历史点迹个数筛选航迹）
        public ushort TargetNO; //目标编号	Unsiged Short	2	16bit	0～1000	-	目标的编号
        public ushort TargetPointCount; //历史点迹个数 Unsiged Short	2	16bit	0～65535	1个 雷达发现目标个数（航迹个数）
        public ushort TargetAngle; //目标航向角 Unsiged Short	2	16bit	0～35999	0.01°	目标运动的方向
        public short TargetElevation; //目标仰角	Siged Short	2	16bit	-9000～9000	0.01°	目标与雷达的连线与水平面夹角
        public uint TargetDistance; //目标距离	Unsiged Int	4	32bit	0 ～（2^32-1）	1m	目标距雷达斜距
        public ushort TargetSailSpeed; //目标航速	Unsigned Short	2	16bit	0～10000	0.1m/s	目标运动的速度
        public short TargetSpeed; //目标速度	Signed Short	2	16bit	-1000～1000	0.1m/s	目标相对雷达的径向速度,远离雷达为正，靠近雷达为负;
        public short TargetNorthPoint; //目标偏北角 Siged Short	2	16bit	-18000～18000	0.01度 为目标和雷达连线与雷达正北的夹角，目标北偏西时值域为0到-18000度，北偏东时值域为0到18000度。
        public double TargetLat; //目标纬度	Double	8	64bit	-	1度	WG84坐标系下纬度
        public double TargetLng; //目标经度	Double	8	64bit	-	1度	WG84坐标系下经度
        public float TargetHeight; //目标高度	Float	4	32bit	-	1m	目标海拔高度
        public char SendModel; //发送航迹模式	Char	1	8bit	0～1	-	0为发送所有航迹1为发送单条选中航迹
        public byte Revs2; //Revs	Usigned Char	1	8bit	-	-	预留
        public uint CheckCode; //校验和	Unsigned Int	4	32bit	-	-	32bit累加和
    }
}
