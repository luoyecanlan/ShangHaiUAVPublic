using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    /// <summary>
    /// 服务信息
    /// </summary>
    public class PeerServerInfo
    {
        public PeerServerInfo(int port)
        {
            ServerName = Guid.NewGuid().ToString();
            ListionPort = port;
        }

        public PeerServerInfo(string ip, int port)
        {
            ServerName = Guid.NewGuid().ToString();
            ListionIp = ip;
            ListionPort = port;
        }

        public PeerServerInfo(string name, string ip, int port)
        {
            ServerName = name;
            ListionIp = ip;
            ListionPort = port;
        }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName { get; internal set; }

        /// <summary>
        /// 监听地址
        /// </summary>
        public string ListionIp { get; internal set; }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int ListionPort { get; internal set; }

        /// <summary>
        /// 服务状态
        /// </summary>
        public PeerServerState State { get; internal set; } = PeerServerState.Stop;

        /// <summary>
        /// 运行状态
        /// </summary>
        public bool IsRun { get => State == PeerServerState.Run; }
    }
}
