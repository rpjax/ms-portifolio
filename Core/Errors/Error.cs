using System.Text.Json.Serialization;

namespace Aidan.Core;

public class ErrorFlag
{
    public string Value { get; set; }
}

public class ErrorDetail
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class ErrorData
{
    public string Name { get; set; }
    public string Value { get; set; }
}

/// <summary>
/// Represents an error with a title, description, code, flags, details, and debug data.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets the title of the error.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets the description of the error.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the code associated with the error.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets the list of flags associated with the error.
    /// </summary>
    public string[] Flags { get; set; }

    /// <summary>
    /// Gets the dictionary of details associated with the error.
    /// </summary>
    public Dictionary<string, string> Details { get; set; }

    /// <summary>
    /// Gets the dictionary of debug data associated with the error.
    /// </summary>
    public Dictionary<string, string> Data { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    [JsonConstructor]
    public Error()
    {
        Flags = Array.Empty<string>();
        Details = new Dictionary<string, string>();
        Data = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="title">The title of the error.</param>
    /// <param name="description">The description of the error.</param>
    /// <param name="code">The code associated with the error.</param>
    /// <param name="flags">The list of flags associated with the error.</param>
    /// <param name="details">The dictionary of details associated with the error.</param>
    /// <param name="data">The dictionary of data associated with the error.</param>
    public Error(
        string? title,
        string? description = null,
        string? code = null,
        IEnumerable<string>? flags = null,
        IEnumerable<KeyValuePair<string, string>>? details = null,
        IEnumerable<KeyValuePair<string, string>>? data = null)
    {
        Title = title;
        Code = code;
        Description = description;
        Flags = flags?.ToArray() ?? Array.Empty<string>();
        Details = details?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>();
        Data = data?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>(); 
    }

    /// <summary>
    /// Creates an <see cref="Error"/> object from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static Error FromException(Exception exception)
    {
        return new ErrorBuilder()
            .SetTitle(exception.GetType().Name)
            .SetDescription(exception.Message)
            .SetCode(exception.HResult.ToString())
            .AddFlag(ErrorFlags.Exception)
            .AddDetail("stackTrace", exception.StackTrace)
            .Build();
    }

    /// <summary>
    /// Returns a string representation of the error.
    /// </summary>
    /// <returns>A string representation of the error.</returns>
    public override string ToString()
    {
        var title = string.IsNullOrEmpty(Title)
            ? "No error message provided. "
            : Title;

        var description = string.IsNullOrEmpty(Description)
            ? null
            : $"{Details}: ";

        var code = string.IsNullOrEmpty(Code)
            ? null
            : $"({Code}) ";

        var details = Details.Any()
            ? "Details:\n" + string.Join(Environment.NewLine, Details.Select(d => $"- {d.Key}:\n{d.Value}"))
            : "No details provided for this error.";

        return $"{code}{title}{description}\n{details}";
    }

}
