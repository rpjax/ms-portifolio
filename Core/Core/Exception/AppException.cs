namespace ModularSystem.Core;

/// <summary>
/// Represents application-specific exceptions that can include custom error codes and additional data.
/// </summary>
public class AppException : Exception
{
    /// <summary>
    /// Gets the custom message associated with the exception.
    /// </summary>
    public override string Message => _message;

    /// <summary>
    /// Gets or sets the custom error code associated with this exception.
    /// </summary>
    public ExceptionCode Code { get; set; }

    /// <summary>
    /// Gets or sets the custom details associated with this exception.
    /// </summary>
    public List<KeyValuePair<string, string>>? Details { get; set; } = new();

    /// <summary>
    /// Counter to create unique keys for additional data.
    /// </summary>
    private int DataCounter { get; set; }

    /// <summary>
    /// The custom message associated with the exception.
    /// </summary>
    private readonly string _message;

    /// <summary>
    /// Object to provide thread-safety when adding additional data.
    /// </summary>
    private readonly object _dataLock = new object();

    /// <summary>
    /// Initializes a new instance of the AppException class with a default message and error code.
    /// </summary>
    public AppException()
    {
        Code = ExceptionCode.Internal;
        _message = "An error has ocured *improve me gpt*";
    }

    /// <summary>
    /// Initializes a new instance of the AppException class with a custom message.
    /// </summary>
    /// <param name="message">The custom message to associate with the exception.</param>
    public AppException(string message) : base(message)
    {
        _message = message;
        Code = ExceptionCode.Internal;
    }

    /// <summary>
    /// Initializes a new instance of the AppException class with a custom message, error code, optional inner exception, and additional data.
    /// </summary>
    /// <param name="message">The custom message to associate with the exception.</param>
    /// <param name="code">The custom error code.</param>
    /// <param name="inner">The inner exception causing this exception, if applicable.</param>
    /// <param name="data">Additional data to include with the exception.</param>
    /// <param name="details">Additional details to include with the exception.</param>
    public AppException(
        string message,
        ExceptionCode code = ExceptionCode.Internal,
        Exception? inner = null,
        object? data = null,
        IEnumerable<KeyValuePair<string, string>>? details = null) : base(message, inner)
    {
        _message = message;
        Code = code;
        Details = details?.ToList();
        AddData(data);
    }

    /// <summary>
    /// Adds additional data to the exception.
    /// </summary>
    /// <param name="data">The additional data to add.</param>
    /// <returns>The AppException object, allowing for chaining.</returns>
    public AppException AddData(object? data)
    {
        if (data == null)
        {
            return this;
        }

        lock (_dataLock)
        {
            var key = data.GetType().FullName ?? DataCounter.ToString();

            while (Data.Contains(key))
            {
                DataCounter++;
                key = DataCounter.ToString();
            }

            Data.Add(key, data);
        }

        return this;
    }
}
