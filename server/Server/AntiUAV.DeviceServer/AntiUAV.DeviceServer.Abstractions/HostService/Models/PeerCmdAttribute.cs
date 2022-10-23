using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    public class PeerCmdAttribute : Attribute
    {
        public PeerCmdAttribute()
        {

        }
        public PeerCmdAttribute(bool must = false, bool only = false)
        {
            Must = must;
            Only = only;
        }
        public bool Must { get; }

        public bool Only { get; }
    }
}
