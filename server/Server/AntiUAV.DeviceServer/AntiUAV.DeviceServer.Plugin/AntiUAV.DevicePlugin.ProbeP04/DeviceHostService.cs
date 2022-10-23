using AntiUAV.DevicePlugin.ProbeP04.Cmd;
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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeP04
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        private RestClient _client;
        private ILogger<DeviceHostUdpServerBase> _logger;
        //public string SpectrumUrl = "http://192.168.0.100:5000/api/v4/system";
        //public string LoginUrl = "http://192.168.0.100:5000/api/v4/login";
        public string SpectrumUrl = "http://192.168.0.100:5000/api/v4/system";
        public string LoginUrl = "http://192.168.0.100:5000/api/v4/system";
        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
            var dev = _memory.GetDevice();
            SpectrumUrl = $"http://{dev.Ip}/api/v4/system";
            LoginUrl= $"http://{dev.Ip}/api/v4/login";
            _logger = logger;
            _client = new RestClient();
            _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证
        }
      

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
              
                while (true)
                {
                    try
                    {
                      
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
        string token = "";
        public void Login()
        {
            var dev = _memory.GetDevice();
            var request = new RestRequest(LoginUrl, Method.GET);
            token = GetEncodedCredentials();
            request.AddHeader("Authorization", "Basic "+ token);
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
                if (token == "")
                {
                    Login();
                }
                var dev = _memory.GetDevice();
                var request = new RestRequest(SpectrumUrl, Method.GET);
                
                request.AddHeader("Content-Type", "application/json");
                //string base64Credentials = GetEncodedCredentials();
                request.AddHeader("Authorization", "Basic " + token);
                var response = _client?.Execute(request);
                // 遍历字典 并将各个条目保存到数据库(智空未来频谱协议，json串的key值是不确定的，只能用key value的方式一层一层的解，fuuuuuuuuuck）
                var temp = JsonConvert.DeserializeObject<JObject>(response.Content);

               
                //var data_jsonObj = JObject.Parse(temp["data"].ToString());//接收到的json串
                var devices_info_jsonObj = JObject.Parse(temp?["devices_info"]?.ToString());//目标信息json对象
                var sensors_info_jsonObj = JObject.Parse(temp?["sensors_info"]?.ToString());//设备信息json对象
               
                var devices_info_jsonStr = JsonConvert.SerializeObject(devices_info_jsonObj);
                var sensors_info_jsonStr = JsonConvert.SerializeObject(sensors_info_jsonObj);
                //_logger.LogInformation(JsonConvert.SerializeObject(devices_info_jsonStr) + "DEVICEINFOJSONSTR9999999999999999999999999999999999------------");
                //_logger.LogInformation(JsonConvert.SerializeObject(sensors_info_jsonStr) + "SENSORSINFOJSONSTR888888888888888888888888888888888888----------");
                //X509Certificate


                var status = $"S_{sensors_info_jsonStr}";
                var targets = $"T_{devices_info_jsonStr}";
                _peer.SendAsync(Encoding.UTF8.GetBytes(status ?? ""), dev.Lip, dev.Lport);
                _peer.SendAsync(Encoding.UTF8.GetBytes(targets ?? ""), dev.Lip, dev.Lport);

                return Task.CompletedTask;


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return null;
            }

        }
    }
    
}
