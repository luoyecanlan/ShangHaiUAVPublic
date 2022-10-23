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

namespace AntiUAV.DevicePlugin.MonitorP13.Cmd
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
            //Array.Reverse(buff);
            var data = buff.ToStuct<P_MW_Status>();
#if DEBUG
            //buff.OutPutByte();

#endif
            switch (data.Body.Model)
            {
                case 0:
                    _memory.UpdateDeviceRun(DeviceStatusCode.Free);
                    break;
                //case 1:
                //    _memory.UpdateDeviceRun(DeviceStatusCode.Search);
                //    break;
                case 2:
                    _memory.UpdateDeviceRun(DeviceStatusCode.Running);
                    break;

                default:
                    _memory.UpdateDeviceRun(DeviceStatusCode.Free);
                    break;

            }
            //if (data.Body.Status == 0)
            //{
            //    _memory.UpdateDeviceBit(data.Body.Bit);

            //}


            //_logger.LogError("RectifyInfo=" + JsonConvert.SerializeObject(deviceRectifyInfo));
            //_logger.LogError("PositionInfo" + JsonConvert.SerializeObject(devicePositionInfo));
            return Task.CompletedTask;
        }
    }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct P_MW_Status
        {
            public uint Head;//0x8A808988
            public int ProtocalNum;
            public int Length;//20+bodylength
            public int Order;//4
            public long Time;
            public P_MW_Status_Body Body;
            public int Index;
            public uint Check;
            public uint End;//0x8B8A8089
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        public struct P_MW_Status_Body
        {
            public uint Id;//0x8A808988
            public long TimeStamp;
            public uint Status;//0：异常；1：正常
            public uint Model;//0：空闲;1:搜索；2：跟踪
            public long Bit;
            public int Reverse;
        }
        
    
}
