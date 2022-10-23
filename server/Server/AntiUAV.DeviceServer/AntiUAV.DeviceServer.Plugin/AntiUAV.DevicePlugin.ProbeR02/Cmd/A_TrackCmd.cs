using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DevicePlugin.ProbeD01.Cmd;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
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

namespace AntiUAV.DevicePlugin.ProbeR02.Cmd
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
        }

        public int Category => PluginConst.Category;

        public string Key => PluginConst.TrackCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly ILogger<A_TrackCmd> _logger;
        private readonly IMemoryCache _memory;
        private readonly GisTool _tool;
        private readonly IDeviceHostService _host;
        public async Task Invoke(IPeerContent content)
        {
            var dev = _memory.GetDevice();
            var data = content.Source.ToStuct<R_SWJ_Track>();
            var t_time = DateTime.Now;//data.Body.Time.ToDateTime();
            var tgs = new List<TargetInfo>();
            var index = Marshal.SizeOf<R_SWJ_Track>();
            var step = Marshal.SizeOf<R_SWJ_Plot>();
            //var test_thread = 0d;
            if (data.Body.Count > 0)
            {
                var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };
                while (index + 12 < content.Source.Length)
                {
                    var tmp = new byte[step];
                    Buffer.BlockCopy(content.Source, index, tmp, 0, step);
                    var tgp = tmp.ToStuct<R_SWJ_Plot>();
                    var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{tgp.Snum}";
                    var tg = new TargetInfo()
                    {
                        DeviceId = dev.Id,
                        Id = id,
                        CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                        Freq = -1,
                        ProbeAz = Math.Round(tgp.Az, 3),
                        PeobeEl = Math.Round(tgp.El, 3),
                        ProbeDis = Math.Round(tgp.Dis, 1),
                        ProbeHigh = Math.Round(tgp.Alt * 1000, 1),
                        Category = 1,
                        Mode = tgp.Mode,
                        Vr = Math.Abs(Math.Round(tgp.Vr, 1)),
                        Vt = Math.Abs(Math.Round(tgp.Vt, 1)),
                        TrackTime = t_time,
                        AppAddr = "美国加利福尼亚州靠山屯镇卞庄子村车道沟路马甲屯桥北街第二个路口左转第一个小茅房左边蹲坑",

                        AppLat = 31.344178,
                        AppLng = 121.609196,
                        BeginAt=DateTime.Now,
                        UAVSn="56757usau72742b",
                        UAVType="大疆精灵4",
                        Threat = _memory.GetThreat(id)
                    };
                    //var last=_memory.GetTargetById(id);
                    //if (tg.AppLat != last?.Last.AppLat || tg.AppLng !=last?.Last.AppLng)
                    //{
                    //    string lng = "121.5029907";
                    //    string lat = "31.2389796";
                    //    var AdrObj = AliAdrCode(lng, lat);

                    //    //var AdrObj = AliAdrCode(tg.HomeLng.ToString(), tg.HomeLat.ToString());
                    //    string Addr = AdrObj?.regeocode.formatted_address;
                    //    tg.AppAddr = Addr;
                    //}
                    var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔
                    tg.Lat = Math.Round(position.Lat, 6);
                    tg.Lng = Math.Round(position.Lng, 6);
                    tg.Alt = Math.Round(position.Altitude, 1);
                    //tg.Lat = Math.Round(tgp.Az, 3);
                    //tg.Lng = Math.Round(tgp.El, 3);
                    //tg.Alt = Math.Round(tgp.Alt * 1000, 1);
                    tgs.Add(tg);
                    index += step;
                    //test_thread++;
                    //if (test_thread > 3) test_thread = 0;
                }
            }

            await _memory.UpdateTarget(tgs.ToArray());
            content.SourceAys = tgs;
            //_logger.LogDebug($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()}.");
        }
        public AliGeoCodeRootobject AliAdrCode(string HomeLng, string HomeLat)
        {
            string geoCodeUrl = $"{AliGeoCodeUrl}key={PrivateKey}&location={HomeLng},{HomeLat}";


            var request = new RestRequest(geoCodeUrl, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute<AliGeoCodeRootobject>(request).Content;
            var data = JsonConvert.DeserializeObject<AliGeoCodeRootobject>(response);
            return data;
        }
        //高德逆地理编码url
        string AliGeoCodeUrl = "https://restapi.amap.com/v3/geocode/regeo?";
        string PrivateKey = "d08f89376ca2e8be4e4cfd8b6a7bd907";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_SWJ_Track
    {
        public int Head;//0x7A707978
        public int ProtocalNum;//0x2329
        public int Length;//20+bodylength
        public int Order;
        public long Time;
        public R_SWJ_Track_Body Body;
        //public int Index;
        //public uint Check;
        //public int End;//0x7B7A7079
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_SWJ_Track_Body
    {
        public uint Snum;
        public long Time;
        public float Bo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserve;
        public uint Count;
        //[MarshalAsAttribute(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct)]
        //public R_SWJ_Plot[] Plots;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_SWJ_Plot
    {
        public uint Snum;
        public float Az;
        public float Dis;
        public float Alt;
        public float Vr;//径向速度
        public float Vt;//切向速度
        public byte Mode;
        public byte CategoryS;//疑似类别
        public byte Category;//类别
        public float RCS;
        public byte Hover;
        public float El;
    }
}
