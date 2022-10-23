using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.Service;
using AntiUAV.DeviceServer.Abstractions.Service.ServiceImpl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Serrvice
{
    /// <summary>
    /// 服务综合操作
    /// </summary>
    public class ServiceOpt : ServiceOptBase
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _device;
        private readonly IPreWarningZoneService _perWarn;

        public ServiceOpt(ILogger<ServiceOpt> logger, IMemoryCache memory, IDeviceService device,
            IPreWarningZoneService perWarn, IPeerServer peer, IDeviceOptService devOpt,
            IMemoryBusEvent memoryBus) : base(memory, peer, devOpt, memoryBus)
        {
            _logger = logger;
            _device = device;
            _perWarn = perWarn;
        }

        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public override async Task<bool> InitializationCacheAsync(int deviceId)
        {
            var info = ReloadDeviceInfoAsync(deviceId);//初始化设备信息
            var zone = ReloadDeviceZoneAsync(deviceId);//初始化预警区信息
            if (info.Result && zone.Result)
            {
                var status = await _device.GetStatusOne(deviceId);
                if (status == null)
                {
                    var dev = _memory.GetDevice();//获取设备信息
                    _logger.LogInformation($"device server initialization finished. (devId:{dev?.Id},devCategory:{dev?.Category})");
                    return true;
                }
                _logger.LogInformation($"device server initialization finished, but devServ already exist. (devId:{deviceId})");
                return false;
            }
            _logger.LogInformation($"device server initialization fail. (devId:{deviceId})");
            return false;
        }

        /// <summary>
        /// 重新加载设备信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public override async Task<bool> ReloadDeviceInfoAsync(int deviceId)
        {
            static DeviceInfo ConvertToDsServMod(DbOrm.AntiUAV.Entity.DeviceInfo dbInfo)
            {
                if (dbInfo == null) return default;
                return new DeviceInfo()
                {
                    Id = dbInfo.Id,
                    Name = dbInfo.Name,
                    Display = dbInfo.Display,
                    Alt = dbInfo.Alt,
                    Lat = dbInfo.Lat,
                    Lng = dbInfo.Lng,
                    RectifyAz = dbInfo.RectifyAz,
                    RectifyEl = dbInfo.RectifyEl,
                    Category = dbInfo.Category,
                    CoverR = dbInfo.CoverR,
                    CoverS = dbInfo.CoverS,
                    CoverE = dbInfo.CoverE,
                    Ip = dbInfo.Ip,
                    Lip = dbInfo.Lip,
                    Port = dbInfo.Port,
                    Lport = dbInfo.Lport,
                    ProbeReportingInterval = dbInfo.ProbeReportingInterval,
                    StatusReportingInterval = dbInfo.StatusReportingInterval,
                    ThreadAssessmentCount = dbInfo.ThreadAssessmentCount,
                    TargetTimeOut = dbInfo.TargetTimeOut
                };
            }
            var info = await _device.GetAsync(deviceId);
            return _memory.ReloadDevice(ConvertToDsServMod(info));
        }

        /// <summary>
        /// 重新加载预警区
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public override async Task<bool> ReloadDeviceZoneAsync(int deviceId)
        {
            var zones = await _perWarn.GetAnyAsync(x => 1 == 1);
            return _memory.ReloadZones(ConvertToDsServMod(zones));
        }

        public override Task<bool> SetRunMode(string mode, int timeout = 3000)
        {
            return Task.FromResult(false);
        }

        public static PerWarningZoneInfoCollection ConvertToDsServMod(IEnumerable<DbOrm.AntiUAV.Entity.PerWarningZoneInfo> zones)
        {
            var perZones = new PerWarningZoneInfoCollection();
            if (zones?.Count() > 0)
            {
                perZones.Zones.Clear();
                perZones.Zones.AddRange(zones.Select(x => new PerWarningZoneInfo()
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalDistance = x.NormalDistance,
                    ADistance = x.ADistance,
                    BDistance = x.BDistance,
                    CircumcircleLat = x.CircumcircleLat,
                    CircumcircleLng = x.CircumcircleLng,
                    CircumcircleR = x.CircumcircleR,
                    CircumcircleRadiationR = x.CircumcircleRadiationR,
                    Updatetime = x.Updatetime,
                    ZPoints = x.ZonePoints,
                    ZPointsPosition = x.ZonePointsPosition.ToObject<List<GisTool.Position>>()
                }));
            }
            return perZones;
        }
    }
}
