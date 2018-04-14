using System;
using System.Collections.Generic;
using System.Linq;
using Q.Lib.Core.Linq;
using Q.Lib.Core.Misc;

namespace Q.Lib.Core.Data
{
  public class QSortedList<TKey, TValue> : QConcurrentIDictionary<TKey, TValue> where TKey : IComparable<TKey>
  {
    #region Lockers
    public T ReadLockFunc<T>(Func<SortedList<TKey, TValue>, T> f) => ReadLockFunc(() => f(dict as SortedList<TKey, TValue>));
    public void ReadLockAction(Action<SortedList<TKey, TValue>> f) => ReadLockAction(() => f(dict as SortedList<TKey, TValue>));
    public T WriteLockFunc<T>(Func<SortedList<TKey, TValue>, T> f) => WriteLockFunc(() => f(dict as SortedList<TKey, TValue>));
    public void WriteLockAction(Action<SortedList<TKey, TValue>> f) => WriteLockAction(() => f(dict as SortedList<TKey, TValue>));
    #endregion
    public QSortedList() => WriteLockAction(() => dict = new SortedList<TKey, TValue>());
    public QSortedList(IDictionary<TKey, TValue> data) => WriteLockAction(() => dict = new SortedList<TKey, TValue>(data));
    public QSortedList(IEnumerable<KeyValuePair<TKey, TValue>> data, bool forceUpdate) => WriteLockAction(() => dict = new SortedList<TKey, TValue>(data.ToDictionary(forceUpdate)));

    public IEnumerable<KeyValuePair<TKey, TValue>> GetRows((TKey st, TKey ed) rng) => Where(y => rng.RngContains(y.Key, EdgeMode.Inc));
  }
}