using System.Text.Json;

namespace ModularSystem.Core.Helpers;

public class JsonStorage<T> where T : class, new()
{
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    protected FileInfo FileInfo { get; init; }

    public JsonStorage(FileInfo fileInfo, bool initializeFile = false, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (fileInfo.Extension != ".json")
        {
            fileInfo = new FileInfo($"{fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length)}.json");
        }

        FileInfo = FileSystemHelper.GetFileInfo(fileInfo.FullName, true, initializeFile);
        JsonSerializerOptions = jsonSerializerOptions;
    }

    public FileInfo GetFileInfo()
    {
        return FileInfo;
    }

    public T? Read()
    {
        EnsureFileInitialization();
        var json = File.ReadAllText(FileInfo.FullName);
        return JsonSerializer.Deserialize<T>(json);
    }

    public FileStream ReadAsStream()
    {
        EnsureFileInitialization();
        return File.OpenRead(FileInfo.FullName);
    }

    public void Write(T data)
    {
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(FileInfo.FullName, json);
    }

    public FileStream OpenWrite()
    {
        return File.OpenWrite(FileInfo.FullName);
    }

    public void EnsureFileInitialization()
    {
        FileInfo.Refresh();

        if (!FileInfo.Exists || FileInfo.Length == 0)
        {
            Write(new T());
        }
    }

    public void EnsureFileInitialization(T initialValue)
    {
        FileInfo.Refresh();

        if (!FileInfo.Exists || FileInfo.Length == 0)
        {
            Write(initialValue);
        }
    }
}