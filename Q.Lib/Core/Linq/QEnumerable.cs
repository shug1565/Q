using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Q.Lib.Core.Concurrency;

namespace Q.Lib.Core.Linq {
  public static class QEnumerable {
    #region Misc
    public static bool NotNullOrEmpty<T>(this IEnumerable<T> x) => x != null && x.Count() > 0;
    #endregion
    #region ToType
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> data) => data.ToDictionary(x => x.Key, x => x.Value);
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
      Func<TSource, TKey> keySelector) {
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
      Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) {
      if (source == null) throw new ArgumentNullException("source");
      if (keySelector == null) throw new ArgumentNullException("keySelector");
      return DistinctByImpl(source, keySelector, comparer);
    }

    private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) {
      var knownKeys = new HashSet<TKey>(comparer);
      foreach (var element in source) {
        if (knownKeys.Add(keySelector(element))) {
          yield return element;
        }
      }
    }
    #endregion

    

    #region Array
    public static List<T> ToList<T>(this IList<T> a) => a.ToList();
    #endregion
    public static(T min, T max) MinMax<T>(this IEnumerable<T> data) where T : IComparable<T> {
      T first = data.First();
      var ret = (min: first, max: first);
      data.Skip(1).ForEach(x => {
        if (x.CompareTo(ret.min) < 0)
          ret.min = x;
        else if (x.CompareTo(ret.max) > 0)
          ret.max = x;
      });
      return ret;
    }
    public static T2 F<T, T2>(this IList<T> source, Func<IList<T>, T2> f) {
      return f(source);
    }
    public static List<T2> F<T, T2>(this IList<T> source, Func<T, T2> f) {
      List<T2> ret = new List<T2>(source.Count);
      for (int i = 0; i < source.Count; i++) {
        ret.Add(f(source[i]));
      }
      return ret;
    }
    public static int Diff(this IList<int> a) {
      if (a.Count != 2)
        throw new InvalidOperationException("length");
      return a[0] - a[1];
    }
    public static int Prod(this IList<int> a) {
      int ret = 1;
      a.ForEach(x => { ret *= x; });
      return ret;
    }
    public static int Div(this IList<int> a) {
      if (a.Count != 2)
        throw new InvalidOperationException("length");
      return a[0] / a[1];
    }
    public static T[] SubArray<T>(this T[] data, int index, int length) {
      T[] result = new T[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }
    public static string Concat(this IEnumerable<string> a, string delimiter = " ") {
      var ret = a.Aggregate((i, j) => i + delimiter + j);
      return ret;
    }
  }
}