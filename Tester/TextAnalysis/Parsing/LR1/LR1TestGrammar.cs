using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Debug;

public class LR1TestGrammar : LR1Grammar
{
    public LR1TestGrammar() : base(GetStart(), GetProductions())
    {
    }

    private static NonTerminal GetStart() => new NonTerminal("S′");

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            new ProductionRule(
                "S′",
                new NonTerminal("S")
            ),

            new ProductionRule(
                "S",
                new NonTerminal("X"),
                new NonTerminal("X")
            ),

            new ProductionRule(
                "X",
                new Terminal("y"),
                new NonTerminal("X")
            ),

            new ProductionRule(
                "X",
                new Terminal("z")
            )
        };
    }
}

public class LR1TestGrammar2 : LR1Grammar
{
    public LR1TestGrammar2() : base(GetStart(), GetProductions())
    {
    }

    private static NonTerminal GetStart() => new NonTerminal("E′");

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            new ProductionRule(
                "E′",
                new NonTerminal("E")
            ),

            new ProductionRule(
                "E",
                new NonTerminal("E"),
                new Terminal("+"),
                new NonTerminal("T")
            ),
            new ProductionRule(
                "E",
                new NonTerminal("T")
            ),

            new ProductionRule(
                "T",
                new NonTerminal("T"),
                new Terminal("*"),
                new NonTerminal("F")
            ),
            new ProductionRule(
                "T",
                new NonTerminal("F")
            ),

            new ProductionRule(
                "F",
                new Terminal(Tokenization.TokenType.Integer)
            )
        };
    }
}
