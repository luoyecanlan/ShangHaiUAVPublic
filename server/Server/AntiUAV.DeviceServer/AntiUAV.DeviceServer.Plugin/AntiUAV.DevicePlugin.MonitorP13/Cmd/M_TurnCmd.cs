using AntiUAV.DeviceServer.Abstractions.HostService.Cmds;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.MonitorP13.Cmd
{
    public class M_TurnCmd : TurnCmdBase
    {
        public M_TurnCmd(IMemoryCache memory) : base(memory)
        {
        }

        public override int Category => PluginConst.Category;

        public override string Key => PluginConst.Category.ToString();

        public override bool IsTrackTarget(string route) => route == PluginConst.GuidanceCmdKey;
    }
}
