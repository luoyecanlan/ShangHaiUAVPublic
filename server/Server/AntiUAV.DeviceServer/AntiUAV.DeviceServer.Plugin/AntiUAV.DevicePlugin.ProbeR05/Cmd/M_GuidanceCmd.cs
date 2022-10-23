using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Cmds;
using Microsoft.Extensions.Caching.Memory;

namespace AntiUAV.DevicePlugin.ProbeR05.Cmd
{
    public class M_GuidanceCmd : GuidanceCmdBase
    {
        public M_GuidanceCmd(IMemoryCache memory, IPeerServer peer) : base(memory, peer)
        {
        }

        public override int Category => PluginConst.Category;

        public override string Key => PluginConst.Category.ToString();

        public override bool IsTrackTarget(string route) => route == PluginConst.TrackCmdKey;

    }
}
