using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.NoticeModels
{
    public class DeviceInfoNotice 
    {
        public DeviceInfoNotice(int devId, DeviceInfoNoticeCode code)
        {
            DeviceId = devId;
            Code = code;
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        public DeviceInfoNoticeCode Code { get; set; }
    }

    /// <summary>
    /// 设备信息通知码
    /// </summary>
    public enum DeviceInfoNoticeCode
    {
        /// <summary>
        /// 信息变化
        /// </summary>
        InfoChange,
        /// <summary>
        /// 位置变化
        /// </summary>
        PositionChange,
    }
}
