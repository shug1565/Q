using System;

namespace Q.Sql.Core
{
    public interface IId<T> where T : IComparable<T>
    {
        T Id { get; set; }
    }
}