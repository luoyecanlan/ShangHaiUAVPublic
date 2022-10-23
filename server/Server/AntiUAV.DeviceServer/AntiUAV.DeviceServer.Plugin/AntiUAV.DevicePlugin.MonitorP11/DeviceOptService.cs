using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using System.Runtime.InteropServices;
using EasyNetQ.Logging;
using System.Diagnostics;
using AntiUAV.Bussiness;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AntiUAV.DevicePlugin.MonitorP11
{
    public class DeviceOptService : DeviceOptServiceBase
    {

        public DeviceOptService(ILogger<DeviceOptService> logger, IMemoryCache memory)
        {
            _memory = memory;
            _logger = logger;
        }
        private readonly ILogger _logger;
        private uint index = 1;
        private readonly IMemoryCache _memory;
        
        public override int DeviceCategory => PluginConst.Category;

        /// <summary>
        /// 位置修正纠偏设置包（0x201）
        /// </summary>
        /// <param name="az">方位</param>
        /// <param name="el">俯仰</param>
        /// <returns></returns>
        public override byte[] GetRectifyBuff(double az, double el)
        {
            var rectify=_memory.GetDevRectify();
            az = rectify.Az;
            el = rectify.El;
            var dev = _memory.GetDevice();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            byte[] timeBuff = BitConverter.GetBytes((long)(DateTime.Now - startTime).TotalMilliseconds);
            Array.Reverse(timeBuff);
            byte[] targetAz = BitConverter.GetBytes(Convert.ToSingle(az));
            Array.Reverse(targetAz);
            byte[] targetEz = BitConverter.GetBytes(Convert.ToSingle(el));
            Array.Reverse(targetEz);
            byte[] targetLng = BitConverter.GetBytes(Convert.ToSingle(dev.Lng));
            Array.Reverse(targetLng);
            byte[] targetLat = BitConverter.GetBytes(Convert.ToSingle(dev.Lat));
            Array.Reverse(targetLat);
            byte[] targetAlt = BitConverter.GetBytes(Convert.ToSingle(dev.Alt));
            Array.Reverse(targetAlt);
            M_TargetRt targetRequest = new M_TargetRt
            {
                Head = 0x7A707978,
                ProtocalNum = 0x8b130000,
                Length = 0x32000000,
                Order = 0x01020000,
                Time = timeBuff,

                Az = BitConverter.ToSingle(targetAz),
                El = BitConverter.ToSingle(targetEz),
                Lng = BitConverter.ToSingle(targetLng),
                Lat = BitConverter.ToSingle(targetLat),
                Alt = BitConverter.ToSingle(targetAlt),

                Index = index++,
                End = 0x7B7A7079
            };

            byte[] result = ConvertExtension.ToBytes<M_TargetRt>(targetRequest);
            uint cacl = 0;
            for (int i = 8; i < result.Length - 8; i++)
            {
                cacl += result[i];
            }
            byte[] cacL = BitConverter.GetBytes(Convert.ToUInt32(cacl));
            Array.Reverse(cacL);
            targetRequest.Check = BitConverter.ToUInt32(cacL);
            result = ConvertExtension.ToBytes<M_TargetRt>(targetRequest);
#if DEBUG
            result.OutPutByte("位置纠偏设置包（0x201）");
#endif

            return result;
        }

        /// <summary>
        /// 跟踪操作指令包（0x202）
        /// </summary>
        /// <param name="json"></param>
        /// <param name="sw">true 开始  false 停止</param>
        /// <returns></returns>
        public override byte[] GetMonitorBuff(string json, bool sw)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            byte[] timeBuff = BitConverter.GetBytes((long)(DateTime.Now - startTime).TotalMilliseconds);
            Array.Reverse(timeBuff);
            var obj = JsonConvert.DeserializeObject<DeviceFollow>(json);
            byte[] operateCode = BitConverter.GetBytes(Convert.ToInt16((int)obj.operateCode));
            Array.Reverse(operateCode);
            M_TargetMt targetRequest = new M_TargetMt
            {
                Head = 0x7A707978,
                ProtocalNum = 0x8b130000,
                Length = 0x1E000000,
                Order = 0x02020000,
                Time = timeBuff,

                OperateCode = BitConverter.ToInt16(operateCode),

                Index = index++,
                End = 0x7B7A7079
            };
            byte[] result = ConvertExtension.ToBytes<M_TargetMt>(targetRequest);
            uint cacl = 0;
            for (int i = 8; i < result.Length - 8; i++)
            {
                cacl += result[i];
            }
            byte[] cacL = BitConverter.GetBytes(Convert.ToUInt32(cacl));
            Array.Reverse(cacL);
            targetRequest.Check = BitConverter.ToUInt32(cacL);
            result = ConvertExtension.ToBytes<M_TargetMt>(targetRequest);
#if DEBUG
            result.OutPutByte("跟踪操作指令包（0x202）");
#endif
            return result;
        }

        /// <summary>
        /// 设备操作指令包（0x203）
        /// </summary>
        /// <param name="oper">1-水平 / 2-俯仰 / 3-归零（转台0位） / 4-指北（修正后0位） / 5-视场角 / 6-焦距</param>
        /// <param name="speed">转台速度 / 视场角增减速度 / 焦距增减速度</param>
        /// <param name="status">true 开始 false 关闭</param>
        /// <returns></returns>
        public override byte[] GetDeviceOpBuff(short operationItem, int speed, short operationCode)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            byte[] timeBuff = BitConverter.GetBytes((long)(DateTime.Now - startTime).TotalMilliseconds);
            Array.Reverse(timeBuff);
            byte[] operateCode = BitConverter.GetBytes(Convert.ToInt16(operationCode));
            Array.Reverse(operateCode);
            byte[] _speed = BitConverter.GetBytes(speed);
            Array.Reverse(_speed);
            byte[] operateItem = BitConverter.GetBytes(Convert.ToInt16(operationItem));
            Array.Reverse(operateItem);
            M_TargetFo targetRequest = new M_TargetFo
            {
                Head = 0x7A707978,
                ProtocalNum = 0x8b130000,
                Length = 0x1e000000,
                Order = 0x03020000,
                Time = timeBuff,
                OperateItem = BitConverter.ToInt16(operateItem),
                Speed = BitConverter.ToInt32(_speed),
                OperateCode = BitConverter.ToInt16(operateCode),
                Index = index++,
                End = 0x7B7A7079
            };
            byte[] result = ConvertExtension.ToBytes<M_TargetFo>(targetRequest);
            uint cacl = 0;
            for (int i = 8; i < result.Length - 8; i++)
            {
                cacl += result[i];
            }
            byte[] cacL = BitConverter.GetBytes(Convert.ToUInt32(cacl));
            Array.Reverse(cacL);
            targetRequest.Check = BitConverter.ToUInt32(cacL);
            result = ConvertExtension.ToBytes<M_TargetFo>(targetRequest);
#if DEBUG
            result.OutPutByte("设备操作指令包（0x203）");
#endif
            return result;
        }

        /// <summary>
        /// 目址引导指令包（0x204）
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override byte[] GetGuidanceBuff(GuidancePositionInfo position)
        {
            var dev = _memory.GetDevice();

            var rectify = _memory.GetDevRectify();
            double? az = rectify?.Az??0;
            double? el = rectify?.El??0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            byte[] head = { 0x78, 0x79, 0x70, 0x7A };
            //水平角  要到达的角度 * 100得到10进制数转换成16进制，高位前低位后
            int Hor = Convert.ToInt32(position.TargetX * 100);
            //俯仰角  当角度为负（平台前倾）时，高字节<<8+低字节=角度*100。当角度为正时，高字节<<8+低字节=36000-角度*100
            int pitch = position.TargetY > 0 ? Convert.ToInt32(36000 - position.TargetY * 100) : Convert.ToInt32(position.TargetY * 100);
            byte[] timeBuff = BitConverter.GetBytes((long)(DateTime.Now - startTime).TotalMilliseconds);
            Array.Reverse(timeBuff);
            byte[] timeBuff1 = BitConverter.GetBytes((long)(position.TrackTime - startTime).TotalMilliseconds);
            Array.Reverse(timeBuff1);
            byte[] coordinateType = BitConverter.GetBytes(Convert.ToInt16(1));
            Array.Reverse(coordinateType);
            byte[] targetId = new byte[20];
            var temp = Encoding.UTF8.GetBytes(position.TargetId);
            for (int i = 0; i < 20 && temp.Length > i ; i++)
            {
                targetId[i] = temp[i];
            }
            Array.Reverse(targetId);
            byte[] TargetXBuff = BitConverter.GetBytes(Convert.ToSingle(position.TargetX+ az));
            Array.Reverse(TargetXBuff);
            byte[] TargetYBuff = BitConverter.GetBytes(Convert.ToSingle(position.TargetY+ el + 0.3));
            Array.Reverse(TargetYBuff);
            byte[] TargetZBuff = BitConverter.GetBytes(Convert.ToSingle(position.TargetZ));
            Array.Reverse(TargetZBuff);
            M_TargetGd m_TargetGd = new M_TargetGd()
            {
                Head = 0x7A707978,
                ProtocalNum = 0x8b130000,
                Length = 0x46000000,
                Order = 0x04020000,
                Time = timeBuff,
                ID = targetId,
                CoordinateType = BitConverter.ToInt16(coordinateType),
                TargetX = BitConverter.ToSingle(TargetXBuff),
                TargetY = BitConverter.ToSingle(TargetYBuff),
                TargetZ = BitConverter.ToSingle(TargetZBuff),
                Time1 = timeBuff1, // 相差毫秒数
                SearchTip = 0x0100,
                Index = index++,
                End = 0x7B7A7079
            };
            byte[] result = ConvertExtension.ToBytes<M_TargetGd>(m_TargetGd);
            uint cacl = 0;
            for (int i = 8; i < result.Length - 8; i++)
            {
                cacl += result[i];
            }

            byte[] cacL = BitConverter.GetBytes(Convert.ToUInt32(cacl));
            Array.Reverse(cacL);
            m_TargetGd.Check = BitConverter.ToUInt32(cacL);
            result = ConvertExtension.ToBytes<M_TargetGd>(m_TargetGd);
#if DEBUG
            //result.OutPutByte("目址引导指令包（0x204）");
#endif
            return result;
        }


    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct M_TargetGd
    {
        public uint Head;//0x7A707978
        public uint ProtocalNum;//0x2329
        public uint Length;//20+bodylength
        public uint Order;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Time;
        //内容30byte
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ID;
        public short CoordinateType;
        public float TargetX;
        public float TargetY;
        public float TargetZ;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Time1;
        public short SearchTip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Reserve;

        public uint Index;
        public uint Check;
        public int End;//0x7B7A7079
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct M_TargetRt
    {
        public uint Head;//0x7A707978
        public uint ProtocalNum;//0x2329
        public uint Length;//20+bodylength
        public uint Order;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Time;
        //内容30byte
        public float Az;//方位
        public float El;//俯仰
        public float Lng;//经度
        public float Lat;//纬度
        public float Alt;//海拔
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] Reserve;

        public uint Index;
        public uint Check;
        public uint End;//0x7B7A7079
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct M_TargetMt
    {
        public uint Head;//0x7A707978
        public uint ProtocalNum;//0x2329
        public uint Length;//20+bodylength ====/ 20 + 50
        public uint Order;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Time;

        public short OperateCode;//操作码

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserve;
        public uint Index;
        public uint Check;
        public uint End;//0x7B7A7079


    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct M_TargetFo
    {
        public uint Head;//0x7A707978
        public uint ProtocalNum;//0x2329
        public uint Length;//20+bodylength ====/ 20 + 50
        public uint Order;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Time;

        public short OperateItem;//操作项
        public int Speed;//速度
        public short OperateCode;//操作码

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reserve;
        public uint Index;
        public uint Check;
        public uint End;//0x7B7A7079


    }



}
