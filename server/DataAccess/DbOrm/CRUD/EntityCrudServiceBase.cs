using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbOrm.CRUD
{
    public abstract class EntityCrudServiceBase<Tdb> : IEntityCrudService<Tdb>
           where Tdb : DataConnection, new()
    {
        Task<int> IEntityCrudService.AddAsync<TAdd>(TAdd add)
        {
            using var db = new Tdb();
            return db.InsertWithInt32IdentityAsync(add);
        }

        Task<long> IEntityCrudService.AddAsync<TAdd>(IEnumerable<TAdd> add)
        {
            using var db = new Tdb();
            var count = db.BulkCopy(add)?.RowsCopied ?? -1;
            return Task.FromResult(count);
        }

        Task<int> IEntityCrudService.DelAsync<TInfo>(int id)
        {
            using var db = new Tdb();
            return db.DeleteAsync(new TInfo() { Id = id });
        }

        Task<int> IEntityCrudService.DelAsync<TInfo>(IEnumerable<int> ids)
        {
            using var db = new Tdb();
            return db.GetTable<TInfo>().Where(x => ids.Contains(x.Id)).DeleteAsync();
        }

        async Task<IEnumerable<TInfo>> IEntityCrudService.GetAnyAsync<TInfo>(Expression<Func<TInfo, bool>> predicate, Expression<Func<TInfo, object>> keySelector, bool desc)
        {
            using var db = new Tdb();
            var query = from q in db.GetTable<TInfo>()
                        select q;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (keySelector != null)
            {
                if (desc)
                    query = query.OrderByDescending(keySelector);
                else
                    query = query.OrderBy(keySelector);
            }
            return await query.ToListAsync();
        }

        Task<TInfo> IEntityCrudService.GetAsync<TInfo>(int id)
        {
            using var db = new Tdb();
            return db.GetTable<TInfo>().FirstOrDefaultAsync(x => x.Id == id);
        }

        async Task<PagingModel<TInfo>> IEntityCrudService.GetAnyAsync<TInfo>(int pageSize, int pageIndex, Expression<Func<TInfo, bool>> predicate, Expression<Func<TInfo, object>> keySelector, bool desc)
        {
            using var db = new Tdb();
            var query = from q in db.GetTable<TInfo>()
                        select q;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (keySelector != null)
            {
                if (desc)
                    query = query.OrderByDescending(keySelector);
                else
                    query = query.OrderBy(keySelector);
            }
            return new PagingModel<TInfo>(pageSize: pageSize, pageIndex: pageIndex)
            {
                SnumSize = query.Count(),
                Data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            };
        }

        async Task<bool> IEntityCrudService.UpdateAsync<TUpdate>(TUpdate update)
        {
            using var db = new Tdb();
            return await db.UpdateAsync(update) > 0;
        }
    }
}
