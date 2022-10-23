using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Cmds
{
    public abstract class TurnCmdBase : IPeerCmd
    {
        public TurnCmdBase(IMemoryCache memory)
        {
            _memory = memory;
            _udp = new UdpClient();
        }

        protected readonly IMemoryCache _memory;
        private readonly UdpClient _udp;

        public abstract int Category { get; }

        public abstract string Key { get; }

        public PeerCmdType Order => PeerCmdType.Middleware;

        /// <summary>
        /// 判断是不是目标指令
        /// </summary>
        /// <returns></returns>
        public abstract bool IsTrackTarget(string route);

        public virtual Task Invoke(IPeerContent content)
        {
            var turns = _memory.GetRelationshipsTurn();//获取所有转发信息
            if (IsTrackTarget(content?.Route))
            {
                var tgs = content.SourceAys as List<TargetInfo>;
                foreach (var turn in turns)
                {
                    var tg = tgs?.FirstOrDefault(x => x.Id == turn.TargetId);
                    if (tg != null)
                    {
                        var buff = ConvertData(tg);
                        _ = _udp.SendAsync(buff, buff.Length, turn.ToAddressIp, turn.ToAddressPort);//task
                    }
                }
            }
            return Task.CompletedTask;
        }

        protected virtual byte[] ConvertData(TargetInfo tg)
        {
            return Encoding.UTF8.GetBytes(tg?.ToJson() ?? "");
        }
    }
}
