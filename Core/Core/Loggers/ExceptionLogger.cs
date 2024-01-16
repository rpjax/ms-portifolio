using ModularSystem.Core.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModularSystem.Core.Logging;

class ExceptionLoggerInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    {
        ExceptionLogger.EnableDiskLogging = options.EnableDiskExceptionLogger;
        ExceptionLogger.EnableConsoleLogging = options.EnableConsoleExceptionLogger;
        return Task.CompletedTask;
    }
}

/// <summary>
/// Represents a log entry specifically designed for exceptions using Entity Framework for storage.
/// </summary>
/// <remarks>
/// The <c>ExceptionEntry</c> class extends the <see cref="EFLogEntry"/> to provide specialized functionality <br/>
/// for logging exceptions, including serialization and deserialization of exception objects.
/// </remarks>
public class ExceptionEntry : EFLogEntry, IExceptionLogEntry
{
    /// <summary>
    /// Gets or sets the stack trace at the point where the log was generated, if applicable.
    /// </summary>
    public string? StackTrace { get; set; } = null;

    /// <summary>
    /// Gets or sets the serialized representation of the exception.
    /// </summary>
    public string? SerializedException { get; set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionEntry"/> class with default properties.
    /// </summary>
    [JsonConstructor]
    public ExceptionEntry()
    {
        
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionEntry"/> class with default properties.
    /// </summary>
    public ExceptionEntry(string? type = null)
    {
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionEntry"/> class with a given exception.
    /// </summary>
    /// <param name="exception">The exception to be logged.</param>
    /// <param name="type">The error type associated to this exception to be logged.</param>
    public ExceptionEntry(Exception exception, string? type = LogTypes.Error) 
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new InternalContractResolver()
        };

        Type = type;
        Message = exception.Message;
        StackTrace = exception.StackTrace;
        SerializedException = JsonConvert.SerializeObject(exception, settings);
    }

    /// <summary>
    /// Creates an instance of the <see cref="ExceptionEntry"/> using a given exception and occurrence time.
    /// </summary>
    /// <param name="exception">The exception to be logged.</param>
    /// <param name="occurredAt">The time the exception occurred.</param>
    /// <returns>An instance of <see cref="ExceptionEntry"/>.</returns>
    public static ExceptionEntry From(Exception exception, DateTime occurredAt, string? type = LogTypes.Error)
    {
        return new ExceptionEntry(exception, type)
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
/// Provides a centralized mechanism for logging exceptions.<br/>
/// This logger has the capability to log exceptions both to the console and to a disk-based log file.
/// </summary>
/// <remarks>
/// By default, this logger saves exceptions to a file named 'e_debug.db' located in the '/LOGS' directory.<br/>
/// Disk logging can be toggled on or off, and it uses a SQLite-based logger for persistence.
/// </remarks>
public static class ExceptionLogger
{
    /// <summary>
    /// The default filename used to save the log entries to disk.
    /// </summary>
    public const string DefaultFileName = "e_debug.db";

    /// <summary>
    /// Gets or sets a value indicating whether to enable logging of exceptions to disk.
    /// </summary>
    public static bool EnableDiskLogging { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable logging of exceptions to the console.
    /// </summary>
    public static bool EnableConsoleLogging { get; set; } = true;

    /// <summary>
    /// Logs the provided exception. Depending on the logger's settings, this could be to the console, to disk, or both.
    /// </summary>
    /// <param name="e">The exception to be logged.</param>
    /// <param name="type">The error type associated with this exception.</param>
    public static void Log(Exception e, string? type = LogTypes.Error)
    {
        if (EnableConsoleLogging)
        {
            ConsoleLogger.Error(e.ToString());
        }

        if (EnableDiskLogging)
        {
            var logger = GetLogger();
            var entry = ExceptionEntry.From(e, TimeProvider.UtcNow(), type);
            var job = new LoggerJob<ExceptionEntry>(logger, entry);

            JobQueue.Enqueue(job);
        }
    }

    /// <summary>
    /// Retrieves an instance of the logger configured for SQLite-based persistence.
    /// </summary>
    /// <returns>A logger instance.</returns>
    public static Logger<ExceptionEntry> GetLogger()
    {
        return new SqliteLogger<ExceptionEntry>(Logger.DefaultPathFile(DefaultFileName));
    }

    /// <summary>
    /// Retrieves an instance of the log reader associated with the SQLite-based logger.
    /// </summary>
    /// <returns>A log reader instance.</returns>
    public static ILogReader<ExceptionEntry> GetReader()
    {
        return GetLogger().GetReader();
    }

    /// <summary>
    /// Retrieves an instance of the log writer associated with the SQLite-based logger.
    /// </summary>
    /// <returns>A log writer instance.</returns>
    public static ILogWriter<ExceptionEntry> GetWriter()
    {
        return GetLogger().GetWriter();
    }
}