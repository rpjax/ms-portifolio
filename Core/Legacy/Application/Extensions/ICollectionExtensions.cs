namespace ModularSystem.Core;

// fluent API add-on
public static partial class ICollectionExtensions
{
    public static ICollection<T> FluentAdd<T>(this ICollection<T> collection, params T[] items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }

        return collection;
    }
}
