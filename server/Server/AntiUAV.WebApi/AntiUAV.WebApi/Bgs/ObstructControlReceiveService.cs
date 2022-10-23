using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Bgs
{
    /// <summary>
    /// 接收公安发出的干扰控制指令
    /// </summary>
    public class ObstructControlReceiveService : BackgroundService
    {
        private readonly IDeviceService _service;
        private readonly ITargetService _target;
        private readonly ILogger _logger;       
        private readonly IHubContext<SignalRHub> _hub;

        private readonly int _reportPort;
        private readonly string _reportIp;
        private readonly string _ZQCDeviceId;
        private readonly string _ZQCPassword;
        private readonly string _DSNDeviceId;
        private readonly string _DSNPassword;
        private readonly string _ZYDeviceId;
        private readonly string _ZYPassword;

        private readonly string _serverIp;
        private readonly int _localPort;



        TcpClient client_socket =null;
        IPEndPoint ipEndpoint = null;
        IPEndPoint localEndpoint = null;
        public ObstructControlReceiveService(ILogger<ObstructControlReceiveService> logger, IDeviceService service, ITargetService target, ReportServiceConfig config, IHubContext<SignalRHub> hub)
        {
            _target = target;
            _service = service;
            _logger = logger;
            _reportPort = config.ReportPort;
            _reportIp = config.ReportIp;
            _ZQCDeviceId = config.ZQCDeviceId;
            _ZQCPassword = config.ZQCPassword;
            _DSNDeviceId = config.DSNDeviceId;
            _DSNPassword = config.DSNPassword;
            _ZYDeviceId = config.ZYDeviceId;
            _ZYPassword = config.ZYPassword;
            _serverIp = config.ServerIp;
            _localPort = config.LocalPort;
            _hub = hub;

            
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    
                    
                    Connect();
                    

                }
                catch (Exception ex)
                {
                    _logger.LogError($"市管网控制Socket连接异常:{ex.Message}");
                }
                //while (!stoppingToken.IsCancellationRequested)
                Thread th_ReceiveData = new Thread(Receive);
                th_ReceiveData.IsBackground = true;
                th_ReceiveData.Start();
              
            });
        }
        private void Connect()
        {
            client_socket = new TcpClient();
            localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _localPort);
            //client_socket.Client.Bind(localEndpoint);
            IPAddress ipAdress = IPAddress.Parse(_reportIp);
            //网络端点：为待请求连接的IP地址和端口号          
            ipEndpoint = new IPEndPoint(ipAdress, _reportPort);
            client_socket.Connect(ipEndpoint);
            login();

            Thread th_ReceiveData = new Thread(ListenConnectStatus);
            th_ReceiveData.IsBackground = true;
            th_ReceiveData.Start();
        }
        private void ListenConnectStatus()
        {
           
            while (true)
            {
                try
                {
                    if (!client_socket.Connected)
                    {
                        client_socket.Connect(ipEndpoint);
                        login();
                        _logger.LogError($"市管网tcp连接异常:5s后尝试重新连接");
                    }
                    //else
                       
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {

                    _logger.LogError($"市管网tcp连接异常:{ex.Message}");
                }
                

            }
        }
        private void login()
        {
            var loginStruct = new LoginSendStruct()
            {
                Head = 1610552320,
                Password = Encoding.ASCII.GetBytes(_ZYPassword),
                DeviceId=Encoding.ASCII.GetBytes(_ZYDeviceId)
            };
            var bytes = StructToBytes(loginStruct);
            Send(bytes);

        }
        private DateTime time = DateTime.Now;
        //hit/{tgid}/{hitId}/{ hitFreq}
        private void OperateObstruts(string sn,bool isOn)
        {
            RestClient _client = new RestClient();
            //_client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证
            string Url = _serverIp+":8099/api/";
            RestRequest request = null;
            var _updatas = new List<TargetPosition>();
            var work = Task.Run(async () =>
            {
                _updatas = (await _target.GetLastUpdateTargetsPositionAsync(4, time)).ToList();
                
                var target = _updatas?.Where(f => f.UAVSn == sn).FirstOrDefault();
                var allShips = await _service?.GetRelationships();
                var _upTargets = _updatas?.Select(up =>
                {


                    var mode = new TargetModel()
                    {
                        Id = up.Id,
                        DeviceId = up.DeviceId,
                        CoordinateType = up.CoordinateType,
                        Lat = up.Lat,
                        Lng = up.Lng,
                        Alt = up.Alt,
                        Vt = up.Vt,
                        Vr = up.Vr,
                        Category = up.Category,
                        Mode = up.Mode,

                        TrackTime = up.TrackTime,
                        AppLat = up.AppLat,
                        AppLng = up.AppLng,
                        HomeLat = up.HomeLat,
                        HomeLng = up.HomeLng,
                        UAVSn = up.UAVSn,
                        UAVType = up.UAVType,
                        BeginAt = up.BeginAt,
                        AppAddr = up.AppAddr,
                        MaxHeight = up.MaxHeight
                    };
                    if (allShips?.Count() > 0)
                    {
                        mode.MonitorRelationShip = allShips?.FirstOrDefault(
                            ship => ship?.RType == RelationshipsType.MonitorGd && ship?.TargetId == up.Id)?.Id;
                        mode.TranspondRelationShip = allShips?.FirstOrDefault(
                            ship => ship?.RType == RelationshipsType.PositionTurn && ship?.TargetId == up.Id)?.Id;
                        mode.HitRelationShip = allShips?.FirstOrDefault(
                            ship => ship?.RType == RelationshipsType.AttackGd && ship?.TargetId == up.Id)?.Id;
                        mode.TickRelationShip = allShips?.FirstOrDefault(
                            ship => ship?.RType == RelationshipsType.DecoyGd && ship?.TargetId == up.Id)?.Id;
                    }
                    return mode;
                });
                var id = target?.Id;
                var hitId = 4;
                var hitFreq = 0;

                if (isOn)
                {
                    Url += $"hit/{id}/{hitId}/{hitFreq}";
                    request = new RestRequest(Url, Method.Post);
                }
                else
                {
                    var rid = _upTargets.Where(f => f.HitRelationShip != null).FirstOrDefault()?.HitRelationShip;
                    Url += $"hit/{rid}/{hitId}";
                    request = new RestRequest(Url, Method.Delete);
                }

                //request.AddJsonBody(requestLogin);
                request.AddHeader("Content-Type", "application/json");
                var response = _client?.ExecuteAsync(request);
            });
            
            
            
                  
        }
        private readonly int _receiveBufferSize = 1024 * 1024 * 2;
        private byte[] _receiveBuffer;
        //// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
        public FreqSwitchReceiveStruct BytesToStuct(byte[] bytes)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf<FreqSwitchReceiveStruct>();
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return default;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            FreqSwitchReceiveStruct obj = Marshal.PtrToStructure<FreqSwitchReceiveStruct>(structPtr);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }
        private void Send(byte[] bytes)
        {
            //string rl = Console.ReadLine();
            //发送消息到服务端
            client_socket.Client.Send(bytes);
        }

        private void Receive()
        {
            _receiveBuffer = new byte[_receiveBufferSize];
            while (client_socket.Client.Connected)
            {
                byte[] buff1 = null;
                try
                {
                    //_send = _tcp.AcceptTcpClient();
                    //client_socket.Client.Receive(buff1);
                    var netStream = client_socket?.GetStream();//获取用于读取和写入的流对象
                    int receiveLength;
                    //lock (netStream)
                    {
                        receiveLength = netStream.Read(_receiveBuffer, 0, _receiveBufferSize);
                    }
                    if (receiveLength > 0)
                    {
                        buff1 = new byte[receiveLength];
                        Array.Copy(_receiveBuffer, buff1, receiveLength);
                        if (receiveLength == 52)
                        {
                            var freqSwitchReceiveStruct = ControlReceive(buff1);
                            var sn = Encoding.ASCII.GetString(freqSwitchReceiveStruct.Id).Split('*')[0];
                            var freq= (freqSwitchReceiveStruct.Freq[0]);
                            if (freq != 0)//开干扰
                            {
                         
                                OperateObstruts(sn,true);
                            }
                            else//关干扰
                            {
                                OperateObstruts(sn, false);
                            }
                            
                        }
                        //return retBytes;
                    }                    
                    
                }
                catch (SocketException ex)
                {
                    //socket 异常处理
                    _logger.LogError($"市管网控制Socket接收异常:{ex.Message}");
                }
                //Thread.Sleep(100);
            }
           
            
        }
        private FreqSwitchReceiveStruct ControlReceive(byte[] bytes)
        {
            
            var freq = new FreqSwitchReceiveStruct();
            Type type = freq.GetType();
            freq=BytesToStuct(bytes);

            return freq;
        }
        
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LoginSendStruct
    {
        public int Head;//0x00 0x14 0xFF 0x5F
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Password;//例：密码为12345678，参照ASCII码转为16进制，即3132333435363738
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] DeviceId;//例：设备ID1234567890，参照ASCII码转为16进制，即31323334353637383930
        
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LoginReceiveStruct
    {
        public int Head;//00 0D FF F5
        public byte Status;//例：密码为12345678，参照ASCII码转为16进制，即3132333435363738
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] DeviceId;//例：设备ID1234567890，参照ASCII码转为16进制，即31323334353637383930

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FreqSwitchReceiveStruct
    {
        public int Head;//00 32 ff 5d
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Id;
        public int Distance;//整型，比如距离为3660米，即0x00000E4C
        public float Az;//Float，正北为0度顺时针，方位为354.88，即A470B143
        public float El;//Float，例：值为1.1269847即0941903F
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Freq;//例：设备ID1234567890，参照ASCII码转为16进制，即31323334353637383930
        public byte time;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] DeviceId;//例：设备ID1234567890，参照ASCII码转为16进制，即31323334353637383930


    }
    //00 32 FF 5D 31 36 33 43 48 39 4D 52 30 41 34 42 55 39 2A 2A 2A 2A 2A 2A 00 00 29 7F 48 61 9E 43 11 36 AB 3E 01 00 00 00 00 01 32 30 30 30 30 30 30 34 30 35
    
    //00 32 FF 5D 31 36 33 43 48 39 4D 52 30 41 34 42 55 39 2A 2A 2A 2A 2A 2A 00 00 29 7F 48 61 9E 43 11 36 AB 3E 00 00 00 00 00 00 32 30 30 30 30 30 30 34 30 35
}
