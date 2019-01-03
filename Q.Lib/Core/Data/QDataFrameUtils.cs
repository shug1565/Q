using System;
using System.Collections.Generic;
using System.Linq;
using Q.Lib.Core.Concurrency;
using Q.Lib.Core.Linq;

namespace Q.Lib.Core.Data
{
  public static class QDataFrameUtils
  {
    public static QDataFrame<TRowKey, TColKey, TValue> ToQDataFrame<TRowKey, TColKey, TValue>(this IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>>> rows, bool forceUpdate)
      where TRowKey : IComparable<TRowKey>
      => new QDataFrame<TRowKey, TColKey, TValue>(rows, forceUpdate);
    public static ISet<TColKey> GetColumnKeys<TRowKey, TColKey, TValue>(this IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>>> rows)
    {
      var ret = new HashSet<TColKey>();
      rows.ForEach(y => ret.UnionWith(y.Value.Keys));
      return ret;
    }
    public static void Assign<TRowKey, TColKey, TValue>(this IDictionary<TRowKey, IDictionary<TColKey, TValue>> df, TRowKey rowKey, TColKey colKey, TValue value)
    {
      if (df.TryGetValue(rowKey, out IDictionary<TColKey, TValue> row))
        row[colKey] = value;
      else
        df[rowKey] = new Dictionary<TColKey, TValue>() { { colKey, value } };
    }
    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey colKey, TValue defaultValue)
      => col.TryGetValue(colKey, out TValue ret) ? ret : defaultValue;
    public static Dictionary<TKey, TValue> SubDict<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
    {
      Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
      keys.ForEach(x =>
      {
        if (dict.TryGetValue(x, out TValue val))
          ret.Add(x, val);
      });
      return ret.Count == 0 ? null : ret;
    }
  }
}