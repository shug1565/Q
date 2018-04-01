using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Q.Sql.Core {
  public static class QEntity {
    public static T Update<T>(this DbSet<T> dbSet, Func<T> entityConstructor) where T : class //, new()
    {
      var ret = entityConstructor();
      dbSet.Update(ret);
      return ret;
    }
    public static T AddIfNotExists<T>(this DbSet<T> dbSet, Func<T> entityConstructor, params object[] keyValues) where T : class //, new()
    {
      var exists = dbSet.Find(keyValues);
      if (exists == null) {
        var ret = entityConstructor();
        dbSet.Add(ret);
        return ret;
      }
      return null;
    }
    public async static Task<T> AddIfNotExistsAsync<T>(this DbSet<T> dbSet, Func<T> entityConstructor, params object[] keyValues) where T : class //, new()
    {
      var exists = await dbSet.FindAsync(keyValues);
      if (exists == null) {
        var ret = entityConstructor();
        await dbSet.AddAsync(ret);
        return ret;
      }
      return null;
    }
    public static T AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new() {
      var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
      if (!exists) {
        dbSet.Add(entity);
        return entity;
      }
      return null;
    }
    public async static Task<T> AddIfNotExistsAsync<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class //, new()
    {
      var exists = await (predicate != null ? dbSet.AnyAsync(predicate) : dbSet.AnyAsync());
      if (!exists) {
        await dbSet.AddAsync(entity);
        return entity;
      }
      return null;
    }
    // Throw exceptions
    // public async static Task AddIfNotExistsAsync<T>(this DbSet<T> dbSet, IEnumerable<T> entities, Func<T, T, bool> predicate = null) where T : class//, new()
    // {
    //     await entities.ForEachAsync(x => AddIfNotExistsAsync(dbSet, x, y => predicate(y, x)));
    // }
    // Code below will throw exceptions
    // public async static Task AddIfNotExistsAsync<T, TId>(this DbSet<T> dbSet, IEnumerable<T> entities) 
    //     where T : class, IId<TId>, new()
    //     where TId : IComparable<TId>
    // {
    //     await dbSet.AddIfNotExistsAsync(entities, (x, y) => x.Id.CompareTo(y.Id) >= 0 && x.Id.CompareTo(y.Id) <= 0);
    // }
  }
}