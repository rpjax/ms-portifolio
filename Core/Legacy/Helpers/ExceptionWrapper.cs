namespace ModularSystem.Core.Helpers;

/// <summary>
/// Represents a wrapper for an exception, allowing an exception to be encapsulated within another exception.
/// </summary>
/// <remarks>
/// This class can be used in scenarios where there's a need to capture and rethrow an exception 
/// with additional context or in a different format.
/// </remarks>
public class ExceptionWrapper : Exception
{
    /// <summary>
    /// Gets the original exception that has been wrapped.
    /// </summary>
    public Exception WrappedException { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionWrapper"/> class with the specified wrapped exception.
    /// </summary>
    /// <param name="wrappedException">The original exception to be wrapped.</param>
    public ExceptionWrapper(Exception wrappedException)
    {
        WrappedException = wrappedException;
    }
}
