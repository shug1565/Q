using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Q.Lib.Core.Concurrency
{
  public static class Loops
  {
    #region ForEach
    /// <summary>
    /// Immediately executes the given action on each element in the source sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence of elements</param>
    /// <param name="action">The action to execute on each element</param>

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (action == null) throw new ArgumentNullException("action");

      foreach (var element in source)
        action(element);
    }
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (action == null) throw new ArgumentNullException("action");

      var index = 0;
      foreach (var element in source)
        action(element, index++);
    }
    public static Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> action)
    {
      return source.ForEachAsync(x => Task.Run(() => action(x)));
    }
    // ForEachAsync is a sync operation, chain all async action one after anther. WhenAll is the real parallel operation
    public async static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (action == null) throw new ArgumentNullException("action");

      foreach (var element in source)
        await action(element);
    }
    public static Task WhenAll<T>(this IEnumerable<T> source, Action<T> action)
    {
      return source.WhenAll(x => Task.Run(() => action(x)));
    }
    public static Task WhenAll<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (action == null) throw new ArgumentNullException("action");

      return Task.WhenAll(source.Select(x => action(x)));
    }
    public static Task WhenAll<T>(this IEnumerable<T> source, Func<T, Task> body, int dop)
    {
      return Task.WhenAll(Partitioner.Create(source).GetPartitions(dop).Select(async partition =>
      {
        using (partition)
          while (partition.MoveNext()) await body(partition.Current);
      }));
    }
    public static Task WhenAll<T>(this IEnumerable<T> source, Action<T> body, int dop)
    {
      return source.WhenAll(x => Task.Run(() => body(x)), dop);
    }
    public static Task WhenAny<T>(this IEnumerable<T> source, Action<T> action)
    {
      return source.WhenAny(x => Task.Run(() => action(x)));
    }
    public static Task WhenAny<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (action == null) throw new ArgumentNullException("action");

      return Task.WhenAny(source.Select(x => action(x)));
    }

    /// <summary>
    /// Immediately executes the given action on each element in the source sequence.
    /// Each element's index is used in the logic of the action.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence of elements</param>
    /// <param name="action">The action to execute on each element; the second parameter
    /// of the action represents the index of the source element.</param>


    #endregion
  }
}