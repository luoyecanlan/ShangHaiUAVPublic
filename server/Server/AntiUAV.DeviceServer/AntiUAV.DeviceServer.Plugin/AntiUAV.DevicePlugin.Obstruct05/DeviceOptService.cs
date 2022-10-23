using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using EasyNetQ.Logging;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public DeviceOptService(IMemoryCache memory)
        {
            _memory = memory;
            //_log = log;
        }
        //ILog _log;
        public override int DeviceCategory => PluginConst.Category;

        private readonly IMemoryCache _memory;

        public override byte[] GetAttackBuff(string json, bool sw)
        {
            var dev = _memory.GetDevice();
            byte[] result;
            var relation = JsonConvert.DeserializeObject<Relationships>(json);
            byte[] data = new byte[6];
            int id = 1;
            if (dev.Ip == "192.168.0.101")
            {
                id = 1;
            }
            else
            {
                id = 2;
            }
            //if (relation == null)
            //{
            //   data=new byte[] {}
            //}
            if (relation != null)
            {
                switch (relation.hitFreq)
                {
                    case HitFreqModel.hit_24_58_900: data = new byte[6] { 0x0f, 0x00, 0x0f, 0x0f, 0x00, 0x00 }; break;
                    case HitFreqModel.hit_900: data = new byte[6] { 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00 }; break;
                    case HitFreqModel.Hit_15: data = new byte[6] { 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00 }; break;
                    case HitFreqModel.hit_24_58: data = new byte[6] { 0x00, 0x00, 0x0f, 0x0f, 0x00, 0x00 }; break;
                    case HitFreqModel.hit_All: data = new byte[6] { 0x0f, 0x0f, 0x0f, 0x0f, 0x00, 0x00 }; break;

                }
            }

            //if (relation?.ToDeviceId == dev.Id)
            //{


                P_ObstructSet_Send StatusSend = new P_ObstructSet_Send();

                if (sw)
                {


                    StatusSend = new P_ObstructSet_Send()
                    {
                        Head = new byte[] { 0xff, 0xa5, 0xa5 },
                        deviceId = (byte)id,
                        reverse = 0x00,
                        commandType = 0x33,
                        command = 0x03,
                        dataLength = 0x06,
                        data = data,
                        End = 0xaa
                    };
                }
                else
                {
                    StatusSend = new P_ObstructSet_Send()
                    {
                        Head = new byte[] { 0xff, 0xa5, 0xa5 },
                        deviceId = (byte)id,
                        reverse = 0x00,
                        commandType = 0x33,
                        command = 0x03,
                        dataLength = 0x06,
                        data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                        End = 0xaa
                    };
                }
                byte[] temp = StatusSend.ToBytes<P_ObstructSet_Send>();
                byte check = 0x00;
                for (int i = 3; i < temp.Length - 2; i++)
                {
                    check += temp[i];
                }
                StatusSend.checkSum = check;
                result = StatusSend.ToBytes();
                return result;


            //}
            //else
            //{
            //    var StatusSend = new P_ObstructSet_Send()
            //    {
            //        Head = new byte[] { 0xff, 0xa5, 0xa5 },
            //        deviceId = (byte)id,
            //        reverse = 0x00,
            //        commandType = 0x33,
            //        command = 0x03,
            //        dataLength = 0x06,
            //        data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            //        End = 0xaa
            //    };
            //    byte[] temp = StatusSend.ToBytes<P_ObstructSet_Send>();
            //    byte check = 0x00;
            //    for (int i = 3; i < temp.Length - 2; i++)
            //    {
            //        check += temp[i];
            //    }
            //    StatusSend.checkSum = check;
            //    result = StatusSend.ToBytes();
            //    return result;
            //}




        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct P_ObstructSet_Send
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Head;//"ff a5 a5"

            public byte deviceId;//0x01
            public byte reverse;//00
            public byte commandType; //读：0x22,PC 读取设备 MCU 数据。MCU 以命令类型：0x82 应答.写：0x33, PC 设置设备的数据。如果写成功，MCU 以命令类型：0x88 应答。
            public byte command;//0x01-0x07
            public byte dataLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] data;

            public byte checkSum;
            public byte End;//0xaa
        }


    }
}
