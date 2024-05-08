using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Grammars;

public class MyComplexGrammar : Grammar
{
    public MyComplexGrammar() : base(productions: CreateProductions(), start: GetStartSymbol())
    {
    }

    private static ProductionRule[] CreateProductions()
    {
        /*
         * S -> A a X
         * A -> B | C
         * B -> B b | g
         * C -> e D
         */

        return new[]
        {
            // S -> A a X
            new ProductionRule("S",
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "a"),
                new NonTerminal("X")
            ),

            // A -> B | C
            new ProductionRule("A",
                new NonTerminal("B"),
                new PipeMacro(),
                new NonTerminal("C")
            ),

            // B -> B b | g
            new ProductionRule("B",
                new NonTerminal("B"),
                new PipeMacro(),
                new NonTerminal("C")
            ),

            // C -> e D
            new ProductionRule("C",
                new Terminal(TokenType.Identifier, "e"),
                new NonTerminal("D")
            )
        };
    }

    private static NonTerminal GetStartSymbol()
    {
        return new NonTerminal("S");
    }
}
