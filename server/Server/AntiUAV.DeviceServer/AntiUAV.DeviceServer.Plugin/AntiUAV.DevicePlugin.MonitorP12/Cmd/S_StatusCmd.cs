using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP11.Cmd
{
    public class S_StatusCmd : IPeerCmd
    {
        public S_StatusCmd(IMemoryCache memory, ILogger<S_StatusCmd> logger)
        {
            _memory = memory;
            _logger = logger;
            //var sss= LogManager.GetCurrentClassLogger();
        }

        private readonly ILogger _logger;
        public int Category => PluginConst.Category;

        public string Key => PluginConst.StatusCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;

        public Task Invoke(IPeerContent content)
        {
            byte[] buff = content.Source;
            Array.Reverse(buff);
            var data = buff.ToStuct<P_Status>();
#if DEBUG
            //buff.OutPutByte();

#endif
            switch (data.Status)
            {
                case 1:
                    _memory.UpdateDeviceRun(DeviceStatusCode.Free);
                    break;
                case 2:
                    _memory.UpdateDeviceRun(DeviceStatusCode.Search);
                    break;
                case 3:
                    _memory.UpdateDeviceRun(DeviceStatusCode.Running);
                    break;
                case 4://其他状态 预留位
                    _memory.UpdateDeviceRun(DeviceStatusCode.Free);
                    break;
                default:
                    _memory.UpdateDeviceRun(DeviceStatusCode.OffLine);
                    break;

            }
            
            var deviceRectifyInfo = new DevRectifyInfo()
            {
                Az = data.DirectionRectify,
                El = data.PitchRectify
            };

            var devicePositionInfo = new DevPositionInfo()
            {
                Lat = data.DeviceLat,
                Lng = data.DeviceLng,
                Alt = data.DeviceAlt
            };
            //_logger.LogError("RectifyInfo=" + JsonConvert.SerializeObject(deviceRectifyInfo));
            //_logger.LogError("PositionInfo" + JsonConvert.SerializeObject(devicePositionInfo));
            return Task.CompletedTask;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        struct P_Status
        {
            public int End;//0x7B7A7079
            public uint Check;

            public int Index;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] Reserve;

            public Int64 Time1;
            public float Focus;
            public float CurrentFiled;
            public float CurrentDistance;
            public float CurrentPitch;
            public float CurrentDirection;
            public float DeviceAlt;
            public float DeviceLat;
            public float DeviceLng;
            public float PitchRectify;
            public float DirectionRectify;
            public short Status;
            public long Time;


            public int Order;
            public int Length;//20+bodylength
            public int ProtocalNum;//0x2329
            public int Head;//0x7A707978

        }
        //struct P_Status
        //{
        //    public int Head;//0x7A707978
        //    public int ProtocalNum;//0x2329
        //    public int Length;//20+bodylength
        //    public int Order;
        //    public long Time;

        //    public short Status;
        //    public float DirectionRectify;
        //    public float PitchRectify;
        //    public float DeviceLng;
        //    public float DeviceLat;
        //    public float DeviceAlt;
        //    public float CurrentDirection;
        //    public float CurrentPitch;
        //    public float CurrentDistance;
        //    public float CurrentFiled;
        //    public float Focus;
        //    public Int64 Time1;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        //    public byte[] Reserve;
        //    public int Index;
        //    public uint Check;
        //    public int End;//0x7B7A7079
        //}
    }
}
