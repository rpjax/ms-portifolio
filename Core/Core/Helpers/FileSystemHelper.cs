namespace ModularSystem.Core.Helpers;

public static class FileSystemHelper
{
    /// <summary>
    /// Returns an aboslute path normalzied to the environment.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string NormalizeAbsolutePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath)
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static string NormalizeRelativePath(string path)
    {
        return new Uri(path).LocalPath
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static void InitializeDirectory(string abosolutePath)
    {
        if (!Directory.Exists(abosolutePath))
        {
            _ = Directory.CreateDirectory(abosolutePath);
        }
    }

    public static void InitializeFile(string abosolutePath)
    {
        if (!File.Exists(abosolutePath))
        {
            using var stream = File.Create(abosolutePath);
            stream.Flush();
            stream.Dispose();
        }
    }

    public static FileInfo GetFileInfo(string abosolutePath, bool initializeFolder = false, bool initializeFile = false)
    {
        var fileInfo = new FileInfo(NormalizeAbsolutePath(abosolutePath));

        fileInfo.Refresh();

        if (initializeFolder)
        {
            if (fileInfo.DirectoryName == null)
            {
                throw new InvalidOperationException();
            }

            InitializeDirectory(fileInfo.DirectoryName);
        }
        if (initializeFile)
        {
            InitializeFile(fileInfo.FullName);
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

            FileSystemHelper.InitializeDirectory(fileInfo.DirectoryName);
        }
        if (initializeFile)
        {
            FileSystemHelper.InitializeFile(fileInfo.FullName);
        }

        return fileInfo;
    }
}
