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

namespace AntiUAV.DevicePlugin.Obstruct04
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        private RestClient _client;

        public string SpectrumUrl = "http://172.16.88.101:5000/api/v4/system";
        public string LoginUrl = "http://172.16.88.101:5000/api/v4/login";

        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
            _client = new RestClient();
            _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证
        }
        //public DeviceHostService(ILogger<DeviceHostService> logger, IMemoryCache memory)
        //{
        //    _logger = logger;
        //    _memory = memory;
        //    _client = new RestClient();
        //}

        public override int DeviceCategory => PluginConst.Category;

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

            int processActive = System.Diagnostics.Process.GetProcessesByName("AntiUAV").ToList().Count;
      

            Task.Run(() =>
            {
                //while (processActive > 0)
                while (true)
                {
                    try
                    {
                        //_logger.LogInformation("server check run.");
                        var check = HttpGet();
                        Task.WaitAll(check, Task.Delay(1000));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "check persistence & run data server error.");
                        Task.Delay(2000).Wait();
                    }
                }
            });

        }
        string m_User = "admin";
        string m_Password = "123456";

        public void Login()
        {
            var dev = _memory.GetDevice();
            var request = new RestRequest(LoginUrl, Method.GET);
            GlobalVarAndFunc.token = GetEncodedCredentials();
            request.AddHeader("Authorization", "Basic " + GlobalVarAndFunc.token);
            request.AddHeader("Content-Type", "application/json");

            var response = _client?.Execute(request);
            //token=JsonConvert.DeserializeObject<LoginObject>(response.Content).token;
            //return Task.CompletedTask;
        }
        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", m_User, m_Password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
        public Task HttpGet()
        {
            try
            {
                if (GlobalVarAndFunc.token == "")
                {
                    Login();
                }
                
                var dev = _memory.GetDevice();
                var request = new RestRequest(SpectrumUrl, Method.GET);

                request.AddHeader("Content-Type", "application/json");
                
                request.AddHeader("Authorization", "Basic " + GlobalVarAndFunc.token);
                var response = _client?.Execute(request);
                
                var temp = JsonConvert.DeserializeObject<JObject>(response.Content);
               
                var sensors_info_jsonObj = JObject.Parse(temp["sensors_info"].ToString());//设备信息json对象

                
                var sensors_info_jsonStr = JsonConvert.SerializeObject(sensors_info_jsonObj);



                var status = $"S_{sensors_info_jsonStr}";
                
                _peer.SendAsync(Encoding.UTF8.GetBytes(status ?? ""), dev.Lip, dev.Lport);
                

                return Task.CompletedTask;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
    

}
