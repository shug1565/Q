using System.Collections.Generic;
using System.Linq;
using Q.Lib.Core.Linq;

namespace Q.Lib.Core.Data
{
    public class QSortedList<TKey, TValue> : QConcurrentIDictionary<TKey, TValue>
    {
        public QSortedList() => WriteLockAction(() => dict = new SortedList<TKey, TValue>());
        public QSortedList(IDictionary<TKey, TValue> data) => WriteLockAction(() => dict = new SortedList<TKey, TValue>(data));
        public QSortedList(IEnumerable<KeyValuePair<TKey, TValue>> data) => WriteLockAction(() => dict = new SortedList<TKey, TValue>(data.ToDictionary()));
        
    }
}