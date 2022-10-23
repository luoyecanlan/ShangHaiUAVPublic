using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    public interface IMetadataService<TInfo, TUpdate, TDelete, TAdd>
        where TInfo : class, IEntityKeyProperty, new()
        where TUpdate : IEntityKeyProperty
        where TAdd : class
    {
        /// <summary>
        /// 实体新增
        /// </summary>
        /// <typeparam name="TAdd">新增的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="add">新增的实体对象</param>
        /// <returns>实体信息</returns>
        Task<TInfo> AddAsync(TAdd add);

        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <param name="id">获取数据的ID</param>
        /// <returns>实体信息</returns>
        Task<TInfo> GetAsync(int id);

        /// <summary>
        /// 获取实体信息（默认类型集合）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>实体信息集合</returns>
        Task<IEnumerable<TInfo>> GetAnyAsync(Expression<Func<TInfo, bool>> predicate = null,
            Expression<Func<TInfo, object>> keySelector = null,
            bool desc = false);

        /// <summary>
        /// 获取实体信息（自定义类型集合）
        /// </summary>
        /// <typeparam name="TCustomInfo"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="keySelector"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        Task<IEnumerable<TCustomInfo>> GetAnyAsync<TCustomInfo>(Expression<Func<TCustomInfo, bool>> predicate = null,
            Expression<Func<TCustomInfo, object>> keySelector = null,
            bool desc = false) where TCustomInfo : class, IEntityKeyProperty, new();

        /// <summary>
        /// 获取实体信息（默认类型分页集合）
        /// </summary>
        /// <param name="pageSize">分页参数</param>
        /// <param name="pageIndex">分页参数</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>分页实体信息集合</returns>
        Task<PagingModel<TInfo>> GetAnyAsync(int pageSize, int pageIndex,
            Expression<Func<TInfo, bool>> predicate = null,
            Expression<Func<TInfo, object>> keySelector = null,
            bool desc = false);

        /// <summary>
        /// 获取实体信息（自定义类型分页集合）
        /// </summary>
        /// <typeparam name="TCustomInfo"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="predicate"></param>
        /// <param name="keySelector"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        Task<PagingModel<TCustomInfo>> GetAnyAsync<TCustomInfo>(int pageSize, int pageIndex,
            Expression<Func<TCustomInfo, bool>> predicate = null,
            Expression<Func<TCustomInfo, object>> keySelector = null,
            bool desc = false) where TCustomInfo : class, IEntityKeyProperty, new();

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TUpdate">更新的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="update">更新的实体对象</param>
        /// <returns></returns>
        Task<TInfo> UpdateAsync(TUpdate update);

        /// <summary>
        /// 实体删除
        /// </summary>
        /// <param name="id">删除数据的ID</param>
        /// <returns>删除数量</returns>
        Task<int> DelAsync(int id);

        /// <summary>
        /// 实体删除（批量）
        /// </summary>
        /// <param name="ids">删除数据的ID集合</param>
        /// <returns>删除数量</returns>
        Task<int> DelAsync(IEnumerable<int> ids);
    }
}
