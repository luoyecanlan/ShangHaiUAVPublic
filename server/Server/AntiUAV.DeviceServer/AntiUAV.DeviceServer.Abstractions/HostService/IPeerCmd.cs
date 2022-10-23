using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService
{
    public interface IPeerCmd
    {
        int Category { get; }

        string Key { get; }

        PeerCmdType Order { get; }

        Task Invoke(IPeerContent content);
    }
}
