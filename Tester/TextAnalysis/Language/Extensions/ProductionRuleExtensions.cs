using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language;

public static class ProductionRuleExtensions
{
    public static ProductionSymbol? GetLefmostSymbol(this ProductionRule rule)
    {
        return rule.Body
            .FirstOrDefault();
    }

    public static ProductionSymbol? GetRightmostSymbol(this ProductionRule rule)
    {
        return rule.Body
            .LastOrDefault();
    }

    public static NonTerminal? GetLeftmostNonTerminal(this ProductionRule rule)
    {
        return rule.Body
            .OfType<NonTerminal>()
            .FirstOrDefault();
    }

    public static NonTerminal? GetRightmostNonTerminal(this ProductionRule rule)
    {
        return rule.Body
            .OfType<NonTerminal>()
            .LastOrDefault();
    }

    public static bool IsLeftRecursive(this ProductionRule rule)
    {
        return GetLefmostSymbol(rule) is NonTerminal nonTerminal
            && nonTerminal == rule.Head;
    }

    public static bool IsRightRecursive(this ProductionRule rule)
    {
        return GetRightmostSymbol(rule) is NonTerminal nonTerminal
            && nonTerminal == rule.Head;
    }

    public static Terminal? GetTerminalPrefix(this ProductionRule rule)
    {
        return rule.Body
            .OfType<Terminal>()
            .FirstOrDefault();
    }

    public static int IndexOfSymbol(this ProductionRule production, ProductionSymbol symbol)
    {
        var index = -1;

        foreach (var item in production.Body)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    public static void ReplaceNonTerminal(this ProductionRule production, NonTerminal current, NonTerminal replacement)
    {
        for (int i = 0; i < production.Body.Length; i++)
        {
            if (production.Body[i] == current)
            {
                production.Body[i] = replacement;
            }
        }
    }

}
