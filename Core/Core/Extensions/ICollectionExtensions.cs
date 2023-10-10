namespace ModularSystem.Core;

// fluent API add-on
public static partial class ICollectionExtensions
{
    public static ICollection<T> FluentAdd<T>(this ICollection<T> collection, T item)
    {
        collection.Add(item);
        return collection;
    }
}
