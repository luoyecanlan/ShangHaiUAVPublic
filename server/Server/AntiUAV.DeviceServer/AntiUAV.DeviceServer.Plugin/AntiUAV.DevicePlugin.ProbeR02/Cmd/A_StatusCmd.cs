using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using AntiUAV.DeviceServer;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AntiUAV.Bussiness.Models;

namespace AntiUAV.DevicePlugin.ProbeR02.Cmd
{
    public class A_StatusCmd : IPeerCmd
    {
        public A_StatusCmd(IMemoryCache memory)
        {
            _memory = memory;
        }
        public int Category => PluginConst.Category;

        public string Key => PluginConst.StatusCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;

        public Task Invoke(IPeerContent content)
        {
            var data = content.Source.ToStuct<R_Status>();

            _memory.UpdateDeviceBit(data.Bit, GetBit(data.Bit));//记录设备bit信息
            if (data.Power)
            {
                if (data.Amplifier > 0)
                {
                    _memory.UpdateDeviceRun(DeviceStatusCode.Running);//设备正常运行
                }
                else
                {
                    _memory.UpdateDeviceRun(DeviceStatusCode.Free);//待机
                }
            }
            else
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Free);//停机
            }

            return Task.CompletedTask;
        }
        private string GetBit(long bit)
        {
            if (bit == 0) return string.Empty;
            return bit.ToString();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct R_Status
        {
            public int Head;//0x7A707978
            public int ProtocalNum;//0x2329
            public int Length;//20+bodylength
            public int Order;
            public long Time;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Id;
            public uint Snum;
            public long Time1;
            [MarshalAs(UnmanagedType.Bool, SizeConst = 1)]
            public bool Power;
            public uint Amplifier;
            public uint Mode;
            public long Bit;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] Reserve;
            public int Index;
            public uint Check;
            public int End;//0x7B7A7079
        }
    }
}
