using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeP02.Cmd
{
    public class A_StatusCmd : IPeerCmd
    {
        public A_StatusCmd(ILogger<A_StatusCmd> logger, IMemoryCache memory)
        {
            _memory = memory;
            _logger = logger;
        }
        public int Category => PluginConst.Category;
        public string Key => PluginConst.StatusCmdKey;
        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;

        public Task Invoke(IPeerContent content)
        {


            //var dev = _memory.GetDevice();
            //var status_json = Encoding.UTF8.GetString(content.Source, 2, content.Source.Length - 2);
            //var datas = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, devs>>(status_json);
            ////var sensor_info = _memory.Get("spectrum_sensors_info");
            ////devs[] datas = sensor_info as devs[];

            //if (datas != null)
            //{
            //    foreach (KeyValuePair<string, devs> data in datas)
            //    {
            //        //dev.Alt = data.Value.altitude;
            //        //dev.Lng = data.Value.gps.lng;
            //        //dev.Lat = data.Value.gps.lat;
            //        if (data.Value.status != "disconnected")
            //        {
            //            _memory.UpdateDeviceRun(DeviceStatusCode.Free);//设备正常运行

            //        }
            //        else
            //        {
            //            _memory.UpdateDeviceRun(DeviceStatusCode.OffLine);//连接状态 0-异常
            //        }
            //    }

            return Task.CompletedTask;
            //}
            //else //无设备（空闲）
            //{
            //    _memory.UpdateDeviceRun(DeviceStatusCode.Free);
            //}
            //return Task.CompletedTask;

            //else
            //{
            //    _logger.LogWarning("The A_StatusCmd Command failed detection");
            //    return Task.FromCanceled(new System.Threading.CancellationToken());
            //}
        }
    }

}
