using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ManagedXLL;
using Q.Lib.Core.Concurrency;
using Q.Lib.Core.Data;
using Q.Lib.Core.Linq;
using Q.Lib.Core.Misc;

namespace Q.XLS
{
  public static class ManagedXLLUtils
  {
    public static XlOper ToXlOper(this double a) => (double.IsInfinity(a) || double.IsNaN(a)) ? Excel.Error.NA : (XlOper)a;
    public static XlOper ToXlOper<T>(this T a)
    {
      if (a == null) return Excel.Error.NA;
      if (XlOper.CanConvertFrom(a.GetType())) return XlOper.ConvertFrom(a);
      return Excel.Error.NA;
    }
    public static XlOper[,] ToXlOper<T, T2>(this IEnumerable<KeyValuePair<T, T2>> a)
    {
      if (a == null) return null;
      int n = a.Count();
      if (n == 0) return null;
      XlOper[,] ret = new XlOper[n, 2];
      a.ForEach((x, i) => { ret[i, 0] = x.Key.ToXlOper(); ret[i, 1] = x.Value.ToXlOper(); });
      return ret;
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
    public static XlOper[,] GetPropertiesInXlOper<T>(this IEnumerable<T> a)
    {
      if (a.IsNullOrEmpty()) return null;
      var p = a.First().GetProperties();
      List<IEnumerable<XlOper>> ret = new List<IEnumerable<XlOper>>();
      ret.Add(p.GetPropertyNames().Select(x => x.ToXlOper()));
      ret.AddRange(a.Select(x => p.GetPropertyValues(x)));
      return ret.To2dArray();
    }
    public static PropertyInfo[] GetProperties(this object a) => a.GetType().GetProperties();

    public static IEnumerable<string> GetPropertyNames(this IEnumerable<PropertyInfo> p) => p.Select(x => x.Name);

    public static IEnumerable<XlOper> GetPropertyValues(this IEnumerable<PropertyInfo> p, object a) => p.Select(x => x.GetValue(a).ToXlOper());
  }
}