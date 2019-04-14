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
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> a) => IsNullOrEmpty(a) ? null : new HashSet<T>(a);
    #endregion

    #region KeyValuePair utils
    public static IDictionary<TKey, TValue> ToIDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> a) => a.ToDictionary(x => x.Key, x => x.Value);
    public static KeyValuePair<TKey, TValue> ToKVP<TKey,TValue>(this (TKey key, TValue value) t) => new KeyValuePair<TKey, TValue>(t.key, t.value);
    public static IEnumerable<KeyValuePair<TKey, TValue>> WhereValueNotNull<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> a) => a.Where(x => x.Value != null);
    #endregion

    #region Tuple utils
    public static IDictionary<TKey, TValue> ToIDictionary<TKey, TValue>(this IEnumerable<(TKey k, TValue v)> a) => a.ToDictionary(x => x.k, x => x.v);
    public static bool RngContain<T>(this (T st, T ed) rng, T a, EdgeMode em = EdgeMode.Inc) where T: IComparable<T>
    {
      if (em == EdgeMode.Inc) return a.CompareTo(rng.st) >= 0 && a.CompareTo(rng.ed) <= 0;
      if (em == EdgeMode.Exc) return a.CompareTo(rng.st) > 0 && a.CompareTo(rng.ed) < 0;
      if (em == EdgeMode.LExcRInc) return a.CompareTo(rng.st) > 0 && a.CompareTo(rng.ed) <= 0;
      if (em == EdgeMode.LIncRExc) return a.CompareTo(rng.st) >= 0 && a.CompareTo(rng.ed) < 0;
      throw new InvalidOperationException("EdgeMode unrecognised.");
    }
    public static bool Within<T>(this T a, (T st, T ed) rng, EdgeMode em = EdgeMode.Inc) where T : IComparable<T>
      => a.Within(rng.st, rng.ed, em);
    public static bool Within<T>(this T a, T st, T ed, EdgeMode em = EdgeMode.Inc) where T : IComparable<T>
      => (st, ed).RngContain(a, em);
    public static (T, T2) TrimLast<T, T2, T3>(this (T, T2, T3) a) => (a.Item1, a.Item2);
    public static (T, T2, T3, T4) Extend<T, T2, T3, T4>(this (T, T2, T3) a, T4 b) => (a.Item1, a.Item2, a.Item3, b);
    public static (T, T2, T3) Extend<T, T2, T3>(this (T, T2) a, T3 b) => (a.Item1, a.Item2, b);
    public static (T, T)? ToPair<T>(this T[] a) => a == null ? ((T, T)?)null : (a[0], a[1]);
    public static (T, T, T)? ToTriple<T>(this T[] a) => a == null ? ((T, T, T)?)null : (a[0], a[1], a[2]);
    #endregion

    #region Dict
    public static bool DictCompare<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> dict2, Func<TValue, TValue, bool> equals = null)
    {
      if (dict == null && dict2 == null) return true;
      if (dict == null || dict2 == null) return false;
      if (dict.Count != dict2.Count) return false; // Require equal count.
      if (equals == null) equals = (x, y) => x.Equals(y);
      foreach (var pair in dict)
        if (!dict2.TryGetValue(pair.Key, out TValue value) || !equals(value, pair.Value)) return false;
      return true;
    }
    #endregion

    #region Network
    public static bool CheckForInternetConnection()
    {
      try
      {
        using (var client = new System.Net.WebClient())
        using (client.OpenRead("http://clients3.google.com/generate_204"))
        {
          return true;
        }
      }
      catch
      {
        return false;
      }
    }
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