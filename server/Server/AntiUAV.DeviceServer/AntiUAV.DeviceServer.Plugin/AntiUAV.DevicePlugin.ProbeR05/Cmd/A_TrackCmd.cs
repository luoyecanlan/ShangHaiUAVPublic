using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
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

namespace AntiUAV.DevicePlugin.ProbeR05.Cmd
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
        private readonly GisTool _tool;
        private readonly IDeviceHostService _host;
        public async Task Invoke(IPeerContent content)
        {
            var dev = _memory.GetDevice();
            //var data = content.Source.ToStuct<R_SWJ_Track>();
            var t_time = DateTime.Now;//data.Body.Time.ToDateTime();
            var tgs = new List<TargetInfo>();
            //var index = Marshal.SizeOf<R_SWJ_Track>();
            var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };

            byte[] buff = content.Source;
            if (buff.Length > 12)
            {
                int trackNum=(buff.Length-12)/ 80;
                byte[] trackBuff;
                while (trackNum > 0)
                {
                    trackBuff=buff.Skip((trackNum-1)*80).Take(80).ToArray();
                    var temp = trackBuff.ToStuct<R_SWJ_Track_Body>();
                    var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{temp.Id}";
                    var tg = new TargetInfo()
                    {
                        DeviceId = dev.Id,
                        Id = id,
                        CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                        Freq = -1,
                        ProbeAz = Math.Round(temp.Azimuth, 3),
                        PeobeEl = Math.Round(temp.Pitch, 3),
                        ProbeDis = Math.Round(temp.Dis, 1),
                        ProbeHigh = Math.Round(temp.Altitude, 1),
                        Category = 1,
                        Mode = temp.category,
                        Vr = Math.Abs(Math.Round(temp.RadialSpeed, 1)),
                        Vt = Math.Abs(Math.Round(temp.RadialSpeed, 1)),
                        TrackTime = t_time,
                        Threat = _memory.GetThreat(id)
                    };
                    var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔
                    tg.Lat = Math.Round(position.Lat, 6);
                    tg.Lng = Math.Round(position.Lng, 6);
                    tg.Alt = Math.Round(position.Altitude, 1);
                    tgs.Add(tg);
                    trackNum--;
                }
                
                
            }
            await _memory.UpdateTarget(tgs.ToArray());
            content.SourceAys = tgs;
            //_logger.LogDebug($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()}.");
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_SWJ_Track
    {
        public uint Head;//0xaa aa aa aa
        public byte ProtocalNum;//0x01
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reverse1;//预留
        public byte StationNum;//站点编号
        public ushort TrackNum;//航迹个数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reverse2;//预留
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        //public R_SWJ_Track_Body r_SWJ_Track_Body;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_SWJ_Track_Body
    {
        public uint Id;
        public float Azimuth;
        public float Dis;
        public float Lon;//经度

        public float Lat;//纬度
        public float RadialSpeed;//径向速度
        public int IsExist;
        public float Pitch;

        public float Direction;
        public float Speed;
        public float Altitude;
        public uint OrderTime;
        public ushort Range;//幅度
        public byte category;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Reverse;//预留
        public byte Check;

       
    }

    
}
