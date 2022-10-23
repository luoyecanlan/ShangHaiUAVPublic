using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using EasyNetQ.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct01
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
            byte[] result;
            if (sw)
            {
                //byte[] result;
                IEnumerable<Relationships> relationships = _memory.GetRelationshipsGuidance();
                //先从内存里取所有设备的引导关系 然后在引导关系里获取这个目标id进而获取这个目标的频点
                TargetCacheInfo targetCacheInfo = _memory.GetAllTargets().Where(i => i.Last.Id == relationships.Where(x => x.RType == RelationshipsType.AttackGd).FirstOrDefault().TargetId).FirstOrDefault();
                double freq = Math.Round(targetCacheInfo.Last.Freq, 1);
                #region 解析干扰开关协议
                //Bit0:400MHz  bit1:900MHz
                //Bit2: 1.5GHz bit3:2.4GHz
                // Bit4:5.8GHz
                byte data = 0x00;

                switch (freq)
                {
                    case 0.4: data = 0x01; break;
                    case 0.9: data = 0x02; break;
                    case 1.5: data = 0x04; break;
                    case 2.4: data = 0x08; break;
                    case 5.8: data = 0x18; break;
                    default: Console.WriteLine("频点获取错误"); break;
                }

                P_Status_Send StatusSend = new P_Status_Send()
                {
                    Head = Encoding.Default.GetBytes("SZ"),
                    ProtocalNum = 0x09,
                    Length = 0x07,
                    data = data,
                    Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 0x09 + 0x07 + 1 + data)),
                };
                int size = Marshal.SizeOf(StatusSend);
                result = new byte[size];
                IntPtr structPtr = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
                Marshal.StructureToPtr(StatusSend, structPtr, false);//将结构体拷到分配好的内存空间
                Marshal.Copy(structPtr, result, 0, size);//从内存空间拷到byte数组
                Marshal.FreeHGlobal(structPtr);//释放内存空间
                //Console.WriteLine(BitConverter.ToString(result));
                #endregion

                return result;
            }
            else//每次关闭时关闭所有攻放
            {

                P_Status_Send StatusSend = new P_Status_Send()
                {
                    Head = Encoding.Default.GetBytes("SZ"),
                    ProtocalNum = 0x08,
                    Length = 0x07,
                    data = 0,
                    Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 0x08 + 0x07 + 1 + 0)),
                };
                int size = Marshal.SizeOf(StatusSend);
                result = new byte[size];
                IntPtr structPtr = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
                Marshal.StructureToPtr(StatusSend, structPtr, false);//将结构体拷到分配好的内存空间
                Marshal.Copy(structPtr, result, 0, size);//从内存空间拷到byte数组
                Marshal.FreeHGlobal(structPtr);//释放内存空间
                Console.WriteLine(BitConverter.ToString(result));

                return result;
            }

        }

        public override byte[] GetGuidanceBuff(GuidancePositionInfo position)
        {
            byte[] result;
            var dev = _memory.GetDevice();
            //水平角  要到达的角度 * 100得到10进制数转换成16进制，高位前低位后
            int Hor = Convert.ToInt32(position.TargetX * 100);
            //俯仰角  当角度为负（平台前倾）时，高字节<<8+低字节=角度*100。当角度为正时，高字节<<8+低字节=36000-角度*100
            int pitch = position.TargetY > 0 ? Convert.ToInt32(36000 - position.TargetY * 100) : Convert.ToInt32(position.TargetY * 100);
            #region 转台协议解析
            P_HorizontalSet_Send HoriReceive = new P_HorizontalSet_Send()
            {
                Head = Encoding.Default.GetBytes("SZ"),
                ProtocalNum = 0x01,
                Length = 0x08,
                data = BitConverter.GetBytes((short)(Hor)).Reverse().ToArray(),
                Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 1 + 8 + 1 + BitConverter.GetBytes((short)(Hor)).Reverse().ToArray()[0] + BitConverter.GetBytes((short)(Hor)).Reverse().ToArray()[1])),
            };
            int size = Marshal.SizeOf(HoriReceive);
            byte[] HoriReceiveresult = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
            Marshal.StructureToPtr(HoriReceive, structPtr, false);//将结构体拷到分配好的内存空间
            Marshal.Copy(structPtr, HoriReceiveresult, 0, size);//从内存空间拷到byte数组
            Marshal.FreeHGlobal(structPtr);//释放内存空间
            //Console.WriteLine(BitConverter.ToString(HoriReceiveresult));

            P_PitchSet_Send pitchSet_Send = new P_PitchSet_Send()
            {
                Head = Encoding.UTF8.GetBytes("SZ"),
                ProtocalNum = 0x03,
                Length = 8,
                data = BitConverter.GetBytes((short)(pitch)).Reverse().ToArray(),
                Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 1 + 8 + 1 + BitConverter.GetBytes((short)(pitch)).Reverse().ToArray()[0] + BitConverter.GetBytes((short)(pitch)).Reverse().ToArray()[1])),
            };

            int pitchsize = Marshal.SizeOf(pitchSet_Send);
            byte[] PitchSendresult = new byte[pitchsize];
            IntPtr pitchstructPtr = Marshal.AllocHGlobal(pitchsize);//分配结构体大小的内存空间
            Marshal.StructureToPtr(pitchSet_Send, pitchstructPtr, false);//将结构体拷到分配好的内存空间
            Marshal.Copy(pitchstructPtr, PitchSendresult, 0, pitchsize);//从内存空间拷到byte数组
            Marshal.FreeHGlobal(pitchstructPtr);//释放内存空间
            //Console.WriteLine(BitConverter.ToString(PitchSendresult));
            #endregion
            int len = size + pitchsize;
            result = new byte[len];
            HoriReceiveresult.CopyTo(result, 0);
            PitchSendresult.CopyTo(result, size);

            //水平俯仰一起发出
            return result;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct P_HorizontalSet_Send
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Head;//"SZ"
            public byte ProtocalNum;//0x01
            public byte Length;//8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] devId;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] data;//要到达的角度 * 100得到10进制数转换成16进制，高位前低位后
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] Check;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct P_PitchSet_Send
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Head;//"SZ"
            public byte ProtocalNum;//0x01
            public byte Length;//8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] devId;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] data;//要到达的角度 * 100得到10进制数转换成16进制，高位前低位后
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] Check;
        }
        /// <summary>
        /// 多个功放开关控制
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct P_Status_Send
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Head; //"SZ"
            public byte ProtocalNum;//0x0D
            public byte Length;//6
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] devId;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] Check;
        }

    }
}
