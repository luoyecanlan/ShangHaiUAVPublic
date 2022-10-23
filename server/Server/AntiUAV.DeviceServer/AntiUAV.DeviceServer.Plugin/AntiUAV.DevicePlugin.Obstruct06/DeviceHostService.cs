using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace AntiUAV.DevicePlugin.Obstruct06
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
      
        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
           
        }
      
        public override int DeviceCategory => PluginConst.Category;
        //Socket tcpClient = null;
        public void Connect()
        {
            var dev = _memory.GetDevice();
            GlobleObj.tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse(dev.Ip);
            EndPoint point = new IPEndPoint(ipaddress, dev.Port);
            try
            {
                GlobleObj.tcpClient.Connect(point);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
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

            Task.Run(() =>
            {

                int i = 0;
                while (!GlobleObj.tcpClient.Connected)
                {
                    try
                    {
                        
                        
                        {

                            Connect();
                            Task.WaitAll(Task.Delay(1000));
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(string.Format("check persistence & run data server error.{0}", ex.Message));
                        Thread.Sleep(1000);
                    }
                }
            });

        }


    }
    public static class GlobleObj{
        public static Socket tcpClient;
    }
    

}
