using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Grammars;

public class MacroTesterGrammar : GrammarDefinition
{
    public MacroTesterGrammar() : base(GetStart(), GetProductions())
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
