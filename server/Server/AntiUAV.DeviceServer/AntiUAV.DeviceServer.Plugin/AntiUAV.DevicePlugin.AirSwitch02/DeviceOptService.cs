using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using EasyNetQ.Logging;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.AirSwitch02
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public DeviceOptService(IMemoryCache memory, IPeerServer peer)
        {

            _memory = memory;
            Connect();
            //_peer = peer;
            //_log = log;
        }
        //        通道1开：55 01 12 00 00 00 01 69
        //通道1关：55 01 11 00 00 00 01 68

        //通道2开：55 01 12 00 00 00 02 6A
        //通道2关：55 01 11 00 00 00 02 69

        //通道3开：55 01 12 00 00 00 03 6B
        //通道3关：55 01 11 00 00 00 03 6A

        //通道4开：55 01 12 00 00 00 04 6C
        //通道4关：55 01 11 00 00 00 04 6B
        //            四通道全开设置55 01 13 00 00 00 0F 78
        //四通道全关设置55 01 13 00 00 00 00 69
        byte[] ReturnToBasenBytes = new byte[8] { 0x55, 0x01, 0x12, 0x00, 0x00, 0x00, 0x01, 0x69 };
        byte[] ForceLandingBytes = new byte[16] { 0x55, 0x01, 0x12, 0x00, 0x00, 0x00, 0x01, 0x69, 0x55, 0x01, 0x12, 0x00, 0x00, 0x00, 0x02, 0x6a };
        byte[] AllOffBytes = new byte[8] { 0x55, 0x01, 0x13, 0x00, 0x00, 0x00, 0x00, 0x69 };
        //ILog _log;
        public override int DeviceCategory => PluginConst.Category;

        private readonly IMemoryCache _memory;
        Socket tcpClient = null;
        public void Connect()
        {
            var dev=_memory.GetDevice();
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse("192.168.0.110");
            EndPoint point = new IPEndPoint(ipaddress, 6000);
            try
            {
                tcpClient.Connect(point);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }
        public override byte[] GetAttackBuff(string json, bool sw)
        {
            //Connect();
            var dev = _memory.GetDevice();
            var data = JsonConvert.DeserializeObject<Rootobject>(json);
            if (data == null)
            {
                tcpClient.Send(AllOffBytes);
                return AllOffBytes;
            }
            if (data.hitFreq == 2)
            {
                tcpClient.Send(ForceLandingBytes);
                return ForceLandingBytes;

            }
            if (data.hitFreq == 3)
            {
                tcpClient.Send(ReturnToBasenBytes);
                return ReturnToBasenBytes;

            }


            return AllOffBytes;
        }

        public class Rootobject
        {
            public string Id { get; set; }
            public string TargetId { get; set; }
            public int FromDeviceId { get; set; }
            public int ToDeviceId { get; set; }
            public string ToAddressIp { get; set; }
            public int ToAddressPort { get; set; }
            public int RType { get; set; }
            public DateTime UpdateTime { get; set; }
            public int hitFreq { get; set; }
        }


    }
}
