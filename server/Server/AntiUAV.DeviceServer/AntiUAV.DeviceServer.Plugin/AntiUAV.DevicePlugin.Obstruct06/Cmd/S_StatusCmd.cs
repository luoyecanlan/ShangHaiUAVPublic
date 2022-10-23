using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct06.Cmd
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


            var dev = _memory.GetDevice();

            var data = content.Source.ToStuct<ObstructQueryDeviceStatusReceive>();
            if (data.Data.Status1!=(byte)0|| data.Data.Status2 != (byte)0 || data.Data.Status3 != (byte)0 || data.Data.Status4 != (byte)0 || data.Data.Status5 != (byte)0 || data.Data.Status6 != (byte)0)
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Running);
            }
            else
            {
                _memory.UpdateDeviceRun(DeviceStatusCode.Free);
            }
           
            return Task.CompletedTask;

         
        }
    }
}
