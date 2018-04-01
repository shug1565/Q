using System.Collections.Generic;
using System.Linq;
using Q.Lib.Core.Concurrency;

namespace Q.Lib.Core.Data {
  public class QDataFrame<TRowKey, TColKey, TValue> : QSortedList<TRowKey, IDictionary<TColKey, TValue>> {

    #region ctor
    public QDataFrame() : base() { }
    public QDataFrame(IDictionary<TRowKey, IDictionary<TColKey, TValue>> data) : base(data) { }
    public QDataFrame(IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>> > data) : base(data) { }
    #endregion

    public ISet<TColKey> ColumnKeys => ReadLockFunc(x => x.GetColumnKeys());
    public int ColumnCount => ReadLockFunc(x => x.GetColumnKeys().Count);
    // Not sure what happens if TRowKey and TColKey is the same type
    public IEnumerable<KeyValuePair<TRowKey, TValue>> this[TColKey key] {
      get => ReadLockFunc(x => x.Select(y => new KeyValuePair<TRowKey, TValue>(y.Key, y.Value[key])));
      set => WriteLockAction(x => value.ForEach(y => x.Assign(y.Key, key, y.Value)));
    }
    public TValue this[TRowKey rowKey, TColKey colKey] {
      get => ReadLockFunc(x => x[rowKey][colKey]);
      set => WriteLockAction(x => x.Assign(rowKey, colKey, value));
    }
    public TValue DefaultValue { get; set; }
    // again, if TRowKey == TColKey, this probably won't work
    public bool ContainsKey(TColKey key) => ReadLockFunc(x => x.GetColumnKeys().Contains(key));
  }
}