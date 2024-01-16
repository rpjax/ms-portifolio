using ModularSystem.Core.Helpers;

namespace ModularSystem.Core.Logging;

/// <summary>
/// Defines a mechanism to read log entries.
/// </summary>
/// <typeparam name="T">The type of log entry this reader supports.</typeparam>
public interface ILogReader<T> : IDisposable
{
    /// <summary>
    /// Retrieves all the log entries.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> containing the log entries.</returns>
    IQueryable<T> AsQueryable();

    IAsyncEnumerable<T> AsAsyncQueryable();
}

/// <summary>
/// Defines a mechanism to write log entries.
/// </summary>
/// <typeparam name="T">The type of log entry this writer supports.</typeparam>
public interface ILogWriter<T> : IDisposable
{
    /// <summary>
    /// Writes a single log entry.
    /// </summary>
    /// <param name="entry">The log entry to be written.</param>
    Task WriteAsync(T entry);

    /// <summary>
    /// Writes multiple log entries.
    /// </summary>
    /// <param name="entries">The log entries to be written.</param>
    Task WriteAsync(IEnumerable<T> entries);
}

class LoggerInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    {
        Logger.InitDefaultDirectory();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provides utility methods for logger implementations, including default directory and file path management.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Gets the default directory where log files are stored.
    /// </summary>
    /// <returns>The path to the default log directory.</returns>
    public static string DefaultDirectory()
    {
        return $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}LOGS";
    }

    /// <summary>
    /// Computes and returns the full path of the specified log file inside the default log directory.
    /// </summary>
    /// <param name="file">The name of the log file.</param>
    /// <returns>The full path of the specified log file.</returns>
    public static FileInfo DefaultPathFile(string file)
    {
        return new FileInfo(FileSystemHelper
            .NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}LOGS{Path.DirectorySeparatorChar}{file}"));
    }

    /// <summary>
    /// Initializes and creates the default logging directory if it doesn't already exist.
    /// </summary>
    public static void InitDefaultDirectory()
    {
        Directory.CreateDirectory(DefaultDirectory());
    }
}

/// <summary>
/// Provides an abstract base for logger implementations.
/// </summary>
/// <typeparam name="T">The type of log entry this logger supports.</typeparam>
public abstract class Logger<T> where T : ILogEntry
{
    /// <summary>
    /// Retrieves the file information associated with the logger's log file.
    /// </summary>
    /// <returns>A <see cref="FileInfo"/> representing the logger's log file.</returns>
    public abstract FileInfo GetFileInfo();

    /// <summary>
    /// Retrieves an instance of a log reader associated with this logger.
    /// </summary>
    /// <returns>A log reader instance.</returns>
    public abstract ILogReader<T> GetReader();

    /// <summary>
    /// Retrieves an instance of a log writer associated with this logger.
    /// </summary>
    /// <returns>A log writer instance.</returns>
    public abstract ILogWriter<T> GetWriter();
}
