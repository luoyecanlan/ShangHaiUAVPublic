using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 历史目标POCO实体基类
    /// </summary>
    [Table("target")]
    public abstract class HistoryTargetBase { }

    /// <summary>
    /// 历史目标POCO实体基类（带主键）
    /// </summary>
    public abstract class HistoryTargetKeyBase : HistoryTargetBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey(1), Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 历史目标删除POCO实体
    /// </summary>
    public class HistoryTargetDel : HistoryTargetKeyBase { }

    /// <summary>
    /// 新增历史目标POCO实体
    /// </summary>
    public class HistoryTargetAdd : HistoryTargetKeyBase
    {
        [Column("tgId"), PrimaryKey(2), NotNull]
        public string TgId { get; set; }
        /// <summary>
        /// 唯一SN码（若有则写入）
        /// </summary>
        [Column("sn"), Nullable] public string Sn { get; set; } // varchar(255)
        /// <summary>
        /// 目标类型，目标最多出现次数的类型
        /// </summary>
        [Column("category"), NotNull]
        public int Category { get; set; }
        /// <summary>
        /// 无人机类型
        /// </summary>
        [Column("uavmodel"), NotNull]
        public int UAVModel { get; set; }
        /// <summary>
        /// 目标类型概率
        /// </summary>
        [Column("categoryProportion"), NotNull]
        public double CategoryProportion { get; set; }
        /// <summary>
        /// 设备数量，目前暂时定：1，融合时候需要填写
        /// </summary>
        [Column("deviceCount"), NotNull] //NOTO::::后期进行优化
        public int DeviceCount { get; set; }
        /// <summary>
        /// 点迹数
        /// </summary>
        [Column("count"), NotNull]
        public int Count { get; set; }
        /// <summary>
        /// 目标出现时间
        /// </summary>
        [Column("starttime"), NotNull]
        public DateTime Starttime { get; set; }
        /// <summary>
        /// 目标消失时间
        /// </summary>
        [Column("endtime"), NotNull]
        public DateTime Endtime { get; set; }
        /// <summary>
        /// 最大威胁度
        /// </summary>
        [Column("threat"), NotNull]
        public double ThreatMax { get; set; }
        /// <summary>
        /// 飞手位置
        /// </summary>
        [Column("flyer"), NotNull]
        public string FlyerPosition { get; set; }
        /// <summary>
        /// 打击标记
        /// </summary>
        [Column("hitmark"), NotNull]
        public int HitMark { get; set; }
    }

    /// <summary>
    /// 历史目标信息POCO实体
    /// </summary>
    public class HistoryTargetInfo : HistoryTargetKeyBase
    {
        [Column("tgId"), NotNull]
        public string TgId { get; set; }
        /// <summary>
        /// 唯一SN码（若有则写入）
        /// </summary>
        [Column("sn"), Nullable] public string Sn { get; set; } // varchar(255)
        [Column("category"), NotNull]
        public int Category { get; set; }
        [Column("uavmodel"), NotNull]
        public int UAVModel { get; set; }
        [Column("categoryProportion"), NotNull]
        public double CategoryProportion { get; set; }
        [Column("deviceCount"), NotNull]
        public int DeviceCount { get; set; }
        [Column("count"), NotNull]
        public int Count { get; set; }
        [Column("starttime"), NotNull]
        public DateTime Starttime { get; set; }
        [Column("endtime"), NotNull]
        public DateTime Endtime { get; set; }
        [Column("threat"), NotNull]
        public double ThreatMax { get; set; }
        [Column("flyer"), NotNull]
        public string FlyerPosition { get; set; }
        /// <summary>
        /// 打击标记
        /// </summary>
        [Column("hitmark"), NotNull]
        public int HitMark { get; set; }
    }

    /// <summary>
    /// 历史目标更新POCO实体
    /// </summary>
    public class HistoryTargetUpdate : HistoryTargetKeyBase
    {

    }
}
