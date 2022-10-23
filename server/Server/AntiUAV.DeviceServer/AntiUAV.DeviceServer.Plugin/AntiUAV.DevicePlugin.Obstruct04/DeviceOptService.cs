using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using EasyNetQ.Logging;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct04
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public string ObstructOnUrl = "http://172.17.172.17:5000/api/v4/mass-defense/";//post
        public string ObstructOffUrl = "http://172.17.172.17:5000/api/v4/mass-defense/";//delete
        public string LoginUrl = "http://172.17.172.17:5000/api/v4/login";

        public DeviceOptService(IMemoryCache memory)
        {
            _memory = memory;
            _client = new RestClient();


            _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证

            //_log = log;
        }
        //ILog _log;
        public override int DeviceCategory => PluginConst.Category;
        private RestClient _client;

        private readonly IMemoryCache _memory;



        public override byte[] GetAttackBuff(string json, bool sw)
        {
            var dev = _memory.GetDevice();
            byte[] result = new byte[5];
            var relation = JsonConvert.DeserializeObject<Relationships>(json);
            
            
            string devId = GlobalVarAndFunc.sensorname;
            string url = ObstructOffUrl;
            RestRequest request = new RestRequest(url + devId, Method.DELETE);
            if (sw)
            {
                url = ObstructOnUrl;
                var freq = new List<string>();
                //前端hitfreq返回为2时迫降 全开，否则只开2.4 5.8.400 900
                if (relation?.hitFreq == HitFreqModel.Hit_15)
                {
                    
                    freq.Add("2.4GHz");
                    freq.Add("5.8GHz");
                    freq.Add("1.5GHz");
                    freq.Add("1.2GHz");
                    
                    
                }
                else
                {
                    freq.Add("2.4GHz");
                    freq.Add("5.8GHz");
                    freq.Add("433MHz");
                    freq.Add("915MHz");
                }
                
                var body = new ObstrutsOnBody()
                {
                    strategy = "rf",
                    frequency = freq.ToArray(),
                    duration = 60
                };
                request = new RestRequest(url + devId, Method.POST);
                request.AddJsonBody(body);
            }
            else
            {
                url = ObstructOffUrl;
                request = new RestRequest(url + devId, Method.DELETE);
            }


            request.AddHeader("Content-Type", "application/json");
            //string base64Credentials = GetEncodedCredentials();
            request.AddHeader("Authorization", "Basic " + GlobalVarAndFunc.token);
            var response = _client?.Execute(request);

            return result;
        }



    }

    public class ObstrutsOnBody
    {
        public string strategy { get; set; }
        public string[] frequency { get; set; }
        public int duration { get; set; }
    }

}
