using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 关联关系模型
    /// </summary>
    public class Relationships
    {
        public Relationships()
        {
            Id = CacheExtension.GetRandomString(8, true, true, true, false, null);
        }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 目标ID
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 来源设备ID
        /// </summary>
        public int FromDeviceId { get; set; }

        /// <summary>
        /// 目标设备ID
        /// </summary>
        public int ToDeviceId { get; set; }

        /// <summary>
        /// 目标地址Ip
        /// 转发：转发地址
        /// 引导：目标设备的host地址
        /// </summary>
        public string ToAddressIp { get; set; }

        /// <summary>
        /// 目标地址Port
        /// 转发：转发地址
        /// 引导：目标设备的host地址
        /// </summary>
        public int ToAddressPort { get; set; }

        /// <summary>
        /// 关系类型
        /// </summary>
        public RelationshipsType RType { get; set; }
        

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        public HitFreqModel hitFreq { get; set; }
    }
    public enum HitFreqModel
    {
        hit_24_58_900=0,
        hit_900=1,
        Hit_15=2,
        hit_24_58=3,
        hit_All=4
    }
    /// <summary>
    /// 关系类型
    /// </summary>
    public enum RelationshipsType
    {
        /// <summary>
        /// 位置转发
        /// </summary>
        PositionTurn,
        /// <summary>
        /// 打击引导
        /// </summary>
        AttackGd,
        /// <summary>
        /// 监视引导
        /// </summary>
        MonitorGd,
        /// <summary>
        /// 诱骗引导
        /// </summary>
        DecoyGd,
    }
}
