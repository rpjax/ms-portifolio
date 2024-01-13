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

    /// <summary>
    /// Converts the current <see cref="SerializableException"/> instance back to a standard <see cref="Exception"/>.
    /// This method is useful for reconstructing a standard exception from its serializable form.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="Exception"/> that contains the same message, inner exception, source, and help link
    /// as the current <see cref="SerializableException"/>.
    /// </returns>
    /// <remarks>
    /// This method is particularly useful in scenarios where an exception needs to be serialized for cross-domain,
    /// cross-process, or cross-machine communication, and then reconstructed back to its original form.
    /// The method ensures that key exception details are preserved during this process.
    /// </remarks>
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
    /// Gets or sets the details associated with this exception.
    /// </summary>
    public List<KeyValuePair<string, string>>? Details { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableAppException"/> class with a specified error message and error code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="code">The error code associated with this exception. Defaults to <see cref="ExceptionCode.Internal"/>.</param>
    /// <param name="details">The details associated with this exception.</param>
    [JsonConstructor]
    public SerializableAppException(
        string message, 
        ExceptionCode code = ExceptionCode.Internal, 
        List<KeyValuePair<string, string>>? details = null) : base(message)
    {
        Code = code;
        Details = details;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableAppException"/> class with a specified <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception to be serialized.</param>
    public SerializableAppException(Exception exception) : base(exception)
    {
        Code = ExceptionCode.Internal;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableAppException"/> class with a specified <see cref="AppException"/>.
    /// </summary>
    /// <param name="exception">The exception to be serialized.</param>
    public SerializableAppException(AppException exception) : base(exception)
    {
        Code = ExceptionCode.Internal;
        Details = exception.Details;
    }

    /// <summary>
    /// Converts the <see cref="SerializableAppException"/> to a <see cref="AppException"/>. <br/>
    /// This method facilitates the transition from a serializable exception back to 
    /// a non-serializable, application-specific exception which can be used within the application.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="AppException"/> that contains the same error message,
    /// error code, and additional details as the current <see cref="SerializableAppException"/>. 
    /// </returns>
    /// <remarks>
    /// This method is particularly useful in scenarios where exceptions are serialized 
    /// and deserialized across different layers or services in an application. <br/>
    /// It allows for the reconstruction of an application-specific exception 
    /// with rich context from its serializable form.
    /// </remarks>
    public AppException ToAppException()
    {
        return new AppException(Message, Code, InnerException?.ToException(), null, Details)
        {
            Source = Source,
            HelpLink = HelpLink,
        };
    }
}
