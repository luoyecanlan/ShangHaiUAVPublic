using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR01.Cmd
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
            _memory.UpdateDeviceRun(DeviceStatusCode.Running);//设备正常运行
            return Task.CompletedTask;
        }
    }
}
