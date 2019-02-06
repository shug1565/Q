using Q.Lib.Core.Linq;
using Q.Lib.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Q.Lib.Core.Data
{
  public class QTimeSeries<T> : QSortedList<DateTime, T>
  {
    public QTimeSeries() : base() { }
    public QTimeSeries(IEnumerable<KeyValuePair<DateTime, T>> a, bool forceUpdate = false) : base(a, forceUpdate) { }

    public IEnumerable<T2> Bucket<T2>((DateTime st, DateTime ed, TimeSpan intvl) p, Func<IEnumerable<KeyValuePair<DateTime, T>>, T2> f) => Bucket(p, (x, y) => f(y));
    public IEnumerable<T2> Bucket<T2>((DateTime st, DateTime ed, TimeSpan intvl) p, Func<DateTime, IEnumerable<KeyValuePair<DateTime, T>>, T2> f) => Bucket(p.Extend(true), f);
    public IEnumerable<T2> Bucket<T2>((DateTime st, DateTime ed, TimeSpan intvl, bool toRight) p, Func<DateTime, IEnumerable<KeyValuePair<DateTime, T>>, T2> f) => ReadLockFunc(x => x.Bucket(p, f));

    public T Sample(DateTime t, T nullVal) => ReadLockFunc(x => {
      var i = x.Keys.BinarySearch(t);
      if (i >= 0) return x.Values[i];
      var j = ~i;
      if (j == 0) return nullVal;
      return x.Values[j - 1];
    });
    public QTimeSeries<T> Sample((DateTime st, DateTime ed, TimeSpan intvl) p, (FillMode mode, T nullVal) fillPara) => Sample(p.Extend(true), fillPara);
    public QTimeSeries<T> Sample((DateTime st, DateTime ed, TimeSpan intvl, bool toRight) p, (FillMode mode, T nullVal) fillPara)
      => ReadLockFunc(x => {
        var s = x.BucketLast(p).ToDictionary(false);
        if (fillPara.mode == FillMode.Missing) return s.ToQTimeSeries();
        var ret = p.st.Range(p.intvl, p.ed, p.toRight).AggregateSequence(new KeyValuePair<DateTime, T>(DateTime.MinValue, fillPara.nullVal), (y, z) => {
            if (s.TryGetValue(z, out T val))
              return new KeyValuePair<DateTime, T>(z, val);
            else if (fillPara.mode == FillMode.FwdFill)
              return new KeyValuePair<DateTime, T>(z, y.Value);
            else
              return new KeyValuePair<DateTime, T>(z, fillPara.nullVal);
          });
        return ret.ToQTimeSeries();
      });

    public QTimeSeries<T> Compress(Func<T, T, bool> equals = null) => SelectUpdates((x, y) => equals == null ? x.Value.Equals(y.Value) : equals(x.Value, y.Value)).ToQTimeSeries();
    //public T this[int key]
    //{
    //  get => ReadLockFunc(x => x.Values[key]);
    //  set => WriteLockAction(x => x.Values[key] = value);
    //}
  }
  public static class QTimeSeriesUtils
  {
    public static QTimeSeries<T> ToQTimeSeries<T>(this IEnumerable<KeyValuePair<DateTime, T>> a, bool forceUpdate = false) => new QTimeSeries<T>(a, forceUpdate);
    public static QTimeSeries<T> ToQTimeSeries<T>(this IEnumerable<(DateTime, T)> a, bool forceUpdate = false) => new QTimeSeries<T>(a.Select(x => x.ToKVP()), forceUpdate);

  }
  public enum FillMode
  {
    Missing,
    FwdFill,
    FillNullVal
  }
}
