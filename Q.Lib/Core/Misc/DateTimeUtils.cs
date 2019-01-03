using System;
using System.Collections.Generic;
using System.Linq;

namespace Q.Lib.Core.Misc
{
  public static class DateTimeUtils
  {
    public static DateTime Posix = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    public static DateTime FromPosix(this long unixTimeStampMillis) => Posix.AddMilliseconds(unixTimeStampMillis);
    public static IEnumerable<KeyValuePair<DateTime, T>> Pair<T>(this IEnumerable<DateTime> rng, T val) => rng.Select(x => new KeyValuePair<DateTime, T>(x, val));
    public static IEnumerable<DateTime> Range(this DateTime st, TimeSpan intvl, long n)
    {
      DateTime ret = st;
      yield return ret;
      for (int i = 1; i < n; i++)
      {
        ret = ret.Add(intvl);
        yield return ret;
      }
    }
    public static IEnumerable<DateTime> Range(this DateTime st, TimeSpan intvl, DateTime ed, bool toRight)
    {
      var edge = st.BucketFunc(intvl, toRight)(ed);
      var ret = (toRight ? st.Add(intvl) : st).Range(intvl, (edge - st).Ticks / intvl.Ticks);
      return ret;
    }
    public static (DateTime st, DateTime ed) Offset(this DateTime benchmark, (TimeSpan stOffset, TimeSpan edOffset) o) => (benchmark.Add(o.stOffset), benchmark.Add(o.edOffset));
    public static Func<DateTime, DateTime> BucketFunc(this DateTime st, TimeSpan intvl, bool toRight = true) => x =>
    {
      if (intvl.Ticks <= 0) throw new Exception("intvl");
      var n = (x - st).Ticks / intvl.Ticks;
      var edge = st.AddTicks(n * intvl.Ticks);
      if (edge == x) return edge;
      DateTime ret;
      if (toRight)
        ret = x > st ? edge.Add(intvl) : edge;
      else
        ret = x > st ? edge : edge.Add(-intvl);
      return ret;
    };
    public static IEnumerable<T2> Bucket<T, T2>(this IEnumerable<KeyValuePair<DateTime, T>> x, (DateTime st, DateTime ed, TimeSpan intvl, bool toRight) p, Func<DateTime, IEnumerable<KeyValuePair<DateTime, T>>, T2> f)
      => x.Where(y => y.Key.Within(p.st, p.ed, p.toRight ? EdgeMode.LExcRInc : EdgeMode.LIncRExc)).GroupBy(y => p.st.BucketFunc(p.intvl, p.toRight)(y.Key), f);
    public static IEnumerable<KeyValuePair<DateTime, T>> BucketLast<T>(this IEnumerable<KeyValuePair<DateTime, T>> x, (DateTime st, DateTime ed, TimeSpan intvl, bool toRight) p)
      => x.Bucket(p, (y, z) => new KeyValuePair<DateTime, T>(y, z.Last().Value));
  }
}