using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using AntiUAV.DeviceServer;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AntiUAV.Bussiness.Models;

namespace AntiUAV.DevicePlugin.ProbeD01.Cmd
{
    public class A_StatusCmd : IPeerCmd
    {
        public A_StatusCmd(IMemoryCache memory)
        {
            _memory = memory;
        }
        public int Category => PluginConst.Category;

        public string Key => PluginConst.StatusCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;

        public Task Invoke(IPeerContent content)
        {
            var device_json = Encoding.UTF8.GetString(content.Source, 2, content.Source.Length - 2);
            var deviceInfovar = Newtonsoft.Json.JsonConvert.DeserializeObject<ListenerListItem>(device_json);
            var dev = _memory.GetDevice();


            if (deviceInfovar!=null)
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Free);
            }
            else
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Free);
            }
            //_memory.UpdateDeviceBit(data.Bit, GetBit(data.Bit));//记录设备bit信息
            
            return Task.CompletedTask;
        }
   
    }
}
