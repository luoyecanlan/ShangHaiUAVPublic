using AntiUAV.DeviceServer.Abstractions.HostService.Cmds;
using Microsoft.Extensions.Caching.Memory;

namespace AntiUAV.DevicePlugin.ProbeR04.Cmd
{
    public class M_TurnCmd : TurnCmdBase
    {
        public M_TurnCmd(IMemoryCache memory) : base(memory)
        {

        }

        public override int Category => PluginConst.Category;

        public override string Key => PluginConst.Category.ToString();

        public override bool IsTrackTarget(string route) => route == PluginConst.TrackCmdKey;
    }
}
