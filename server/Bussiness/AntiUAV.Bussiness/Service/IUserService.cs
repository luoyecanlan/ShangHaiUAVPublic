using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 用户服务业务
    /// </summary>
    public interface IUserService : IMetadataService<UserInfo, UserUpdate, UserDel, UserAdd>
    {
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码</param>
        /// <returns></returns>
        Task<bool> UpdatePasswordAsync(int id, string oldpwd, string newpwd);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="restpwd">重置密码</param>
        /// <returns></returns>
        Task<bool> RestPassword(int id, string restpwd);
    }
}
