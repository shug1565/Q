using System;

namespace Q.Lib.Finance.Currency {
  public abstract class Currency<T> : IEquatable<Currency<T>>, IComparable<Currency<T>> {
    protected long value; // value in its smallest unit
    public abstract string Sign { get; }
    public abstract string ISO { get; }
    public T DefaultUnit { get; set; }

    public override int GetHashCode() => value.GetHashCode();
    public override bool Equals(object obj) => Equals(obj as Currency<T>);
    public bool Equals(Currency<T> obj) => obj != null && obj.value == this.value;

    public int CompareTo(Currency<T> other) => value.CompareTo(other.value);
  }
}