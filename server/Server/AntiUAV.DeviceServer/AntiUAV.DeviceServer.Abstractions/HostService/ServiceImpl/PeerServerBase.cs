using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService.ServiceImpl
{
    /// <summary>
    /// 管道服务基础实现
    /// </summary>
    public abstract class PeerServerBase : IPeerServer
    {
        /// <summary>
        /// 服务信息
        /// </summary>
        public PeerServerInfo Info { get; internal set; }
        /// <summary>
        /// 路由
        /// </summary>
        public IPeerRoute Route { get; internal set; }

        /// <summary>
        /// 使用自定义路由
        /// </summary>
        /// <param name="route"></param>
        public virtual void UseCustomRoute(IPeerRoute route)
        {
            if (Info?.State == PeerServerState.Stop || Info?.State == null)
                Route = route ?? throw new PeerException("set route is null.");
            else
                throw new PeerException("service not stopped.");
        }
        /// <summary>
        /// 使用自定义服务信息
        /// </summary>
        /// <param name="info"></param>
        public void UseCustomServerInfo(PeerServerInfo info)
        {
            if (Info?.State == PeerServerState.Stop || Info?.State == null)
                Info = info ?? throw new PeerException("set server info is null.");
            else
                throw new PeerException("service not stopped.");
        }

        public abstract void Star();
        public abstract void Stop();
        public abstract int Send(byte[] buff, string ip, int port, string temp = "小雷达收发必须由同一ip端口，用此方法进行发送");
        public abstract int Send(byte[] buff, string ip, int port);
        public abstract Task<int> SendAsync(byte[] buff, string ip, int port);
        

        protected virtual IPeerContent GetContent(byte[] buff, IPEndPoint ep)
        {
            return new PeerContent()
            {
                RecEp = ep,
                ReciveTime = DateTime.Now,
                ServerName = Info.ServerName,
                ServiceIp = Info.ListionIp,
                ServicePort = Info.ListionPort,
                Source = buff,
                ForcedOver = false
            };
        }

        protected virtual IPeerContent GetErrorContent(byte[] buff, IPEndPoint ep, string message, Exception ex = null)
        {
            return new PeerContent()
            {
                RecEp = ep,
                ReciveTime = DateTime.Now,
                ServerName = Info.ServerName,
                ServiceIp = Info.ListionIp,
                ServicePort = Info.ListionPort,
                Source = buff,
                ForcedOver = false,
                Error = new PeerException(message, ex)
            };
        }
    }
}
