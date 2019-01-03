using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedXLL;
using Q.Lib.Core.Data;
using Q.Lib.Core.Misc;

namespace Q.XLS
{
  public static class Test
  {
    #region Bucket
    [WorksheetFunction("QTestBucket")]
    public static XlOper[,] QBucketTest(DateTime[] t, double[] v, DateTime st, DateTime ed, int intvl, bool toright)
    {
      var a = t.Zip(v, (x, y) => (x, y).ToKVP());
      var b = a.Bucket((st, ed, TimeSpan.FromSeconds(intvl), toright), (x, y) => (x, y.Sum(z => z.Value)).ToKVP());
      var ret = b.ToXlOper();
      return ret;
    }
    [WorksheetFunction("QTestSample")]
    public static XlOper[,] QSampleTest(DateTime[] t, double[] v, DateTime st, DateTime ed, int intvl, bool toright, FillMode fillMode)
    {
      var a = t.Zip(v, (x, y) => (x, y).ToKVP()).ToQTimeSeries();
      var b = a.Sample((st, ed, TimeSpan.FromSeconds(intvl), toright), (fillMode, 0));
      var ret = b.Select(x => x).ToXlOper();
      return ret;
    }
    [WorksheetFunction("QTestRange")]
    public static DateTime[] QRangeTest(DateTime st, int intvl, DateTime ed, bool toright) => st.Range(TimeSpan.FromSeconds(intvl), ed, toright).ToArray();
    #endregion
  }
}
