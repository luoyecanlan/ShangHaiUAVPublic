using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    [Table("dic_sys_config")]
    public abstract class SysConfigBase { }
    public abstract class SysConfigKeyBase : SysConfigBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    public class SysConfigAdd:SysConfigBase
    {
        [Column("name"), NotNull] public string Name { get; set; } // varchar(255)
        [Column("info"), NotNull] public string Info { get; set; } // varchar(5000)
    }

    public class SysConfig : SysConfigKeyBase
    {
        [Column("name"), NotNull] public string Name { get; set; } // varchar(255)
        [Column("info"), NotNull] public string Info { get; set; } // varchar(5000)
    }

    public class SysConfigDel : SysConfigKeyBase
    {
    }
}
