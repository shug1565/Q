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
      public bool Any(TId id) => dbSet.Any(x => x.Id.CompareTo(id) == 0);
      public Task<T> GetAsync(TId id, bool includeRelated = true) => includeRelated ? IncludeQueryAsync(id) : dbSet.FindAsync(id);
      public Task<List<T>> GetAsync(Expression<Func<T, bool>> f, bool includeRelated = true) => includeRelated ? IncludeQuery(dbSet.Where(f)).ToListAsync() : dbSet.Where(f).ToListAsync();
      public Task<List<T>> GetAsync(bool includeRelated = true) => includeRelated ? IncludeQuery(dbSet).ToListAsync() : dbSet.ToListAsync();
      public Task AddAsync(T t) => dbSet.AddAsync(t);
      public Task AddRangeAsync(IEnumerable<T> ts) => dbSet.AddRangeAsync(ts);
      // The predicate expression has to be implemented in the repo class (quite trick, may need linqkit)
      public abstract Task AddIfNotExistsAsync(T t);
      public virtual Task AddIfNotExistsAsync(IEnumerable<T> ts) => ts.ForEachAsync(x => AddIfNotExistsAsync(x));
      public void Update(T t) => dbSet.Update(t);
      public void UpdateRange(IEnumerable<T> ts) => dbSet.UpdateRange(ts);
      public void Remove(T t) => dbSet.Remove(t);
      public void RemoveRange(IEnumerable<T> ts) => dbSet.RemoveRange(ts);
      public void SetAutoDetectChangesEnabled(bool enable) => context.ChangeTracker.AutoDetectChangesEnabled = enable;

    }
}