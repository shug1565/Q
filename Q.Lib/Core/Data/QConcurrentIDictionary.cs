using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Q.Lib.Core.Concurrency;
using Q.Lib.Core.Linq;

namespace Q.Lib.Core.Data {
  public class QConcurrentIDictionary<TKey, TValue> : QReadWriteLocker, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>> {
    protected virtual IDictionary<TKey, TValue> dict { get; set; }

    public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) => WriteLockAction(x => items.ForEach(y => x.Add(y.Key, y.Value)));
    public void RemoveRange(IEnumerable<KeyValuePair<TKey, TValue>> items) => WriteLockAction(x => items.ForEach(y => x.Remove(y)));
    public void RemoveRange(IEnumerable<TKey> keys) => WriteLockAction(x => keys.ForEach(y => x.Remove(y)));
    public void ForEach(Action<KeyValuePair<TKey, TValue>> g) => ReadLockAction(x => x.ForEach(y => g(y)));
    public void ForEach(Action<KeyValuePair<TKey, TValue>, int> act, Func<KeyValuePair<TKey, TValue>, int, bool> breakPredicate = null, Func<KeyValuePair<TKey, TValue>, int, bool> continueClause = null)
      => ReadLockAction(x => x.ForEach(act, breakPredicate, continueClause));
    public void ForEach(Action<KeyValuePair<TKey, TValue>> act, Func<KeyValuePair<TKey, TValue>, bool> breakPredicate = null, Func<KeyValuePair<TKey, TValue>, bool> continueClause = null)
      => ReadLockAction(x => x.ForEach(act, breakPredicate, continueClause));
    public IEnumerable<KeyValuePair<TKey, TValue>> Where(Func<KeyValuePair<TKey, TValue>, bool> p) => ReadLockFunc(x => x.Where(p));
    public IEnumerable<T> Select<T>(Func<KeyValuePair<TKey, TValue>, T> f) => ReadLockFunc(x => x.Select(f));
    public IEnumerable<IGrouping<T, KeyValuePair<TKey, TValue>>> GroupBy<T>(Func<KeyValuePair<TKey, TValue>, T> f) => ReadLockFunc(x => x.GroupBy(f));
    public IEnumerable<TRes> GroupBy<T, TRes>(Func<KeyValuePair<TKey, TValue>, T> f, Func<T, IEnumerable<KeyValuePair<TKey, TValue>>, TRes> g) => ReadLockFunc(x => x.GroupBy(f, g));
    public KeyValuePair<TKey, TValue> Aggregate(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> f) => ReadLockFunc(x => x.Aggregate(f));
    public T Aggregate<T>(T seed, Func<T, KeyValuePair<TKey, TValue>, T> f) => ReadLockFunc(x => x.Aggregate(seed, f));
    public IEnumerable<T> AggregateSequence<T>(T seed, Func<T, KeyValuePair<TKey, TValue>, T> aggregator) => ReadLockFunc(x => x.AggregateSequence(seed, aggregator));
    public IEnumerable<T2> AggregateSequence<T, T2>(T seed, Func<T, KeyValuePair<TKey, TValue>, T> aggregator, Func<T, T2> resultSelector) => ReadLockFunc(x => x.AggregateSequence(seed, aggregator, resultSelector));
    public KeyValuePair<TKey, TValue> ElementAt(int i) => ReadLockFunc(x => i >= 0 ? x.ElementAt(i) : x.ElementAt(x.Count() + i));
    public List<KeyValuePair<TKey, TValue>> SelectUpdates(KeyValuePair<TKey, TValue> seed, Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, bool> equals = null) => ReadLockFunc(x => x.SelectUpdates(seed, equals));
    public List<KeyValuePair<TKey, TValue>> SelectUpdates(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, bool> equals = null) => ReadLockFunc(x => x.SelectUpdates(x.First(), equals));

    #region LockHelpers
    public virtual T ReadLockFunc<T>(Func<IDictionary<TKey, TValue>, T> f) => ReadLockFunc(() => f(dict));
    public virtual void ReadLockAction(Action<IDictionary<TKey, TValue>> f) => ReadLockAction(() => f(dict));
    public virtual T WriteLockFunc<T>(Func<IDictionary<TKey, TValue>, T> f) => WriteLockFunc(() => f(dict));
    public virtual void WriteLockAction(Action<IDictionary<TKey, TValue>> f) => WriteLockAction(() => f(dict));
    #endregion

    #region Interface implementation
    public TValue this [TKey key] {
      get => ReadLockFunc(x => x[key]);
      set => WriteLockAction(x => x[key] = value);
    }

    public ICollection<TKey> Keys => ReadLockFunc(x => x.Keys);

    public ICollection<TValue> Values => ReadLockFunc(x => x.Values);

    public KeyValuePair<TKey, TValue> First => ReadLockFunc(x => x.First());
    public KeyValuePair<TKey, TValue> Last => ReadLockFunc(x => x.Last());

    public int Count => ReadLockFunc(x => x.Count);

    public bool IsReadOnly => ReadLockFunc(x => x.IsReadOnly);

    public void Add(TKey key, TValue value) => WriteLockAction(x => x.Add(key, value));

    public void Add(KeyValuePair<TKey, TValue> item) => WriteLockAction(x => x.Add(item));

    public void Clear() => WriteLockAction(x => x.Clear());

    public bool Contains(KeyValuePair<TKey, TValue> item) => ReadLockFunc(x => x.Contains(item));

    public bool ContainsKey(TKey key) => ReadLockFunc(x => x.ContainsKey(key));

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ReadLockAction(x => x.CopyTo(array, arrayIndex));
    // Don't want to expose the enumerator for the concurrency reason
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => throw new System.NotImplementedException();

    public bool Remove(TKey key) => WriteLockFunc(x => x.Remove(key));

    public bool Remove(KeyValuePair<TKey, TValue> item) => WriteLockFunc(x => x.Remove(item));

    public bool TryGetValue(TKey key, out TValue value)
    {
      syncRoot.EnterReadLock();
      try { return dict.TryGetValue(key, out value); }
      finally { syncRoot.ExitReadLock(); }
    }

    IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
    #endregion
  }
}