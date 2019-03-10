using Q.Lib.Core.Misc;
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
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> act, Func<T, bool> breakPredicate = null, Func<T, bool> continueClause = null)
      => source.ForEach((x, i) => act(x), breakPredicate.NotNullThen<Func<T, bool>, Func<T, int, bool>>(f => (x, i) => f(x)), continueClause.NotNullThen<Func<T, bool>, Func<T, int, bool>>(f => (x, i) => f(x)));

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> act, Func<T, int, bool> breakPredicate = null, Func<T, int, bool> continueClause = null)
    {
      if (source == null) return; // throw new ArgumentNullException("source");
      if (act == null) throw new ArgumentNullException("act");

      var i = 0;
      foreach (var element in source)
      {
        if (breakPredicate.NotNullAnd(f => f(element, i))) break;
        if (continueClause.NotNullAnd(f => f(element, i))) continue;
        act(element, i++);
      }
    }

    public static Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> action) => source.ForEachAsync(x => Task.Run(() => action(x)));

    public async static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (action == null) throw new ArgumentNullException("action");

      foreach (var element in source)
        await action(element);
    }
    public static Task WhenAll<T>(this IEnumerable<T> source, Action<T> action) => source.WhenAll(x => Task.Run(() => action(x)));

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
    public static Task WhenAll<T>(this IEnumerable<T> source, Action<T> body, int dop) => source.WhenAll(x => Task.Run(() => body(x)), dop);

    public static Task WhenAny<T>(this IEnumerable<T> source, Action<T> action) => source.WhenAny(x => Task.Run(() => action(x)));

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