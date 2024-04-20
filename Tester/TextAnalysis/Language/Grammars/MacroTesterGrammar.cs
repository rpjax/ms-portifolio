using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class MacroTesterGrammar : GrammarDefinition
{
    public MacroTesterGrammar() : base(GetProductions(), GetStart())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new[]
        {
            // S -> A [ "b" ] C
            new ProductionRule(
                "S",
                new NonTerminal("A"),
                new OptionMacro(
                    new Terminal(TokenType.Identifier, "b")
                ),
                new NonTerminal("C")
            ),
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("S");
    }
}
