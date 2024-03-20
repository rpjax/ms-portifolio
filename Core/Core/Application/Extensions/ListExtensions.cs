namespace ModularSystem.Core;

public static partial class ListExtensions
{
    /// <summary>
    /// Not compatible with LINQ providers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static List<T> RemoveWhere<T>(this List<T> list, Func<T, bool> selector)
    {
        return list
            .Where(x => !selector.Invoke(x))
            .ToList();
    }

    /// <summary>
    /// Pops the first element out of the list and returns its value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Pop<T>(this List<T> list)
    {
        if (list.IsEmpty())
        {
            throw new InvalidOperationException("Cannot pop an empty list.");
        }

        T element = list[0];
        list.RemoveAt(0);
        return element;
    }

    public static T RemoveLast<T>(this List<T> list)
    {
        if(list.IsEmpty())
        {
            throw new InvalidOperationException("The list is empty.");
        }

        var element = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return element;
    }

}
