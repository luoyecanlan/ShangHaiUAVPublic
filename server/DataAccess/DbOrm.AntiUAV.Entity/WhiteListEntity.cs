using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 用户POCO实体基类
    /// </summary>
    [Table("whitelist")]
    public abstract class WhiteListBase
    {

    }

    /// <summary>
    /// 用户POCO实体基类（带主键）
    /// </summary>
    public abstract class WhiteListKeyBase : WhiteListBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 新建白名单POCO实体
    /// </summary>
    public class WhiteListAdd : WhiteListBase
    {
        [Column("sn"), NotNull]
        public string Sn { get; set; }
        /// <summary>
        /// 录入部门
        /// </summary>
        [Column("department"), NotNull]
        public string Department { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Column("startime"), NotNull]
        public DateTime StarTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("endtime"), NotNull]
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// 白名单信息POCO实体
    /// </summary>
    public class WhiteListInfo : WhiteListKeyBase
    {
        /// <summary>
        /// 飞机SN码（唯一标识码）
        /// </summary>
        [Column("sn"), NotNull] 
        public string Sn { get; set; } // varchar(255)
        /// <summary>
        /// 录入部门
        /// </summary>
        [Column("department"), NotNull]
        public string Department { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Column("startime"), NotNull]
        public DateTime StarTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("endtime"), NotNull]
        public DateTime EndTime { get; set; }
        [Column("createtime"), NotNull]
        public DateTime CreateTime { get; set; }
        [Column("updatetime"), NotNull]
        public DateTime UpdateTime { get; set; }
    }


    /// <summary>
    /// 删除白名单POCO实体
    /// </summary>
    public class WhiteListDel : WhiteListKeyBase
    {

    }

    /// <summary>
    /// 白名单信息更新POCO实体
    /// </summary>
    public class WhiteListUpdate : WhiteListKeyBase
    {
        [Column("sn"), NotNull]
        public string Sn { get; set; }
        /// <summary>
        /// 录入部门
        /// </summary>
        [Column("department"), NotNull]
        public string Department { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Column("startime"), NotNull]
        public DateTime StarTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("endtime"), NotNull]
        public DateTime EndTime { get; set; }
    }
}
