using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DB;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeP01.Cmd
{
    public class A_TrackCmd : IPeerCmd
    {
        public A_TrackCmd(ILogger<A_TrackCmd> logger, IMemoryCache memory, GisTool tool, IServiceProvider provider)
        {
            _logger = logger;
            _memory = memory;
            _tool = tool;
            var dev = _memory.GetDevice();
            _host = provider.GetServices<IDeviceHostService>().FirstOrDefault(x => x.DeviceCategory == dev.Category);
        }

        public int Category => PluginConst.Category;
        public string Key => PluginConst.TrackCmdKey;
        public PeerCmdType Order => PeerCmdType.Action;

        private readonly ILogger<A_TrackCmd> _logger;
        private readonly IMemoryCache _memory;
        private readonly IDeviceHostService _host;
        private readonly GisTool _tool;

        public async Task Invoke(IPeerContent content)
        {
            try
            {
                var targets_json = Encoding.UTF8.GetString(content.Source, 2, content.Source.Length - 2);
                var targetInfovar = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, TargetInfoSpectrum>>(targets_json);
                var dev = _memory.GetDevice();
                var tgs = new List<TargetInfo>();
                //var track = content.Source.ToStuct<R_ProbeR04_Track>();
                //var devInfovar = _memory.Get("spectrum_devices_info");
                if (targetInfovar != null)
                {
                    
                    foreach (KeyValuePair<string, TargetInfoSpectrum> temp in targetInfovar)
                    {
                        if (temp.Key.Contains("dr"))
                        {
                            var tg = MapToTargetInfo(temp.Value, dev);
                            tgs.Add(tg);
                        }
                    }
                    //var tg = MapToTargetInfo(track, dev);
                    //tgs.Add(tg);
                    await _memory.UpdateTarget(tgs.ToArray());
                    //content.SourceAys = tgs;
                    _logger.LogDebug($"recive dev:{dev.Id}({dev.Category}) target {tgs.Count()},source track count:{"TrackCount"}.");
                    
                }
                //return Task.CompletedTask;
                
            }
            catch (Exception ex)
            {
                _logger.LogWarning("The A_TrackCmd Command failed detection");
                await Task.FromCanceled(new System.Threading.CancellationToken());
                throw ex;
            }
        }
        GisTool.VelocityVector targetSpeed = null;
        TargetInfo MapToTargetInfo(TargetInfoSpectrum track, DeviceInfo dev)
        {
            int idNo = Int32.Parse(track.id, System.Globalization.NumberStyles.HexNumber);
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{idNo}";
            float altitude = 100;
            #region 计算目标速度
            IEnumerable<TargetCacheInfo> targets = _memory.GetAllTargets();
            TargetCacheInfo currentTarget = null;
            foreach (TargetCacheInfo targetCacheInfo in targets)
            {
                if (targetCacheInfo?.Points[0]?.Id == id)
                {
                    currentTarget = targetCacheInfo;
                    break;
                }
            }

            if (currentTarget != null && currentTarget.Points.Count > 6)
            {
                TargetPosition centerTarget = currentTarget.Points[currentTarget.Points.Count - 6];
                TargetPosition lastTarget = currentTarget.Points[currentTarget.Points.Count - 1];
                //TargetCacheInfo centerTarget = targets.ElementAt(targets.Count() / 2);
                //TargetCacheInfo lastTarget = targets.ElementAt(targets.Count());

                GisTool.Position position1 = new GisTool.Position()
                {
                    Lng = centerTarget.Lng,
                    Lat = centerTarget.Lat,
                    UpdateTime = centerTarget.TrackTime,
                    Altitude = altitude

                };
                GisTool.Position position2 = new GisTool.Position()
                {
                    Lng = lastTarget.Lng,
                    Lat = lastTarget.Lat,
                    UpdateTime = lastTarget.TrackTime,
                    Altitude = altitude

                };
                targetSpeed = _tool.GetTargetAbsoluteSpeed(position1, position2, (position2.UpdateTime - position1.UpdateTime).TotalSeconds);
            }
            #endregion

            var tg = new TargetInfo
            {
                ProbeAz=track.azimuth,
                Id = id,
                Alt = altitude,
                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                Lat = track.gps.lat,
                Lng = track.gps.lng,
                Freq = Convert.ToDouble(track.frequency.Substring(0,track.frequency.Length - 3)),
                ProbeDis = _tool.CalculatingDistance(new GisTool.Position() { Lng = dev.Lng, Lat = dev.Lat }, new GisTool.Position() { Lng = track.gps.lng, Lat = track.gps.lat }),
                Category = 1,
                //Mode = track.SendModel,
                Vr = targetSpeed==null?0: targetSpeed.Speed,
                Vt = targetSpeed == null ? 0 : targetSpeed.Speed,
                TrackTime = DateTime.Now,
                Threat = _memory.GetThreat(id)
            };
            //var devPosition = new GisTool.Position() { Lat = dev.Lat, Lng = dev.Lng, Altitude = dev.Alt };
            //var position = _tool.Convert3DPosition(devPosition, tg.ProbeAz, tg.ProbeDis, tg.ProbeHigh);//计算经纬海拔
            //tg.Lat = Math.Round(position.Lat, 6);
            //tg.Lng = Math.Round(position.Lng, 6);
            //tg.Alt = Math.Round(position.Altitude, 1);
            return tg;
        }
    }


}
