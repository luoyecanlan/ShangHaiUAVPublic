using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR01.Cmd
{
    public class A_TrackCmd : IPeerCmd
    {
        public A_TrackCmd(ILogger<A_TrackCmd> logger, IMemoryCache memory, IServiceProvider provider)
        {
            _logger = logger;
            _memory = memory;
            var dev = _memory.GetDevice();
            _host = provider.GetServices<IDeviceHostService>().FirstOrDefault(x => x.DeviceCategory == dev.Category);
        }
        public int Category => PluginConst.Category;

        public string Key => PluginConst.TrackCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;


        private readonly ILogger<A_TrackCmd> _logger;
        private readonly IMemoryCache _memory;
        private readonly IDeviceHostService _host;

        public async Task Invoke(IPeerContent content)
        {
            //var gistool = new GisTool();

            //提取json中的数据,给到tg
            var dev = _memory.GetDevice();
            var json = Encoding.UTF8.GetString(content.Source) ?? "";
            var data = json.ToObject<TrackData>();
            if (data != null)
            {
                var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{data.Snum}";
                var tg = new TargetInfo()
                {
                    Id = id,
                    DeviceId = dev.Id,
                    
                    Alt = data.Alt==Double.NaN?0:data.Alt,
                    Category = data.Category,
                    CoordinateType = TargetCoordinateType.Perception,
                    TrackTime = DateTime.Now,
                    Vt = data.V,
                    Vr = data.VAz,
                    Lat = data.Lat == Double.NaN ? 0 : data.Lat,
                    Lng = data.Lng == Double.NaN ? 0 : data.Lng,
                    Threat = _memory.GetThreat(id),
                    Freq = -1
                };
                var po = new GisTool().Convert3DPositionAzimuthAndPitchInfo(new GisTool.Position() {Lat=tg.Lat,Lng=tg.Lng,Altitude=tg.Alt }, new GisTool.Position() { Lat = 38.0803712, Lng = 115.7667678, Altitude = 16.093 });
                if (po != null)
                {
                    tg.ProbeAz = po.Az;
                    tg.PeobeEl = po.El;
                    tg.ProbeDis = po.Dis;
                }
                
                var tgs = new List<TargetInfo>() { tg };
                await _memory.UpdateTarget(tgs.ToArray());
                content.SourceAys = tgs;
                _logger.LogInformation($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()}.");
            }
            else
                _logger.LogWarning($"recive dev:{dev.Id}({dev.Category}) target but unknow data.({json})");
        }
    }

    public class TrackData
    {
        public int Snum { get; set; }

        public int Category { get; set; }

        public double Alt { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public bool IsHover { get; set; }

        public double V { get; set; }

        public double VAz { get; set; }

        public long TrackTime { get; set; }
    }
}
