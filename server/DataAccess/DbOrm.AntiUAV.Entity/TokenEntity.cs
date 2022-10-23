using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 令牌POCO实体基类
    /// </summary>
    [Table("token")]
    public abstract class TokenBase
    {

    }

    /// <summary>
    /// 令牌POCO实体基类（带主键）
    /// </summary>
    public abstract class TokenKeyBase : TokenBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 新建令牌POCO实体
    /// </summary>
    public class TokenAdd : TokenBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }

        [Column("username"), NotNull]
        public string Username { get; set; }

        [Column("password"), NotNull]
        public string Password { get; set; }

        [Column("nick"), Nullable]
        public string Nick { get; set; }

        [Column("phone"), Nullable]
        public string Phone { get; set; }

        [Column("role"), NotNull]
        public int Role { get; set; }

        [Column("email"), Nullable]
        public string Email { get; set; }
    }


    /// <summary>
    /// 删除令牌POCO实体
    /// </summary>
    public class TokenDel : TokenKeyBase
    {

    }

    /// <summary>
    /// 更新令牌POCO实体
    /// </summary>
    public class TokenUpdate: TokenKeyBase
    {

    }
}
