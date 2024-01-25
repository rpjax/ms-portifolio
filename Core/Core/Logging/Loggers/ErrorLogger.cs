using ModularSystem.Core.Threading;
using ModularSystem.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModularSystem.Core.Logging;

class ExceptionLoggerInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    {
        ErrorLogger.EnableDiskLogging = options.EnableDiskExceptionLogger;
        ErrorLogger.EnableConsoleLogging = options.EnableConsoleExceptionLogger;

        return Task.CompletedTask;
    }
}

/// <summary>
/// Represents an error entry that can be logged and stored in an Entity Framework context. <br/>
/// Inherits from <see cref="Error"/> and implements <see cref="IEFModel"/> for database storage.
/// </summary>
public class ErrorEntry : EFErrorEntry
{
    /// <summary>
    /// Gets or sets the serialized representation of the exception.
    /// </summary>
    public string? SerializedException 
    { 
        get => GetData(ExceptionDataKey); 
        set => SetData(ExceptionDataKey, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorEntry"/> class with default properties.
    /// </summary>
    [JsonConstructor]
    public ErrorEntry()
    {
        
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorEntry"/> class using an <see cref="Error"/> object.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> object to initialize the <see cref="ErrorEntry"/> from.</param>
    /// <remarks>
    /// This constructor creates an <see cref="ErrorEntry"/> by copying the properties of the provided <see cref="Error"/> object.
    /// </remarks>
    public ErrorEntry(Error error)
    {
        Text = error.Text;
        Source = error.Source;
        Code = error.Code;
        Flags = error.Flags;
        Details = error.Details;
        Data = error.Data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorEntry"/> class using an <see cref="Exception"/> object.
    /// </summary>
    /// <param name="exception">The exception to initialize the <see cref="ErrorEntry"/> from.</param>
    /// <param name="code">Optional error code associated with the exception.</param>
    /// <param name="flags">Optional flags to classify the error.</param>
    /// <remarks>
    /// This constructor creates an <see cref="ErrorEntry"/> by extracting information from the provided exception.
    /// <br/>
    /// The exception is serialized to a JSON string for detailed error reporting.
    /// </remarks>
    public ErrorEntry(Exception exception, string? code = null, params string[] flags)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new InternalContractResolver()
        };

        Text = exception.Message;
        Source = exception.Source;
        Code = code;
        SerializedException = JsonConvert.SerializeObject(exception, settings);
        AddFlags(flags);
    }

    /// <summary>
    /// Creates an <see cref="ErrorEntry"/> from an <see cref="Exception"/>, specifying the time of occurrence and additional parameters.
    /// </summary>
    /// <param name="exception">The exception to base the <see cref="ErrorEntry"/> on.</param>
    /// <param name="occurredAt">The date and time when the error occurred.</param>
    /// <param name="code">Optional error code associated with the exception.</param>
    /// <param name="flags">Optional flags to classify the error.</param>
    /// <returns>A new instance of <see cref="ErrorEntry"/> initialized from the exception.</returns>
    /// <remarks>
    /// This static method provides a convenient way to create an <see cref="ErrorEntry"/> with the time of occurrence and additional parameters like code and flags.
    /// </remarks>
    public static ErrorEntry From(
        Exception exception,
        DateTime occurredAt,
        string? code = null,
        params string[] flags)
    {
        return new ErrorEntry(exception, code, flags)
        {
            CreatedAt = occurredAt,
        };
    }

    /// <summary>
    /// Serializes a given exception to its string representation.
    /// </summary>
    /// <param name="exception">The exception to be serialized.</param>
    /// <returns>A serialized string representation of the exception.</returns>
    public static string Serialize(Exception exception)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new InternalContractResolver()
        };

        return JsonConvert.SerializeObject(exception, settings);
    }

    /// <summary>
    /// Deserializes a given serialized exception back into its <see cref="Exception"/> object.
    /// </summary>
    /// <param name="serializedException">The serialized exception string.</param>
    /// <returns>The deserialized <see cref="Exception"/> object, or null if the input string is null.</returns>
    public static Exception? Deserialize(string? serializedException)
    {
        if (serializedException == null)
        {
            return null;
        }

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new InternalContractResolver()
        };

