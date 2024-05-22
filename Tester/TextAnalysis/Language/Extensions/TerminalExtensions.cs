using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Extensions;

public static class TerminalExtensions
{
    public static string ToSententialNotation(this Terminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr.ToCamelCase();
    }

    public static string ToBnfNotation(this Terminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfNotation(this Terminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfKleeneNotation(this Terminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }
}

public static class ITerminalExtensions
{
    public static string ToSententialNotation(this ITerminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr.ToCamelCase();
    }

    public static string ToBnfNotation(this ITerminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfNotation(this ITerminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfKleeneNotation(this ITerminal terminal)
    {
        var typeStr = terminal.TokenType.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }
}

