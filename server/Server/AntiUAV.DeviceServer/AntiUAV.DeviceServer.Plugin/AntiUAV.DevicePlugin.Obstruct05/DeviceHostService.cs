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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        private RestClient _client;
        public string QueryDealRecord = "http://127.0.0.1:9114/webapi/v1/getDisposalData";//获取反制枪的处置记录
        public string QueryDataUrl = "http://127.0.0.1:9114/webapi/v1/getDeviceData/";//获取反制枪实时上报的数据
        public string LoginUrl = "http://127.0.0.1:9114/admin/getToken";

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

        string token;
        public void Login()
        {
            var dev = _memory.GetDevice();
            var request = new RestRequest(LoginUrl, Method.GET);


            request.AddHeader("Content-Type", "application/json");

            var response = _client?.Execute(request);
            token = JsonConvert.DeserializeObject<LoginObject>(response.Content).msg;
            //return Task.CompletedTask;
        }

        public Task HttpGet()
        {
            try
            {
                if (token == "")
                {
                    Login();
                }
                List<int> gunsId = new List<int>();
                var dev = _memory.GetDevice();
                var request = new RestRequest(QueryDataUrl + gunsId, Method.POST);

                request.AddHeader("Content-Type", "application/json");

                request.AddHeader("Token", token);
                QueryDeviceRequest queryDeviceRequest = new QueryDeviceRequest()
                {
                    pageNum = 1,
                    pageSize = 100,
                    startTime = DateTime.Now.AddDays(1).ToString(),
                    deviceIds = gunsId
                };
                
                request.AddJsonBody(queryDeviceRequest);
                var response = _client?.Execute(request);

                var temp = JsonConvert.DeserializeObject<QueryDeviceReportRecordResponse>(response.Content);
                
                //var sensors_info_jsonObj = JObject.Parse(temp["sensors_info"].ToString());//设备信息json对象


                //var sensors_info_jsonStr = JsonConvert.SerializeObject(sensors_info_jsonObj);



                var status = $"S_{response.Content}";
                var targets = $"T_{response.Content}";
                _peer.SendAsync(Encoding.UTF8.GetBytes(status ?? ""), dev.Lip, dev.Lport);


                return Task.CompletedTask;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }

    internal class LoginObject
    {
        public string msg { get; set; }
        public int code { get; set; }
    }

    public class QueryDeviceRequest
    {
        public int pageNum { get; set; }
        public int pageSize { get; set; }
        public string startTime { get; set; }
        public List<int> deviceIds { get; set; }
    }
    public class QueryDeviceReportRecordResponse
    {
        public int code { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int total { get; set; }
        public int pageSize { get; set; }
        public List<DataList> list { get; set; }
        public int pageNum { get; set; }
    }

    public class DataList
    {
        public int id { get; set; }
        public int deviceId { get; set; }
        public DataContent dataContent { get; set; }
        public string addTime { get; set; }
        public string state { get; set; }
        public string moduleTemperature { get; set; }
    }


    public class DataContent
    {
        public int electricQuantity { get; set; }
        public int height { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
        public int remoteStatus { get; set; }
        public int speed { get; set; }
        public string state { get; set; }
        public string temperatures { get; set; }
        public int workModel { get; set; }
        public int workTime { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }


    public class QueryDeviceDealRecordResponse
    {
        public int code { get; set; }
        public QueryDeviceDealRecordResponseData data { get; set; }
    }

    public class QueryDeviceDealRecordResponseData
    {
        public int total { get; set; }
        public int pageSize { get; set; }
        public List<QueryDeviceDealRecordResponseList> list { get; set; }
        public int pageNum { get; set; }
    }

    public class QueryDeviceDealRecordResponseList
    {
        public int id { get; set; }
        public string deviceId { get; set; }
        public string time { get; set; }
        public int buttonStatus { get; set; }
        public string statusStr { get; set; }
        public string createTime { get; set; }
    }


}
