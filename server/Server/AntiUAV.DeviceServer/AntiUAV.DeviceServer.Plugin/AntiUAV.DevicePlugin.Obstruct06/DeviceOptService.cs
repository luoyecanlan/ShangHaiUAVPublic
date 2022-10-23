using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using EasyNetQ.Logging;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct06
{
    public class DeviceOptService : DeviceOptServiceBase
    {
      
        public DeviceOptService(IMemoryCache memory)
        {
            _memory = memory;
           
        }
        //ILog _log;
        byte[] queryDeviceStatus= new byte[9] { 0x7E, 0x01, 0x00, 0x11, 0xFF, 0x00, 0xD7, 0x6E, 0x7F };
        byte[] queryDeviceParam = new byte[9] { 0x7E, 0x01, 0x00, 0x12, 0xFF, 0x00, 0xAD, 0x84, 0x7F };
        byte[] queryDeviceLimit = new byte[9] { 0x7E, 0x01, 0x00, 0x13, 0xFF, 0x00, 0x9D, 0xB3, 0x7F };
        //ILog _log;
        public override int DeviceCategory => PluginConst.Category;

        private readonly IMemoryCache _memory;
        
        public override byte[] GetAttackBuff(string json, bool sw)
        {
            //Connect();
            var dev = _memory.GetDevice();
            ObstructSetParamSend obstruct=new ObstructSetParamSend();
            obstruct.Data = new SetParamSendData()
            {

            };
            GlobleObj.tcpClient.Send(obstruct.ToBytes<ObstructSetParamSend>());

            return queryDeviceStatus;
        }



    }
    //7E 01 00 20 66 FD DD
    //05 00 5D 16 DA 16 00 00 00 00 00 00 00 00 00 00 00 00 2F 00
    //0A 00 5D 16 DA 16 00 00 00 00 00 00 00 00 00 00 00 00 2F 00
    //0F 00 60 09 B5 09 00 00 00 00 00 00 00 00 00 00 00 00 32 00
    //14 00 84 03 A2 03 00 00 00 00 00 00 00 00 00 00 00 00 32 01
    //19 00 0E 06 54 06 00 00 00 00 00 00 00 00 00 00 00 00 32 01
    //XX 00 0E 06 54 06 00 00 00 00 00 00 00 00 00 00 00 00 32 01
    //DC D7 7F

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObstructSetParamSend
    {
        public byte Head;//0x7E
        public short DeviceAddress;//0x2329
        public byte Order;//20+bodylength
        public byte OrderCategory;
        public byte DataLength;
        public SetParamSendData Data;
        public short Check;//CRC
        public byte End;//0x7F
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetParamSendData
    {
        public short DeviceControl;
      
        public short ModelAddress1;
        public short Channel1Start1;
        public short Channel1End1;
        public short Channel2Start1;
        public short Channel2End1;
        public short Channel3Start1;
        public short Channel3End1;
        public short Channel4Start1;
        public short Channel4End1;
        public byte OutputPower1;
        public byte Switch1;
        public short ModelAddress2;
        public short Channel1Start2;
        public short Channel1End2;
        public short Channel2Start2;
        public short Channel2End2;
        public short Channel3Start2;
        public short Channel3End2;
        public short Channel4Start2;
        public short Channel4End2;
        public byte OutputPower2;
        public byte Switch2;
        public short ModelAddress3;
        public short Channel1Start3;
        public short Channel1End3;
        public short Channel2Start3;
        public short Channel2End3;
        public short Channel3Start3;
        public short Channel3End3;
        public short Channel4Start3;
        public short Channel4End3;
        public byte OutputPower3;
        public byte Switch3;
        public short ModelAddress4;
        public short Channel1Start4;
        public short Channel1End4;
        public short Channel2Start4;
        public short Channel2End4;
        public short Channel3Start4;
        public short Channel3End4;
        public short Channel4Start4;
        public short Channel4End4;
        public byte OutputPower4;
        public byte Switch4;
        public short ModelAddress5;
        public short Channel1Start5;
        public short Channel1End5;
        public short Channel2Start5;
        public short Channel2End5;
        public short Channel3Start5;
        public short Channel3End5;
        public short Channel4Start5;
        public short Channel4End5;
        public byte OutputPower5;
        public byte Switch5;
        public short ModelAddress6;
        public short Channel1Start6;
        public short Channel1End6;
        public short Channel2Start6;
        public short Channel2End6;
        public short Channel3Start6;
        public short Channel3End6;
        public short Channel4Start6;
        public short Channel4End6;
        public byte OutputPower6;
        public byte Switch6;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObstructQueryDeviceStatusReceive
    {
        public byte Head;//0x7E
        public short DeviceAddress;//0x0100
        public byte Order;//0x11
        public byte OrderCategory;
        public byte DataLength;
        public QueryDeviceStatusData Data;
        public short Check;//CRC
        public byte End;//0x7F
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QueryDeviceStatusData
    {
        public short Reserve;
        public short DeviceAlert;//BIT0：门禁告警BIT1：用户自定义告警1BIT2：用户自定义告警2BIT3：用户自定义告警3BIT4：KJ_CTL状态BIT5~BIT15：保留

        public short ModelAddress1;
        public byte OutputPower1;
        public byte Temperature1;
        public byte Voltage1;
        public byte Current1;
        public byte Wave1;
        public byte Status1; //BIT0：功放开关BIT1：温度超门限告警BIT2：电压超门限告警BIT3：电流超门限告警BIT4：驻波比超门限告警BIT5：输出功率超门限告警BIT6~BIT7：保留

        public short ModelAddress2;
        public byte OutputPower2;
        public byte Temperature2;
        public byte Voltage2;
        public byte Current2;
        public byte Wave2;
        public byte Status2;
        public short ModelAddress3;
        public byte OutputPower3;
        public byte Temperature3;
        public byte Voltage3;
        public byte Current3;
        public byte Wave3;
        public byte Status3;
        public short ModelAddress4;
        public byte OutputPower4;
        public byte Temperature4;
        public byte Voltage4;
        public byte Current4;
        public byte Wave4;
        public byte Status4;
        public short ModelAddress5;
        public byte OutputPower5;
        public byte Temperature5;
        public byte Voltage5;
        public byte Current5;
        public byte Wave5;
        public byte Status5;
        public short ModelAddress6;
        public byte OutputPower6;
        public byte Temperature6;
        public byte Voltage6;
        public byte Current6;
        public byte Wave6;
        public byte Status6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObstructQueryDeviceParamReceive
    {
        public byte Head;//0x7E
        public short DeviceAddress;//0x0100
        public byte Order;//0x12
        public byte OrderCategory;
        public byte DataLength;
        public QueryDeviceParamData Data;
        public short Check;//CRC
        public byte End;//0x7F
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QueryDeviceParamData
    {
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObstructQueryDeviceLimitReceive
    {
        public byte Head;//0x7E
        public short DeviceAddress;//0x2329
        public byte Order;//0x13
        public byte OrderCategory;
        public byte DataLength;
        public QueryDeviceLimitData Data;
        public short Check;//CRC
        public byte End;//0x7F
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QueryDeviceLimitData
    {
    }



}
