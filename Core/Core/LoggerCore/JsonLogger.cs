using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace ModularSystem.Core.Logging;

public class JsonEntry : Entry
{

}

public class JsonLogger<T> where T : JsonEntry
{
    FileInfo fileInfo;

    public JsonLogger(FileInfo fileInfo)
    {
        this.fileInfo = fileInfo;
    }

    public bool Exists()
    {
        return File.Exists(fileInfo.FullName);
    }

    public void CreateFile()
    {
        var data = new List<T>();
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(fileInfo.FullName, json);
    }

    public IEnumerable<T> GetEntries()
    {
        EnsureFileExists();
        var enumerable = JsonSerializer.DeserializeAsyncEnumerable<T>(fileInfo.OpenRead())!;
        var enumerator = enumerable.GetAsyncEnumerator();

        while (true)
        {
            if (!enumerator.MoveNextAsync().AsTask().Result)
            {
                break;
            }

            yield return enumerator.Current!;
        }

        enumerator.DisposeAsync().AsTask().Wait();
    }

    public void Write(T entry)
    {
        EnsureFileExists();
        var json = JsonSerializer.Serialize(entry);
        using var stream = fileInfo.OpenWrite();
        // [{}, {}]
        // [{}, {}, {}]
        json = $", {json}]";
        stream.Position = stream.Length - 1;

        stream.Write(Encoding.UTF8.GetBytes(json));
        stream.Flush();
        stream.Dispose();
    }

    public void Write(IEnumerable<T> entries)
    {
        var str = new StringBuilder();
        var isFirst = true;

        EnsureFileExists();
        using var stream = fileInfo.OpenWrite();
        stream.Position = stream.Length - 1;

        foreach (var entry in entries)
        {
            if (isFirst)
            {
                str.Append($"{JsonSerializer.Serialize(entry)}");
            }
            else
            {
                str.Append($", {JsonSerializer.Serialize(entry)}");
            }

            stream.Write(Encoding.UTF8.GetBytes(str.ToString()));
            stream.Flush();
            str.Clear();
            isFirst = false;
        }

        str.Append($"]");
        stream.Write(Encoding.UTF8.GetBytes(str.ToString()));
        stream.Flush();
        stream.Dispose();
    }

    void EnsureFileExists()
    {
        if (!Exists())
        {
            CreateFile();
        }
    }
}
