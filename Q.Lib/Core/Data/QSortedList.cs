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

    public IEnumerable<KeyValuePair<TKey, TValue>> GetRows((TKey st, TKey ed)? rng) => rng == null ? dict : Where(y => rng.Value.RngContains(y.Key, EdgeMode.Inc));
    public void ForEach((TKey st, TKey ed)? rng, Action<KeyValuePair<TKey, TValue>> act, Func<KeyValuePair<TKey, TValue>, bool> breakPredicate = null, Func<KeyValuePair<TKey, TValue>, bool> continueClause = null)
      => ForEach(rng, (x, i) => act(x), breakPredicate.NotNullThen<Func<KeyValuePair<TKey, TValue>, bool>, Func<KeyValuePair<TKey, TValue>, int, bool>>(f => (x, i) => f(x)), 
        continueClause.NotNullThen<Func<KeyValuePair<TKey, TValue>, bool>, Func<KeyValuePair<TKey, TValue>, int, bool>>(f => (x, i) => f(x)));
    public void ForEach((TKey st, TKey ed)? rng, Action<KeyValuePair<TKey, TValue>, int> act, Func<KeyValuePair<TKey, TValue>, int, bool> breakPredicate = null, Func<KeyValuePair<TKey, TValue>, int, bool> continueClause = null)
      => ReadLockAction(x => {
        var stIdx = rng == null ? 0 : x.Keys.BinarySearch(rng.Value.st);
        if (stIdx < 0) stIdx = ~stIdx;
        var edIdx = rng == null ? x.Count - 1: x.Keys.BinarySearch(rng.Value.ed);
        if (edIdx < 0) edIdx = ~edIdx - 1;
        for (int i = stIdx; i <= edIdx; i++)
        {
          var kvp = new KeyValuePair<TKey, TValue>(x.Keys[i], x.Values[i]);
          if (breakPredicate.NotNullAnd(f => f(kvp, i))) break;
          if (continueClause.NotNullAnd(f => f(kvp, i))) continue;
          act(kvp, i);
        }
      });
  }
}