        return JsonConvert.DeserializeObject<Exception>(serializedException, settings);
    }

    /// <summary>
    /// Deserializes the <see cref="SerializedException"/> property back into its <see cref="Exception"/> object.
    /// </summary>
    /// <returns>The deserialized <see cref="Exception"/> object, or null if <see cref="SerializedException"/> is null.</returns>
    public Exception? DeserializeException()
    {
        return Deserialize(SerializedException);
    }

    /// <summary>
    /// Gets the <see cref="Exception"/> stored in this entry.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Exception GetException()
    {
        if(SerializedException == null)
        {
            throw new ArgumentException("Cannot get exception from entry with null serialized exception data.", nameof(SerializedException));
        }

        return Deserialize(SerializedException)!;
    }

    /// <summary>
    /// Represents a contract resolver for JSON serialization and deserialization operations specific to exceptions.
    /// </summary>
    /// <remarks>
    /// The primary intent of this resolver is to filter out certain properties from the serialization process,
    /// such as the <see cref="Exception.TargetSite"/>.
    /// </remarks>
    internal class InternalContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var ignoreProps = new string[]
            {
                nameof(Exception.TargetSite)
            };

            var props = base.CreateProperties(type, memberSerialization);

            return props.Where(p => !ignoreProps.Contains(p.PropertyName)).ToList();
        }
    }

}

/// <summary>
/// Provides a centralized mechanism for logging errors.<br/>
/// This logger has the capability to log errors both to the console and to a disk-based log file.
/// </summary>
/// <remarks>
/// By default, this logger saves errors to a file named 'e_debug.db' located in the '/LOGS' directory.<br/>
/// Disk logging can be toggled on or off, and it uses a SQLite-based logger for persistence.
/// </remarks>
public class ErrorLogger : EFEntityService<ErrorEntry>
{
    /// <summary>
    /// The default filename used to save the log entries to disk.
    /// </summary>
    public const string DefaultFileName = "e_debug.db";

    /// <summary>
    /// Gets or sets a value indicating whether to enable logging of errors to disk.
    /// </summary>
    public static bool EnableDiskLogging { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable logging of errors to the console.
    /// </summary>
    public static bool EnableConsoleLogging { get; set; } = true;

    /// <summary>
    /// Gets the data access object used for interacting with the database.
    /// </summary>
    public override IDataAccessObject<ErrorEntry> DataAccessObject { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorLogger"/> class with a specified data access object.
    /// </summary>
    /// <param name="dataAccessObject">The data access object for logging error entries.</param>
    public ErrorLogger(IDataAccessObject<ErrorEntry> dataAccessObject)
    {
        DataAccessObject = dataAccessObject;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorLogger"/> class with an optional log file.
    /// </summary>
    /// <param name="logFile">Optional file info for the log file.</param>
    public ErrorLogger(FileInfo? logFile = null)
    {
        DataAccessObject = CreateDataAccessObject(logFile);
    }

    /// <summary>
    /// Logs an <see cref="Error"/> object, outputting to console and/or disk based on configuration.
    /// </summary>
    /// <param name="error">The error to log.</param>
    /// <param name="logFile">Optional file info for the log file.</param>
    public static void Log(Error error, FileInfo? logFile = null)
    {
        if (EnableConsoleLogging)
        {
            ConsoleLogger.Error(error.ToString());
        }

        if (EnableDiskLogging)
        {
            JobQueue.Enqueue(LogTask(error, logFile));
        }
    }

    /// <summary>
    /// Logs an error derived from an <see cref="Exception"/>, along with additional flags.
    /// </summary>
    /// <param name="e">The exception to derive the error from.</param>
    /// <param name="flags">Additional flags to classify the error.</param>
    public static void Log(Exception e, params string[] flags)
    {
        Log(new Error(e).AddFlags(flags));
    }

    /// <summary>
    /// Logs errors from an <see cref="OperationResult"/>, along with additional flags.
    /// </summary>
    /// <param name="operationResult">The operation result containing the errors.</param>
    /// <param name="flags">Additional flags to classify the error.</param>
    public static void Log(OperationResult operationResult, params string[] flags)
    {
        foreach (var error in operationResult.Errors)
        {
            Log(error.AddFlags(flags));
        }
    }

    private static async Task LogTask(Error error, FileInfo? logFile = null)
    {
        using var service = new ErrorLogger(logFile);
        var entry = new ErrorEntry(error);
    }

    private IDataAccessObject<ErrorEntry> CreateDataAccessObject(FileInfo? fileInfo)
    {
        fileInfo ??= LogsDirectory.GetFileInfo(DefaultFileName);
        return new EFCoreDataAccessObject<ErrorEntry>(new EFCoreSqliteContext<ErrorEntry>(fileInfo));
    }

}
