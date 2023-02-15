namespace Xerris.Extensions.Common;

public static class EnumerableExtensions
{
    /// <summary>
    /// Execute an action over each item in the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The items to iterate over.</param>
    /// <param name="action">The action to execute on the items</param>
    /// <returns>The collection.</returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        var array = source as T[] ?? source.ToArray();

        foreach (var item in array)
            action(item);

        return array;
    }
}
