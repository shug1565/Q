using System;
using System.Collections.Generic;
using System.Text;

namespace Q.Lib.Core.Data
{
  public class QTimeSeries<T> : QSortedList<DateTime, T>
  {
    public T this[int key]
    {
      get => ReadLockFunc(x => x.Values[key]);
      set => WriteLockFunc(x => x.Values[key] = value);
    }
  }
}
