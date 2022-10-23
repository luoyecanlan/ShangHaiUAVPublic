using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    public class PeerContent : IPeerContent
    {
        /// <summary>
        /// 头尾校验结果
        /// </summary>
        public bool? IsHeadAndTail { get; set; }
        /// <summary>
        /// 校验和校验结果
        /// </summary>
        public bool? IsChecksnum { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public byte[] Source { get; set; }

        /// <summary>
        /// 远程终结点
        /// </summary>
        public IPEndPoint RecEp { get; set; }

        /// <summary>
        /// 解析数据
        /// </summary>
        public object SourceAys { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReciveTime { get; set; }

        /// <summary>
        /// 路由值
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 服务IP
        /// </summary>
        public string ServiceIp { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public int ServicePort { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public PeerException Error { get; set; }

        private bool _forceOver = false;
        /// <summary>
        /// 强制结束
        /// </summary>
        public bool ForcedOver
        {
            get
            {
                if (Error == null)
                    return _forceOver;
                return true;
            }
            set
            {
                _forceOver = value;
            }
        }
    }
}
