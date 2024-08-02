namespace Aidan.Core.Extensions;

public static partial class DictionaryExtensions
{
    /// <summary>
    /// Returns the <typeparamref name="TValue"/> value if the <typeparamref name="TKey"/> key is found, <see cref="null"/> if not found.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="map"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue? Get<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key) where TKey : notnull where TValue : class
    {
        if (map.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    ///  Returns the <typeparamref name="TValue"/> value if the <typeparamref name="TKey"/> key is found, if not, the provided <typeparamref name="TValue"/> argument.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="map"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, TValue defaultValue) where TKey : notnull where TValue : class
    {
        if (map.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        return defaultValue;
    }
}