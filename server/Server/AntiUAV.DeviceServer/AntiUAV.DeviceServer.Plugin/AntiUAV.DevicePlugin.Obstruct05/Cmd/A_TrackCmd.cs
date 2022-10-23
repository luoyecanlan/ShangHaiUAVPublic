using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct05.Cmd
{
    public class A_TrackCmd : IPeerCmd
    {
        RestClient client;
        public A_TrackCmd(ILogger<A_TrackCmd> logger, IMemoryCache memory, GisTool tool, IServiceProvider provider)
        {
            _logger = logger;
            _memory = memory;
            _tool = tool;
            var dev = _memory.GetDevice();
            client = new RestClient();
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证
            _host = provider.GetServices<IDeviceHostService>().FirstOrDefault(x => x.DeviceCategory == dev.Category);

            //double lat = 39.777473;
            //double lng = 116.570607;
            //var AdrObj = AliAdrCode(lng.ToString(), lat.ToString());
        }

        public int Category => PluginConst.Category;

        public string Key => PluginConst.TrackCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;
        //百度逆地理编码url
        string BaiduGeoCodeUrl = "http://api.map.baidu.com/reverse_geocoding/v3";

        //高德逆地理编码url
        string AliGeoCodeUrl = "https://restapi.amap.com/v3/geocode/regeo?";
        string PrivateKey = "d08f89376ca2e8be4e4cfd8b6a7bd907";

        private readonly ILogger<A_TrackCmd> _logger;
        private readonly IMemoryCache _memory;
        private readonly GisTool _tool;
        private readonly IDeviceHostService _host;
        public async Task Invoke(IPeerContent content)
        {
            try
            {
                var targets_json = Encoding.UTF8.GetString(content.Source, 2, content.Source.Length - 2);
                var targetInfovar = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryDeviceReportRecordResponse>(targets_json);
                var dev = _memory.GetDevice();
                var tgs = new List<TargetInfo>();
                if (targetInfovar != null)
                {
                    // int index
                    foreach (DataList temp in targetInfovar.data.list)
                    {
                       
                        var tg = MapToTargetInfo(temp, dev);


                        tgs.Add(tg);
                    }
                    await _memory.UpdateTarget(tgs.ToArray());
                    content.SourceAys = tgs;
                    _logger.LogDebug($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()},source track count:{"TrackCount"}.");

                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning(string.Format("The A_TrackCmd Command failed detection ex:{0}", ex.Message));
                await Task.FromCanceled(new System.Threading.CancellationToken());
                throw ex;
            }
        }

        public AliGeoCodeRootobject AliAdrCode(string HomeLng, string HomeLat)
        {
            try
            {
                string geoCodeUrl = $"{AliGeoCodeUrl}key={PrivateKey}&location={HomeLng},{HomeLat}";


                var request = new RestRequest(geoCodeUrl, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                var response = client.Execute<AliGeoCodeRootobject>(request).Content;
                response = response.Replace("\n", "");
                var data = JsonConvert.DeserializeObject<AliGeoCodeRootobject>(response);
                return data;
            }
            catch (Exception)
            {

                return default;
            }
        }

        TargetInfo MapToTargetInfo(DataList track, DeviceInfo dev)
        {
            string idNo = track.deviceId.ToString();
            var id = $"Gun-{idNo}";
           
            var tg = new TargetInfo
            {
               
                Id = id,
                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                Alt = track.dataContent.height,
                Lat = track.dataContent.lat,
                Lng = track.dataContent.lng,
                GUNSStatus=track.dataContent.state== "00000000"?false:true,
                Category = 1,
               
                Vr = Convert.ToSingle(track.dataContent.speed),
                Vt = Convert.ToSingle(track.dataContent.speed),
                TrackTime = DateTime.Now,
                Threat = 0
            };
            var last = _memory.GetTargetById(id);
            if (tg.Lat != last?.Last.AppLat || tg.Lng != last?.Last.AppLng)
            {

                var AdrObj = AliAdrCode(tg.Lng.ToString(), tg.Lat.ToString());
                string Addr = AdrObj?.regeocode.formatted_address;
                tg.AppAddr = Addr;
            }

            else
            {
                tg.AppAddr = last?.Last.AppAddr;
            }

            return tg;
        }
       
    }

    




    public class AliGeoCodeRootobject
    {
        public string status { get; set; }
        public Regeocode regeocode { get; set; }
        public string info { get; set; }
        public string infocode { get; set; }
    }

    public class Regeocode
    {
        // public Addresscomponent addressComponent { get; set; }
        public string formatted_address { get; set; }
    }

    public class Addresscomponent
    {
        public object[] city { get; set; }
        public string province { get; set; }
        public string adcode { get; set; }
        public string district { get; set; }
        public string towncode { get; set; }
        public Streetnumber streetNumber { get; set; }
        public string country { get; set; }
        public string township { get; set; }
        public Businessarea[] businessAreas { get; set; }
        //public Building building { get; set; }
        public Neighborhood neighborhood { get; set; }
        public string citycode { get; set; }
    }

    public class Streetnumber
    {
        public string number { get; set; }
        public string location { get; set; }
        public string direction { get; set; }
        public string distance { get; set; }
        public string street { get; set; }
    }

    public class Building
    {
        public string[] name { get; set; }
        public string[] type { get; set; }
    }

    public class Neighborhood
    {
        public string[] name { get; set; }
        public string[] type { get; set; }
    }

    public class Businessarea
    {
        public string location { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }





}
