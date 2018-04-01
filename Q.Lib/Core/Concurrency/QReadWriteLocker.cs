using System;
using System.Threading;
using Q.Lib.Core.Misc;

namespace Q.Lib.Core.Concurrency {
  public class QReadWriteLocker : QDisposeFinalisePattern {
    protected ReaderWriterLockSlim syncRoot = new ReaderWriterLockSlim();
    public T ReadLockFunc<T>(Func<T> f) {
      syncRoot.EnterReadLock();
      try { return f(); } finally { syncRoot.ExitReadLock(); }
    }
    public void ReadLockAction(Action f) {
      syncRoot.EnterReadLock();
      try { f(); } finally { syncRoot.ExitReadLock(); }
    }
    public T WriteLockFunc<T>(Func<T> f) {
      syncRoot.EnterWriteLock();
      try { return f(); } finally { syncRoot.ExitWriteLock(); }
    }
    public void WriteLockAction(Action f) {
      syncRoot.EnterWriteLock();
      try { f(); } finally { syncRoot.ExitWriteLock(); }
    }
    protected override void Dispose(bool isDisposing) { if (syncRoot != null) syncRoot.Dispose(); }
  }
}