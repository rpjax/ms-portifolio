using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents an operation error with optional message, source, code, and flags. <br/>
/// This class is designed to encapsulate all relevant information about an error that occurs during an operation.
/// </summary>
public class Error
{
    /// <summary>
    /// The key used to store the serialized exception data in the error's debug data. <br/>
    /// </summary>
    public const string ExceptionDataKey = "exception";

    /// <summary>
    /// Gets or sets the message description of the error. <br/>
    /// This property is designed to provide a human-readable description of the error.
    /// </summary>
    public string? Message { get; set; }

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
        Message = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with specified message, code, and target.
    /// </summary>
    /// <param name="message">The message description of the error.</param>
    /// <param name="code">The code associated with the error.</param>
    /// <param name="source">The source associated with the error.</param>
    /// <param name="flags">The flags associated with the error.</param>
    public Error(string? message, string? source = null, string? code = null, params string[] flags)
    {
        Message = message;
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
        Message = exception.Message;
        Source = exception.StackTrace;
        Code = exception.HResult.ToString();

        this.AddFlags(ErrorFlags.Exception);
        this.AddJsonData(ExceptionDataKey, new SerializableException(exception));
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
    /// Creates a public error with the specified message, source, and code. This error is marked with the 'public' flag, <br/>
    /// indicating it is safe to display to end users. Additional flags can be added as needed.
    /// </summary>
    /// <param name="text">The message description of the error. Optional.</param>
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
    /// Creates a critical error with the specified message, source, and code. This error is marked with the 'critical' flag,
    /// <br/>
    /// indicating it represents a severe issue requiring immediate attention. Additional flags can be added as needed.
    /// </summary>
    /// <param name="text">The message description of the error. Optional.</param>
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
    /// Returns a string representation of the <see cref="Error"/>, combining target, code, and message.
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

        var text = string.IsNullOrEmpty(Message)
            ? "No error message provided. "
            : Message;

        var details = Details.Any()
            ? "Details:\n" + string.Join(Environment.NewLine, Details.Select(d => $"-{d.Key}:\n{d.Value}"))
            : "No details provided for this error.";

        return $"{source}{code}{text}\n{details}";
    }

}
