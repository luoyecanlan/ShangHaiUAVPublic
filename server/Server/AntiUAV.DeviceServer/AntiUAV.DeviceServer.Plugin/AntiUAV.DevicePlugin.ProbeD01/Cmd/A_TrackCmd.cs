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

namespace AntiUAV.DevicePlugin.ProbeD01.Cmd
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
                var targetInfovar = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UavListItem>>(targets_json);
                var dev = _memory.GetDevice();
                var tgs = new List<TargetInfo>();
                if (targetInfovar != null)
                {
                    // int index
                    foreach (UavListItem temp in targetInfovar)
                    {
                        if (temp.uavSn == null && string.IsNullOrEmpty(temp.uavSn))
                            continue;
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

        TargetInfo MapToTargetInfo(UavListItem track, DeviceInfo dev)
        {
            string idNo = track.uavSn;
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{idNo}";
            double dis = 0;
            try
            {
                dis = Convert.ToSingle(track.distance.Substring(0, track.distance.Length - 1));
            }
            catch (Exception)
            {
                dis = 0;
                //throw;
            }
            //if(track.distance)
            //int dis=
            var socre = _memory.GetThreat(id);
            var tg = new TargetInfo
            {
                ProbeAz = Convert.ToSingle(track.flyDirection),
                ProbeDis = dis,
                ProbeHigh = Convert.ToDouble(track.altitude) - dev.Alt,
                Id = id,
                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                Alt = Convert.ToSingle(track.altitude),
                Lat = Convert.ToSingle(track.uavLatitude),
                Lng = Convert.ToSingle(track.uavLongitude),
                FlyDirection = track.flyDirection,
                Category = 1,
                AppLat = Convert.ToSingle(track.appLatitude),
                AppLng = Convert.ToSingle(track.appLongitude),
                HomeLat = Convert.ToSingle(track.homeLatitude),
                HomeLng = Convert.ToSingle(track.homeLongitude),
                BeginAt = Convert.ToDateTime(track.beginAt),
                UAVType = track.productType,
                UAVSn = track.uavSn,
                //Mode = track.SendModel,
                Vr = Convert.ToSingle(track.flySpeed),
                Vt = Convert.ToSingle(track.flySpeed),
                TrackTime = DateTime.Now,
                Threat = socre >= 50 ? socre : 50
            };
            var last = _memory.GetTargetById(id);
            if (tg.AppLat != last?.Last.AppLat || tg.AppLng != last?.Last.AppLng)
            {

                var AdrObj = AliAdrCode(tg.AppLng.ToString(), tg.AppLat.ToString());
                string Addr = AdrObj?.regeocode.formatted_address;
                tg.AppAddr = Addr;
            }

            else
            {
                tg.AppAddr = last?.Last.AppAddr;
            }

            return tg;
        }
        #region 百度逆地理解析服务（弃用）
        public string BaiduAddrCode(string HomeLat, string HomeLng)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ak", "WZuZAMGcnO71Taixdj7ofGuCmB7Ivm6d");
            dic.Add("output", "json");
            dic.Add("coordtype", "wgs84ll");
            dic.Add("location", $"{HomeLat},{HomeLng}");

            string strAk = CaculateAKSN(AK, SK, BaiduGeoCodeUrl, dic);
            string geoCodeUrl = $"{BaiduGeoCodeUrl}/?ak={AK}&output=json&coordtype=wgs84ll&location={HomeLat},{HomeLng}&sn={strAk}";

            var request = new RestRequest(geoCodeUrl, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            return client.Execute(request).Content;
        }
        #region 百度计算AK码
        private static string UrlEncode(string str)
        {
            str = System.Web.HttpUtility.UrlEncode(str);
            byte[] buf = Encoding.ASCII.GetBytes(str);//等同于Encoding.ASCII.GetBytes(str)
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] == '%')
                {
                    if (buf[i + 1] >= 'a') buf[i + 1] -= 32;
                    if (buf[i + 2] >= 'a') buf[i + 2] -= 32;
                    i += 2;
                }
            return Encoding.ASCII.GetString(buf);//同上，等同于Encoding.ASCII.GetString(buf)
        }

        private static string HttpBuildQuery(IDictionary<string, string> querystring_arrays)
        {

            StringBuilder sb = new StringBuilder();
            foreach (var item in querystring_arrays)
            {
                sb.Append(UrlEncode(item.Key));
                sb.Append("=");
                sb.Append(UrlEncode(item.Value));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        string SK = "vpRPKQWO7lEGfOGbGd9BR3Kx7NiG6Bi4";
        string AK = "WZuZAMGcnO71Taixdj7ofGuCmB7Ivm6d";
        public static string CaculateAKSN(string ak, string sk, string url, IDictionary<string, string> querystring_arrays)
        {
            var queryString = HttpBuildQuery(querystring_arrays);

            var str = UrlEncode(url + "?" + queryString + sk);

            return MD5(str);
        }
        private static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #endregion
    }

    public class WarningItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int alarm { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int wsPush { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int blink { get; set; }
        /// <summary>
        /// 监控对象飞入监控范围
        /// </summary>
        public string reason { get; set; }
    }

    public class UavListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string uuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uavSn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uavLongitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uavLatitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string altitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flyDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flySpeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appLongitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appLatitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string homeLongitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string homeLatitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uavStartLongitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uavStartLatitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string beginAt { get; set; }
        /// <summary>
        /// 520.00米
        /// </summary>
        public string distance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int isMonitored { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<WarningItem> warning { get; set; }
    }

    public class ListenerListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string deviceSn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deviceLongitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deviceLatitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deviceStartLongitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deviceStartLatitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string beginAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<UavListItem> uavList { get; set; }
    }

    public class StaticInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int listenerSum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int uavSum { get; set; }
    }

    public class TrackData
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ListenerListItem> listenerList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StaticInfo staticInfo { get; set; }
    }

    public class TrackMessage
    {
        /// <summary>
        /// 成功
        /// </summary>
        public string chinese { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string english { get; set; }
    }

    public class TrackRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public TrackData data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TrackMessage message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
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
