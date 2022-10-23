using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DB;
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

namespace AntiUAV.DevicePlugin.Query01.Cmd
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
            try
            {
                if (true)
                {
                    var dev = _memory.GetDevice();
                    var tgs = new List<TargetInfo>();
                    var track = content.Source.ToStuct<ASK_Trigger03_Whole_Struct>();
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
            catch (Exception ex)
            {
                _logger.LogWarning("The A_TrackCmd Command failed detection");
                await Task.FromCanceled(new System.Threading.CancellationToken());
                throw ex;
            }
        }

        TargetInfo MapToTargetInfo(ASK_Trigger03_Whole_Struct track, DeviceInfo dev)
        {
            byte[] temp = new byte[4];
            //Array.Reverse(track.trigger.content.TargetId);
            Array.Copy(track.trigger.content.TargetId, 0, temp, 0, 3);
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{BitConverter.ToInt32(temp)}";
            string date = $"{DateTime.Now.ToString("yyyy-MM-dd")} {track.trigger.content.Time[0].ToString()}:{track.trigger.content.Time[1].ToString()}:{track.trigger.content.Time[2].ToString()}";
            DateTime dt = Convert.ToDateTime(date);
            var tg = new TargetInfo
            {
                Id = id,

                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                Freq = -1,
                Lat = _tool.ConverToDegrade(track.trigger.content.Latitude[0], track.trigger.content.Latitude[1], track.trigger.content.Latitude[2]),
                Lng = _tool.ConverToDegrade(track.trigger.content.Longtude[0], track.trigger.content.Longtude[1], track.trigger.content.Longtude[2]),

                TrackTime = dt,
                Threat = _memory.GetThreat(id)
            };
            //var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };
            //var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔
            //tg.Lat = Math.Round(position.Lat, 6);
            //tg.Lng = Math.Round(position.Lng, 6);
            //tg.Alt = Math.Round(position.Altitude, 1);
            return tg;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    /// <summary>
    /// 总长度19+N（N为内容长度
    /// </summary>
    public struct ASK_Trigger03_Whole_Struct
    {

        public byte Head;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Length;

        public long TimePiont;//时间戳
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] SerialNumber;
        public byte AskType;

        public ASK_Trigger03_receive_struct trigger;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Check;
        public byte End;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASK_Trigger03_receive_struct
    {
        /// /////////////////////消息内容 /////////////////////
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessId1;
        public byte ResponseId;
        public ASK_Trigger03_receive content;
        /// /////////////////////消息内容/////////////////////
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASK_Trigger03_receive
    {
        /// /////////////////////消息内容 /////////////////////
        public byte TargetNumber;
        public byte IsWithAdress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] TargetId;
        public short Distance;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Longtude;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Latitude;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Time;
        /// /////////////////////消息内容/////////////////////
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ask_Ans_Model_Two
    {
        public byte Logal_Start;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _infor_length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] _Time_Piont;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _Serial_Number;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] _Ask_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]//代表三个字节
        public byte[] _ID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]//代表三个字节
        public byte[] Data_Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]//代表三个字节
        public byte[] Batch_Number;
        public float _longtude;
        public float _latitude;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _Time;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _check;
        public byte Logal_End;
    }

}
