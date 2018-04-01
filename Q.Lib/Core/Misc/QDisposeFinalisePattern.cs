using System;
using System.Collections;

namespace Q.Lib.Core.Misc {
  public abstract class QDisposeFinalisePattern : IDisposable {
    public bool IsDisposed { get; private set; }
    private int disposeAttempt;
    public static Queue failToClose = new Queue();
    protected object dispSyncRoot = new object();
    ~QDisposeFinalisePattern() {
      try{
        DisposeWrapper(false);
      }
      catch
      {
        if (disposeAttempt++ < 3) GC.ReRegisterForFinalize(this);
        else
        {
          lock (failToClose.SyncRoot)
            failToClose.Enqueue(this);
          throw new Exception("fail to finalise!");
        }
      }
    }
    protected void CheckIsDisposed()
    {
      if (IsDisposed) throw new Exception("Object has already been disposed!");
    }
    public void Dispose()
    {
      DisposeWrapper(true);
      GC.SuppressFinalize(this);
    }
    private void DisposeWrapper(bool isDisposing)
    {
      lock (dispSyncRoot)
      {
        if (IsDisposed) return;
        Dispose(isDisposing);
        IsDisposed = true;
      }
    }
    /// isDisposing telling it's called by dispose or finalise action
    protected abstract void Dispose(bool isDisposing);
  }
}