using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 历史航迹POCO实体基类
    /// </summary>
    [Table("track")]
    public abstract class HistoryTrackBase { }

    /// <summary>
    /// 历史航迹POCO实体基类（带主键）
    /// </summary>
    public abstract class HistoryTrackKeyBase : HistoryTrackBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 历史航迹删除POCO实体
    /// </summary>
    public class HistoryTrackDel : HistoryTrackKeyBase { }

    /// <summary>
    /// 历史新增航迹POCO实体
    /// </summary>
    public class HistoryTrackAdd: HistoryTrackBase
    {
        [Column("targetId"), NotNull] 
        public string TargetId { get; set; } // varchar(255)
        [Column("deviceId"), NotNull] 
        public int DeviceId { get; set; } // int(11)
        [Column("lat"), NotNull] 
        public double Lat { get; set; } // double
        [Column("lng"), NotNull] 
        public double Lng { get; set; } // double
        [Column("alt"), NotNull] 
        public double Alt { get; set; } // double
        [Column("category"), NotNull] 
        public int Category { get; set; } // int(11)
        [Column("mode"), NotNull] 
        public int Mode { get; set; } // int(11)
        [Column("threat"), NotNull] 
        public double Threat { get; set; } // double
        [Column("tracktime"), NotNull] 
        public DateTime TrackTime { get; set; } // datetime
    }

    /// <summary>
    /// 历史航迹信息POCO实体
    /// </summary>
    public class HistoryTrackInfo: HistoryTrackKeyBase
    {
        [Column("targetId"), NotNull]
        public string TargetId { get; set; } // varchar(255)
        [Column("deviceId"), NotNull]
        public int DeviceId { get; set; } // int(11)
        [Column("lat"), NotNull]
        public double Lat { get; set; } // double
        [Column("lng"), NotNull]
        public double Lng { get; set; } // double
        [Column("alt"), NotNull]
        public double Alt { get; set; } // double
        [Column("category"), NotNull]
        public int Category { get; set; } // int(11)
        [Column("mode"), NotNull]
        public int Mode { get; set; } // int(11)
        [Column("threat"), NotNull]
        public double Threat { get; set; } // double
        [Column("tracktime"), NotNull]
        public DateTime TrackTime { get; set; } // datetime
    }
}
