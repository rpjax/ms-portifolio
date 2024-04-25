using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class MyComplexGrammar : GrammarDefinition
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

public class DebugGrammars
{
    // private ProductionSet Productions { get; }

    // public DebugGrammarCreator()
    // {
    //     Productions = new();

    // }

    public static GrammarDefinition CreateCommonFactorGrammar()
    {
        var set = new ProductionSet();
        var startSymbol = new NonTerminal("S");

        set.Add(
            head: "S",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "a"),
                new NonTerminal("B1")
            )
        );

        set.Add(
            head: "S",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "a"),
                new NonTerminal("B2")
            )
        );

        set.Add(
            head: "B1",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "a")
            )
        );

        set.Add(
            head: "B2",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "b")
            )
        );

        return new GrammarDefinition(set, startSymbol);
    }

    // public DebugGrammarCreator AddStartSymbol()
    // {
    //     var head = new NonTerminal("S");
    //     var body = new Sentence(
    //         new NonTerminal("A"),
    //         new Terminal(TokenType.Identifier, "a"),
    //         new NonTerminal("X")
    //     );

    //     Productions.Add();
    // }

    // public DebugGrammarCreator AddBasicProductions()
    // {

    // }
}
