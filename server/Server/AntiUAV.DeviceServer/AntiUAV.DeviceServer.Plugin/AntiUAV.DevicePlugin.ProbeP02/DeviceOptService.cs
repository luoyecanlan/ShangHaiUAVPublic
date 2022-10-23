using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeP02
{
    public class DeviceOptService : DeviceOptServiceBase
    {
        public override int DeviceCategory => PluginConst.Category;
    }
}
