using System;
using System.Collections.Generic;
using System.Linq;
using ManagedXLL;
using Q.Lib.Core.Concurrency;
using Q.Lib.Core.Data;
using Q.Lib.Core.Linq;

namespace Q.XLS
{
  public static class ManagedXLLUtils
  {
    public static XlOper ToXlOper(this double a) => (double.IsInfinity(a) || double.IsNaN(a)) ? Excel.Error.NA : (XlOper)a;
    public static XlOper ToXlOper<T>(this T a)
    {
      if (a == null) return Excel.Error.NA;
      if (a is double) return Convert.ToDouble(a).ToXlOper();
      if (XlOper.CanConvertFrom(typeof(T))) return XlOper.ConvertFrom(a);
      return Excel.Error.NA;
    }
    public static XlOper[,] ToXlOper<TKey, TCol, TValue>(this QDataFrame<TKey, TCol, TValue> df, Func<TCol, TValue, XlOper> f, bool noHeader = false) where TKey : IComparable<TKey>
      => df.ReadLockFunc(x => x.ToXlOper(x.GetColumnKeys(), f, noHeader, df.DefaultValue));

    public static XlOper[,] ToXlOper<TKey, TCol, TValue>(this QDataFrame<TKey, TCol, TValue> df, IEnumerable<TCol> cols, Func<TCol, TValue, XlOper> f, bool noHeader = false) where TKey : IComparable<TKey>
      => df.ReadLockFunc(x => x.ToXlOper(cols, f, noHeader, df.DefaultValue));

    public static XlOper[,] ToXlOper<TRow, TCol, TValue>(this IEnumerable<KeyValuePair<TRow, IDictionary<TCol, TValue>>> d, IEnumerable<TCol> cols, Func<TCol, TValue, XlOper> f, bool noHeader, TValue defaultValue)
    {
      if (f == null) f = (x, y) => y.ToXlOper();
      List<List<XlOper>> tret = new List<List<XlOper>>();
      if (cols == null) cols = d.GetColumnKeys();
      if (!noHeader)
      {
        var header = new List<XlOper> { string.Empty };
        cols.ForEach(y => header.Add(y.ToXlOper()));
        tret.Add(header);
      }
      d.ForEach(x =>
      {
        var row = new List<XlOper> { x.Key.ToXlOper() };
        cols.ForEach(y => row.Add(f(y, x.Value.GetValue(y, defaultValue))));
        tret.Add(row);
      });
      return tret.To2dArray();
    }
  }
}