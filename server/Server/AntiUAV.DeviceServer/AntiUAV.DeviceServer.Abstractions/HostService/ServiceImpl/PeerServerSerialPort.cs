using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService.ServiceImpl
{
    public class PeerServerSerialPort : PeerServerBase
    {
        public PeerServerSerialPort(ILogger<PeerServerSerialPort> logger, IPeerRoute route)
        {
            Route = route;
            _serialPort = new SerialPort();
            _logger = logger;
        }

        private SerialPort _serialPort;
        //private readonly UdpClient _serialPort;
        private Thread _listion;
        private readonly ILogger _logger;
        private int _receiveBufferSize = 4096;
        private readonly byte[] _receiveBuffer;
        public void ConnectSerial()
        {
            _serialPort.Parity= Parity.None;
            _serialPort.BaudRate = 115200;
            _serialPort.Open();
        }

        public sealed override void Star()
        {
            ConnectSerial();
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
                //Info.ListionIp = "0.0.0.0";
                ep = new IPEndPoint(IPAddress.Any, Info.ListionPort);
            }
            //_serialPort = new UdpClient(ep);

            _listion = new Thread(new ThreadStart(() =>
            {
                //var ep = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {

                    int readSize = _serialPort.Read(_receiveBuffer, 0, _receiveBufferSize);
                    byte[] buff = new byte[readSize];
                    try
                    {
                        
                        Array.Copy(_receiveBuffer, buff, readSize);
                        _ = Route?.ExcutePipeLineAsync(GetContent(buff, ep));
                    }
                    catch (SocketException sex)
                    {
                        //socket 异常处理
                        Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "udp server recice data socket error.", sex));
                    }
                    catch (Exception ex)
                    {
                        Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "udp server recice data error.", ex));
                    }
                }
            }));
            _listion.Start();
            Info.State = PeerServerState.Run;
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
            Info.State = PeerServerState.Stop;
        }
        public override int Send(byte[] buff, string ip, int port,string temp="小雷达收发必须同一ip，用此方法进行发送")
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
        public override int Send(byte[] buff, string ip, int port)
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

        public override Task<int> SendAsync(byte[] buff, string ip, int port)
        {
            var res = -1;
            try
            {
                if (_serialPort != null)
                {
                    if (buff != null)
                    {
                        _serialPort?.Write(buff, 0, buff.Length);
                    }
                    else
                        res = -2;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"udp send data({buff?.Length}) fail.");
                res = -3;
            }
            return Task.FromResult(res);
        }
    }
}
