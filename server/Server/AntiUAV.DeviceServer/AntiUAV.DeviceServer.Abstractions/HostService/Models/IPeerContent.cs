using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    public interface IPeerContent
    {
        /// <summary>
        /// 头尾校验结果
        /// </summary>
        bool? IsHeadAndTail { get; set; }
        /// <summary>
        /// 校验和校验结果
        /// </summary>
        bool? IsChecksnum { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        byte[] Source { get; set; }

        /// <summary>
        /// 远程终结点
        /// </summary>
        IPEndPoint RecEp { get; set; }

        /// <summary>
        /// 解析数据
        /// </summary>
        object SourceAys { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        DateTime ReciveTime { get; set; }

        /// <summary>
        /// 路由值
        /// </summary>
        string Route { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// 服务IP
        /// </summary>
        string ServiceIp { get; }

        /// <summary>
        /// 服务端口
        /// </summary>
        int ServicePort { get; }

        /// <summary>
        /// 异常信息
        /// </summary>
        PeerException Error { get; set; }

        /// <summary>
        /// 强制结束
        /// </summary>
        bool ForcedOver { get; set; }
    }
}
