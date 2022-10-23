﻿using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP13.Cmd
{
    public class S_HeadTailCmd : IPeerSysCmd
    {
        public string Key => PluginConst.Category.ToString();

        public PeerSysCmdType Order => PeerSysCmdType.CheckHeadAndTail;

        /// <summary>
        /// 头
        /// </summary>
        public static readonly byte[] Head = { 0x88, 0x89, 0x80, 0x8A };

        /// <summary>
        /// 尾
        /// </summary>
        public static readonly byte[] End = { 0x89, 0x80, 0x8A, 0x8B };

        public Task<bool> Invoke(IPeerContent content)
        {
            var head = new byte[Head.Length];
            Buffer.BlockCopy(content.Source, 0, head, 0, head.Length);
            var end = new byte[End.Length];
            Buffer.BlockCopy(content.Source, content.Source.Length - end.Length, end, 0, end.Length);
            return Task.FromResult(Enumerable.SequenceEqual(Head, head) && Enumerable.SequenceEqual(End, end));
        }
    }
}
