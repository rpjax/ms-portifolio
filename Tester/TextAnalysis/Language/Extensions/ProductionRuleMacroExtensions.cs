namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class ProductionRuleMacroExtensions
{
    public static bool ContainsMacro(this ProductionRule production)
    {
        return production.Body.Any(x => x.IsMacro);
    }

    public static ProductionMacro? GetLeftmostMacro(this ProductionRule production)
    {
        return production.Body
            .FirstOrDefault(x => x.IsMacro)
            ?.AsMacro();
    }

    public static IEnumerable<ProductionRule> ExpandMacros(this ProductionRule production)
    {
        var macro = production.GetLeftmostMacro();

        if (macro is null)
        {
            yield return production;
            yield break;
        }

        var index = production.IndexOfSymbol(macro);
        var nonTerminal = new NonTerminal($"{production.Head.Name}ʹ");
        var body = new List<Symbol>();

        body.AddRange(production.Body.Take(index));
        body.Add(nonTerminal);
        body.AddRange(production.Body.Skip(index + 1));

        yield return new ProductionRule(
            head: production.Head,
            body: body.ToArray()
        );

        foreach (var sentence in macro.Expand(nonTerminal))
        {
            yield return new ProductionRule(
                head: nonTerminal,
                body: sentence
            );
        }
    }

}

