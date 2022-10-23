using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
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
    public class PeerServerUdp : PeerServerBase
    {
        public PeerServerUdp(ILogger<PeerServerUdp> logger, IPeerRoute route)
        {
            Route = route;
            _send = new UdpClient();
            _logger = logger;
        }

        private UdpClient _udp;
        private readonly UdpClient _send;
        private Thread _listion;
        private readonly ILogger _logger;

        public sealed override void Star()
        {
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
                ep = new IPEndPoint(IPAddress.Parse(Info.ListionIp), Info.ListionPort);
                //Info.ListionIp = "0.0.0.0";
                //ep = new IPEndPoint(IPAddress.Any, Info.ListionPort);
            }
            _udp = new UdpClient(ep);

            _listion = new Thread(new ThreadStart(() =>
            {
                //var ep = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    byte[] buff = null;
                    try
                    {
                        buff = _udp.Receive(ref ep);                       
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
            _udp?.Close();
            _udp?.Dispose();
            Info.State = PeerServerState.Stop;
        }
        public override int Send(byte[] buff, string ip, int port,string temp="小雷达收发必须同一ip，用此方法进行发送")
        {
            try
            {
                if (buff != null)
                {
                    return _udp?.Send(buff, buff.Length, ip, port) ?? -2;
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
                    return _udp?.Send(buff, buff.Length, ip, port) ?? -2;
                }
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"udp send data({buff?.Length})  {ip}:{port}fail.");
                return -3;
            }
        }

        public override Task<int> SendAsync(byte[] buff, string ip, int port)
        {
            var res = -1;
            try
            {
                if (_udp != null)
                {
                    if (buff != null)
                    {
                        return _udp.SendAsync(buff, buff.Length, ip, port);
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
