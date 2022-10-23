
using AntiUAV.DevicePlugin.ProbeP02.Cmd;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using GraphQL.Common.Request;
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

namespace AntiUAV.DevicePlugin.ProbeP02
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        private RestClient _client;

        public string SpectrumLoginUrl = "https://192.178.1.88:8080/lgoin";
        public string SpectrumUrl = "https://192.178.1.88:8080/rf/graphql";

        //string SpectrumUrl = "http://47.92.98.138:8099/api/v3/system/all";
        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
            _client = new RestClient();
            //_client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证
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
            Login();

            var heroRequest = new GraphQLRequest
            {
                Query = queryCommand   //这里填写query的内容。
            };
            //var graphQLClient = new GraphQLClient("https://api.github.com/graphql");
            //graphQLClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Safari", "537.36"));   //上面这行很关键，UserAgent一定要写上，要不然会出现403错误，花了好久才找到这个问题。
            //graphQLClient.DefaultRequestHeaders.Add("Authorization", "bearer token");   //这里的token是个占位，实际需要在Github上生成。
            //var graphQLResponse = graphQLClient.PostAsync(heroRequest).Result;
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
        string queryCommand = "{drone {id    name    description    image    state    direction    whitelisted    can_attack    can_takeover    attacking   created_time    deleted_time    seen_sensor {     sensor_id      detected_freq_khz      detected_time    }    attack_bands}autoAttack}";
        string _tokenKey = "";
        public bool Login()
        {
            var request = new RestRequest(SpectrumLoginUrl, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new Login() { UserName = "demo", PassWord ="secret" });
            var response = _client.Execute<ServiceResponse<LoginUser>>(request);
            if (response == null) return false;
            if (response.Data != null && response.Data.Code == ServiceResponseCode.Success && response.Data.Data != null)
            {
                var data = response.Data.Data;
                //设置token信息
                _memory.Set(_tokenKey, data.Token);
               
                return true;
            }
            return false;
        }
        public Task HttpGet()
        {
            try
            {
                var dev = _memory.GetDevice();
                var request = new RestRequest(SpectrumUrl, Method.POST);

                
                //string loginBody="{ username: 'demo', password: 'secret'}";
                request.AddHeader("Content-Type", "application/json");
                //string base64Credentials = GetEncodedCredentials();
               // request.AddHeader("Authorization", "Bears " + base64Credentials);
                var response = _client?.Execute(request);
                // 遍历字典 并将各个条目保存到数据库(智空未来频谱协议，json串的key值是不确定的，只能用key value的方式一层一层的解，fuuuuuuuuuck）
                var temp = JsonConvert.DeserializeObject<JObject>(response.Content);
                //var data_jsonObj = JObject.Parse(temp["data"].ToString());//接收到的json串
                var devices_info_jsonObj = JObject.Parse(temp["devices_info"].ToString());//目标信息json对象
                var sensors_info_jsonObj = JObject.Parse(temp["sensors_info"].ToString());//设备信息json对象

                var devices_info_jsonStr = JsonConvert.SerializeObject(devices_info_jsonObj);
                var sensors_info_jsonStr = JsonConvert.SerializeObject(sensors_info_jsonObj);

                //X509Certificate


                var status = $"S_{sensors_info_jsonStr}";
                var targets = $"T_{devices_info_jsonStr}";
                _peer.SendAsync(Encoding.UTF8.GetBytes(status ?? ""), dev.Lip, dev.Lport);
                _peer.SendAsync(Encoding.UTF8.GetBytes(targets ?? ""), dev.Lip, dev.Lport);

                return Task.CompletedTask;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }

    internal class Login
    {
        public Login()
        {
        }

        public string UserName { get; set; }
        public object PassWord { get; set; }
    }
    public class LoginUser
    {
        public User UserInfo { get; set; }

        public string Token { get; set; }
    }
    
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string imageUrl { get; set; }
       
    }
    public class ServiceResponse<T>
    {
        public ServiceResponseCode Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
    public enum ServiceResponseCode
    {
        Success = 0,
        Fail,
        Error,
        NoPermission,
        NotFoundService,
        RequestError,
        UnKnowError
    }
   
}
