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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeP04.Cmd
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
               // _logger.LogInformation(JsonConvert.SerializeObject(targets_json) + "11111111111111111.22222222222222");
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
                            var tg = MapToTargetInfo(temp.Value, dev,temp.Key);
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
                _logger.LogWarning(ex.Message+"The A_TrackCmd Command failed detection");
                await Task.FromCanceled(new System.Threading.CancellationToken());
                throw ex;
            }
        }
        GisTool.VelocityVector targetSpeed = null;
        TargetInfo MapToTargetInfo(TargetInfoSpectrum track, DeviceInfo dev,string drid)
        {
           // _logger.LogInformation(JsonConvert.SerializeObject(track) + "TRACK");
            //int idNo = Int32.Parse(track.id, System.Globalization.NumberStyles.HexNumber);
            //var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{track.id}";
            var id = $"P{dev.Category}.{dev.Id}.{_host.RunCode}-{drid}";
            float altitude = 100;
            //var t = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, ProbeInfo>>(JsonConvert.SerializeObject(track.ld_result));
            string str = JsonConvert.SerializeObject(track.ld_result);
            //var obj = JsonConvert.DeserializeObject<JObject>(str);
            //var temp = JObject.Parse(obj["ld_result"].ToString());
            //_logger.LogInformation(JsonConvert.SerializeObject(str) + "************************");
            var targetInfovar = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, ProbeInfo>>(str);
            //_logger.LogInformation(JsonConvert.SerializeObject(targetInfovar)+"==================");
            double az=0;
            if (targetInfovar != null)
            {

                foreach (KeyValuePair<string, ProbeInfo> temp in targetInfovar)
                {
                    if (temp.Key.Contains("SF"))
                    {
                        
                        az = temp.Value.azimuth;
                    }
                }
                //var tg = MapToTargetInfo(track, dev);
                //tgs.Add(tg);
                
            }

            var tg = new TargetInfo
            {
                ProbeAz= az,
                Id = id,
                Alt = altitude,
                DeviceId = dev.Id,
                CoordinateType = TargetCoordinateType.LongitudeAndLatitude,
                Lat = track.gps.lat,
                Lng = track.gps.lng,
                Freq = Convert.ToDouble(track.frequency.Substring(0,track.frequency.Length - 3)),
                //ProbeDis = _tool.CalculatingDistance(new GisTool.Position() { Lng = dev.Lng, Lat = dev.Lat }, new GisTool.Position() { Lng = track.gps.lng, Lat = track.gps.lat }),
                Category = 1,
                UAVSn=track.id,
                //Mode = track.SendModel,
                //Vr = targetSpeed==null?0: targetSpeed.Speed,
                //Vt = targetSpeed == null ? 0 : targetSpeed.Speed,
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
