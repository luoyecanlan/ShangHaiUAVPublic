using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.NoticeModels
{
    public class RpcRequestModel
    {
        public RpcRequestEnum ReqCode { get; set; }

        public int? TimeOut { get; set; }

        public string Data { get; set; }
    }

    /// <summary>
    /// RPC请求类型
    /// </summary>
    public enum RpcRequestEnum
    {
        /// <summary>
        /// 纠偏设定
        /// </summary>
        RectifySetting,
        /// <summary>
        /// 位置设定
        /// </summary>
        PositionSetting,
        /// <summary>
        /// 跟踪开启
        /// </summary>
        MonitorOpen,
        /// <summary>
        /// 跟踪关闭
        /// </summary>
        MonitorClose,
        /// <summary>
        /// 打击开启
        /// </summary>
        AttackOpen,
        /// <summary>
        /// 打击关闭
        /// </summary>
        AttackClose,
        /// <summary>
        /// 云台控制
        /// </summary>
        PTZ,
        /// <summary>
        /// 手动跟踪
        /// </summary>
        Follow
    }
}
