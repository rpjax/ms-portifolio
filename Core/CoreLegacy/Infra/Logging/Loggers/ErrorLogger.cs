using Microsoft.EntityFrameworkCore;
using ModularSystem.Core.Threading;
using ModularSystem.EntityFramework;

namespace ModularSystem.Core.Logging;

class ExceptionLoggerInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    {
        ErrorLogger.EnableDiskLogging = options.EnableDiskErrorLogger;
        ErrorLogger.EnableConsoleLogging = options.EnableConsoleErrorLogger;

        return Task.CompletedTask;
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
            JobQueue.Enqueue(CreateLogJob(error, logFile));
        }
    }

    /// <summary>
    /// Logs an error derived from an <see cref="Exception"/>, along with additional flags.
    /// </summary>
    /// <param name="exception">The exception to derive the error from.</param>
    /// <param name="flags">Additional flags to classify the error.</param>
    public static void Log(Exception exception, params string[] flags)
    {
        if (exception is ErrorException errorException)
        {
            foreach (var error in errorException.Errors)
            {
                Log(error.AddFlags(flags));
            }

            return;
        }

        Log(new Error(exception).AddFlags(flags));
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

    private static Job CreateLogJob(Error error, FileInfo? logFile = null)
    {
        return new LogJob(error, logFile);
    }

    private IDataAccessObject<ErrorEntry> CreateDataAccessObject(FileInfo? fileInfo)
    {
        fileInfo ??= LogsDirectory.GetFileInfo(DefaultFileName);
        return new EFCoreDataAccessObject<ErrorEntry>(new ErrorEntrySqliteContext(fileInfo));
    }

    internal class ErrorEntrySqliteContext : EFCoreSqliteContext<ErrorEntry>
    {
        public ErrorEntrySqliteContext(FileInfo fileInfo, string tableName = "Entries") : base(fileInfo, tableName)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ErrorEntry>(entity =>
            {
                // Configure the Flags collection
                entity.OwnsMany(e => e.Flags, a =>
                {
                    a.WithOwner().HasForeignKey("ErrorId");
                    a.Property<long>("Id").ValueGeneratedOnAdd(); // Auto-increment primary key
                    a.HasKey("Id"); // Make Id the sole primary key
                });

                // Configure the Details collection
                entity.OwnsMany(e => e.Details, a =>
                {
                    a.WithOwner().HasForeignKey("ErrorId");
                    a.Property<long>("Id").ValueGeneratedOnAdd(); // Auto-increment primary key
                    a.HasKey("Id"); // Make Id the sole primary key
                });

                // Configure the Data collection
                entity.OwnsMany(e => e.Data, a =>
                {
                    a.WithOwner().HasForeignKey("ErrorId");
                    a.Property<long>("Id").ValueGeneratedOnAdd(); // Auto-increment primary key
                    a.HasKey("Id"); // Make Id the sole primary key
                });

            });

        }

    }

    internal class LogJob : Job
    {
        private Error error { get; }
        private FileInfo? logFile { get; }

        public LogJob(Error error, FileInfo? logFile)
        {
            this.error = error;
            this.logFile = logFile;
        }

        protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
        {
            using var service = new ErrorLogger(logFile);
            var entry = new ErrorEntry(error);

            await service.CreateAsync(entry);
        }
    }

}
