namespace ModularSystem.Core.Logging;

/// <summary>
/// Represents an interface for logging errors.
/// </summary>
public interface IErrorLogger
{
    /// <summary>
    /// Logs the specified error.
    /// </summary>
    /// <param name="error">The error to be logged.</param>
    void Log(Error error);
}
