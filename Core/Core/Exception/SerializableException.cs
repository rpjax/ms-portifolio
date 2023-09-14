using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents a serializable version of a standard exception, ensuring compatibility and security during serialization.
/// </summary>
public class SerializableException
{
    /// <summary>
    /// Gets or sets the message describing the exception.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the stack trace information of the exception.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the source of the exception.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the link to the help file associated with this exception.
    /// </summary>
    public string? HelpLink { get; set; }

    /// <summary>
    /// Gets or sets the custom data associated with this exception.
    /// </summary>
    public IDictionary? Data { get; set; }

    /// <summary>
    /// Gets or sets the inner exception, if any, associated with this exception.
    /// </summary>
    public SerializableException? InnerException { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    [JsonConstructor]
    public SerializableException(string message)
    {
        Message = message;
        StackTrace = null;
        Source = null;
        HelpLink = null;
        Data = null;
        InnerException = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableException"/> class with a specified <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception to be serialized.</param>
    public SerializableException(Exception exception)
    {
        Message = exception.Message;
        StackTrace = exception.StackTrace;
        Source = exception.Source;
        HelpLink = exception.HelpLink;
        Data = exception.Data;

        if (exception.InnerException != null)
        {
            InnerException = new SerializableException(exception.InnerException);
        }
    }

    /// <summary>
    /// Converts the current exception to a JSON string.
    /// </summary>
    /// <param name="options">Optional serialization settings.</param>
    /// <returns>The JSON representation of the current exception.</returns>
    public string ToJson(JsonSerializerOptions? options = null)
    {
        return JsonSerializerSingleton.Serialize(this, options);
    }

    public Exception ToException()
    {
        return new Exception(Message, InnerException?.ToException())
        {
            Source = Source,
            HelpLink = HelpLink,
        };
    }
}

/// <summary>
/// Represents a serializable version of an application-specific exception.
/// </summary>
public class SerializableAppException : SerializableException
{
    /// <summary>
    /// Gets or sets the error code associated with this exception.
    /// </summary>
    public ExceptionCode Code { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableAppException"/> class with a specified error message and error code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="code">The error code associated with this exception. Defaults to <see cref="ExceptionCode.Internal"/>.</param>
    [JsonConstructor]
    public SerializableAppException(string message, ExceptionCode code = ExceptionCode.Internal) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableAppException"/> class with a specified <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception to be serialized.</param>
    public SerializableAppException(Exception exception) : base(exception)
    {
        Code = ExceptionCode.Internal;
    }

    public AppException ToAppException()
    {
        return new AppException(Message, Code, InnerException?.ToException())
        {
            Source = Source,
            HelpLink = HelpLink,
        };
    }
}