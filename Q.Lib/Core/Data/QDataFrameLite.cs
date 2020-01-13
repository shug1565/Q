using Q.Lib.Core.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Q.Lib.Core.Data
{
  public class QDataFrameLite<TRowKey, TColKey, TVal>
  {
    public TwoWayDict<TRowKey, int> RowKeys { get; protected set; }
    public TwoWayDict<TColKey, int> ColKeys { get; protected set; }
    public TVal[,] Data { get; protected set; }
    public QDataFrameLite(IEnumerable<TRowKey> rowKeys, IEnumerable<TColKey> colKeys)
    {
      rowKeys.ForEach((x, i) => RowKeys.Add(x, i));
      colKeys.ForEach((x, i) => ColKeys.Add(x, i));
      Data = new TVal[RowKeys.Count, ColKeys.Count];
    }
    public void Add(TRowKey row, TColKey col, TVal val)
      => Data[RowKeys.Forward[row], ColKeys.Forward[col]] = val;
    public void AddRange(IEnumerable<(TRowKey row, TColKey col, TVal val)> data)
      => data.ForEach(x => Add(x.row, x.col, x.val));
    public void AddRow(IEnumerable<(TColKey key, TVal val)> cols, int i)
      => cols.ForEach(x => Data[i, ColKeys.Forward[x.key]] = x.val);
    public void AddRow(IEnumerable<(TColKey key, TVal val)> cols, TRowKey row)
      => AddRow(cols, RowKeys.Forward[row]);
    public void AddCol(IEnumerable<(TRowKey key, TVal val)> rows, int j)
      => rows.ForEach(x => Data[RowKeys.Forward[x.key], j] = x.val);
    public void AddCol(IEnumerable<(TRowKey key, TVal val)> row, TColKey col)
      => AddCol(row, ColKeys.Forward[col]);
  }
}
