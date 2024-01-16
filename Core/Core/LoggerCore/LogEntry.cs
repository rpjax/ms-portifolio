namespace ModularSystem.Core.Logging;

//*
// The new log API starts here.
//*

/// <summary>
/// Represents a generic log entry in the modular system. 
/// This interface is designed to be extendable for various types of log entries.
/// </summary>
public interface ILogEntry : IQueryableModel
{
    /// <summary>
    /// Gets or sets the type of the log entry. 
    /// </summary>
    string? Type { get; set; }

    /// <summary>
    /// Gets or sets the message of the log entry.
    /// </summary>
    string? Message { get; set; }

    /// <summary>
    /// Gets or sets the flags associated with the log entry.
    /// These flags can be used for categorization or filtering.
    /// </summary>
    string? Flags { get; set; }
}

/// <summary>
/// Represents a log entry that specifically logs exceptions.
/// Extends ILogEntry to include stack trace information.
/// </summary>
public interface IExceptionLogEntry : ILogEntry
{
    /// <summary>
    /// Gets or sets the stack trace of the exception.
    /// </summary>
    string? StackTrace { get; set; }

    /// <summary>
    /// Retrieves the <see cref="Exception"/> object stored in this log entry.
    /// </summary>
    /// <returns>The Exception object associated with this log entry.</returns>
    /// <exception cref="ArgumentException">Thrown if the exception cannot be retrieved.</exception>
    Exception GetException();
}

/// <summary>
/// Static class defining constants for common log types.
/// These constants provide a standardized way to categorize log entries.
/// </summary>
public static class LogTypes
{
    public const string Log = "log";
    public const string Error = "error";
    public const string ValidationError = "validation_error"; 
    public const string DatabaseError = "database_error";     
    public const string NetworkError = "network_error";       
    public const string AuthenticationError = "authentication_error"; 
    public const string AuthorizationError = "authorization_error";   
    public const string NotFoundError = "not_found_error";    
    public const string BusinessLogicError = "business_logic_error";   
    public const string TimeoutError = "timeout_error";       
    public const string ExternalServiceError = "external_service_error"; 
    public const string ConfigurationError = "configuration_error";
    public const string CriticalError = "critical_error";
    public const string Info = "info";
    public const string Warn = "warn";
}