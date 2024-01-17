using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents an operation error with optional text, source, code and flags.
/// </summary>
public class Error
{
    /// <summary>
    /// Describes the error.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// An identifier used to bind this error to its validation source. 
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// A code meant to track the error. Used for book-keeping of the error.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the list of flags for this operation error.
    /// </summary>
    public List<string> Flags { get; set; } = new(0);

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    [JsonConstructor]
    public Error()
    {
        Text = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with specified text, code, and target.
    /// </summary>
    /// <param name="text">The text description of the error.</param>
    /// <param name="code">The code associated with the error.</param>
    /// <param name="source">The source associated with the error.</param>
    /// <param name="flags">The flags associated with the error.</param>
    public Error(string? text, string? source = null, string? code = null, IEnumerable<string>? flags = null)
    {
        Text = text;
        Source = source;
        Code = code;

        if (flags != null)
        {
            Flags = flags.ToList();
        }
    }

    /// <summary>
    /// Provides implicit conversion from string to <see cref="Error"/>.
    /// </summary>
    /// <param name="text">The error message to convert into a <see cref="Error"/>.</param>
    public static implicit operator Error(string text)
    {
        return new(text);
    }

    /// <summary>
    /// Returns a string representation of the <see cref="Error"/>, combining target, code, and text.
    /// </summary>
    /// <returns>A string representation of the ValidationError.</returns>
    public override string ToString()
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(Source))
        {
            parts.Add($"Source: \"{Source}\"");
        }

        if (!string.IsNullOrEmpty(Code))
        {
            parts.Add($"Code: \"{Code}\"");
        }

        if (!string.IsNullOrEmpty(Text))
        {
            parts.Add($"Message: \"{Text}\"");
        }

        return parts.Count > 0
            ? string.Join(", ", parts)
            : "Undefined ValidationError";
    }

    public Error AppendSource(string? source, string? separator = ".")
    {
        if (string.IsNullOrEmpty(source))
        {
            return this;
        }
        if (string.IsNullOrEmpty(Source))
        {
            Source = source;
            return this;
        }

        Source = $"{source}{separator}{Source}";
        return this;
    }

}
