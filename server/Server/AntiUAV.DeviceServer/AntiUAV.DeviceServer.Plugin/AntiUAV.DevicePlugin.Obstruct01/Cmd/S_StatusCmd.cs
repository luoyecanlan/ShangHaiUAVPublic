using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct01.Cmd
{
    public class S_StatusCmd : IPeerCmd
    {
        public S_StatusCmd(IMemoryCache memory)
        {
            _memory = memory;
        }
        public int Category => PluginConst.Category;

        public string Key => PluginConst.StatusCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;

        public Task Invoke(IPeerContent content)
        {
            var data = content.Source.ToStuct<P_Status_Receive>();
            byte powerStatus = data.data;
            _memory.UpdateDeviceBit(0, null);//记录设备bit异常信息
            bool bit0, bit1, bit2, bit3, bit4;
            bit0 = (powerStatus & 0xff) == 0x01 ? true : false;
            bit1 = (powerStatus & 0xff) == 0x02 ? true : false;
            bit2 = (powerStatus & 0xff) == 0x04 ? true : false;
            bit3 = (powerStatus & 0xff) == 0x08 ? true : false;
            bit4 = (powerStatus & 0xff) == 0x10 ? true : false;
            if (bit0 || bit1 || bit2 || bit3 || bit4)
            {

                _memory.UpdateDeviceRun(DeviceStatusCode.Running);//设备正常运行


            }
            else
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Free);//待机
            }

            return Task.CompletedTask;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct P_Status_Receive
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Head;//"SZ"
            public byte ProtocalNum;//0x8D
            public byte Length;//7
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] devId;
            //转换为2进制后，从右至左每一位代表一个功放开关.Bit0:400MHz bit1:900MHzBit2:1.5GHz bit3:2.4GHzBit4:5.8GHz bit5~bit7备用
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte data;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte Check;
        }

    }
}
