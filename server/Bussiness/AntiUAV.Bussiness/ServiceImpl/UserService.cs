using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.DB;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class UserService : MetadataService<UserInfo, UserUpdate, UserDel, UserAdd>, IUserService
    {
        public UserService(IEntityCrudService orm) : base(orm)
        {
        }


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="restpwd">重置密码</param>
        /// <returns></returns>
        public Task<bool> RestPassword(int id, string restpwd)
        {
            if (id <= 0)
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId, $"Id:{id}");
            if (string.IsNullOrEmpty(restpwd))
                throw new BussinessException(BussinessExceptionCode.PasswordInvalid);
            try
            {
                return _orm.UpdateAsync(new UserPasswordUpdate()
                {
                    Id = id,
                    Password = restpwd
                });
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptRestPwdFail, ex, $"Id:{id}");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码</param>
        /// <returns></returns>
        public async Task<bool> UpdatePasswordAsync(int id, string oldpwd, string newpwd)
        {
            if (id <= 0)
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId);
            if (string.IsNullOrEmpty(oldpwd) || string.IsNullOrEmpty(newpwd))
                throw new BussinessException(BussinessExceptionCode.PasswordInvalid);
            bool res = false;
            try
            {
                var old_user = await _orm.GetAsync<UserPasswordUpdate>(id);
                if (old_user?.Password.ToLower() == oldpwd)
                {
                    res = await _orm.UpdateAsync(new UserPasswordUpdate()
                    {
                        Id = id,
                        Password = newpwd
                    });
                }
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptUpdatPwdFail, ex, $"更新用户密码 Id:{id}");
            }
            return res;
        }
    }
}
