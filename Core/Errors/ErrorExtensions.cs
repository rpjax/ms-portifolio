namespace ModularSystem.Core.Extensions;

public static class ErrorExtensions
{
    /// <summary>
    /// Checks if the error contains any of the specified flags.
    /// </summary>
    /// <param name="error">The error to check for flags.</param>
    /// <param name="flags">The flags to check for in the error.</param>
    /// <returns><c>true</c> if the error contains any of the specified flags; otherwise, <c>false</c>.</returns>
    public static bool ContainsFlags(this Error error, params string[] flags)
    {
        foreach (var item in flags)
        {
            if (error.Flags.Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Retrieves the value associated with a specified key in the error's data collection.
    /// </summary>
    /// <param name="error">The error to retrieve the data from.</param>
    /// <param name="key">The key for the data to retrieve.</param>
    /// <returns>The value associated with the specified key, or null if the key is not found.</returns>
    public static string? GetData(this Error error, string key)
    {
        foreach (var item in error.Data)
        {
            if (item.Key == key)
            {
                return item.Value;
            }
        }

        return null;
    }

}
