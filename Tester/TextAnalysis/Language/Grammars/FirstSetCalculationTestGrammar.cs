using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Grammars;

public class FirstSetCalculationTestGrammar : Grammar
{
    public FirstSetCalculationTestGrammar() : base(GetStart(), GetProductions())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            new ProductionRule(
                head: "S",
                body: new Sentence(
                    new NonTerminal("A"),
                    new NonTerminal("B"),
                    new NonTerminal("C"),
                    new NonTerminal("D"),
                    new NonTerminal("E")
                )
            ),

            new ProductionRule(
                head: "A",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "a"),
                    new PipeMacro(),
                    new Epsilon()
                )
            ),

            new ProductionRule(
                head: "B",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "b"),
                    new PipeMacro(),
                    new Epsilon()
                )
            ),

            new ProductionRule(
                head: "C",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "c")
                )
            ),

            new ProductionRule(
                head: "D",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "d"),
                    new PipeMacro(),
                    new Epsilon()
                )
            ),

            new ProductionRule(
                head: "E",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "e"),
                    new PipeMacro(),
                    new Epsilon()
                )
            ),
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("S");
    }
}

public class CommonFactorTestGrammar : Grammar
{
    public CommonFactorTestGrammar() : base(GetStart(), GetProductions())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        /*
         *  S -> A B
         *  S -> C D
         *  A -> a
         *  B -> b
         *  C -> a
         *  D -> d
         */
        return new ProductionRule[]
        {
            new ProductionRule(
                head: "S",
                body: new Sentence(
                    new NonTerminal("A"),
                    new NonTerminal("B")
                )
            ),

            new ProductionRule(
                head: "S",
                body: new Sentence(
                    new NonTerminal("C"),
                    new NonTerminal("D")
                )
            ),

            new ProductionRule(
                head: "A",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "a")
                )
            ),

            new ProductionRule(
                head: "B",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "b")
                )
            ),

            new ProductionRule(
                head: "C",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "a")
                )
            ),

            new ProductionRule(
                head: "D",
                body: new Sentence(
                    new Terminal(TokenType.Identifier, "d")
                )
            ),
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("S");
    }
}
