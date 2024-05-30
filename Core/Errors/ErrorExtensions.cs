namespace ModularSystem.Core;

/// <summary>
/// Provides a set of predefined error flags that can be used to categorize or classify errors.
/// </summary>
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
        foreach (var item in error.DebugData)
        {
            if (item.Key == key)
            {
                return item.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Appends additional source information to the existing source of the error. <br/>
    /// If the 'source' argument is null, the operation is cancelled without throwing exceptions.
    /// </summary>
    /// <param name="error">The error to append the source information to.</param>
    /// <param name="source">The additional source information to append.</param>
    /// <param name="separator">The separator used between the existing source and the additional source. Defaults to "."</param>
    /// <returns>The <see cref="Error"/> instance with the appended source information.</returns>
    public static Error AppendSource(this Error error, string? source, string? separator = ".")
    {
        if (string.IsNullOrEmpty(source))
        {
            return error;
        }
        if (string.IsNullOrEmpty(error.Source))
        {
            error.Source = source;
            return error;
        }

        error.Source = $"{source}{separator}{error.Source}";

        return error;
    }

    /// <summary>
    /// Adds additional flags to the error.
    /// </summary>
    /// <param name="error">The error to add flags to.</param>
    /// <param name="flags">The flags to add to the error.</param>
    /// <returns>The <see cref="Error"/> instance with the added flags.</returns>
    public static Error AddFlags(this Error error, params string[] flags)
    {
        foreach (var item in flags)
        {
            if (error.Flags.Contains(item))
            {
                continue;
            }

            error.Flags.Add(item);
        }

        return error;
    }

    /// <summary>
    /// Adds detailed key-value pair information to the error. <br/>
    /// This method is designed for adding human-readable messages that provide more context about the error. <br/>
    /// </summary>
    /// <param name="error">The error to add details to.</param>
    /// <param name="details">The key-value pairs representing the details to add.</param>
    /// <returns>The <see cref="Error"/> instance with the added details.</returns>
    public static Error AddDetails(this Error error, params KeyValuePair<string, string>[] details)
    {
        error.Details.AddRange(details);
        return error;
    }

    /// <summary>
    /// Adds a single key-value pair detail to the error. <br/>
    /// This is used for adding a specific human-readable message that provides additional context about the error.
    /// </summary>
    /// <param name="error">The error to add the detail to.</param>
    /// <param name="key">The key identifying the detail.</param>
    /// <param name="value">The value or message associated with the key.</param>
    /// <returns>The <see cref="Error"/> instance with the added detail.</returns>
    public static Error AddDetails(this Error error, string key, string? value)
    {
        if (value == null)
        {
            return error;
        }

        error.Details.Add(new KeyValuePair<string, string>(key, value));
        return error;
    }

    /// <summary>
    /// Sets or updates the value associated with a specified key in the error's data collection.
    /// </summary>
    /// <param name="error">The error to set or update the data in.</param>
    /// <param name="key">The key for the data to set or update.</param>
    /// <param name="value">The value to set. If null, no action is taken.</param>
    /// <returns>The current <see cref="Error"/> instance with the updated data.</returns>
    public static Error SetData(this Error error, string key, string? value)
    {
        if (value == null)
        {
            return error;
        }

        error.DebugData = error.DebugData.RemoveWhere(x => x.Key == key);
        error.DebugData.Add(new(key, value));
        return error;
    }

    /// <summary>
    /// Adds additional data to the error in the form of key-value pairs. <br/>
    /// This method is intended for storing structured data, such as JSON strings or encoded binary data, which provide additional context or information about the error.
    /// </summary>
    /// <param name="error">The error to add data to.</param>
    /// <param name="values">The key-value pairs representing the data to add.</param>
    /// <returns>The <see cref="Error"/> instance with the added data.</returns>
    public static Error AddData(this Error error, params KeyValuePair<string, string>[] values)
    {
        error.DebugData.AddRange(values);
        return error;
    }

    /// <summary>
    /// Adds a single key-value pair data to the error. <br/>
    /// This is used for adding specific structured data, such as a JSON string or encoded binary data, <br/>
    /// providing additional context or information about the error.
    /// </summary>
    /// <param name="error">The error to add the data to.</param>
    /// <param name="key">The key identifying the data.</param>
    /// <param name="value">The structured data associated with the key.</param>
    /// <returns>The <see cref="Error"/> instance with the added data.</returns>
    public static Error AddData(this Error error, string key, string value)
    {
        error.DebugData.Add(new KeyValuePair<string, string>(key, value));
        return error;
    }

    /// <summary>
    /// Adds a key-value pair to the error's data collection, where the value is serialized into JSON format. <br/>
    /// This method is useful for including complex or structured data with the error.
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized and added.</typeparam>
    /// <param name="error">The error to add the JSON data to.</param>
    /// <param name="key">The key associated with the data to be added.</param>
    /// <param name="value">The value to be serialized into JSON and added. If the value is null, the method does nothing.</param>
    /// <returns>The current <see cref="Error"/> instance with the added JSON data.</returns>
    public static Error SetJsonData<T>(this Error error, string key, T? value)
    {
        if (value == null)
        {
            return error;
        }

        error.SetData(key, JsonSerializerSingleton.Serialize(value));

        return error;
    }

    /// <summary>
    /// Adds a key-value pair to the error's data collection, where the value is serialized into JSON format. <br/>
    /// This method is useful for including complex or structured data with the error.
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized and added.</typeparam>
    /// <param name="error">The error to add the JSON data to.</param>
    /// <param name="key">The key associated with the data to be added.</param>
    /// <param name="value">The value to be serialized into JSON and added. If the value is null, the method does nothing.</param>
    /// <returns>The current <see cref="Error"/> instance with the added JSON data.</returns>
    public static Error AddJsonData<T>(this Error error, string key, T? value)
    {
        if (value == null)
        {
            return error;
        }

        error.DebugData.Add(new(key, JsonSerializerSingleton.Serialize(value)));

        return error;
    }

    /// <summary>
    /// Adds exception details to the error's data collection, serializing the exception into a JSON-formatted string.
    /// </summary>
    /// <param name="error">The error to add the exception details to.</param>  
    /// <param name="exception">The exception to add to the error. If null, the method does nothing.</param>
    /// <returns>The current <see cref="Error"/> instance with the added exception details.</returns>
    /// <remarks>
    /// The serialized exception data is intended primarily for debugging and diagnostic purposes. <br/>
    /// Care should taken to ensure that sensitive information is not inadvertently exposed, <br/>
    /// especially when errors are logged or displayed in user-facing applications.
    /// </remarks>
    public static Error AddException(this Error error, Exception? exception)
    {
        if (exception == null)
        {
            return error;
        }

        error.AddJsonData(Error.ExceptionDataKey, new SerializableException(exception));

        return error;
    }

    /// <summary>
    /// Sets the error code associated with the error.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static Error SetCode(this Error error, string code)
    {
        error.Code = code;
        return error;
    }

    /// <summary>
    /// Sets the error message associated with the error.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Error SetText(this Error error, string text)
    {
        error.Text = text;
        return error;
    }

}
