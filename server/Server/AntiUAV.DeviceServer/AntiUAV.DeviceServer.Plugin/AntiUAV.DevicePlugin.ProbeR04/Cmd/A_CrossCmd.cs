using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR04.Cmd
{
    public class A_CrossCmd : IPeerCmd
    {
        public A_CrossCmd(ILogger<A_StatusCmd> logger, IMemoryCache memory)
        {
            _memory = memory;
            _logger = logger;
        }

        public int Category => PluginConst.Category;
        public string Key => PluginConst.CrossCmdKey;
        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;

        public Task Invoke(IPeerContent content)
        {
            var head = new byte[PluginConst.CrossCheckHead.Length];
            Buffer.BlockCopy(content.Source, 0, head, 0, head.Length);
            var checkCmd = Enumerable.SequenceEqual(PluginConst.CrossCheckHead, head);
            if(checkCmd)
            {
                var data = content.Source.ToStuct<R_ProbeR04_Cross>();
                //TODO 设备运行码待确认
                _memory.UpdateDeviceRunInfo(10001, data.RadarCorner, 0);
                return Task.FromResult(true);
            }
            else
            {
                _logger.LogWarning("The A_CrossCmd Command failed detection");
                return Task.FromCanceled(new System.Threading.CancellationToken());
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_ProbeR04_Cross
    {
        public uint Head; //帧头	Unsigned Int	4	32bit	0x55CCCC55	-	-
        public ushort Lenght; //报文字节数 Unsigned Short	2	16bit	64	-	-
        public ushort RadarId; //雷达ID	Unsigned Short	2	16bit	-	-	-
        public ulong TimeSpan; //时间戳	Unsiged  Int64	8	64bit		ms	UTC时间
        public ushort RadarCorner; // 当前雷达转角	Unsigned short	2	16bit	0～36000	 0.01°	-
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
        public byte[] Resv; //Resv	Unsigned Char	42	8bit*42	-	-	-
        public uint CheckCode; //校验和	Unsigned Int	4	32bit	-	-	32bit累加和
    }
}
