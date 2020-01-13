using System;
using System.Collections.Generic;
using System.Text;

namespace Q.Lib.Core.Data
{
  public class TwoWayDict<T, T2>
  {
    public Dictionary<T, T2> Forward { get; protected set; } = new Dictionary<T, T2>();
    public Dictionary<T2, T> Reverse { get; protected set; } = new Dictionary<T2, T>();
    public int Count => Forward.Count;
    public void Add(T t, T2 t2)
    {
      Forward.Add(t, t2);
      Reverse.Add(t2, t);
    }
  }
}
