using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService.ServiceImpl
{
    public class PeerServerTcp : PeerServerBase
    {
        public PeerServerTcp(ILogger<PeerServerTcp> logger, IPeerRoute route)
        {
            Route = route;
            _send = new TcpClient();
            _logger = logger;
            _receiveBuffer = new byte[_receiveBufferSize];
        }

        private TcpListener _tcp;
        private TcpClient _send;
        private Thread _listion;
        private readonly ILogger _logger;
        private readonly int _receiveBufferSize = 1024*1024*2;
        private readonly byte[] _receiveBuffer;
        public sealed override void Star()
        {
            //string ip = "192.168.1.88";
            //int port = 5000;


            if (Info == null)
                throw new PeerException("server info is null.");
            if (Route == null)
                throw new PeerException("server route is null.");
            if (Info?.ListionPort <= 0)
                throw new PeerException("the listening port must be greater than 0.");
            IPEndPoint ep;
            if (string.IsNullOrEmpty(Info?.ListionIp) || Info.ListionIp == "0.0.0.0" || Info.ListionIp.ToLower() == "any")
            {
                Info.ListionIp = "0.0.0.0";
                ep = new IPEndPoint(IPAddress.Any, Info.ListionPort);
            }
            else
            {
                //ep = new IPEndPoint(IPAddress.Parse(Info.ListionIp), Info.ListionPort);
                Info.ListionIp = "0.0.0.0";
                ep = new IPEndPoint(IPAddress.Any, Info.ListionPort);
            }
            _tcp = new TcpListener(ep);
            _tcp.Start();//开始监听客户端请求
            _listion = new Thread(new ThreadStart(() =>
            {
                //var ep = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    byte[] buff1 = null;
                    try
                    {
                        _send = _tcp.AcceptTcpClient();
                        //EndPoint endPoint=_send?.Client.RemoteEndPoint;
                        Thread th_SendData = new Thread(SendStatusBuff);
                        th_SendData.IsBackground = true;
                        th_SendData.Start(_send);
                        //SendStatusBuff(ip, port);
                        //创建线程接收数据
                        Thread th_ReceiveData = new Thread(Receive);
                        th_ReceiveData.IsBackground = true;
                        th_ReceiveData.Start(_send);
                    }
                    catch (SocketException sex)
                    {
                        //socket 异常处理
                        _logger.LogError(sex, $"tcp listener create fail.");
                        //Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "tcp server recice data socket error.", sex));
                    }
                    
                }
            }));
            _listion.Start();
            Info.State = PeerServerState.Run;
        }
        public void SendStatusBuff(Object endpoint)
        {
            try
            {
                TcpClient _client = endpoint as TcpClient;
                string ip = _client.Client.RemoteEndPoint.ToString().Split(':')[0];

                int port = Convert.ToInt32(_client.Client.RemoteEndPoint.ToString().Split(':')[1]);

                byte[] statusQuery = { 0x53, 0x5a, 0x0d, 0x06, 0x00, 0x01, 0xc1 };
                while (_client.Client.Connected)
                {
                    SendAsync(statusQuery, ip, port);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public void Receive(Object _send)
        {
            IPEndPoint ep=null;
            TcpClient _client = _send as TcpClient;
            while (_client.Client.Connected)
            {
                byte[] buff1 = null;
                try
                {
                    //_send = _tcp.AcceptTcpClient();
                    var netStream = _client?.GetStream();//获取用于读取和写入的流对象
                    int receiveLength;
                    lock (netStream)
                    {
                        receiveLength = netStream.Read(_receiveBuffer, 0, _receiveBufferSize);
                    }
                    if (receiveLength > 0)
                    {
                        buff1 = new byte[receiveLength];
                        Array.Copy(_receiveBuffer, buff1, receiveLength);
                        //return retBytes;
                    }
                    //return null;
                    
                    if (string.IsNullOrEmpty(Info?.ListionIp) || Info.ListionIp == "0.0.0.0" || Info.ListionIp.ToLower() == "any")
                    {
                        Info.ListionIp = "0.0.0.0";
                        ep = new IPEndPoint(IPAddress.Any, Info.ListionPort);
                    }
                    else
                    {
                        //ep = new IPEndPoint(IPAddress.Parse(Info.ListionIp), Info.ListionPort);
                        Info.ListionIp = "0.0.0.0";
                        ep = new IPEndPoint(IPAddress.Any, Info.ListionPort);
                    }
                    //int buff = _tcp.Client.Receive(buff1);
                    _ = Route?.ExcutePipeLineAsync(GetContent(buff1, ep));
                }
                catch (SocketException sex)
                {
                    //socket 异常处理
                    Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff1, ep, "tcp server recice data socket error.", sex));
                }
                //Thread.Sleep(100);
            }
        }
        public sealed override void Stop()
        {
            try
            {
                _listion?.Abort();//会引发全局监视异常，故自己捕获不处理
            }
            catch { }
            _send?.Close();
            _send?.Dispose();
            Info.State = PeerServerState.Stop;
        }

        public override int Send(byte[] buff, string ip, int port)
        {
            try
            {
                if (buff != null)
                {
                    // return _send?.Send(buff, buff.Length, ip, port) ?? -2;
                    return _send.Client.Send(buff);
                    //return 0;
                }
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"tcp send data({buff?.Length}) fail.");
                return -3;
            }
        }

        public override Task<int> SendAsync(byte[] buff, string ip, int port)
        {
            var res = -1;
            try
            {
                if (_tcp != null)
                {
                    if (buff != null)
                    {
                        _send.Client.SendAsync(buff,SocketFlags.None,CancellationToken.None);
                    }
                    else
                        res = -2;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"tcp send data({buff?.Length}) fail.");
                res = -3;
            }
            return Task.FromResult(res);
        }

        public override int Send(byte[] buff, string ip, int port, string temp = "小雷达收发必须由同一ip端口，用此方法进行发送")
        {
            return 0;
        }
    }
}
