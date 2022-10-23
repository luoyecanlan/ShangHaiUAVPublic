using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public class DeviceStatus
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public int DeviceCategory { get; set; }
        /// <summary>
        /// 设备状态码
        /// </summary>
        public DeviceStatusCode Code { get; set; }
        /// <summary>
        /// 异常状态标记
        /// </summary>
        public bool IsError => ErrorMsg.Count() > 0;
        /// <summary>
        /// 异常信息
        /// </summary>
        public IEnumerable<string> ErrorMsg { get; set; } = new List<string>();
        /// <summary>
        /// 运行参数
        /// </summary>
        public DeviceRunInfo RunInfo { get; set; } = new DeviceRunInfo();
        /// <summary>
        /// 最后被引导位置信息
        /// </summary>
        public GuidancePositionInfo LastGdPosition { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 设备状态码
    /// </summary>
    public enum DeviceStatusCode
    {
        /// <summary>
        /// 离线
        /// </summary>
        OffLine,
        /// <summary>
        /// 空闲/待机
        /// </summary>
        Free,
        /// <summary>
        /// 运行/工作中
        /// </summary>
        Running,
        /// <summary>
        /// 光电搜索中
        /// </summary>
        Search
    }
}
