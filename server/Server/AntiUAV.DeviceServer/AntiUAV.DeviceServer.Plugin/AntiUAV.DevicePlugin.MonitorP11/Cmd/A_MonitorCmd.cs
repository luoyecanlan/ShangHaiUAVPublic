using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP11.Cmd
{
    public class A_MonitorCmd : IPeerCmd
    {
        public A_MonitorCmd(IMemoryCache memory, ILogger<A_MonitorCmd> logger)
        {
            _memory = memory;
            _logger = logger;
        }
        private readonly IMemoryCache _memory;
        public int Category => PluginConst.Category;

        public string Key => PluginConst.MonitorTgCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly ILogger _logger;
        public Task Invoke(IPeerContent content)
        {
            byte[] buff = content.Source;
            Array.Reverse(buff);
            var irTrack = buff.ToStuct<M_TargetMt>();



            var model = MapToTargetInfo(irTrack, _memory.GetDevice());
            //_logger.LogError($"收到光电上报目标信息：{JsonConvert.SerializeObject(model)}");

          
            return _memory.UpdateTarget(model);
        }
        TargetInfo MapToTargetInfo(M_TargetMt irTrack, DeviceInfo dev)
        {
            var tg = new TargetInfo
            {
                Id = irTrack.ID.ToString(),
                Alt = irTrack.Alt,
                Lat = irTrack.Lat,
                Lng = irTrack.Lng,
                DeviceId = dev.Id,
                ProbeAz = irTrack.Position,
                PeobeEl = irTrack.Pitch,
                ProbeDis = irTrack.Distance,
                ProbeHigh = irTrack.Height,
                Category = 1,
                Mode = 0,
                Vr = irTrack.TanSpeed,
                Vt = irTrack.RadSpeed,
                TrackTime = DateTime.Now,
            };
            return tg;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct M_TargetMt
        {
            public uint End;//0x7B7A7079
            public uint Check;
            public uint Index;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] Reserve;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] TimeStamp;//时间
            public float RadSpeed;//径向速度
            public float TanSpeed;///切向速度
            public float Alt;//目标海拔
            public float Lat;//目标纬度
            public float Lng;//目标经度
            public float Height;//目标高度
            public float Distance;//目标距离
            public float Pitch;//目标俯仰
            public float Position;//目标方位
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] ID;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Time;
            public uint Order;
            public uint Length;//20+bodylength ====/ 20 + 70
            public uint ProtocalNum;//0x2329
            public uint Head;//0x7A707978

        }

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //struct M_TargetMt
        //{
        //    public uint Head;//0x7A707978
        //    public uint ProtocalNum;//0x2329
        //    public uint Length;//20+bodylength ====/ 20 + 50
        //    public uint Order;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        //    public byte[] Time;

        //    //public uint TargetID;//目标ID
        //    public float Position;//目标方位
        //    public float Pitch;//目标俯仰
        //    public float Distance;//目标距离
        //    public float Height;//目标高度
        //    public float Lng;//目标经度
        //    public float Lat;//目标纬度
        //    public float Alt;//目标海拔
        //    public float TanSpeed;///切向速度
        //    public float RadSpeed;//径向速度            
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        //    public byte[] TimeStamp;//时间

        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        //    public byte[] Reserve;
        //    public uint Index;
        //    public uint Check;
        //    public uint End;//0x7B7A7079


        //}

    }
}

