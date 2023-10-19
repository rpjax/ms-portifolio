namespace ModularSystem.Core.Helpers;

/// <summary>
/// Provides utility methods for file system operations.
/// </summary>
public static class FileSystemHelper
{
    /// <summary>
    /// Returns an absolute path normalized to the environment.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>A normalized absolute path.</returns>
    public static string NormalizeAbsolutePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath)
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Returns a relative path normalized to the environment.
    /// </summary>
    /// <param name="path">The relative path to normalize.</param>
    /// <returns>A normalized relative path.</returns>
    public static string NormalizeRelativePath(string path)
    {
        return new Uri(path).LocalPath
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Ensures a directory exists, creating it if necessary.
    /// </summary>
    /// <param name="absolutePath">The absolute path of the directory.</param>
    public static void EnsureDirectoryExists(string absolutePath)
    {
        if (!Directory.Exists(absolutePath))
        {
            Directory.CreateDirectory(absolutePath);
        }
    }

    /// <summary>
    /// Ensures a file exists, creating it if necessary.
    /// </summary>
    /// <param name="absolutePath">The absolute path of the file.</param>
    public static void EnsureFileExists(string absolutePath)
    {
        if (!File.Exists(absolutePath))
        {
            using var stream = File.Create(absolutePath);
            // Flushing and disposing are not necessary as the using statement 
            // ensures proper disposal which in turn flushes the stream.
        }
    }

    /// <summary>
    /// Retrieves file information and optionally ensures the containing directory or the file itself exists.
    /// </summary>
    /// <param name="absolutePath">The absolute path of the file.</param>
    /// <param name="ensureDirectoryExists">Whether to ensure the containing directory exists.</param>
    /// <param name="ensureFileExists">Whether to ensure the file itself exists.</param>
    /// <returns>Information about the specified file.</returns>
    public static FileInfo GetFileInformation(string absolutePath, bool ensureDirectoryExists = false, bool ensureFileExists = false)
    {
        var fileInfo = new FileInfo(NormalizeAbsolutePath(absolutePath));
        fileInfo.Refresh();

        if (ensureDirectoryExists)
        {
            if (fileInfo.DirectoryName == null)
            {
                throw new InvalidOperationException("Failed to retrieve the directory name.");
            }

            EnsureDirectoryExists(fileInfo.DirectoryName);
        }
        if (ensureFileExists)
        {
            EnsureFileExists(fileInfo.FullName);
        }

        return fileInfo;
    }
}

/// <summary>
/// Default application folder used to store data.
/// </summary>
public static class LocalStorage
{
    public static string DirectoryPath()
    {
        return FileSystemHelper.NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}Storage");
    }

    public static FileInfo GetFileInfo(string file, bool initializeFolder = false, bool initializeFile = false)
    {
        var fileInfo = new FileInfo(FileSystemHelper.NormalizeAbsolutePath($"{DirectoryPath()}{Path.DirectorySeparatorChar}{file}"));

        fileInfo.Refresh();

        if (initializeFolder)
        {
            if (fileInfo.DirectoryName == null)
            {
                throw new InvalidOperationException();
            }

            FileSystemHelper.EnsureDirectoryExists(fileInfo.DirectoryName);
        }
        if (initializeFile)
        {
            FileSystemHelper.EnsureFileExists(fileInfo.FullName);
        }

        return fileInfo;
    }
}
