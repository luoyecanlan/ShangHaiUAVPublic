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

namespace AntiUAV.DevicePlugin.Obstruct02
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
            //if (sw)
            {
                //byte[] result;
                //IEnumerable<Relationships> relationships = _memory.GetRelationshipsGuidance();
                ////先从内存里取所有设备的引导关系 然后在引导关系里获取这个目标id进而获取这个目标的频点
                //TargetCacheInfo targetCacheInfo = _memory.GetAllTargets().Where(i => i.Last.Id == relationships.Where(x => x.RType == RelationshipsType.AttackGd).FirstOrDefault().TargetId).FirstOrDefault();
                //double freq=0;
                //if (targetCacheInfo != null)
                //{
                //    freq = Math.Round(targetCacheInfo.Last.Freq, 1);
                //}

                #region 解析干扰开关协议
                ////Bit0:400MHz  bit1:900MHz
                ////Bit2: 1.5GHz bit3:2.4GHz
                //// Bit4:5.8GHz
                //byte data=0x00;

                //switch (freq)
                //{
                //    case 0.4: data = 0x01; break;
                //    case 0.9: data = 0x02; break;
                //    case 1.5: data = 0x04; break;
                //    case 2.4: data = 0x08; break;
                //    case 5.8: data = 0x18; break;
                //    default: Console.WriteLine("频点获取错误"); break;
                //}
                P_Status_Send StatusSend;
                if (sw)
                {
                    var relation = JsonConvert.DeserializeObject<Relationships>(json);
                    byte data = 0x00;////控制功放开关
                    if (relation != null)
                    {
                        switch (relation.hitFreq)
                        {
                            case HitFreqModel.hit_24_58_900: data = 0x02 | 0x08 | 0x10; break;//2.4+5.8
                            case HitFreqModel.hit_900: data = 0x02; break;//2.4
                            case HitFreqModel.Hit_15: data = 0x04; break;//5.8
                            case HitFreqModel.hit_24_58: data = 0x08 | 0x10; break;//1.5
                            case HitFreqModel.hit_All: data = 0x02 | 0x04 | 0x08 | 0x10; break;//2.4+5.8+1.5

                        }
                    }
                    StatusSend = new P_Status_Send()
                    {
                        Head = Encoding.Default.GetBytes("SZ"),
                        ProtocalNum = 0x09,
                        Length = 0x07,
                        devId = new byte[] { 0x00, 0x01 },
                        data = (byte)data,
                        Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 0x09 + 0x07 + 0x01 + data)),
                    };
                }
                else
                {
                    int status = sw ? 1 : 0;

                    StatusSend = new P_Status_Send()
                    {
                        Head = Encoding.Default.GetBytes("SZ"),
                        ProtocalNum = 0x08,
                        Length = 0x07,
                        devId = new byte[] { 0x00, 0x01 },
                        data = (byte)status,
                        Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 0x08 + 0x07 + 1 + status)),
                    };
                }



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


        }

        public override byte[] GetGuidanceBuff(GuidancePositionInfo position)
        {
            byte[] result;
            var dev = _memory.GetDevice();
            var arp = new GisTool().ConvertAzimutInfo(new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt }, new GisTool.Position() { Lat = position.TargetY, Lng = position.TargetX, Altitude = position.TargetZ });

            //水平角  要到达的角度 * 100得到10进制数转换成16进制，高位前低位后
            var h = arp.Az + dev.RectifyAz;
            h = h > 360 ? h % 360 : h;
            int Hor = Convert.ToInt32((h) * 100);

            //int Hor = Convert.ToInt32(55 * 100);
            //俯仰角  当角度为负（平台前倾）时，高字节<<8+低字节=角度*100。当角度为正时，高字节<<8+低字节=36000-角度*100
            int pitch = (int)(360 - Math.Atan(position.TargetZ / arp.Dis) / Math.PI * 180) * 100;
            //int pitch = position.TargetY > 0 ? Convert.ToInt32(36000 - 38 * 100) : Convert.ToInt32(38 * 100);
            #region 转台协议解析
            P_HorizontalSet_Send HoriReceive = new P_HorizontalSet_Send()
            {
                Head = Encoding.Default.GetBytes("SZ"),
                ProtocalNum = 0x01,
                Length = 0x08,
                devId = new byte[] { 0x00, 0x01 },
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
                devId = new byte[] { 0x00, 0x01 },
                //data = new byte[] { (byte)(pitch >> 8), BitConverter.GetBytes((short)(pitch)).Reverse().ToArray()[1] },
                data = new byte[] { (byte)((pitch >> 8)), (byte)(pitch & 0xff) },
                Check = BitConverter.GetBytes((Encoding.UTF8.GetBytes("SZ")[0] + Encoding.UTF8.GetBytes("SZ")[1] + 3 + 8 + 1 + BitConverter.GetBytes((short)(pitch)).Reverse().ToArray()[0] + BitConverter.GetBytes((short)(pitch)).Reverse().ToArray()[1])),
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
            //return new byte[] { 0 };
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
