using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Extensions;

public static class NonTerminalExtensions
{
    public static string ToSententialNotation(this NonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToBnfNotation(this NonTerminal nonTerminal)
    {
        return $"<{nonTerminal.Name}>";
    }

    public static string ToEbnfNotation(this NonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToEbnfKleeneNotation(this NonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

}

public static class INonTerminalExtensions
{
    public static string ToSententialNotation(this INonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToBnfNotation(this INonTerminal nonTerminal)
    {
        return $"<{nonTerminal.Name}>";
    }

    public static string ToEbnfNotation(this INonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToEbnfKleeneNotation(this INonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

}

