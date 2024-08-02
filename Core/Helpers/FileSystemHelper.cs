namespace Aidan.Core.Helpers;

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
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path), "Path cannot be null or empty.");
        }

        try
        {
            string fullPath = Path.GetFullPath(new Uri(path).LocalPath);
            return fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        catch (UriFormatException)
        {
            throw new ArgumentException("Invalid path format.", nameof(path));
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while normalizing the path.", ex);
        }
    }

    /// <summary>
    /// Returns a relative path normalized to the environment.
    /// </summary>
    /// <param name="path">The relative path to normalize.</param>
    /// <returns>A normalized relative path.</returns>
    public static string NormalizeRelativePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path), "Path cannot be null or empty.");
        }

        // Ensure that the path is relative
        if (Path.IsPathRooted(path))
        {
            throw new ArgumentException("Path must be relative.", nameof(path));
        }

        try
        {
            // Get the full path based on the current directory
            string fullPath = Path.GetFullPath(path, Directory.GetCurrentDirectory());
            // Get the relative path from the base directory
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.GetRelativePath(baseDir, fullPath);
            return relativePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while normalizing the path.", ex);
        }
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
    /// <param name="initializeDirectory">Whether to ensure the containing directory exists.</param>
    /// <param name="initializeFile">Whether to ensure the file itself exists.</param>
    /// <returns>Information about the specified file.</returns>
    public static FileInfo GetFileInfo(string absolutePath, bool initializeDirectory = false, bool initializeFile = false)
    {
        var fileInfo = new FileInfo(NormalizeAbsolutePath(absolutePath));

        fileInfo.Refresh();

        if (initializeDirectory)
        {
            if (fileInfo.DirectoryName == null)
            {
                throw new InvalidOperationException("Failed to retrieve the directory name.");
            }

            EnsureDirectoryExists(fileInfo.DirectoryName);
        }
        if (initializeFile)
        {
            EnsureFileExists(fileInfo.FullName);
        }

        return fileInfo;
    }
}

/// <summary>
/// Provides access to the default application folder for storing data.
/// </summary>
public static class LocalStorage
{
    /// <summary>
    /// The name of the default storage folder.
    /// </summary>
    public const string FolderName = "Storage";

    /// <summary>
    /// Gets the path to the default application folder for data storage.
    /// </summary>
    /// <returns>The normalized absolute path to the storage folder.</returns>
    public static string DirectoryPath()
    {
        return FileSystemHelper.NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}{FolderName}");
    }

    /// <summary>
    /// Gets the <see cref="FileInfo"/> for the specified file within the storage folder.
    /// </summary>
    /// <param name="file">The name of the file.</param>
    /// <param name="initializeFolder">Specifies whether to initialize the folder if it does not exist.</param>
    /// <param name="initializeFile">Specifies whether to initialize the file if it does not exist.</param>
    /// <returns>The <see cref="FileInfo"/> for the specified file.</returns>
    public static FileInfo GetFileInfo(string file, bool initializeFolder = false, bool initializeFile = false)
    {
        var normalizedPath = FileSystemHelper.NormalizeAbsolutePath($"{DirectoryPath()}{Path.DirectorySeparatorChar}{file}");
        var fileInfo = new FileInfo(normalizedPath);

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
