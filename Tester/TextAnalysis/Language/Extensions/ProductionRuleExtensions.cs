using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language;

public static class ProductionRuleExtensions
{
    public static Symbol? GetLefmostSymbol(this ProductionRule rule)
    {
        return rule.Body
            .FirstOrDefault();
    }

    public static Symbol? GetRightmostSymbol(this ProductionRule rule)
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

    public static Terminal? GetTerminalPrefix(this ProductionRule rule)
    {
        return rule.Body
            .OfType<Terminal>()
            .FirstOrDefault();
    }

    public static int IndexOfSymbol(this ProductionRule production, Symbol symbol)
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

    public static bool IsUnitProduction(this ProductionRule rule)
    {
        return rule.Body.Length == 1
            && rule.Body[0].IsNonTerminal;
    }
}
