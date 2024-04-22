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
        return new[]
        {
            new ProductionRule("S", 
                new NonTerminal("A"), 
                new Terminal(TokenType.Identifier, "a"), 
                new NonTerminal("X")
            ),
            new ProductionRule("S", 
                new Terminal(TokenType.Identifier, "b"), 
                new NonTerminal("Y"), 
                new Terminal(TokenType.Identifier, "c")
            ),
            new ProductionRule("S",
                new NonTerminal("D"),
                new Terminal(TokenType.Identifier, "d"),
                new NonTerminal("Z")
            ),
            new ProductionRule("A",
                new NonTerminal("B"), 
                new NonTerminal("C"),
                new Terminal(TokenType.Identifier, "e")
            ),
            new ProductionRule("B",
                new NonTerminal("S"),
                new Terminal(TokenType.Identifier, "e")
            ),

            new ProductionRule("A", new Terminal(TokenType.Identifier, "f"), new NonTerminal("S")),
            new ProductionRule("B", new Terminal(TokenType.Identifier, "g"), new NonTerminal("D")),
            new ProductionRule("B", new Terminal(TokenType.Identifier, "h"), new NonTerminal("E"), new Terminal(TokenType.Identifier, "i")),
            new ProductionRule("C", new NonTerminal("F"), new Terminal(TokenType.Identifier, "j")),
            new ProductionRule("C", new Terminal(TokenType.Identifier, "k"), new NonTerminal("G"), new Terminal(TokenType.Identifier, "l")),
            new ProductionRule("D", new Terminal(TokenType.Identifier, "m"), new NonTerminal("H"), new Terminal(TokenType.Identifier, "n")),
            new ProductionRule("D", new NonTerminal("I"), new Terminal(TokenType.Identifier, "o")),
            new ProductionRule("E", new NonTerminal("J"), new Terminal(TokenType.Identifier, "p"), new NonTerminal("K")),
            new ProductionRule("E", new Terminal(TokenType.Identifier, "q"), new NonTerminal("L")),
            new ProductionRule("F", new NonTerminal("M"), new Terminal(TokenType.Identifier, "r"), new NonTerminal("N")),
            new ProductionRule("F", new Terminal(TokenType.Identifier, "s"), new NonTerminal("O"), new Terminal(TokenType.Identifier, "t")),
            // Add more complex rules as needed...
        };
    }

    private static NonTerminal GetStartSymbol()
    {
        return new NonTerminal("S");
    }
}
