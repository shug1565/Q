using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Q.Sql.Persistence {
  public interface IRepository<T, TId> {
    Task<IList<T>> GetAsync(bool includeRelated = true);
    Task<T> GetAsync(TId id, bool includeRelated = true);
    Task<IList<T>> GetAsync(Expression<Func<T, bool>> f, bool includeRelated = true);
    Task AddAsync(T t);
    Task AddRangeAsync(IEnumerable<T> ts);
    Task AddIfNotExistsAsync(T t);
    Task AddIfNotExistsAsync(IEnumerable<T> t);
    void Update(T t);
    void UpdateRange(IEnumerable<T> ts);
    void Remove(T t);
    void RemoveRange(IEnumerable<T> ts);
    bool Any(TId id);
  }
}