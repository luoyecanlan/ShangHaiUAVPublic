using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 地图信息配置POCO实体基类
    /// </summary>
    [Table("dic_map_config")]
    public abstract class MapConfigBase { }

    /// <summary>
    /// 地图信息配置POCO实体基类  （带主键）
    /// </summary>
    public abstract class MapConfigKeyBase : MapConfigBase, IEntityKeyProperty
    {
        [Column("id"),PrimaryKey,Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 地图信息配置新建POCO实体
    /// </summary>
    public class MapConfigAdd : MapConfigBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; } // varchar(255)
        [Column("url"), NotNull]
        public string Url { get; set; } // varchar(500)
        [Column("zoom_max"), NotNull]
        public int ZoomMax { get; set; } // int(11)
        [Column("zoom_min"), NotNull]
        public int ZoomMin { get; set; } // int(11)
        [Column("zoom_default"), NotNull]
        public int ZoomDefault { get; set; } // int(11)
        [Column("boundary_max_lat"), NotNull]
        public double BoundaryMaxLat { get; set; } //double
        [Column("boundary_max_lng"), NotNull]
        public double BoundaryMaxLng { get; set; } //double
        [Column("boundary_min_lat"), NotNull]
        public double BoundaryMinLat { get; set; } // ouble
        [Column("boundary_min_lng"), NotNull]
        public double BoundaryMinLng { get; set; } //double
        [Column("remark"), Nullable]
        public string Remark { get; set; } // varchar(255)

    }

    /// <summary>
    /// 地图配置信息删除POCO实体
    /// </summary>
    public class MapConfigDel : MapConfigKeyBase
    {

    }

    /// <summary>
    /// 地图配置信息查询POCO实体
    /// </summary>
    public class MapConfigInfo : MapConfigKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; } // varchar(255)
        [Column("url"), NotNull]
        public string Url { get; set; } // varchar(500)
        [Column("zoom_max"), NotNull]
        public int ZoomMax { get; set; } // int(11)
        [Column("zoom_min"), NotNull]
        public int ZoomMin { get; set; } // int(11)
        [Column("zoom_default"), NotNull]
        public int ZoomDefault { get; set; } // int(11)
        [Column("boundary_max_lat"), NotNull]
        public double BoundaryMaxLat { get; set; } //double
        [Column("boundary_max_lng"), NotNull]
        public double BoundaryMaxLng { get; set; } //double
        [Column("boundary_min_lat"), NotNull]
        public double BoundaryMinLat { get; set; } // ouble
        [Column("boundary_min_lng"), NotNull]
        public double BoundaryMinLng { get; set; } //double
        [Column("remark"), Nullable]
        public string Remark { get; set; } // varchar(255)
    }

    /// <summary>
    /// 地图配置信息修改POCO实体
    /// </summary>
    public class MapConfigUpdate : MapConfigKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; } // varchar(255)
        [Column("url"), NotNull]
        public string Url { get; set; } // varchar(500)
        [Column("zoom_max"), NotNull]
        public int ZoomMax { get; set; } // int(11)
        [Column("zoom_min"), NotNull]
        public int ZoomMin { get; set; } // int(11)
        [Column("zoom_default"), NotNull]
        public int ZoomDefault { get; set; } // int(11)
        [Column("boundary_max_lat"), NotNull]
        public double BoundaryMaxLat { get; set; } //double
        [Column("boundary_max_lng"), NotNull]
        public double BoundaryMaxLng { get; set; } //double
        [Column("boundary_min_lat"), NotNull]
        public double BoundaryMinLat { get; set; } // ouble
        [Column("boundary_min_lng"), NotNull]
        public double BoundaryMinLng { get; set; } //double
        [Column("remark"), Nullable]
        public string Remark { get; set; } // varchar(255)
    }
}
