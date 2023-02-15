namespace Xerris.Extensions.Common;

public static class ObjectExtensions
{
    /// <summary>
    /// Yield the object as an <see cref="IEnumerable{T}" />.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns> An <see cref="IEnumerable{T}" /> containing the item.</returns>
    public static IEnumerable<T> Yield<T>(this T? item)
    {
        if (item is null)
            yield break;

        yield return item;
    }
}
