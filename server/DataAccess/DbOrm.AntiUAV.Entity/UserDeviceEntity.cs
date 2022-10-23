using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 用户设备关系POCO实体基类
    /// </summary>
    [Table("user_device")]
    public abstract class UserDeviceBase
    {

    }

    /// <summary>
    /// 用户设备关系POCO实体基类（带主键）
    /// </summary>
    public abstract class UserDeviceKeyBase : UserDeviceBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 用户设备关系信息POCO实体
    /// </summary>
    public class UserDeviceInfo : UserDeviceKeyBase
    {
        [Column("userid"), NotNull] public int Userid { get; set; } // int(11)
        [Column("deviceid"), NotNull] public int Deviceid { get; set; } // int(11)
    }

    /// <summary>
    /// 删除用户设备关系POCO实体
    /// </summary>
    public class UserDeviceDel : UserDeviceKeyBase
    {

    }
}
