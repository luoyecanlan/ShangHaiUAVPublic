using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    /// <summary>
    /// 元数据服务
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    /// <typeparam name="TUpdate"></typeparam>
    /// <typeparam name="TDelete"></typeparam>
    /// <typeparam name="TAdd"></typeparam>
    public abstract class MetadataService<TInfo, TUpdate, TDelete, TAdd> : IMetadataService<TInfo, TUpdate, TDelete, TAdd>
        where TInfo : class, IEntityKeyProperty, new()
        where TUpdate : IEntityKeyProperty
        where TAdd : class
    {
        public MetadataService(IEntityCrudService orm)
        {
            _orm = orm;
        }

        protected readonly IEntityCrudService _orm;

        /// <summary>
        /// 实体新增
        /// </summary>
        /// <typeparam name="TAdd">新增的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="add">新增的实体对象</param>
        /// <returns>实体信息</returns>
        public async Task<TInfo> AddAsync(TAdd add)
        {
            if (add == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull);
            int id;
            try
            {
                id = await _orm?.AddAsync(add);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptAddFail, ex);
            }
            return id > 0 ? await GetAsync(id) : default;
        }

        /// <summary>
        /// 实体删除
        /// </summary>
        /// <param name="id">删除数据的ID</param>
        /// <returns>删除数量</returns>
        public Task<int> DelAsync(int id)
        {
            if (id < 0)
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId, $"Id:{id}");
            try
            {
                return _orm.DelAsync<TInfo>(id);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptDelFail, ex, $"Id:{id}");
            }
        }

        /// <summary>
        /// 实体删除（批量）
        /// </summary>
        /// <param name="ids">删除数据的ID集合</param>
        /// <returns>删除数量</returns>
        public Task<int> DelAsync(IEnumerable<int> ids)
        {
            if (ids?.Count() <= 0 || ids.Any(x => x < 0))
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId, "批量删除ID存在错误.");
            try
            {
                return _orm.DelAsync<TInfo>(ids);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptDelFail, ex, "批量删除发生错误.");
            }
        }

        /// <summary>
        /// 获取实体信息（默认类型集合）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>实体信息集合</returns>
        public Task<IEnumerable<TInfo>> GetAnyAsync(Expression<Func<TInfo, bool>> predicate = null, Expression<Func<TInfo, object>> keySelector = null, bool desc = false)
        {
            try
            {
                return _orm.GetAnyAsync(predicate: predicate, keySelector: keySelector, desc: desc);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex, $"查询数据错误.");
            }
        }

        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <param name="id">获取数据的ID</param>
        /// <returns>实体信息</returns>
        public Task<TInfo> GetAsync(int id)
        {
            try
            {
                return _orm.GetAsync<TInfo>(id);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex, $"查询单条数据错误.");
            }
        }

        /// <summary>
        /// 获取实体信息（分页）
        /// </summary>
        /// <param name="pageSize">分页参数</param>
        /// <param name="pageIndex">分页参数</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>实体信息集合</returns>
        public Task<PagingModel<TInfo>> GetAnyAsync(int pageSize, int pageIndex, Expression<Func<TInfo, bool>> predicate = null, Expression<Func<TInfo, object>> keySelector = null, bool desc = false)
        {
            try
            {
                return _orm.GetAnyAsync(pageSize, pageIndex, predicate: predicate, keySelector: keySelector, desc: desc);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex, $"查询分页数据错误.");
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TUpdate">更新的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="update">更新的实体对象</param>
        /// <returns></returns>
        public async Task<TInfo> UpdateAsync(TUpdate update)
        {
            if (update == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull);
            bool res;
            try
            {
                res = await _orm.UpdateAsync(update);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptUpdateFail, ex, $"Id:{update.Id}");
            }
            return res ? await GetAsync(update.Id) : default;
        }

        /// <summary>
        /// 获取实体信息（自定义类型集合）
        /// </summary>
        /// <typeparam name="TCustomInfo"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>实体信息集合</returns>
        public Task<IEnumerable<TCustomInfo>> GetAnyAsync<TCustomInfo>(Expression<Func<TCustomInfo, bool>> predicate, Expression<Func<TCustomInfo, object>> keySelector, bool desc)
            where TCustomInfo : class, IEntityKeyProperty, new()
        {
            try
            {
                return _orm.GetAnyAsync(predicate: predicate, keySelector: keySelector, desc: desc);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex, $"获取自定义信息错误.");
            }
        }

        /// <summary>
        /// 获取实体信息（自定义类型分页集合）
        /// </summary>
        /// <typeparam name="TCustomInfo">自定义类型</typeparam>
        /// <param name="pageSize">分页参数</param>
        /// <param name="pageIndex">分页参数</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>实体信息集合</returns>
        public Task<PagingModel<TCustomInfo>> GetAnyAsync<TCustomInfo>(int pageSize, int pageIndex, Expression<Func<TCustomInfo, bool>> predicate, Expression<Func<TCustomInfo, object>> keySelector, bool desc)
            where TCustomInfo : class, IEntityKeyProperty, new()
        {
            try
            {
                return _orm.GetAnyAsync(pageSize, pageIndex, predicate: predicate, keySelector: keySelector, desc: desc);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex, $"获取分页自定义信息错误.");
            }
        }
    }
}
