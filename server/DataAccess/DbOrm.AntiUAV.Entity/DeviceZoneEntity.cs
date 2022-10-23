using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 设备预警区关系POCO实体基类
    /// </summary>
    [Table("device_zone")]
    public abstract class DeviceZoneBase
    {

    }

    /// <summary>
    /// 设备预警区关系POCO实体基类（带主键）
    /// </summary>
    public abstract class DeviceZoneKeyBase : DeviceZoneBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 删除设备预警区关系POCO实体
    /// </summary>
    public class DeviceZoneDel : DeviceZoneKeyBase
    {

    }
}
