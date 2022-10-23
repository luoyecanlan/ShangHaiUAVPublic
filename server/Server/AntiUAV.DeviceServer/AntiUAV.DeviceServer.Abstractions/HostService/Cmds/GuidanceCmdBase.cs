using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AntiUAV.DeviceServer.Abstractions.Service;
using AntiUAV.Bussiness;
using EasyNetQ.Logging;
using Newtonsoft.Json;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Cmds
{
    public abstract class GuidanceCmdBase : IPeerCmd
    {
        public GuidanceCmdBase(IMemoryCache memory, IPeerServer peer)
        {
            _memory = memory;
            _peer = peer;
        }
        protected readonly IMemoryCache _memory;
        private readonly IPeerServer _peer;

        public abstract int Category { get; }

        public abstract string Key { get; }

        public PeerCmdType Order => PeerCmdType.Middleware;

        /// <summary>
        /// 判断是不是目标指令
        /// </summary>
        /// <returns></returns>
        public abstract bool IsTrackTarget(string route);

        public virtual GuidancePositionInfo ToGuidance(TargetInfo tg)
        {
            var gpi = new GuidancePositionInfo(tg.Id)
            {
                TrackTime = tg.TrackTime,
                TargetId = tg.Id
            };
            switch (tg.CoordinateType)
            {
                case TargetCoordinateType.LongitudeAndLatitude:
                    gpi.Coordinate = tg.CoordinateType;
                    gpi.TargetX = tg.Lng;
                    gpi.TargetY = tg.Lat;
                    gpi.TargetZ = tg.Alt;
                    break;
                case TargetCoordinateType.Perception:
                    gpi.Coordinate = tg.CoordinateType;
                    gpi.TargetX = tg.ProbeAz;
                    gpi.TargetY = tg.PeobeEl;
                    gpi.TargetZ = tg.ProbeDis;
                    break;
            }
            return gpi;
        }

        public virtual Task Invoke(IPeerContent content)
        {
            var gds = _memory.GetRelationshipsGuidance();
            if (IsTrackTarget(content?.Route))
            {
                var tgs = content.SourceAys as List<TargetInfo>;
                foreach (var gd in gds)
                {
                    var tg = tgs?.FirstOrDefault(x => x.Id == gd.TargetId);
                    if (tg != null)
                    {
                        _ = _peer.SendAsync(ToGuidance(tg).ToBytes("BEGD"), gd.ToAddressIp, gd.ToAddressPort);
                        Console.WriteLine($"引导命令打印,tg:{JsonConvert.SerializeObject(tg)}");
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
