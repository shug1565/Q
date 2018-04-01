using System;
using System.Collections.Generic;

namespace Q.Lib.Core.Misc {
  public static class Utils {
    public static double ToDbl(this decimal x) => Convert.ToDouble(x);
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
    #region KeyValuePair utils
    public static KeyValuePair<TKey, TValue> ToKVP<TKey,TValue>(this (TKey key, TValue value) t) => new KeyValuePair<TKey, TValue>(t.key, t.value);
    #endregion
  }
}