using System.Text.Json;

namespace ModularSystem.Core.Helpers;

/// <summary>
/// Provides methods for storing and retrieving data in JSON format.
/// </summary>
/// <typeparam name="T">The type of the data to be stored. Must be a class and have a parameterless constructor.</typeparam>
public class JsonStorage<T> where T : class
{
    /// <summary>
    /// Gets or sets the options for JSON serialization and deserialization.
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    /// Gets the file information for the JSON storage.
    /// </summary>
    protected FileInfo FileInfo { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonStorage{T}"/> class.
    /// </summary>
    /// <param name="fileInfo">Information about the file used for storage.</param>
    /// <param name="initializeFile">Indicates whether to create the file if it does not exist.</param>
    /// <param name="jsonSerializerOptions">Options for JSON serialization and deserialization.</param>
    public JsonStorage(FileInfo fileInfo, bool initializeFile = false, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (fileInfo.Extension != ".json")
        {
            fileInfo = new FileInfo($"{fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length)}.json");
        }

        FileInfo = FileSystemHelper.GetFileInformation(fileInfo.FullName, true, initializeFile);
        JsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// Gets the file information.
    /// </summary>
    /// <returns>The <see cref="FileInfo"/> instance representing the JSON storage file.</returns>
    public FileInfo GetFileInfo()
    {
        return FileInfo;
    }

    /// <summary>
    /// Opens the JSON storage file as a stream for reading.
    /// </summary>
    /// <returns>A <see cref="FileStream"/> for reading the file.</returns>
    public FileStream? ReadAsStream()
    {
        if (!FileInfo.Exists)
        {
            return null;
        }

        return File.OpenRead(FileInfo.FullName);
    }

    /// <summary>
    /// Reads the JSON data from the file and deserializes it to the specified type.
    /// </summary>
    /// <returns>The deserialized data.</returns>
    public T? Read()
    {
        if (!FileInfo.Exists)
        {
            return default;
        }

        var json = File.ReadAllText(FileInfo.FullName);

        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonSerializerSingleton.Deserialize<T>(json);
    }

    /// <summary>
    /// Asynchronously reads the JSON data from the file and deserializes it to the specified type.
    /// </summary>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the deserialized data.</returns>
    public async Task<T?> ReadAsync()
    {
        if (!FileInfo.Exists)
        {
            return default;
        }

        var json = await File.ReadAllTextAsync(FileInfo.FullName);

        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonSerializerSingleton.Deserialize<T>(json);
    }

    /// <summary>
    /// Serializes the given data to JSON and writes it to the file.
    /// </summary>
    /// <param name="data">The data to serialize and write.</param>
    public void Write(T data)
    {
        var json = JsonSerializerSingleton.Serialize(data);
        File.WriteAllText(FileInfo.FullName, json);
    }

    /// <summary>
    /// Asynchronously serializes the given data to JSON and writes it to the file.
    /// </summary>
    /// <param name="data">The data to serialize and write.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteAsync(T data)
    {
        var json = JsonSerializerSingleton.Serialize(data);
        return File.WriteAllTextAsync(FileInfo.FullName, json);
    }

    /// <summary>
    /// Opens the JSON storage file as a stream for writing.
    /// </summary>
    /// <returns>A <see cref="FileStream"/> for writing to the file.</returns>
    public FileStream OpenWrite()
    {
        return File.OpenWrite(FileInfo.FullName);
    }

    /// <summary>
    /// Ensures the initialization of the storage file. If the file does not exist or is empty, a new instance of T is written.
    /// </summary>
    public void EnsureFileInitialization()
    {
        FileInfo.Refresh();

        if (!FileInfo.Exists)
        {
            FileInfo.Create();
        }
    }

}
