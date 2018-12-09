using System;
using System.Collections.Generic;
using System.Linq;

namespace Q.Lib.Core.Misc {
  public static class Utils {
    #region enum
    public static T? ParseEnum<T>(this string value) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
        if (string.IsNullOrEmpty(value)) return null;

        foreach (T item in Enum.GetValues(typeof(T)))
        {
            if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return item;
        }
        return null;
    }
    #endregion

    #region null/nan/empty
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> a) => a == null || a.Count() == 0;
    public static bool NotNullAnd<T>(this T a, Func<T, bool> f) => a != null && f(a);
    // Need a Nullable version?
    public static T2 NotNullThen<T, T2>(this T a, Func<T, T2> f) where T2: class => a == null ? null : f(a);
    #endregion

    #region type conversion
    public static double ToDbl(this decimal x) => Convert.ToDouble(x);
    public static List<T> ToSingleItemList<T>(this T a) => new List<T> { a };
    public static HashSet<T> ToHashSet<T>(this T a) => new HashSet<T>() { a };
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> a) => new HashSet<T>(a);
    #endregion

    #region KeyValuePair utils
    public static IDictionary<TKey, TValue> ToIDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> a) => a.ToDictionary(x => x.Key, x => x.Value);
    public static KeyValuePair<TKey, TValue> ToKVP<TKey,TValue>(this (TKey key, TValue value) t) => new KeyValuePair<TKey, TValue>(t.key, t.value);
    public static IEnumerable<KeyValuePair<TKey, TValue>> WhereValueNotNull<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> a) => a.Where(x => x.Value != null);
    #endregion

    #region Tuple utils
    public static IDictionary<TKey, TValue> ToIDictionary<TKey, TValue>(this IEnumerable<(TKey k, TValue v)> a) => a.ToDictionary(x => x.k, x => x.v);
    public static bool RngContains<T>(this (T st, T ed) rng, T a, EdgeMode em) where T: IComparable<T>
    {
      if (em == EdgeMode.Inc) return a.CompareTo(rng.st) >= 0 && a.CompareTo(rng.ed) <= 0;
      if (em == EdgeMode.Exc) return a.CompareTo(rng.st) > 0 && a.CompareTo(rng.ed) < 0;
      if (em == EdgeMode.LExcRInc) return a.CompareTo(rng.st) > 0 && a.CompareTo(rng.ed) <= 0;
      if (em == EdgeMode.LIncRExc) return a.CompareTo(rng.st) >= 0 && a.CompareTo(rng.ed) < 0;
      throw new InvalidOperationException("EdgeMode unrecognised.");
    }
    public static (T, T)? ToPair<T>(this T[] a) => a == null ? ((T, T)?)null : (a[0], a[1]);
    public static (T, T, T)? ToTriple<T>(this T[] a) => a == null ? ((T, T, T)?)null : (a[0], a[1], a[2]);
    #endregion
  }
  public enum EdgeMode
  {
    Inc,
    Exc,
    LIncRExc,
    LExcRInc
  }
}