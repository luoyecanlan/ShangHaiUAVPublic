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
    [Table("user")]
    public abstract class UserBase
    {

    }

    /// <summary>
    /// 用户POCO实体基类（带主键）
    /// </summary>
    public abstract class UserKeyBase : UserBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 新建用户POCO实体
    /// </summary>
    public class UserAdd : UserBase
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
        public string Role { get; set; }

        [Column("email"), Nullable]
        public string Email { get; set; }
    }

    /// <summary>
    /// 用户信息POCO实体
    /// </summary>
    public class UserInfo : UserKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }
        [Column("username"), NotNull]
        public string Username { get; set; }
        [Column("nick"), Nullable]
        public string Nick { get; set; }
        [Column("phone"), Nullable]
        public string Phone { get; set; }
        [Column("email"), Nullable]
        public string Email { get; set; }
        [Column("createtime"), NotNull]
        public DateTime CreateTime { get; set; }
        [Column("updatetime"), NotNull]
        public DateTime UpdateTime { get; set; }
        [Column("role"), NotNull]
        public string Role { get; set; }
    }

    /// <summary>
    /// 用户完整信息POCO实体
    /// </summary>
    public class UserAllInfo: UserInfo
    {
        [Column("password"), NotNull]
        public string Password { get; set; }
    }

    /// <summary>
    /// 用户简要信息POCO实体
    /// </summary>
    public class UserSimpleInfo: UserKeyBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Column("username"), NotNull]
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Column("nick"), Nullable]
        public string Nick { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Column("role"), NotNull]
        public string Role { get; set; }
    }

    /// <summary>
    /// 删除用户POCO实体
    /// </summary>
    public class UserDel : UserKeyBase
    {

    }

    /// <summary>
    /// 用户信息更新POCO实体
    /// </summary>
    public class UserUpdate : UserKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }

        [Column("nick"), Nullable]
        public string Nick { get; set; }

        [Column("phone"), Nullable]
        public string Phone { get; set; }

        [Column("email"), Nullable]
        public string Email { get; set; }
    }

    /// <summary>
    /// 用户角色更新POCO实体
    /// </summary>
    public class UserRoleUpdate : UserKeyBase
    {

        [Column("role"), NotNull]
        public string Role { get; set; }
    }

    /// <summary>
    /// 用户密码更新POCO实体
    /// </summary>
    public class UserPasswordUpdate: UserKeyBase
    {
        [Column("password"), NotNull] 
        public string Password { get; set; }
    }
}
