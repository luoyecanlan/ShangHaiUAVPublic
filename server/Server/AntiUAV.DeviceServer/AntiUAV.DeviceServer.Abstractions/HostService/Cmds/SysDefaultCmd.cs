using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Cmds
{
    public class SysDefaultCmd : IPeerSysCmd
    {
        public SysDefaultCmd(PeerSysCmdType cmdType)
        {
            Order = cmdType;
        }

        public string Key => "default";

        public PeerSysCmdType Order { get; }

        public Task<bool> Invoke(IPeerContent content) => Task.FromResult(true);
    }
}
