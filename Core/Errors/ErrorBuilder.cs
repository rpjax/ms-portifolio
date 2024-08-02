namespace Aidan.Core;

/// <summary>
/// Represents a builder for creating <see cref="Error"/> objects.
/// </summary>
public class ErrorBuilder
{
    private string? Title { get; set; }
    private string? Description { get; set; }
    private string? Code { get; set; }
    private List<string> Flags { get; } = new();
    private Dictionary<string, string> Details { get; } = new();
    private Dictionary<string, string> DebugData { get; } = new();

    /// <summary>
    /// Sets the title of the error.
    /// </summary>
    /// <param name="title">The title of the error.</param>
    /// <returns>The ErrorBuilder instance.</returns>
    public ErrorBuilder SetTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    /// Sets the description of the error.
    /// </summary>
    /// <param name="description">The description of the error.</param>
    /// <returns>The ErrorBuilder instance.</returns>
    public ErrorBuilder SetDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    /// Sets the code of the error.
    /// </summary>
    /// <param name="code">The code of the error.</param>
    /// <returns>The ErrorBuilder instance.</returns>
    public ErrorBuilder SetCode(string code)
    {
        Code = code;
        return this;
    }

    /// <summary>
    /// Adds a flag to the error.
    /// </summary>
    /// <param name="flag">The flag to add.</param>
    /// <returns>The ErrorBuilder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the flag already exists in the flags list.</exception>
    public ErrorBuilder AddFlag(string flag)
    {
        if (Flags.Contains(flag))
        {
            throw new ArgumentException($"Flag {flag} already exists in the flags list.");
        }

        Flags.Add(flag);
        return this;
    }

    /// <summary>
    /// Adds a detail to the error.
    /// </summary>
    /// <param name="key">The key of the detail.</param>
    /// <param name="value">The value of the detail.</param>
    /// <returns>The ErrorBuilder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the key already exists in the details dictionary.</exception>
    public ErrorBuilder AddDetail(string key, string? value)
    {
        if(value is null)
        {
            return this;
        }
        if (Details.ContainsKey(key))
        {
            throw new ArgumentException($"Key {key} already exists in the details dictionary.");
        }

        Details[key] = value;
        return this;
    }

    /// <summary>
    /// Adds data to the error.
    /// </summary>
    /// <param name="key">The key of the data.</param>
    /// <param name="value">The value of the debug data.</param>
    /// <returns>The ErrorBuilder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the key already exists in the debug data dictionary.</exception>
    public ErrorBuilder AddData(string key, string? value)
    {
        if (value is null)
        {
            return this;
        }
        if (DebugData.ContainsKey(key))
        {
            throw new ArgumentException($"Key {key} already exists in the debug data dictionary.");
        }

        DebugData[key] = value;
        return this;
    }

    /// <summary>
    /// Builds the Error object with the provided properties.
    /// </summary>
    /// <returns>The Error object.</returns>
    public Error Build()
    {
        return new Error(
            title: Title,
            description: Description,
            code: Code,
            flags: Flags,
            details: Details,
            data: DebugData
        );
    }
}
