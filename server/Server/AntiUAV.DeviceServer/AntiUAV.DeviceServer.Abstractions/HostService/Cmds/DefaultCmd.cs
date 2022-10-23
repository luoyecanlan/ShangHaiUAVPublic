using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Cmds
{
    public class DefaultCmd : IPeerCmd
    {
        public DefaultCmd(PeerCmdType cmdType)
        {
            Order = cmdType;
        }
        public int Category => -1;

        public string Key => "default";

        public PeerCmdType Order { get; }

        public Task Invoke(IPeerContent content) => Task.CompletedTask;
    }
}
