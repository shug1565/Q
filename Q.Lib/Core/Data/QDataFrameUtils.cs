using System.Collections.Generic;
using Q.Lib.Core.Concurrency;

namespace Q.Lib.Core.Data {
  public static class QDataFrameUtils {
    public static QDataFrame<TRowKey, TColKey, TValue> ToQDataFrame<TRowKey, TColKey, TValue>(this IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>> > rows) => new QDataFrame<TRowKey, TColKey, TValue>(rows);
    public static ISet<TColKey> GetColumnKeys<TRowKey, TColKey, TValue>(this IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>> > rows) {
      var ret = new HashSet<TColKey>();
      rows.ForEach(y => ret.UnionWith(y.Value.Keys));
      return ret;
    }
    public static void Assign<TRowKey, TColKey, TValue>(this IDictionary<TRowKey, IDictionary<TColKey, TValue>> df, TRowKey rowKey, TColKey colKey, TValue value) {
      if (df.TryGetValue(rowKey, out IDictionary<TColKey, TValue> row))
        row[colKey] = value;
      else
        df[rowKey] = new Dictionary<TColKey, TValue>() { { colKey, value } };
    }
  }
}