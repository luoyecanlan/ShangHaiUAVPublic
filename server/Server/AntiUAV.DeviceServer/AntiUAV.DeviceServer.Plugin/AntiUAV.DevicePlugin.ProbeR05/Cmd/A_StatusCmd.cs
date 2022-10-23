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

namespace AntiUAV.DevicePlugin.ProbeR05.Cmd
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

            if (data.ParamType == 0x01)
            {
                byte Bit_a = data.Status1[0];//bit0～7 对应接收通道 1～8 状态 0/1 —— 正常/故障
                bool bita0 = Bit_a>>7 == 0 ? true : false;
                bool bita1 = Bit_a >> 6 == 0 ? true : false; 
                bool bita2 = Bit_a >> 5 == 0 ? true : false;
                bool bita3 = Bit_a >> 4 == 0 ? true : false;
                bool bita4 = Bit_a >> 3 == 0 ? true : false;
                bool bita5 = Bit_a >> 2 == 0 ? true : false;
                bool bita6 = Bit_a >> 1 == 0 ? true : false;
                bool bita7 = Bit_a >> 0 == 0 ? true : false;
               

                byte Bit_c = data.Status3[0];//bit0～7 对应发射通道 1～8 状态 0/1 —— 正常/故障
                bool bitc0 = Bit_c >> 7 == 0 ? true : false;
                bool bitc1 = Bit_c >> 6 == 0 ? true : false;
                bool bitc2 = Bit_c >> 5 == 0 ? true : false;
                bool bitc3 = Bit_c >> 4 == 0 ? true : false;
                bool bitc4 = Bit_c >> 3 == 0 ? true : false;
                bool bitc5 = Bit_c >> 2 == 0 ? true : false;
                bool bitc6 = Bit_c >> 1 == 0 ? true : false;
                bool bitc7 = Bit_c >> 0 == 0 ? true : false;
                if (bitc0 || bitc1 || bitc2 || bitc3 || bitc4 || bitc5 || bitc6 || bitc7)
                {
                    if (bita0 || bita1 || bita2 || bita3 || bita4 || bita5 || bita6 || bita7)
                    {
                        _memory.UpdateDeviceRun(DeviceStatusCode.Running);//设备正常运行
                    }
                }
                else
                {
                    _memory.UpdateDeviceBit(Bit_c+Bit_a, GetBit(Bit_c + Bit_a));//记录设备bit信息
                }
            }
            if (data.ParamType == 0x02)
            {
                float lat = BitConverter.ToSingle(data.Status1);//范围：-90～+90
                float lon = BitConverter.ToSingle(data.Status1);//范围：-180～+180
                float alt = BitConverter.ToSingle(data.Status1);//范围：0～1000m
                float angle = BitConverter.ToSingle(data.Status1);//主阵面朝向：范围：0～360
            }
            
           

            return Task.CompletedTask;
        }
        private string GetBit(long bit)
        {
            if (bit == 0) return string.Empty;
            return bit.ToString();
        }               
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_Status
    {
        public uint Head;//0xaa aa aa aa
        public uint InfoType;//0x03 00 00 00
        public byte ParamType;//20+bodylength
        public byte TargetStation;
        public byte TargetFront;
        public byte CheckSum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reverse;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Status1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Status2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Status3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Status4;

    }
}
