
using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Service;
using AntiUAV.DevicePlugin.ProbeD01.Cmd;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeD01
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        private RestClient _client;

        //密钥
        private string secret = "";

        static int timeInterval = 7200;
        System.Timers.Timer timer = new System.Timers.Timer(timeInterval * 1000);


        //云哨ip
        public string SpectrumUrl = "http://222.190.143.158:8180/uavplatform_test";
        //public string SpectrumUrl = "http://192.168.0.18:8180/uavplatform_test";
        //private List<List<GisTool.Position>> _zones = new List<List<GisTool.Position>>();

        //上报公安ip
        public string ReportUrl = "http://xxxxxxxxxx";
        public string ReportUrlNEW = "https://gaj.sh.gov.cn/wrj/index.php/admin/outfunc/";
        public string ReportUrlNEWTest = "http://zjlin123.com/shba_gk/index.php/admin/outfunc/";
        public DeviceHostService(ILogger<DeviceHostUdpServerBase> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
            _client = new RestClient();
            timer.Elapsed += Timer_Elapsed;
            _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;//跳过ssl验证
            //var zones = _memory.GetZones()?.Zones;
            //if (zones != null)
            //{
            //    foreach (var zone in zones)
            //    {
            //        _zones.Add(zone.ZPointsPosition);
            //    }
            //}
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Login();
        }

        public override int DeviceCategory => PluginConst.Category;

        /// <summary>
        /// 上报上海公安无人机数据
        /// </summary>
        /// <param name="trackRoot"></param>
        public void Report(TrackRoot trackRoot)
        {
            try
            {
                var targets = _memory.GetAllTargets();
                var dev = _memory.GetDevice();
                var Reports = new List<SHReportModelNEW>();
                if (targets != null)
                {
                    foreach (TargetCacheInfo temp in targets)
                    {
                        SHReportModelNEW sHReportModel = new SHReportModelNEW()
                        {
                            longitude = Convert.ToSingle(temp.Last.Lng).ToString(),
                            latitude = Convert.ToSingle(temp.Last.Lat).ToString(),
                            altitude = Convert.ToSingle(temp.Last.Alt).ToString(),
                            roll = temp.Last.ProbeAz.ToString(),
                            source = 4,
                            aeroscopeID = trackRoot.data?.listenerList[0]?.deviceSn.ToString(),
                            absoluteHeight = Convert.ToSingle(temp.Last.Alt + dev.Alt).ToString(),
                            pitch = temp.Last.PeobeEl.ToString(),
                            orderID = 0.ToString(),
                            appLongitude = temp.Last.AppLng.ToString(),
                            appLatitude = temp.Last.AppLat.ToString(),
                            homeLongitude = temp.Last.HomeLng.ToString(),
                            homeLatitude = temp.Last.HomeLat.ToString(),
                            droneType = temp.Last.UAVType,
                            droneID = temp.Last.UAVSn,
                            flightTime = (DateTime.Now.ToUniversalTime().ToLong() / 1000).ToString(),
                            deviceid = "2000000195"//测试服171 正式服195
                        };


                        //Reports.Add(sHReportModel);

                        string url = $"{ReportUrlNEW}xintan_data?timestamp={ToTimeStamp(DateTime.Now)}&token={HMACTokenNew(ToTimeStamp(DateTime.Now).ToString())}";
                        var request = new RestRequest(url, Method.POST);

                        request.AddHeader("Content-Type", "application/json");
                        //string base64Credentials = GetEncodedCredentials();
                        //request.AddHeader("x-authorization", token);
                        request.AddJsonBody(sHReportModel.ToJson());
                        //request.AddParameter(Reports.ToJson(),ParameterType.RequestBody);
                        var response = _client?.Execute<ReportResponseRoot>(request);

                    }
                }
                //string url = $"{ReportUrlNEWTest}xintan_data?timestamp={DateTime.Now.ToTimeStamp()}&token={HMACTokenNew(DateTime.Now.ToTimeStamp().ToString())}";
                //var request = new RestRequest(url, Method.POST);

                //request.AddHeader("Content-Type", "application/json");
                ////string base64Credentials = GetEncodedCredentials();
                ////request.AddHeader("x-authorization", token);
                //request.AddJsonBody(Reports.ToJson());
                ////request.AddParameter(Reports.ToJson(),ParameterType.RequestBody);
                //var response = _client?.Execute<ReportResponseRoot>(request);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static long ToTimeStamp(DateTime time)
        {
            DateTime dt = time.ToUniversalTime();
            DateTime st = new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();
            return (long)(dt - st).TotalSeconds;
        }
        public string HMACTokenNew(string timestamp)
        {

            string message = timestamp + "xintan2019";
            byte[] buffer = System.Text.Encoding.Default.GetBytes(message);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] somme = check.ComputeHash(buffer);
                string ret = "";
                foreach (byte a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }


        }
        //public void Report(TrackRoot trackRoot)
        //{
        //    try
        //    {
        //        var dev = _memory.GetDevice();
        //        string token = HMACToken(DateTime.Now.ToTimeStamp().ToString());
        //        string reportUrl = $"https://gaj.sh.gov.cn/wrj/index.php/admin/outfunc/xintan_data?timestamp={DateTime.Now.ToTimeStamp()}&token={token}";
        //        //string reportUrl = $"http://zjlin123.com/shba_gk/index.php/admin/outfunc/xintan_data?timestamp={DateTime.Now.ToTimeStamp()}&token={token}";
        //        List<PoliceReport> policeReports = new List<PoliceReport>();
        //        string device_id = trackRoot.data.listenerList[0].deviceSn;
        //        //string device_lat = trackRoot.data.listenerList[0].deviceLatitude;
        //        //string device_lng = trackRoot.data.listenerList[0].deviceLongitude;

        //        string device_lat = dev.Lat.ToString();
        //        string device_lng = dev.Lng.ToString();
        //        string device_status = "1";
        //        var tracks = trackRoot.data.listenerList[0].uavList;
        //        var targets = _memory.GetAllTargets();

        //        var request = new RestRequest(reportUrl, Method.POST);
        //        request.AddHeader("Content-Type", "application/json");
        //        if (targets != null)
        //        {
        //            foreach (TargetCacheInfo temp in targets)
        //            {
        //                PoliceReport sHReportModel = new PoliceReport()
        //                {
        //                    droneID = temp.Last.UAVSn,
        //                    aeroscopeID = device_id,
        //                    absoluteHeight = temp.Last.Alt.ToString("f1"),
        //                    altitude = temp.Last.ProbeHigh.ToString("f1"),
        //                    appLatitude = temp.Last.AppLat.ToString("f5"),
        //                    appLongitude = temp.Last.AppLng.ToString("f5"),
        //                    deviceid = "2000000405",
        //                    //deviceid = "2000000411",
        //                    droneType = temp.Last.UAVType,
        //                    flightTime = (DateTime.Now.ToUniversalTime().ToLong() / 1000).ToString(),
        //                    homeLatitude = temp.Last.HomeLat.ToString("f5"),
        //                    homeLongitude = temp.Last.HomeLng.ToString("f5"),
        //                    latitude = temp.Last.Lat.ToString("f5"),
        //                    longitude = temp.Last.Lng.ToString("f5"),
        //                    orderID = "1",
        //                    pitch = "0",
        //                    roll = "0",
        //                    source = "4"

        //                };

        //                policeReports.Add(sHReportModel);



        //                //string base64Credentials = GetEncodedCredentials();
        //                //request.AddHeader("x-authorization", token);
        //                request.AddJsonBody(sHReportModel.ToJson());
        //                var response = _client?.Execute(request);


        //            }
        //        }
        //        var json = policeReports.ToJson();
        //        //PoliceReport sHReportModel1 = new PoliceReport()
        //        //{
        //        //    droneID = "test999999",
        //        //    aeroscopeID = device_id,
        //        //    absoluteHeight = "11",
        //        //    altitude = "231",
        //        //    appLatitude = "33.14",
        //        //    appLongitude = "l11.44",
        //        //    //deviceid = "2000000405",
        //        //    deviceid = "2000000411",
        //        //    droneType = "精灵4",
        //        //    flightTime = "300",
        //        //    homeLatitude = "33.28",
        //        //    homeLongitude = "111.35",
        //        //    latitude = "34.12",
        //        //    longitude = "111.58",
        //        //    orderID = "1",
        //        //    pitch = "0",
        //        //    roll = "0",
        //        //    source = "4"

        //        //};
        //        //policeReports.Add(sHReportModel1);
        //        //var request = new RestRequest(reportUrl, Method.POST);

        //        //request.AddHeader("Content-Type", "application/json");
        //        ////string base64Credentials = GetEncodedCredentials();
        //        ////request.AddHeader("x-authorization", token);
        //        //request.AddJsonBody(sHReportModel1.ToJson());
        //        //var response = _client?.Execute(request);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }


        //}
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
                while (true)
                {
                    try
                    {
                        Login();
                        if (isSuccess)
                        {

                            var check = HttpGet();
                            i++;
                            //公安上报要求五秒请求一次，注意，更改下面时间间隔的时候一定要保证满足report间隔大于五秒
                            if (i == 5)
                            {
                                i = 0;
                                //Report(1, "");
                                if (check.Result != null)
                                    Report(check.Result);
                            }
                            Task.WaitAll(check, Task.Delay(1000));
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
        string token = "";
        private bool isSuccess = false;
        public void Login()
        {
            try
            {
                if (!isSuccess)
                {
                    string loginUrl = SpectrumUrl + "/api/user/login";
                    var request = new RestRequest(loginUrl, Method.POST);
                    RequestLogin requestLogin = new RequestLogin()
                    {
                        username = "xhzadmin",
                        password = "xhz20190305"
                        //username = "admin",
                        //password = "xjh870310"
                    };
                    request.AddJsonBody(requestLogin);
                    request.AddHeader("Content-Type", "application/json");
                    var response = _client?.Execute<LoginRoot>(request);
                    if (response.Data != null && response.Data.data != null && response.Data.data.token != null)
                    {
                        token = response.Data.data.token;
                        isSuccess = response.Data.success;
                        _logger.LogDebug($"登录云哨成功");

                    }

                }
                //return response.Data.success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"云哨登录错误，错误信息{ex.Message}");
            }


        }
        public Task<TrackRoot> HttpGet()
        {
            try
            {
                var dev = _memory.GetDevice();
                #region----------------测试虚拟目标------------------------
                //var track = new List<UavListItem>();
                //track.Add(new UavListItem()
                //{
                //    altitude = "215.7",
                //    appLatitude = "31.138279",
                //    appLongitude = "121.662912",
                //    beginAt = "2021-05-29 14:08:25",
                //    distance = "3750.00米",
                //    flyDirection = "-161.11",
                //    flySpeed = "0.05",
                //    homeLatitude = "31.141819",
                //    homeLongitude = "121.661712",
                //    isMonitored = 0,
                //    productType = "Mavic Mini 2",
                //    uavLatitude = "31.238330",
                //    uavLongitude = "121.509206",
                //    uavSn = "sn:Test1",
                //    uavStartLatitude = "31.147434",
                //    uavStartLongitude = "121.665873",
                //    uuid = "vvvf"
                //});
                //track.Add(new UavListItem()
                //{
                //    altitude = "215.7",
                //    appLatitude = "31.138279",
                //    appLongitude = "121.509206",
                //    beginAt = "2021-05-29 14:08:25",
                //    distance = "3750.00米",
                //    flyDirection = "-161.11",
                //    flySpeed = "0.05",
                //    homeLatitude = "31.141819",
                //    homeLongitude = "121.661712",
                //    isMonitored = 0,
                //    productType = "Mavic Mini 2",
                //    uavSn = "sn:Test3",
                //    uavLatitude = "31.238861",
                //    uavLongitude = "121.494999",
                //    uavStartLatitude = "31.147434",
                //    uavStartLongitude = "121.665873",
                //    uuid = "gggh"
                //});

                //var sensors_info_jsonObj_test = track;//目标信息json对象
                //var sensors_info_jsonStr_test = JsonConvert.SerializeObject(sensors_info_jsonObj_test);
                //var targets_test = $"S_{sensors_info_jsonStr_test}";
                //_peer.SendAsync(Encoding.UTF8.GetBytes(targets_test ?? ""), dev.Lip, dev.Lport);
                //return default;
                #endregion
                string loginUrl = SpectrumUrl + "/api/realtime/data/get";
                var request = new RestRequest(loginUrl, Method.GET);

                request.AddHeader("Content-Type", "application/json");
                //string base64Credentials = GetEncodedCredentials();
                request.AddHeader("x-authorization", token);
                var response = _client?.Execute<TrackRoot>(request);
                if (response.Data != null && response.Data.data != null && response.Data.success)
                {
                    if (response?.Data.data.staticInfo.listenerSum > 0)
                    {

                        //foreach (var uav in response?.Data?.data?.listenerList.FirstOrDefault()?.uavList ?? new List<UavListItem>())
                        //{
                        //    foreach (var zone in _zones)
                        //    {
                        //        var res = GisTool.isPointInPolygon(new GisTool.Position() { Lat = double.Parse(uav.uavLatitude), Lng = double.Parse(uav.uavLongitude) }, zone);
                        //        if (res)
                        //        {
                        //            track.Add(uav);
                        //            break;
                        //        }
                        //    }
                        //}
                        

                        var sensors_info_jsonObj = response?.Data.data.listenerList[0].uavList;//目标信息json对象

                        var sensors_info_jsonStr = JsonConvert.SerializeObject(sensors_info_jsonObj);



                        var devices_info_jsonObj = response?.Data.data.listenerList[0];//设备信息json对象

                        var devices_info_jsonStr = JsonConvert.SerializeObject(devices_info_jsonObj);

                        var targets = $"S_{sensors_info_jsonStr}";
                        var status = $"T_{devices_info_jsonStr}";
                        _peer.SendAsync(Encoding.UTF8.GetBytes(status ?? ""), dev.Lip, dev.Lport);
                        _peer.SendAsync(Encoding.UTF8.GetBytes(targets ?? ""), dev.Lip, dev.Lport);


                    }

                    return Task.FromResult(response.Data);
                }
                else
                {
                    isSuccess = false;
                    return default;
                }



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Report(int id, string freq)
        {
            try
            {
                string token = HMACToken(DateTime.Now.ToTimeStamp().ToString());
                ReportUrl = $"https://gaj.sh.gov.cn/wrj/index.php/admin/outfunc/xintan_devices?timestamp={DateTime.Now.ToTimeStamp()}&token={token}";
                //ReportUrl = $"http://zjlin123.com/shba_gk/index.php/admin/outfunc/xintan_devices?timestamp={DateTime.Now.ToTimeStamp()}&token={token}";
                //ReportUrl = $"http://15.75.0.124/wrj/index.php/admin/outfunc/handle_data3?timestamp={DateTime.Now.ToTimeStamp()}&token={token}&sign=uavhf2020";

                //var targets = _memory.GetAllTargets();
                var dev = _memory.GetDevice();
                string devId = "2000000405";
                //string devId = "2000000411";
                var Reports = new ReportOpt()
                {
                    deviceid = devId,
                    latitude = dev.Lat.ToString(),
                    longitude = dev.Lng.ToString(),
                    version = "0",
                    statusCode = "1",
                    aeroscopeID = "0QRDG9RR03QSE0",
                    source = "4",
                    timestamp = DateTime.Now.ToTimeStamp().ToString(),
                    remarkInfo = ""

                };

                var request = new RestRequest(ReportUrl, Method.POST);

                request.AddHeader("Content-Type", "application/json");

                request.AddJsonBody(Reports.ToJson());

                var response = _client?.Execute(request);
            }
            catch (Exception ex)
            {


            }
        }
        public string HMACToken(string timestamp)
        {

            string message = timestamp + "uavhf2020";
            byte[] buffer = System.Text.Encoding.Default.GetBytes(message);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] somme = check.ComputeHash(buffer);
                string ret = "";
                foreach (byte a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }


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

    }
    public class RequestLogin
    {
        public string username { get; set; }
        public string password { get; set; }

    }
    public class UserBaseInfoDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string area { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string logo { get; set; }
        /// <summary>
        /// 迈疆智能
        /// </summary>
        public string company { get; set; }
        /// <summary>
        /// 迈疆无人机预警管理平台
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lang { get; set; }
    }

    public class LoginData
    {
        /// <summary>
        /// 
        /// </summary>
        public UserBaseInfoDTO userBaseInfoDTO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> permissionList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
    }
    /// <summary>
    /// 目标位置信息
    /// </summary>
    public class SHReportModelNEW
    {
        /// <summary>
        /// 经度
        /// </summary>
        public string longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string latitude { get; set; }
        /// <summary>
        /// 相对高度
        /// </summary>
        public string altitude { get; set; }
        /// <summary>
        /// 翻滚
        /// </summary>
        public string roll { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public double source { get; set; }
        /// <summary>
        /// 设备SN号
        /// </summary>
        public string aeroscopeID { get; set; }
        /// <summary>
        /// 绝对高度
        /// </summary>
        public string absoluteHeight { get; set; }

        /// <summary>
        /// 俯仰
        /// </summary>
        public string pitch { get; set; }
        /// <summary>
        /// 飞行架次
        /// </summary>
        public string orderID { get; set; }
        public string appLongitude { get; set; }
        public string appLatitude { get; set; }
        public string homeLongitude { get; set; }
        public string homeLatitude { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string droneType { get; set; }
        /// <summary>
        /// sn
        /// </summary>
        public string droneID { get; set; }
        public string flightTime { get; set; }
        /// <summary>
        /// 阵地ID
        /// </summary>
        public string deviceid { get; set; }

    }
    public class LoginMessage
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

    public class LoginRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public LoginData data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LoginMessage message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
    }

    //public class PoliceReport
    //{
    //    public string device_id { get; set; }
    //    public string device_status { get; set; }
    //    public string device_lng { get; set; }
    //    public string device_lat { get; set; }
    //    public string speed { get; set; }
    //    public string height { get; set; }
    //    ///记录时间
    //    public string addtime { get; set; }
    //    /// <summary>
    //    /// 航向
    //    /// </summary>
    //    public string direction { get; set; }
    //    /// <summary>
    //    /// 起飞点经度
    //    /// </summary>
    //    public string lng_home { get; set; }
    //    /// <summary>
    //    /// 起飞点纬度
    //    /// </summary>
    //    public string lat_home { get; set; }
    //    /// <summary>
    //    /// 机头指向
    //    /// </summary>
    //    public string yaw { get; set; }
    //    /// <summary>
    //    /// 俯仰角
    //    /// </summary>
    //    public string pitch { get; set; }
    //    /// <summary>
    //    /// 目标翻滚角
    //    /// </summary>
    //    public string roll { get; set; }
    //    /// <summary>
    //    /// 目标型号
    //    /// </summary>
    //    public string uav_type { get; set; }
    //    /// <summary>
    //    /// 目标sn
    //    /// </summary>

    //    public string uav_sn_number { get; set; }
    //    /// <summary>
    //    /// 目标经度
    //    /// </summary>
    //    public string tlng { get; set; }
    //    ///目标纬度
    //    public string tlat { get; set; }
    //    /// <summary>
    //    /// 飞手经度
    //    /// </summary>
    //    public string lng_gps { get; set; }
    //    /// <summary>
    //    /// 飞手纬度
    //    /// </summary>
    //    public string lat_gps { get; set; }


    //}

    public class PoliceReport
    {
        public string longitude { get; set; }

        public string latitude { get; set; }

        public string altitude { get; set; }

        public string roll { get; set; }

        public string source { get; set; }

        public string aeroscopeID { get; set; }

        public string absoluteHeight { get; set; }

        public string pitch { get; set; }

        public string orderID { get; set; }

        public string appLongitude { get; set; }

        public string appLatitude { get; set; }

        public string homeLongitude { get; set; }

        public string homeLatitude { get; set; }

        public string droneType { get; set; }

        public string droneID { get; set; }

        public string flightTime { get; set; }

        public string deviceid { get; set; }


    }
    public class ReportResponseRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
    }
    public class ReportOpt
    {
        /// <summary>
        /// 设备id
        /// </summary>
        public string deviceid { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        public string statusCode { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        public string longitude { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        public string latitude { get; set; }
        /// <summary>
        /// 设备sn号
        /// </summary>
        public string aeroscopeID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarkInfo { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 电量剩余
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 设备id
        /// </summary>
        public string version { get; set; }





    }
    public class SHReportModel
    {
        /// <summary>
        /// 目标ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属设备ID
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        public double Alt { get; set; }
        /// <summary>
        /// 航向
        /// </summary>
        public string FlyDirection { get; set; }
        /// <summary>
        /// 切向速度
        /// </summary>
        public double V { get; set; }

        /// <summary>
        /// 航迹时间
        /// </summary>
        public DateTime TrackTime { get; set; }
        public string AppLng { get; set; }
        public string AppLat { get; set; }
        public string HomeLng { get; set; }
        public string HomeLat { get; set; }
        public DateTime BeginAt { get; set; }
        public string UAVType { get; set; }
        public string UAVSn { get; set; }
        public string AppAddr { get; set; }
    }

    public class TempZpointModel
    {
        public string type { get; set; }

        public List<List<double>> coordinates { get; set; }

        public string layout { get; set; }
    }
}
