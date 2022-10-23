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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP13.Cmd
{
    public class A_MonitorCmd : IPeerCmd
    {
        public A_MonitorCmd(IMemoryCache memory, ILogger<A_MonitorCmd> logger, IServiceProvider provider)
        {
            _memory = memory;
            _logger = logger;
            var dev = _memory.GetDevice();
            _host = provider.GetServices<IDeviceHostService>().FirstOrDefault(x => x.DeviceCategory == dev.Category);
        }
        private readonly IMemoryCache _memory;
        public int Category => PluginConst.Category;
        private readonly IDeviceHostService _host;
        public string Key => PluginConst.MonitorTgCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly ILogger _logger;
        public Task Invoke(IPeerContent content)
        {
            byte[] buff = content.Source;
            //foreach(byte t in buff)
            //{
            //    Console.Write(t.ToString("x2")+"\t");

            //}
            //Console.WriteLine();
            //Array.Reverse(buff);
            //var tmp = BitConverter.ToInt32(buff, 36);
            //if (tmp == 0) { return Task.FromResult()}

            TargetInfo model=null;
           // if (tmp == 2)
            {
                var dev = _memory.GetDeviceStatus();
                var irTrack = buff.ToStuct<P_MW_DevInfo>();
                if (dev.Code==DeviceStatusCode.Running )
                {
                    model = MapToTargetInfo(irTrack, _memory.GetDevice());
                    //_logger.LogError($"收到光电上报目标信息：{JsonConvert.SerializeObject(model)}");
                    return _memory.UpdateTarget(model);
                }
                
            }
            //var irTrack = buff.ToStuct<P_MW_Monitor>();

           
            return Task.CompletedTask;
            //var model = MapToTargetInfo(irTrack, _memory.GetDevice());
            //_logger.LogError($"收到光电上报目标信息：{JsonConvert.SerializeObject(model)}");
                      
        }
        TargetInfo MapToTargetInfo(P_MW_DevInfo irTrack, DeviceInfo dev)
        {
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{irTrack.Id}";

            var tg = new TargetInfo
            {

                DeviceId = dev.Id,
                Id = id,
                ProbeAz = irTrack.Az,
                PeobeEl = irTrack.El,
                Lng = irTrack.Az,
                Lat = irTrack.El,
                ProbeDis=irTrack.Dis,
                Alt=irTrack.Alt,
                //ProbeDis = irTrack.Data.,
                //ProbeHigh = irTrack.Height,
                Category = 1,
                Mode = 0,
                //Vr = irTrack.TanSpeed,
                //Vt = irTrack.RadSpeed,
                TrackTime = DateTime.Now,
            };
            
            
            return tg;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct P_MW_DevInfo
        {
            public uint Head;//0x8A808988
            public int ProtocalNum;
            public int Length;//20+bodylength
            public int Order;//8
            public long Time;

            public uint Id;//
            public long TimeStamp;
            public double Az;
            public double El;
            public double Dis;
            public double AzAngleV;
            public double ElAngleV;
            public short Alt;
            public byte Zoom;
            public byte Reverse;

            public int Index;
            public uint Check;
            public uint End;//0x8B8A8089
        }
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct P_MW_Free_Monitor
        //{
        //    public uint Head;//0x8A808988
        //    public int ProtocalNum;
        //    public int Length;//20+bodylength
        //    public int Order;//8
        //    public long Time;

        //    public uint Id;//
        //    public long TimeStamp;
        //    public uint Status;//0：空闲;1:搜索；2：跟踪
        //    public uint BodyLength;//
        //    public uint BodyTime;

        //    public int Index;
        //    public uint Check;
        //    public uint End;//0x8B8A8089
        //}
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct P_MW_Search_Monitor
        //{
        //    public uint Head;//0x8A808988
        //    public int ProtocalNum;
        //    public int Length;//20+bodylength
        //    public int Order;//8
        //    public long Time;

        //    public uint Id;//0x8A808988
        //    public long TimeStamp;
        //    public uint Status;//0：空闲;1:搜索；2：跟踪
        //    public uint BodyLength;//
        //    public uint BodyTime;
        //    public long TargetCount;
        //    public uint DataLength;//
        //    public P_MW_TargetInfo_Monitor[] Data;//


        //    public int Index;
        //    public uint Check;
        //    public uint End;//0x8B8A8089
        //}
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct P_MW_Follow_Monitor
        //{
        //    public uint Head;//0x8A808988
        //    public int ProtocalNum;
        //    public int Length;//20+bodylength
        //    public int Order;//8
        //    public long Time;

        //    public uint Id;//0x8A808988
        //    public long TimeStamp;
        //    public uint Status;//0：空闲;1:搜索；2：跟踪
        //    public uint BodyLength;//
        //    public uint BodyTime;
        //    public uint TargetStatus;
        //    public P_MW_TargetInfo_Monitor Data;//


        //    public int Index;
        //    public uint Check;
        //    public uint End;//0x8B8A8089
        //}
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct P_MW_TargetInfo_Monitor
        //{
        //    public uint Id;//
        //    public uint Category;
        //    public uint Similar;//
        //    public uint Width;//
        //    public uint Height;
        //    public double Az;//
        //    public double El;

        //}



    }
}

