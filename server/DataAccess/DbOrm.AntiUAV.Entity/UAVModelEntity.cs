using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 无人机类型POCO实体基类
    /// </summary>
    [Table("uavmodel")]
    public abstract class UAVModelBase
    {

    }

    /// <summary>
    /// 无人机类型POCO实体基类（带主键）
    /// </summary>
    public abstract class UAVModelKeyBase : UAVModelBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 新建无人机类型POCO实体
    /// </summary>
    public class UAVModelAdd : UAVModelKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }
    }

    /// <summary>
    /// 无人机类型信息POCO实体
    /// </summary>
    public class UAVModelInfo : UAVModelKeyBase
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [Column("name"), NotNull]
        public string Name { get; set; } // varchar(255)
        [Column("createtime"), NotNull]
        public DateTime CreateTime { get; set; }
        [Column("updatetime"), NotNull]
        public DateTime UpdateTime { get; set; }
    }


    /// <summary>
    /// 删除无人机类型POCO实体
    /// </summary>
    public class UAVModelDel : UAVModelKeyBase
    {

    }

    /// <summary>
    /// 无人机类型信息更新POCO实体
    /// </summary>
    public class UAVModelUpdate : UAVModelKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }
    }
}
