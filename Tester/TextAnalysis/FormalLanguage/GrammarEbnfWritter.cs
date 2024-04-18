namespace ModularSystem.Core.TextAnalysis.Language;

/*
 *  Grammar representation of a context-free grammar (type 2). 
 *  A context-free grammar can be defined as a 4-tuple G = (N, T, P, S), where:
 *  - N is a set of non-terminal symbols.
 *  - T is a set of terminal symbols.
 *  - P is a set of production rules.
 *  - S is the start symbol.
 */

public class GrammarEbnfWritter
{
    private GrammarDefinition Grammar { get; set; }

    public GrammarEbnfWritter(GrammarDefinition grammar)
    {
        Grammar = grammar;
    }

    public void Write(TextWriter writer)
    {
        var productionGroups = Grammar.Productions
            .GroupBy(x => x.Head.Name);

        foreach (var group in productionGroups)
        {
            var lhs = group.Key;
            var productionBodies = group
                .Select(x => x.Body)
                .ToArray();

            var firstBody = productionBodies.FirstOrDefault();

            writer.Write($"{lhs} = ");

            for (int i = 0; i < productionBodies.Length; i++)
            {
                if(i > 0)
                {
                    writer.Write(" | ");
                }

                writer.Write(Stringify(productionBodies[i]));
            }

            writer.Write(";\n");
        }
    }

    public string Write()
    {
        using var writer = new StringWriter();
        Write(writer);
        return writer.ToString();
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        Write(writer);
        return writer.ToString();
    }

    private string Stringify(ProductionSymbol[]? productionSymbols)
    {
        if(productionSymbols is null)
        {
            return "";
        }

        return string.Join(" ", productionSymbols.Select(x => x.ToString()));
    }
}
