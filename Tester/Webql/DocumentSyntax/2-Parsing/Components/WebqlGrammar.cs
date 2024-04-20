using ModularSystem.Core.TextAnalysis.Language;
using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class WebqlGrammar : GrammarDefinition
{
    public WebqlGrammar() : base(productions: GetProductions(), start: GetStart())
    {
    }

    /*
     * Complex grammar with several layers of derivation and cycles:
     * S → A "a" | B "b" | "c"
     * A → D "d" | S "e" | ε
     * B → A "f" | C "g" | S "h" | ε
     * C → F "i" | "j"
     * D → B "k" | E "l" | "m"
     * E → G "n" | A "o" | C "p"
     * F → "q" | D "r"
     * G → "s" | F "t" | E "u"
     */
    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            // S → A "a" | B "b" | "c"
            new ProductionRule("S", new ProductionSymbol[]
            {
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "a"),
            }),
            new ProductionRule("S", new ProductionSymbol[]
            {
                new NonTerminal("B"),
                new Terminal(TokenType.Identifier, "b"),
            }),
            new ProductionRule("S", new ProductionSymbol[]
            {
                new Terminal(TokenType.Identifier, "c"),
            }),

            // A → D "d" | S "e" | ε
            new ProductionRule("A", new ProductionSymbol[]
            {
                new NonTerminal("D"),
                new Terminal(TokenType.Identifier, "d"),
            }),
            new ProductionRule("A", new ProductionSymbol[]
            {
                new NonTerminal("S"),
                new Terminal(TokenType.Identifier, "e"),
            }),
            new ProductionRule("A", new ProductionSymbol[]
            {
                new Epsilon(),
            }),

            // B → A "f" | C "g" | S "h" | ε
            new ProductionRule("B", new ProductionSymbol[]
            {
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "f"),
            }),
            new ProductionRule("B", new ProductionSymbol[]
            {
                new NonTerminal("C"),
                new Terminal(TokenType.Identifier, "g"),
            }),
            new ProductionRule("B", new ProductionSymbol[]
            {
                new NonTerminal("S"),
                new Terminal(TokenType.Identifier, "h"),
            }),
            new ProductionRule("B", new ProductionSymbol[]
            {
                new Epsilon(),
            }),

            // C → F "i" | "j"
            new ProductionRule("C", new ProductionSymbol[]
            {
                new NonTerminal("F"),
                new Terminal(TokenType.Identifier, "i"),
            }),
            new ProductionRule("C", new ProductionSymbol[]
            {
                new Terminal(TokenType.Identifier, "j"),
            }),

            // D → B "k" | E "l" | "m"
            new ProductionRule("D", new ProductionSymbol[]
            {
                new NonTerminal("B"),
                new Terminal(TokenType.Identifier, "k"),
            }),
            new ProductionRule("D", new ProductionSymbol[]
            {
                new NonTerminal("E"),
                new Terminal(TokenType.Identifier, "l"),
            }),
            new ProductionRule("D", new ProductionSymbol[]
            {
                new Terminal(TokenType.Identifier, "m"),
            }),

            // E → G "n" | A "o" | C "p"
            new ProductionRule("E", new ProductionSymbol[]
            {
                new NonTerminal("G"),
                new Terminal(TokenType.Identifier, "n"),
            }),
            new ProductionRule("E", new ProductionSymbol[]
            {
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "o"),
            }),
            new ProductionRule("E", new ProductionSymbol[]
            {
                new NonTerminal("C"),
                new Terminal(TokenType.Identifier, "p"),
            }),

            // F → "q" | D "r"
            new ProductionRule("F", new ProductionSymbol[]
            {
                new Terminal(TokenType.Identifier, "q"),
            }),
            new ProductionRule("F", new ProductionSymbol[]
            {
                new NonTerminal("D"),
                new Terminal(TokenType.Identifier, "r"),
            }),

            // G → "s" | F "t" | E "u"
            new ProductionRule("G", new ProductionSymbol[]
            {
                new Terminal(TokenType.Identifier, "s"),
            }),
            new ProductionRule("G", new ProductionSymbol[]
            {
                new NonTerminal("F"),
                new Terminal(TokenType.Identifier, "t"),
            }),
            new ProductionRule("G", new ProductionSymbol[]
            {
                new NonTerminal("E"),
                new Terminal(TokenType.Identifier, "u"),
            }),
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("S");
    }
}
