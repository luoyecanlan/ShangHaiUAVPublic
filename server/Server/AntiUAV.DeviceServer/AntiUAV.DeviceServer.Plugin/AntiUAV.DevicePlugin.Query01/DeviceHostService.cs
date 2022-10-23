using AntiUAV.DevicePlugin.Query01.Cmd;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Query01
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {


        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
            _serialPort = new SerialPort();
        }
        //public DeviceHostService(ILogger<DeviceHostService> logger, IMemoryCache memory)
        //{
        //    _logger = logger;
        //    _memory = memory;
        //    _client = new RestClient();
        //}

        public override int DeviceCategory => PluginConst.Category;
        private SerialPort _serialPort;
        private Thread _listion;
        private byte[] _receiveBuffer;
        public void ConnectSerial()
        {

            _serialPort.PortName = "COM3";
            _serialPort.Parity = Parity.None;
            _serialPort.BaudRate = 115200;
            _serialPort.Open();
            _serialPort.DataReceived += _serialPort_DataReceived;
        }

        public byte[] SlipBuff(byte[] recive_buffer)
        {
            List<byte> byteList = recive_buffer.ToList();

            for (int i = 0; i < byteList.Count; i++)//按照协议slip协议转义字符
            {
                if (byteList[i] == 0xdb && byteList[i + 1] == 0xdc)
                {
                    byteList.Remove(byteList[i + 1]);
                    byteList[i] = 0xc0;
                }
                if (byteList[i] == 0xdb && byteList[i + 1] == 0xdd)
                {
                    byteList.Remove(byteList[i + 1]);
                    byteList[i] = 0xdb;
                }
            }
            byte[] buff = new byte[byteList.Count];
            //byte[] bu = (byte[])byteList;
            for (int i = 0; i < byteList.Count; i++)
            {
                buff[i] = byteList[i];
            }
            return buff;
        }
        byte[] ask03_No3 = { 0xc0, 0x02, 0x01, 0x00, 0x19, 0x00, 0x00, 0x00, 0x00, 0x5f, 0x19, 0x40, 0xf2, 0x00, 0x08, 0x01, 0x04, 0x0a, 0x00, 0x04, 0x03, 0x0b, 0xf2, 0x9b, 0xa6, 0xfd, 0xc0 };
        byte[] ask03_No2 = { 0xc0, 0x02, 0x01, 0x00, 0x19, 0x00, 0x00, 0x00, 0x00, 0x5f, 0x19, 0x40, 0xed, 0x00, 0x07, 0x01, 0x04, 0x0a, 0x00, 0x04, 0x03, 0x0b, 0xf2, 0x9a, 0xbd, 0xd9, 0xc0 };
        byte[] ask03_No1 = { 0xc0, 0x02, 0x01, 0x00, 0x19, 0x00, 0x00, 0x00, 0x00, 0x5f, 0x19, 0x40, 0xe9, 0x00, 0x06, 0x01, 0x04, 0x0a, 0x00, 0x04, 0x03, 0x0b, 0xf2, 0x99, 0xb6, 0x39, 0xc0 };
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var dev = _memory.GetDevice();
            SerialPort sp = (SerialPort)sender;
            byte[] recive_buffer = new byte[sp.BytesToRead];
            //var sss = sp.ReadExisting();
            sp.Read(recive_buffer, 0, recive_buffer.Length);
            //recive_buffer.Where(x => x == 0xdb);

            byte[] buff = SlipBuff(recive_buffer);

            try
            {

                if (buff.Length == 44 && buff[20] == 0x04 && buff[21] == 0x0a)///////收到的03业务类型数据
                {
                    _peer.SendAsync(buff, dev.Lip, dev.Lport);
                }
                if (buff.Length == 34 || buff.Length == 40 || buff.Length == 46 || buff.Length == 52 || buff.Length == 58)///////收到的02业务类型数据
                {
                    int trackNum = buff[23];

                    byte[] trackBuff;

                    while (trackNum > 0)
                    {
                        trackBuff = buff.Skip((trackNum - 1) * 6 + 25).Take(6).ToArray();
                        var temp = trackBuff.ToStuct<Ask_Trigger02_Content_receive>();
                        byte[] buffid = new byte[4];
                        Array.Reverse(temp.TargetId);
                        Array.Copy(temp.TargetId, 0, buffid, 0, 3);
                        int id = BitConverter.ToInt32(buffid);
                        ///////////////有选址能力的查询，即把这个目标编号用03询问的方式下发查询位置目标信息；
                        ///////////////否则不查询，00表示无选址能力

                        if ((temp.IsWithAdress & 3) == 0) break;
                        else
                        {
                            switch (id)
                            {
                                case 783001: Send(ask03_No1); break;
                                case 783002: Send(ask03_No2); break;
                                case 783003: Send(ask03_No3); break;

                            }

                        }
                        // TargetList.Add(temp.TargetId);


                        trackNum--;
                    }
                }

                //_ = Route?.ExcutePipeLineAsync(GetContent(buff, ep));
            }
            catch (SocketException sex)
            {
                //socket 异常处理
                //Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "udp server recice data socket error.", sex));
            }
            catch (Exception ex)
            {
                //Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "udp server recice data error.", ex));
            }
        }

        public sealed override void Stop()
        {
            try
            {
                _listion?.Abort();//会引发全局监视异常，故自己捕获不处理
            }
            catch { }
            _serialPort?.Close();
            _serialPort?.Dispose();
            //Info.State = PeerServerState.Stop;
        }
        public int Send(byte[] buff)
        {
            try
            {
                if (buff != null)
                {
                    _serialPort?.Write(buff, 0, buff.Length);
                }
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"udp send data({buff?.Length}) fail.");
                return -3;
            }
        }
        public override void Start()
        {
            var dev = _memory.GetDevice();
            if (dev == null || _peer == null)
                _logger.LogError("devServ udp listion start fail.(no dev info or no serv)");
            else
            {
                _peer.UseCustomServerInfo(new PeerServerInfo($"deviceServer:{dev.Id}", dev.Lip, dev.Lport));
                _peer?.Star();
                _logger.LogInformation($"devServ udp listion start.(devId:{dev.Id},ip:{dev.Lip},port:{dev.Lport})");
            }




            //问答机02命令
            byte[] SendBuff = { 0xc0, 0x02, 0x01, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x5f, 0x19, 0x03, 0xfa, 0x00, 0x0c, 0x01, 0x04, 0x0a, 0x00, 0x01, 0x02, 0xda, 0x15, 0xc0 };
            Task.Run(() =>
            {
                while (true)
                {
                    if (!_serialPort.IsOpen)
                    {
                        ConnectSerial();
                    }
                    Send(SendBuff);
                    Thread.Sleep(5000);
                }
            });

            //var position = new ASK_Whole_Struct()
            //{
            //    Head = 0xc0,
            //    Version = new byte[] { 0x02, 0x01 },
            //    Length = new byte[] { 0x00, 0x1d },
            //    TimePiont = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000),
            //    SerialNumber = new byte[] { 0x00, 0x01 },
            //    AskType = 0x00,
            //    position = new ASK_Position()
            //    {
            //        BussinessId = new byte[] { 0x04, 0x07 },
            //        BussinessLength = new byte[] { 0x00, 0x08 },
            //        DeviceLongtude = Convert.ToSingle(dev.Lng),
            //        DeviceLatitude = Convert.ToSingle(dev.Lat)

            //    },
            //    Check = new byte[] {0x00,0x00},
            //    End=0xc0
            //};
            //byte[] PositionBuff = position.ToBytes<ASK_Whole_Struct>();
            //SlipBuff(PositionBuff);

        }


    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    /// <summary>
    /// 总长度19+N（N为内容长度
    /// </summary>
    public struct ASK_Trigger02_Whole_Send
    {

        public byte Head;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Length;

        public long TimePiont;//时间戳
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] SerialNumber;
        public byte AskType;

        public ASK_Trigger02_send trigger;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Check;
        public byte End;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASK_Trigger02_send
    {
        /// /////////////////////消息内容 /////////////////////
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessLength;

        public byte type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] TargetId;
        /// /////////////////////消息内容/////////////////////
    }
    /// <summary>
    /// 总长度19+N（N为内容长度
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASK_Trigger02_Whole_Struct
    {

        public byte Head;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Length;

        public long TimePiont;//时间戳
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] SerialNumber;
        public byte AskType;

        public ASK_Trigger02_receive trigger;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Check;
        public byte End;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASK_Trigger02_receive
    {
        /// /////////////////////消息内容 /////////////////////
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessId1;
        public byte ResponseId;
        public Ask_Trigger02_Content_receive[] content;
        /// /////////////////////消息内容/////////////////////
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ask_Trigger02_Content_receive
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] TargetId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] distance;
        public byte IsWithAdress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASK_Position
    {
        /// /////////////////////消息内容 /////////////////////
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BussinessLength;

        public float DeviceLongtude;

        public float DeviceLatitude;

        /// /////////////////////消息内容/////////////////////
    }

}
