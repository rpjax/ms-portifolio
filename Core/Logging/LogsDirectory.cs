using ModularSystem.Core.Helpers;

namespace ModularSystem.Core.Logging;

/// <summary>
/// Provides utility methods for managing directories and file paths used by logger implementations.
/// </summary>
public static class LogsDirectory
{
    const string FolderName = "Logs";

    /// <summary>
    /// Gets the absolute path of the default logging directory.
    /// </summary>
    /// <returns>The absolute path to the logging directory.</returns>
    public static string AbsolutePath()
    {
        var @base = AppDomain.CurrentDomain.BaseDirectory;
        var separator = Path.DirectorySeparatorChar;
        return $"{@base}{separator}{FolderName}";
    }

    /// <summary>
    /// Retrieves file information for a given relative path within the logging directory.
    /// </summary>
    /// <param name="relativePath">The relative path to the log file within the logging directory.</param>
    /// <param name="initializeDirectory">Whether to create the directory if it doesn't already exist.</param>
    /// <param name="initializeFile">Whether to create the file if it doesn't already exist.</param>
    /// <returns>A <see cref="FileInfo"/> object representing the file at the specified relative path.</returns>
    public static FileInfo GetFileInfo(string relativePath, bool initializeDirectory = false, bool initializeFile = false)
    {
        var @base = AppDomain.CurrentDomain.BaseDirectory;
        var separator = Path.DirectorySeparatorChar;
        var path = $"{@base}{separator}{FolderName}{separator}{relativePath}";

        return FileSystemHelper.GetFileInfo(
            absolutePath: path, 
            initializeDirectory: initializeDirectory, 
            initializeFile: initializeFile);
    }

}
