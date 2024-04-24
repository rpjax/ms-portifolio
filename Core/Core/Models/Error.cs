using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents an operation error with optional text, source, code, and flags. <br/>
/// This class is designed to encapsulate all relevant information about an error that occurs during an operation.
/// </summary>
public class Error
{
    public const string ExceptionDataKey = "exception";

    /// <summary>
    /// Gets or sets the descriptive text of the error. This field can provide a human-readable explanation of the error.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the identifier used to bind this error to its source. <br/>
    /// This can be used to specify where or what part of the system the error originated from.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets a code meant for tracking or categorizing the error. <br/>
    /// This can be useful for error classification or internationalization purposes.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets a list of flags associated with this operation error. <br/>
    /// Flags can be used to convey additional context about the error, such as its severity or visibility.
    /// </summary>
    public List<string> Flags { get; set; } = new(0);

    /// <summary>
    /// Gets or sets a list of key-value-pair details associated with this error. <br/>
    /// This property is designed for human-readable messages that provide additional context about the error.
    /// </summary>
    public List<KeyValuePair<string, string>> Details { get; set; } = new(0);

    /// <summary>
    /// Gets or sets a list of key-value-pair data associated with this error,  <br/>
    /// designed for additional data about the error, including JSON or encoded binary data.
    /// </summary>
    public List<KeyValuePair<string, string>> DebugData { get; set; } = new(0);

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    [JsonConstructor]
    public Error()
    {
        Text = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with specified text, code, and target.
    /// </summary>
    /// <param name="text">The text description of the error.</param>
    /// <param name="code">The code associated with the error.</param>
    /// <param name="source">The source associated with the error.</param>
    /// <param name="flags">The flags associated with the error.</param>
    public Error(string? text, string? source = null, string? code = null, params string[] flags)
    {
        Text = text;
        Source = source;
        Code = code;
        Flags = flags.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class using an exception object. <br/>
    /// This constructor extracts information from the exception to populate the error details.
    /// </summary>
    /// <param name="exception">The exception from which to create the error.</param>
    /// <remarks>
    /// The constructor extracts the message, stack trace, and HRESULT from the exception. <br/>
    /// It also adds a flag indicating that this error is based on an exception and includes a serialized form of the exception.
    /// </remarks>
    public Error(Exception exception)
    {
        Text = exception.Message;
        Source = exception.StackTrace;
        Code = exception.HResult.ToString();
        AddFlags(ErrorFlags.Exception);
        AddJsonData(ExceptionDataKey, new SerializableException(exception));
    }

    /// <summary>
    /// Provides implicit conversion from string to <see cref="Error"/>.
    /// </summary>
    /// <param name="text">The error message to convert into a <see cref="Error"/>.</param>
    public static implicit operator Error(string text)
    {
        return new(text);
    }

    /// <summary>
    /// Creates a public error with the specified text, source, and code. This error is marked with the 'public' flag, <br/>
    /// indicating it is safe to display to end users. Additional flags can be added as needed.
    /// </summary>
    /// <param name="text">The text description of the error. Optional.</param>
    /// <param name="source">The source of the error. Optional.</param>
    /// <param name="code">The code associated with the error. Optional.</param>
    /// <param name="flags">Additional flags to be associated with the error. Optional.</param>
    /// <returns>A new <see cref="Error"/> instance marked as public.</returns>
    public static Error Public(string? text = null, string? source = null, string? code = null, params string[] flags)
    {
        return new Error(text, source, code, ErrorFlags.Public)
            .AddFlags(flags);
    }

    /// <summary>
    /// Creates a critical error with the specified text, source, and code. This error is marked with the 'critical' flag,
    /// <br/>
    /// indicating it represents a severe issue requiring immediate attention. Additional flags can be added as needed.
    /// </summary>
    /// <param name="text">The text description of the error. Optional.</param>
    /// <param name="source">The source of the error. Optional.</param>
    /// <param name="code">The code associated with the error. Optional.</param>
    /// <param name="flags">Additional flags to be associated with the error. Optional.</param>
    /// <returns>A new <see cref="Error"/> instance marked as critical.</returns>
    public static Error Critical(string? text = null, string? source = null, string? code = null, params string[] flags)
    {
        return new Error(text, source, code, ErrorFlags.Critical)
            .AddFlags(flags);
    }

    /// <summary>
    /// Returns a string representation of the <see cref="Error"/>, combining target, code, and text.
    /// </summary>
    /// <returns>A string representation of the ValidationError.</returns>
    public override string ToString()
    {
        var source = string.IsNullOrEmpty(Source)
            ? null 
            : $"{Source}: ";

        var code = string.IsNullOrEmpty(Code)
            ? null 
            : $"({Code}) ";

        var text = string.IsNullOrEmpty(Text)
            ? "No error message provided. "
            : Text;

        var details = Details.Any()
            ? "Details:\n" + string.Join(Environment.NewLine, Details.Select(d => $"-{d.Key}:\n{d.Value}"))
            : "No details provided for this error.";

        return $"{source}{code}{text}\n{details}";
    }

    /// <summary>
    /// Checks if the error contains any of the specified flags.
    /// </summary>
    /// <param name="flags">The flags to check for in the error.</param>
    /// <returns><c>true</c> if the error contains any of the specified flags; otherwise, <c>false</c>.</returns>
    public bool ContainsFlags(params string[] flags)
    {
        foreach (var item in flags)
        {
            if (Flags.Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Retrieves the value associated with a specified key in the error's data collection.
    /// </summary>
    /// <param name="key">The key for the data to retrieve.</param>
    /// <returns>The value associated with the specified key, or null if the key is not found.</returns>
    public string? GetData(string key)
    {
        foreach (var item in DebugData)
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
    /// <param name="source">The additional source information to append.</param>
    /// <param name="separator">The separator used between the existing source and the additional source. Defaults to "."</param>
    /// <returns>The <see cref="Error"/> instance with the appended source information.</returns>
    public Error AppendSource(string? source, string? separator = ".")
    {
        if (string.IsNullOrEmpty(source))
        {
            return this;
        }
        if (string.IsNullOrEmpty(Source))
        {
            Source = source;
            return this;
        }

        Source = $"{source}{separator}{Source}";

        return this;
    }

    /// <summary>
    /// Adds additional flags to the error.
    /// </summary>
    /// <param name="flags">The flags to add to the error.</param>
    /// <returns>The <see cref="Error"/> instance with the added flags.</returns>
    public Error AddFlags(params string[] flags)
    {
        foreach (var item in flags)
        {
            if (Flags.Contains(item))
            {
                continue;
            }

            Flags.Add(item);
        }

        return this;
    }

    /// <summary>
    /// Adds detailed key-value pair information to the error. <br/>
    /// This method is designed for adding human-readable messages that provide more context about the error. <br/>
    /// </summary>
    /// <param name="details">The key-value pairs representing the details to add.</param>
    /// <returns>The <see cref="Error"/> instance with the added details.</returns>
    public Error AddDetails(params KeyValuePair<string, string>[] details)
    {
        Details.AddRange(details);
        return this;
    }

    /// <summary>
    /// Adds a single key-value pair detail to the error. <br/>
    /// This is used for adding a specific human-readable message that provides additional context about the error.
    /// </summary>
    /// <param name="key">The key identifying the detail.</param>
    /// <param name="value">The value or message associated with the key.</param>
    /// <returns>The <see cref="Error"/> instance with the added detail.</returns>
    public Error AddDetails(string key, string? value)
    {
        if (value == null)
        {
            return this;
        }

        Details.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }

    /// <summary>
    /// Sets or updates the value associated with a specified key in the error's data collection.
    /// </summary>
    /// <param name="key">The key for the data to set or update.</param>
    /// <param name="value">The value to set. If null, no action is taken.</param>
    /// <returns>The current <see cref="Error"/> instance with the updated data.</returns>
    public Error SetData(string key, string? value)
    {
        if (value == null)
        {
            return this;
        }

        DebugData = DebugData.RemoveWhere(x => x.Key == key);
        DebugData.Add(new(key, value));
        return this;
    }

    /// <summary>
    /// Adds additional data to the error in the form of key-value pairs. <br/>
    /// This method is intended for storing structured data, such as JSON strings or encoded binary data, which provide additional context or information about the error.
    /// </summary>
    /// <param name="values">The key-value pairs representing the data to add.</param>
    /// <returns>The <see cref="Error"/> instance with the added data.</returns>
    public Error AddData(params KeyValuePair<string, string>[] values)
    {
        DebugData.AddRange(values);
        return this;
    }

    /// <summary>
    /// Adds a single key-value pair data to the error. <br/>
    /// This is used for adding specific structured data, such as a JSON string or encoded binary data, <br/>
    /// providing additional context or information about the error.
    /// </summary>
    /// <param name="key">The key identifying the data.</param>
    /// <param name="value">The structured data associated with the key.</param>
    /// <returns>The <see cref="Error"/> instance with the added data.</returns>
    public Error AddData(string key, string value)
    {
        DebugData.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }

    /// <summary>
    /// Adds a key-value pair to the error's data collection, where the value is serialized into JSON format. <br/>
    /// This method is useful for including complex or structured data with the error.
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized and added.</typeparam>
    /// <param name="key">The key associated with the data to be added.</param>
    /// <param name="value">The value to be serialized into JSON and added. If the value is null, the method does nothing.</param>
    /// <returns>The current <see cref="Error"/> instance with the added JSON data.</returns>
    public Error SetJsonData<T>(string key, T? value)
    {
        if (value == null)
        {
            return this;
        }

        SetData(key, JsonSerializerSingleton.Serialize(value));

        return this;
    }

    /// <summary>
    /// Adds a key-value pair to the error's data collection, where the value is serialized into JSON format. <br/>
    /// This method is useful for including complex or structured data with the error.
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized and added.</typeparam>
    /// <param name="key">The key associated with the data to be added.</param>
    /// <param name="value">The value to be serialized into JSON and added. If the value is null, the method does nothing.</param>
    /// <returns>The current <see cref="Error"/> instance with the added JSON data.</returns>
    public Error AddJsonData<T>(string key, T? value)
    {
        if (value == null)
        {
            return this;
        }

        DebugData.Add(new(key, JsonSerializerSingleton.Serialize(value)));

        return this;
    }

    /// <summary>
    /// Adds exception details to the error's data collection, serializing the exception into a JSON-formatted string.
    /// </summary>
    /// <param name="exception">The exception to add to the error. If null, the method does nothing.</param>
    /// <returns>The current <see cref="Error"/> instance with the added exception details.</returns>
    /// <remarks>
    /// The serialized exception data is intended primarily for debugging and diagnostic purposes. <br/>
    /// Care should taken to ensure that sensitive information is not inadvertently exposed, <br/>
    /// especially when errors are logged or displayed in user-facing applications.
    /// </remarks>
    public Error AddException(Exception? exception)
    {
        if (exception == null)
        {
            return this;
        }

        AddJsonData(ExceptionDataKey, new SerializableException(exception));

        return this;
    }

}

/// <summary>
/// Provides a set of constants representing different categories of errors. <br/>
/// These flags are used to classify errors based on their visibility, severity, and purpose.
/// </summary>
public static class ErrorFlags
{
    /// <summary>
    /// Signals that the error is safe to display to the user. <br/>
    /// This flag can be used to indicate that the error message does not contain sensitive information
    /// and is intended for general visibility.
    /// </summary>
    public const string Public = "public";

    /// <summary>
    /// Indicates that the error is relevant for debugging purposes. <br/>
    /// This flag can be used to classify errors that are useful for developers during the debugging process.
    /// It might contain technical details relevant for troubleshooting.
    /// </summary>
    public const string Debug = "debug";

    /// <summary>
    /// Marks the error as critical. <br/>
    /// This flag is used to denote errors of a severe nature, often indicating major failures
    /// or issues that require immediate attention.
    /// </summary>
    public const string Critical = "critical";

    /// <summary>
    /// Denotes that the error is derived from an exception. <br/>
    /// This flag can be used to indicate that the error encapsulates or is directly related to an exception, <br/>
    /// and typically contains detailed information about the exception. It helps in distinguishing <br/>
    /// standard operational errors from those specifically arising from exceptions.
    /// </summary>
    public const string Exception = "exception";

    /// <summary>
    /// Identifies the error as a bug. <br/>
    /// This flag is used to categorize errors as bugs, implying defects or problems in the code or logic. <br/>
    /// It indicates that the error is a result of a flaw in the system that may require a code review or a fix.
    /// </summary>
    public const string Bug = "bug";
}

