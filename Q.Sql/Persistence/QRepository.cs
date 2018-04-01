using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Q.Lib.Core.Concurrency;
using Q.Sql.Core;

namespace Q.Sql.Persistence {
  public abstract class QRepository<TDbContext, T, TId> : IRepository<T, TId>
    where TDbContext : DbContext
    where T : class, IId<TId>
    where TId : IComparable<TId> {
      protected readonly TDbContext context;
      protected DbSet<T> dbSet;
      public QRepository(TDbContext context) => this.context = context;
      protected abstract IIncludableQueryable<T, Object> IncludeQuery(IQueryable<T> q);
      protected abstract Task<T> IncludeQueryAsync(TId id);
      public bool Any(TId id) {
        return dbSet.Any(x => x.Id.CompareTo(id) == 0);
      }
      public async Task<T> GetAsync(TId id, bool includeRelated = true) {
        if (!includeRelated)
          return await dbSet.FindAsync(id);

        return await IncludeQueryAsync(id);

      }
      public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> f, bool includeRelated = true) {
        var q = dbSet.Where(f);
        if (!includeRelated) return await q.ToListAsync();

        return await IncludeQuery(q).ToListAsync();
      }
      public async Task<IList<T>> GetAsync(bool includeRelated = true) {
        if (!includeRelated) return await dbSet.ToListAsync();

        return await IncludeQuery(dbSet).ToListAsync();
      }
      public async Task AddAsync(T t) {
        await dbSet.AddAsync(t);
      }
      public async Task AddRangeAsync(IEnumerable<T> ts) {
        await dbSet.AddRangeAsync(ts);
      }
      // The predicate expression has to be implemented in the repo class (quite trick, may need linqkit)
      public abstract Task AddIfNotExistsAsync(T t);
      public virtual async Task AddIfNotExistsAsync(IEnumerable<T> ts) {
        await ts.ForEachAsync(x => AddIfNotExistsAsync(x));
      }
      public void Update(T t) {
        dbSet.Update(t);
      }
      public void UpdateRange(IEnumerable<T> ts) {
        dbSet.UpdateRange(ts);
      }
      public void Remove(T t) {
        dbSet.Remove(t);
      }
      public void RemoveRange(IEnumerable<T> ts) {
        dbSet.RemoveRange(ts);
      }
    }
}