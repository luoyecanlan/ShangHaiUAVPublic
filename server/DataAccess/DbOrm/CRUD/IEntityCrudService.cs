using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbOrm.CRUD
{
    public interface IEntityCrudService
    {
        /// </summary>
        /// 实体新增
        /// </summary>
        /// <typeparam name="TAdd">新增的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="add">新增的实体对象</param>
        /// <returns>实体信息</returns>
        Task<int> AddAsync<TAdd>(TAdd add)
            where TAdd : class;

        /// <summary>
        /// 实体新增（批量）
        /// </summary>
        /// <typeparam name="TAdd">新增的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="add">新增的实体对象</param>
        /// <returns>新增数量</returns>
        Task<long> AddAsync<TAdd>(IEnumerable<TAdd> add) where TAdd : class;

        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <param name="id">获取数据的ID</param>
        /// <returns>实体信息</returns>
        Task<TInfo> GetAsync<TInfo>(int id)
            where TInfo : class, IEntityKeyProperty, new();

        /// <summary>
        /// 获取实体信息（默认类型集合）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>实体信息集合</returns>
        Task<IEnumerable<TInfo>> GetAnyAsync<TInfo>(Expression<Func<TInfo, bool>> predicate = null,
            Expression<Func<TInfo, object>> keySelector = null,
            bool desc = false)
            where TInfo : class, IEntityKeyProperty, new();

        /// <summary>
        /// 获取实体信息（分页）
        /// </summary>
        /// <param name="pageSize">分页参数</param>
        /// <param name="pageIndex">分页参数</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="desc">是否倒序</param>
        /// <returns>分页实体信息集合</returns>
        Task<PagingModel<TInfo>> GetAnyAsync<TInfo>(int pageSize, int pageIndex,
            Expression<Func<TInfo, bool>> predicate = null,
            Expression<Func<TInfo, object>> keySelector = null,
            bool desc = false)
            where TInfo : class, IEntityKeyProperty, new();

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TUpdate">更新的实体类型，TableName必须与Info一致</typeparam>
        /// <param name="update">更新的实体对象</param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TUpdate>(TUpdate update) where TUpdate : IEntityKeyProperty;

        /// <summary>
        /// 实体删除
        /// </summary>
        /// <param name="id">删除数据的ID</param>
        /// <returns>删除数量</returns>
        Task<int> DelAsync<TInfo>(int id) where TInfo : class, IEntityKeyProperty, new();

        /// <summary>
        /// 实体删除（批量）
        /// </summary>
        /// <param name="ids">删除数据的ID集合</param>
        /// <returns>删除数量</returns>
        Task<int> DelAsync<TInfo>(IEnumerable<int> ids) where TInfo : class, IEntityKeyProperty, new();
    }

    public interface IEntityCrudService<Tdb> : IEntityCrudService where Tdb : DataConnection, new()
    {

    }
}
