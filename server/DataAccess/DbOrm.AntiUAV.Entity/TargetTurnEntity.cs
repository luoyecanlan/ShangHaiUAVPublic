using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    [Table("target_turn")]
    public abstract class TargetTurnBase { }

    public abstract class TargetTurnKeyBase : TargetTurnBase, IEntityKeyProperty
    {
        [Column("id"),PrimaryKey,Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 添加
    /// </summary>
    public class TargetTurnAdd:TargetTurnBase
    {
        [Column("ip"), NotNull] 
        public string Ip { get; set; } // varchar(255)
        [Column("port"), NotNull]
        public int Port { get; set; } // int(11)
        [Column("remark"), NotNull] 
        public string Remark { get; set; } // varchar(255)
    }

    /// <summary>
    /// 修改
    /// </summary>
    public class TargetTurnUpdate : TargetTurnKeyBase
    {
        [Column("ip"), NotNull] 
        public string Ip { get; set; } // varchar(255)
        [Column("port"), NotNull] 
        public int Port { get; set; } // int(11)
        [Column("remark"), NotNull]
        public string Remark { get; set; } // varchar(255)
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class TargetTurnDel : TargetTurnKeyBase
    {

    }

    /// <summary>
    /// 查询
    /// </summary>
    public class TargetTurnInfo : TargetTurnKeyBase
    {
        [Column("ip"), NotNull]
        public string Ip { get; set; } // varchar(255)
        [Column("port"), NotNull] 
        public int Port { get; set; } // int(11)
        [Column("remark"), NotNull] 
        public string Remark { get; set; } // varchar(255)
    }
}
