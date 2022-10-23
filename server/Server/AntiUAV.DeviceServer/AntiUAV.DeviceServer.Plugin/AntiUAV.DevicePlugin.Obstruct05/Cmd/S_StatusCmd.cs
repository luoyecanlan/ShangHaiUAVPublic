using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05.Cmd
{
    public class S_StatusCmd : IPeerCmd
    {
        public S_StatusCmd(IMemoryCache memory, ILogger<S_StatusCmd> logger)
        {
            _memory = memory;
            _logger = logger;
        }

        private readonly ILogger _logger;
        public int Category => PluginConst.Category;

        public string Key => PluginConst.StatusCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;

        public Task Invoke(IPeerContent content)
        {
            var data = content.Source.ToStuct<P_Status_Receive>();
            //byte powerStatus = data.data;
            ////_memory.UpdateDeviceBit(0, null);//记录设备bit异常信息
            //bool bit0, bit1, bit2, bit3, bit4;
            //bit0 = (powerStatus & 0xff) == 0x01 ? true : false;
            //bit1 = (powerStatus & 0xff) == 0x02 ? true : false;
            //bit2 = (powerStatus & 0xff) == 0x04 ? true : false;
            //bit3 = (powerStatus & 0xff) == 0x08 ? true : false;
            //bit4 = (powerStatus & 0xff) == 0x10 ? true : false;
            if (data.data.Any(x => x == 0x0f))
            {

                _memory.UpdateDeviceRun(DeviceStatusCode.Running);//设备正常运行
                _logger.LogInformation("收到设备运行状态上报.");

            }
            else
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Free);//待机
                _logger.LogInformation("收到设备待机状态上报.");
            }

            return Task.CompletedTask;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct P_Status_Receive
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Head;//"SZ"
            public byte deviceid;
            public byte yuliu;//0x8D
            public byte ordertype;//7
            public byte order;//7
            public byte length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] data;
            //转换为2进制后，从右至左每一位代表一个功放开关.Bit0:400MHz bit1:900MHzBit2:1.5GHz bit3:2.4GHzBit4:5.8GHz bit5~bit7备用
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            //public byte data;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Check;
            public byte end;
        }

    }
}
