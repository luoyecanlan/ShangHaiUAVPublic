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

namespace AntiUAV.DevicePlugin.MonitorP13
{
    public class DeviceOptService : DeviceOptServiceBase
    {

        public DeviceOptService(ILogger<DeviceOptService> logger, IMemoryCache memory, GisTool tool)
        {
            _memory = memory;
            _logger = logger;
        }
        private readonly ILogger _logger;
        private uint index = 0;
        private readonly IMemoryCache _memory;
        private readonly GisTool _tool;
        public override int DeviceCategory => PluginConst.Category;

        /// <summary>
        /// 目址引导指令包
        /// </summary>
        /// <param name = "position" ></ param >
        /// < returns ></ returns >
        public override byte[] GetGuidanceBuff(GuidancePositionInfo position)
        {
            index++;
            uint order = 0;
            if (position.TargetId == "999999")
            {
                order = 3;
            }
            else { order =4; }
            var device = _memory.GetDevice();
            double az = device.RectifyAz;
            string el = device.RectifyEl;
            string[] els = el?.Split(",");
            double RealEl = 0;
            if (els?.Length > 1)
            {
                
                //Dictionary<double, double> elDic = new Dictionary<double, double>();
                //position.TargetZ = 420;
                for (int i = 0; i < els.Length; i++)
                {
                    string t = els[i].Substring(1, els[i].Length - 2);
                    double dis = Convert.ToDouble(t.Split(":")[0]);
                    double Eltmp = Convert.ToDouble(t.Split(":")[1]);
                    if (position.TargetZ > dis)
                    {
                        RealEl = Eltmp;
                    }

                }
            }
            else
            {
                _logger.LogWarning("俯仰值未按规定格式设定，请检查");
            }
            
           
            var devPos = new GisTool.Position() { Altitude = device.Alt, Lng = device.Lng, Lat = device.Lat };
            var per = new GisTool().Convert3DPositionAzimuthAndPitchInfo(new GisTool.Position() { Altitude = position.TargetZ, Lat = position.TargetY, Lng = position.TargetX }, devPos);

            P_MW_Follow_Guidance_Body pgdb = new P_MW_Follow_Guidance_Body()
            {
                EqSnum = 0,

                Order = order,
                Az = position.TargetX+az,
                El = 70 - position.TargetY+RealEl,
                //Dis = position.TargetZ,
                Time = DateTime.Now.ToLong()
            };

            P_MW_Follow_Guidance pgd = new P_MW_Follow_Guidance 
            {
                Head = 0x8A808988,
                End = 0x8B8A8089,
                ProtocalNum = 9002,
                Order = 4,
                Length = 20 + 36,
                Time = DateTime.Now.ToLong(),
                Body = pgdb
            };
            byte[] buff = pgd.ToBytes();
            var check = 0u;
            for (int i = 8; i < buff.Length - 8; i++)
            {
                check += buff[i];
            }
            pgd.Check = check;
            buff = pgd.ToBytes();

            P_MW_Guidance_Body pgdb1 = new P_MW_Guidance_Body()
            {
                EqSnum = 0,
                SysSnum = 0,

                Az = position.TargetX + az,
                El = 70 - position.TargetY + RealEl,
                Dis = position.TargetZ,
                Time = DateTime.Now.ToLong()
            };


            P_MW_Guidance pgd1 = new P_MW_Guidance
            {
                Head = 0x8A808988,
                End = 0x8B8A8089,
                ProtocalNum = 9002,
                Order = 3,
                Length = 20 + 68,
                Time = DateTime.Now.ToLong(),
                Body = pgdb1
            };
            byte[] buff1 = pgd1.ToBytes();
            var check1 = 0u;
            for (int i = 8; i < buff1.Length - 8; i++)
            {
                check1 += buff1[i];
            }
            pgd1.Check = check1;
            buff1 = pgd1.ToBytes();

            int len = buff.Length + pgd1.ToBytes().Length;
            byte[] result = new byte[len];
            Array.Copy(pgd1.ToBytes(), result, pgd1.ToBytes().Length);
            Array.Copy(buff, 0, result, pgd1.ToBytes().Length, buff.Length);
            //Console.WriteLine($"【{DateTime.Now}】开始引导，{pgdb1.Az},{pgdb1.El}、{pgdb1.Dis}");
            //Array.Copy(buff, result, buff.Length);
            //Array.Copy(buff1, 0, result, buff.Length, buff1.Length);
            //return result.ToBytes();
            //foreach (byte a in result)
            //{
            //    Console.Write(a.ToString("X2") + " ");

            //}
            // return result;
            //if (index % 2 == 0)
            //{
            //    return buff;
            //}
            //else
            {

                return buff1;

            }

        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct P_MW_Guidance
    {
        public uint Head;//0x8A808988
        public int ProtocalNum;
        public int Length;//20+bodylength
        public int Order;//4
        public long Time;
        public P_MW_Guidance_Body Body;
        public int Index;
        public uint Check;
        public uint End;//0x8B8A8089
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct P_MW_Guidance_Body
    {
        public uint EqSnum;
        public uint SysSnum;
        public long Time;
        public double Lng;
        public double Lat;
        public double Alt;
        public double Dis;
        public double Az;
        public double El;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserve;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct P_MW_Follow_Guidance
    {
        public uint Head;//0x8A808988
        public int ProtocalNum;
        public int Length;//20+bodylength
        public int Order;//4
        public long Time;
        public P_MW_Follow_Guidance_Body Body;
        public int Index;
        public uint Check;
        public uint End;//0x8B8A8089
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct P_MW_Follow_Guidance_Body
    {
        public uint EqSnum;
        //public uint SysSnum;
        public long Time;
        public uint Order;//1: 搜索并自动跟踪2：跟踪指定目标；3：释放4:仅搜索

        
        public double Az;
        public double El;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserve;//“1:搜索并自动跟踪”时表示搜索方式: 0--在当前视场里搜索
    }



}
