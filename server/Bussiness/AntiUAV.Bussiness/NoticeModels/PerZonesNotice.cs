using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.NoticeModels
{
    public class PerZonesNotice
    {
        public PerZonesNotice(int zoneId, PerZonesNoticeCode code)
        {
            ZoneId = zoneId;
            Code = code;
        }

        /// <summary>
        /// 预警区ID
        /// </summary>
        public int ZoneId { get; set; }
        /// <summary>
        /// 预警区通知类型
        /// </summary>
        public PerZonesNoticeCode Code { get; set; }
    }

    public enum PerZonesNoticeCode
    {
        /// <summary>
        /// 新增
        /// </summary>
        Add,
        /// <summary>
        /// 数据更新
        /// </summary>
        Update,
        /// <summary>
        /// 删除
        /// </summary>
        Delete
    }
}
