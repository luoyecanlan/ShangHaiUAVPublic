using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 预警区POCO实体基类
    /// </summary>
    [Table("zone")]
    public abstract class PerWarningZoneBase { }

    /// <summary>
    /// 预警区POCO实体基类（带主键）
    /// </summary>
    public abstract class PerWarningZoneKeyBase : PerWarningZoneBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 预警区删除POCO实体
    /// </summary>
    public class PerWarningZoneDel : PerWarningZoneKeyBase { }

    /// <summary>
    /// 预警区新建POCO实体
    /// </summary>
    public class PerWarningZoneAdd : PerWarningZoneBase
    {
        [Column("name"), NotNull] public string Name { get; set; } // varchar(255)
        //[Column("fullColor"), NotNull] public string FullColor { get; set; } // varchar(255)
        //[Column("lineColor"), NotNull] public string LineColor { get; set; } // varchar(255)
        //[Column("lineThickness"), NotNull] public double LineThickness { get; set; } // double
        //[Column("zonePoints"), NotNull] public string ZonePoints { get; set; } // varchar(255)
        //[Column("circumcircleR"), NotNull] public double CircumcircleR { get; set; } // double
        //[Column("circumcircleRadiationR"), NotNull] public double CircumcircleRadiationR { get; set; } // double
        //[Column("circumcircleLat"), NotNull] public double CircumcircleLat { get; set; } // double
        //[Column("circumcircleLng"), NotNull] public double CircumcircleLng { get; set; } // double
        //[Column("bDistance"), NotNull] public double BDistance { get; set; } // double
        //[Column("aDistance"), NotNull] public double ADistance { get; set; } // double
        //[Column("normalDistance"), NotNull] public double NormalDistance { get; set; } // double
    }

    /// <summary>
    /// 预警区信息POCO实体
    /// </summary>
    public class PerWarningZoneInfo : PerWarningZoneKeyBase
    {
        [Column("name"), NotNull] public string Name { get; set; } // varchar(255)
        [Column("fullColor"), NotNull] public string FullColor { get; set; } // varchar(255)
        [Column("lineColor"), NotNull] public string LineColor { get; set; } // varchar(255)
        [Column("lineThickness"), NotNull] public double LineThickness { get; set; } // double
        [Column("zonePoints"), NotNull] public string ZonePoints { get; set; } // varchar(255)
        [Column("zonePointsPosition"), NotNull] public string ZonePointsPosition { get; set; } // varchar(255)
        [Column("circumcircleR"), NotNull] public double CircumcircleR { get; set; } // double
        [Column("circumcircleRadiationR"), NotNull] public double CircumcircleRadiationR { get; set; } // double
        [Column("circumcircleLat"), NotNull] public double CircumcircleLat { get; set; } // double
        [Column("circumcircleLng"), NotNull] public double CircumcircleLng { get; set; } // double
        [Column("bDistance"), NotNull] public double BDistance { get; set; } // double
        [Column("aDistance"), NotNull] public double ADistance { get; set; } // double
        [Column("normalDistance"), NotNull] public double NormalDistance { get; set; } // double
        [Column("createtime"), NotNull] public DateTime Createtime { get; set; } // datetime
        [Column("updatetime"), NotNull] public DateTime Updatetime { get; set; } // datetime
    }

    /// <summary>
    /// 预警区简要信息POCO实体
    /// </summary>
    public class PerWarningZoneSimpleInfo: PerWarningZoneKeyBase
    {
        [Column("name"), NotNull] public string Name { get; set; } // varchar(255)
    }

    /// <summary>
    /// 预警区更新POCO实体
    /// </summary>
    public class PerWarningZoneUpdate : PerWarningZoneKeyBase
    {
        [Column("name"), NotNull] public string Name { get; set; } // varchar(255)
        [Column("fullColor"), NotNull] public string FullColor { get; set; } // varchar(255)
        [Column("lineColor"), NotNull] public string LineColor { get; set; } // varchar(255)
        [Column("lineThickness"), NotNull] public double LineThickness { get; set; } // double
    }

    /// <summary>
    /// 预警区几何图形更新POCO实体
    /// </summary>
    public class PerWarningZoneGeoUpdate : PerWarningZoneKeyBase
    {
        [Column("zonePoints"), NotNull] public string ZonePoints { get; set; } // varchar(255)
        [Column("zonePointsPosition"), NotNull] public string ZonePointsPosition { get; set; } // varchar(255)
        [Column("circumcircleR"), NotNull] public double CircumcircleR { get; set; } // double
        [Column("circumcircleRadiationR"), NotNull] public double CircumcircleRadiationR { get; set; } // double
        [Column("circumcircleLat"), NotNull] public double CircumcircleLat { get; set; } // double
        [Column("circumcircleLng"), NotNull] public double CircumcircleLng { get; set; } // double
        [Column("bDistance"), NotNull] public double BDistance { get; set; } // double
        [Column("aDistance"), NotNull] public double ADistance { get; set; } // double
        [Column("normalDistance"), NotNull] public double NormalDistance { get; set; } // double
    }
}
