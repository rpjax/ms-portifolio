namespace ModularSystem.Core.Logging;

[Serializable]
public abstract class ConsoleEntry
{
    public string? Prefix { get; set; } = null;
    public string? Message { get; set; } = null;
    public string? Suffix { get; set; } = null;

    public ConsoleColor? PrefixColor { get; set; } = null;
    public ConsoleColor? PrefixBackgroundColor { get; set; } = null;

    public ConsoleColor? MessageColor { get; set; } = null;
    public ConsoleColor? MessageBackgroundColor { get; set; } = null;

    public ConsoleColor? SuffixColor { get; set; } = null;
    public ConsoleColor? SuffixBackgroundColor { get; set; } = null;

    public virtual string PrefixToString()
    {
        return $"{Prefix}";
    }

    public virtual string MessageToString()
    {
        return $" {Message}";
    }

    public virtual string SuffixToString()
    {
        return $"{Suffix}";
    }

    public override string ToString()
    {
        return $"{PrefixToString()}{MessageToString()}{SuffixToString()}";
    }
}