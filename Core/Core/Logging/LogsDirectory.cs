using ModularSystem.Core.Helpers;

namespace ModularSystem.Core.Logging;

/// <summary>
/// Internal initializer responsible for setting up the default logging directory during the application's initialization process.
/// </summary>
internal class LogsDirectoryInitializer : Initializer
{
    /// <summary>
    /// Performs internal initialization tasks specific to the logging system. <br/>
    /// This method ensures that the default directory for logs is initialized.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    /// <returns>A completed task, as the initialization is a synchronous process.</returns>
    protected internal override Task InternalInitAsync(Options options)
    {
        LogsDirectory.InitDefaultDirectory();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provides utility methods for managing directories and file paths used by logger implementations.
/// </summary>
public static class LogsDirectory
{
    /// <summary>
    /// Gets the absolute path of the default logging directory.
    /// </summary>
    /// <returns>The absolute path to the logging directory.</returns>
    public static string AbsolutePath()
    {
        return $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}LOGS";
    }

    /// <summary>
    /// Retrieves file information for a given relative path within the logging directory.
    /// </summary>
    /// <param name="relativePath">The relative path to the log file within the logging directory.</param>
    /// <returns>A <see cref="FileInfo"/> object representing the file at the specified relative path.</returns>
    public static FileInfo GetFileInfo(string relativePath)
    {
        return new FileInfo(FileSystemHelper
            .NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}LOGS{Path.DirectorySeparatorChar}{relativePath}"));
    }

    /// <summary>
    /// Initializes and creates the default logging directory if it doesn't already exist.
    /// </summary>
    /// <remarks>
    /// This method is called during application startup to ensure that the logging directory is available for use by logger implementations.
    /// </remarks>
    public static void InitDefaultDirectory()
    {
        Directory.CreateDirectory(AbsolutePath());
    }
}
