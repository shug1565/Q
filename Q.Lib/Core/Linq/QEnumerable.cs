using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Q.Lib.Core.Concurrency;

namespace Q.Lib.Core.Linq
{
  public static class QEnumerable
  {
    #region Misc
    public static bool NotNullOrEmpty<T>(this IEnumerable<T> x) => x != null && x.Count() > 0;
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> x) => x.Where(e => e != null);
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> x) where T : struct => x.Where(e => e != null).Select(e => e.Value);
    public static int IndexOf<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> predicate)
    {
      var index = 0;
      foreach (var item in items)
      {
        if (predicate(item)) return index;
        index++;
      }
      return -1;
    }
    public static IEnumerable<R> AggregateSequence<A, R>(this IEnumerable<A> items, R seed, Func<R, A, R> aggregator) => items.AggregateSequence(seed, aggregator, x => x);
    public static IEnumerable<T> AggregateSequence<A, R, T>(this IEnumerable<A> items, R seed, Func<R, A, R> aggregator, Func<R, T> resultSelector)
    {
      // Error cases go here.
      R t = seed;
      foreach (A item in items)
      {
        t = aggregator(t, item);
        yield return resultSelector(t);
      }
    }
    public static List<T> SelectUpdates<T>(this IEnumerable<T> rows, T seed, Func<T, T, bool> equals = null)
    {
      if (equals == null) equals = (x, y) => x.Equals(y);
      var ret = rows.Aggregate((new List<T>(), seed), (x, y) =>
      {
        if (!equals(y, x.Item2)) x.Item1.Add(y);
        return (x.Item1, y);
      }, x => x.Item1);
      return ret.Count == 0 ? null : ret;
    }
    public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b)
      => a.Intersect(b).Count() == a.Count();
    #endregion

    #region ToType
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> data, bool forceUpdate)
    {
      if (!forceUpdate) return data.ToDictionary(x => x.Key, x => x.Value);
      var ret = new Dictionary<TKey, TValue>();
      data.ForEach(x => ret[x.Key] = x.Value);
      return ret;
    }
    #endregion

    #region DistinctBy
    /// <summary>
    /// Returns all distinct elements of the given source, where "distinctness"
    /// is determined via a projection and the default equality comparer for the projected type.
    /// </summary>
    /// <remarks>
    /// This operator uses deferred execution and streams the results, although
    /// a set of already-seen keys is retained. If a key is seen multiple times,
    /// only the first element with that key is returned.
    /// </remarks>
    /// <typeparam name="TSource">Type of the source sequence</typeparam>
    /// <typeparam name="TKey">Type of the projected element</typeparam>
    /// <param name="source">Source sequence</param>
    /// <param name="keySelector">Projection for determining "distinctness"</param>
    /// <returns>A sequence consisting of distinct elements from the source sequence,
    /// comparing them by the specified key projection.</returns>

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      return source.DistinctBy(keySelector, null);
    }

    /// <summary>
    /// Returns all distinct elements of the given source, where "distinctness"
    /// is determined via a projection and the specified comparer for the projected type.
    /// </summary>
    /// <remarks>
    /// This operator uses deferred execution and streams the results, although
    /// a set of already-seen keys is retained. If a key is seen multiple times,
    /// only the first element with that key is returned.
    /// </remarks>
    /// <typeparam name="TSource">Type of the source sequence</typeparam>
    /// <typeparam name="TKey">Type of the projected element</typeparam>
    /// <param name="source">Source sequence</param>
    /// <param name="keySelector">Projection for determining "distinctness"</param>
    /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
    /// If null, the default equality comparer for <c>TSource</c> is used.</param>
    /// <returns>A sequence consisting of distinct elements from the source sequence,
    /// comparing them by the specified key projection.</returns>

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (keySelector == null) throw new ArgumentNullException("keySelector");
      return DistinctByImpl(source, keySelector, comparer);
    }

    private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      var knownKeys = new HashSet<TKey>(comparer);
      foreach (var element in source)
      {
        if (knownKeys.Add(keySelector(element)))
        {
          yield return element;
        }
      }
    }
    #endregion

    #region Array
    //public static object[,] To2dArray(this IEnumerable<ITuple> a)
    //{
    //  var t = a.Select(x =>
    //  {
    //    var tret = new List<object>();
    //    for (int j = 0; j < x.Length; j++)
    //      tret.Add(x[j]);
    //    return tret;
    //  });
    //  var ret = t.To2dArray();
    //  return ret;
    //}
    public static T[,] To2dArray<T>(this IEnumerable<(T, T)> a)
    {
      T[,] ret = new T[a.Count(), 2];
      a.ForEach((x, i) => { ret[i, 0] = x.Item1; ret[i, 1] = x.Item2; });
      return ret;
    }
    public static T[,] To2dArray<T>(this IEnumerable<(T, T, T)> a)
    {
      T[,] ret = new T[a.Count(), 3];
      a.ForEach((x, i) => { ret[i, 0] = x.Item1; ret[i, 1] = x.Item2; ret[i, 2] = x.Item3; });
      return ret;
    }
    public static T[,] To2dArray<T>(this IEnumerable<(T, T, T, T)> a)
    {
      T[,] ret = new T[a.Count(), 4];
      a.ForEach((x, i) => { ret[i, 0] = x.Item1; ret[i, 1] = x.Item2; ret[i, 2] = x.Item3; ret[i, 3] = x.Item4; });
      return ret;
    }
    public static T[,] To2dArray<T>(this IEnumerable<IEnumerable<T>> a)
    {
      T[,] ret = new T[a.Count(), a.Max(x => x.Count())];
      a.ForEach((x, i) => x.ForEach((y, j) => ret[i, j] = y));
      return ret;
    }
    public static T[,] Transpose<T>(this T[,] a)
    {
      int n = a.GetLength(0);
      int m = a.GetLength(1);
      T[,] ret = new T[m, n];
      for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
          ret[j, i] = a[i, j];
      return ret;
    }
    #endregion

    public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T> comparer = null)
    {
      if (list == null)
        throw new ArgumentNullException(nameof(list));

      comparer = comparer ?? Comparer<T>.Default;

      int lower = 0;
      int upper = list.Count - 1;

      while (lower <= upper)
      {
        int middle = lower + (upper - lower) / 2;
        int comparisonResult = comparer.Compare(value, list[middle]);
        if (comparisonResult == 0)
          return middle;
        else if (comparisonResult < 0)
          upper = middle - 1;
        else
          lower = middle + 1;
      }

      return ~lower;
    }
    public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
    {
      for (int i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
        yield return new List<T>(source.Skip(size * i).Take(size));
    }
    public static (T min, T max) MinMax<T>(this IEnumerable<T> data) where T : IComparable<T>
    {
      T first = data.First();
      var ret = (min: first, max: first);
      data.Skip(1).ForEach(x =>
      {
        if (x.CompareTo(ret.min) < 0)
          ret.min = x;
        else if (x.CompareTo(ret.max) > 0)
          ret.max = x;
      });
      return ret;
    }
    public static T[,] InitArr<T>(this (int r, int c) dim, T val)
    {
      T[,] ret = new T[dim.r, dim.c];
      for (int i = 0; i < dim.r; i++)
        for (int j = 0; j < dim.c; j++)
          ret[i, j] = val;
      return ret;
    }
    public static T2 F<T, T2>(this IList<T> source, Func<IList<T>, T2> f)
    {
      return f(source);
    }
    public static List<T2> F<T, T2>(this IList<T> source, Func<T, T2> f)
    {
      List<T2> ret = new List<T2>(source.Count);
      for (int i = 0; i < source.Count; i++)
      {
        ret.Add(f(source[i]));
      }
      return ret;
    }
    public static int Diff(this IList<int> a)
    {
      if (a.Count != 2)
        throw new InvalidOperationException("length");
      return a[0] - a[1];
    }
    public static int Prod(this IList<int> a)
    {
      int ret = 1;
      a.ForEach(x => { ret *= x; });
      return ret;
    }
    public static int Div(this IList<int> a)
    {
      if (a.Count != 2)
        throw new InvalidOperationException("length");
      return a[0] / a[1];
    }
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
      T[] result = new T[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }
    public static string Concat(this IEnumerable<string> a, string delimiter = " ")
    {
      var ret = a.Aggregate((i, j) => i + delimiter + j);
      return ret;
    }
  }
}