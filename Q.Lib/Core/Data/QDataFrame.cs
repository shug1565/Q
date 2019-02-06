using System;
using System.Collections.Generic;
using System.Linq;
using Q.Lib.Core.Concurrency;
using Q.Lib.Core.Misc;

namespace Q.Lib.Core.Data {
  public class QDataFrame<TRowKey, TColKey, TValue> : QSortedList<TRowKey, IDictionary<TColKey, TValue>>
    where TRowKey : IComparable<TRowKey>
  {
    public TValue DefaultValue { get; set; }

    #region ctor
    public QDataFrame() : base() { }
    public QDataFrame(TValue defaultValue) : base() => DefaultValue = defaultValue;
    public QDataFrame(IDictionary<TRowKey, IDictionary<TColKey, TValue>> data) : base(data) { }
    public QDataFrame(IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>>> data, bool forceUpdate) : base(data, forceUpdate) { }
    #endregion

    public ISet<TColKey> ColumnKeys => ReadLockFunc(x => x.GetColumnKeys());
    public int ColumnCount => ReadLockFunc(x => x.GetColumnKeys().Count);
    // Not sure what happens if TRowKey and TColKey is the same type
    public IEnumerable<KeyValuePair<TRowKey, TValue>> this[TColKey colKey] {
      get => ReadLockFunc(x => x.Select(y => new KeyValuePair<TRowKey, TValue>(y.Key, y.Value.GetValue(colKey, DefaultValue))));
      set => WriteLockAction(x => value.ForEach(y => x.Assign(y.Key, colKey, y.Value)));
    }
    public TValue this[TRowKey rowKey, TColKey colKey] {
      get => ReadLockFunc(x => x[rowKey].GetValue(colKey, DefaultValue));
      set => WriteLockAction(x => x.Assign(rowKey, colKey, value));
    }
    // again, if TRowKey == TColKey, this probably won't work
    public bool ContainsKey(TColKey key) => ReadLockFunc(x => x.GetColumnKeys().Contains(key));
    public IEnumerable<KeyValuePair<TRowKey, IDictionary<TColKey, TValue>>> GetColumns(IEnumerable<TColKey> colKeys) => ReadLockFunc(x =>
      x.Select(y => (y.Key, y.Value.SubDict(colKeys) as IDictionary<TColKey, TValue>).ToKVP()).WhereValueNotNull());
    public void AddColumn(TColKey colKey, IEnumerable<KeyValuePair<TRowKey, TValue>> col) => WriteLockAction(x => x.AddColumn(colKey, col));
    public QDataFrame<TRowKey, TColKey, TValue> Compress(Func<TValue, TValue, bool> equals = null) => SelectUpdates((x, y) => x.Value.DictCompare(y.Value, equals)).ToQDataFrame(false);
  }
}