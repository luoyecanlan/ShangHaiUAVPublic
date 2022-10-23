using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.HostService
{
    public interface IPeerServer
    {
        /// <summary>
        /// 服务信息
        /// </summary>
        PeerServerInfo Info { get; }

        /// <summary>
        /// 路由
        /// </summary>
        IPeerRoute Route { get; }

        /// <summary>
        /// 使用自定义路由
        /// </summary>
        /// <param name="route"></param>
        void UseCustomRoute(IPeerRoute route);

        /// <summary>
        /// 使用自定义服务信息
        /// </summary>
        /// <param name="info"></param>
        void UseCustomServerInfo(PeerServerInfo info);

        /// <summary>
        /// 开启服务
        /// </summary>
        void Star();
       
        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        int Send(byte[] buff, string ip, int port);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task<int> SendAsync(byte[] buff, string ip, int port);

        public int Send(byte[] buff, string ip, int port, string temp = "小雷达收发必须由同一ip端口进行，用此方法进行发送");
    }
}
