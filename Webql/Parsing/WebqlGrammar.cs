using ModularSystem.TextAnalysis.Language;
using ModularSystem.TextAnalysis.Language.Components;
using ModularSystem.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class WebqlGrammar : Grammar
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
            new ProductionRule("S", new Symbol[]
            {
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "a"),
            }),
            new ProductionRule("S", new Symbol[]
            {
                new NonTerminal("B"),
                new Terminal(TokenType.Identifier, "b"),
            }),
            new ProductionRule("S", new Symbol[]
            {
                new Terminal(TokenType.Identifier, "c"),
            }),

            // A → D "d" | S "e" | ε
            new ProductionRule("A", new Symbol[]
            {
                new NonTerminal("D"),
                new Terminal(TokenType.Identifier, "d"),
            }),
            new ProductionRule("A", new Symbol[]
            {
                new NonTerminal("S"),
                new Terminal(TokenType.Identifier, "e"),
            }),
            new ProductionRule("A", new Symbol[]
            {
                new Epsilon(),
            }),

            // B → A "f" | C "g" | S "h" | ε
            new ProductionRule("B", new Symbol[]
            {
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "f"),
            }),
            new ProductionRule("B", new Symbol[]
            {
                new NonTerminal("C"),
                new Terminal(TokenType.Identifier, "g"),
            }),
            new ProductionRule("B", new Symbol[]
            {
                new NonTerminal("S"),
                new Terminal(TokenType.Identifier, "h"),
            }),
            new ProductionRule("B", new Symbol[]
            {
                new Epsilon(),
            }),

            // C → F "i" | "j"
            new ProductionRule("C", new Symbol[]
            {
                new NonTerminal("F"),
                new Terminal(TokenType.Identifier, "i"),
            }),
            new ProductionRule("C", new Symbol[]
            {
                new Terminal(TokenType.Identifier, "j"),
            }),

            // D → B "k" | E "l" | "m"
            new ProductionRule("D", new Symbol[]
            {
                new NonTerminal("B"),
                new Terminal(TokenType.Identifier, "k"),
            }),
            new ProductionRule("D", new Symbol[]
            {
                new NonTerminal("E"),
                new Terminal(TokenType.Identifier, "l"),
            }),
            new ProductionRule("D", new Symbol[]
            {
                new Terminal(TokenType.Identifier, "m"),
            }),

            // E → G "n" | A "o" | C "p"
            new ProductionRule("E", new Symbol[]
            {
                new NonTerminal("G"),
                new Terminal(TokenType.Identifier, "n"),
            }),
            new ProductionRule("E", new Symbol[]
            {
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "o"),
            }),
            new ProductionRule("E", new Symbol[]
            {
                new NonTerminal("C"),
                new Terminal(TokenType.Identifier, "p"),
            }),

            // F → "q" | D "r"
            new ProductionRule("F", new Symbol[]
            {
                new Terminal(TokenType.Identifier, "q"),
            }),
            new ProductionRule("F", new Symbol[]
            {
                new NonTerminal("D"),
                new Terminal(TokenType.Identifier, "r"),
            }),

            // G → "s" | F "t" | E "u"
            new ProductionRule("G", new Symbol[]
            {
                new Terminal(TokenType.Identifier, "s"),
            }),
            new ProductionRule("G", new Symbol[]
            {
                new NonTerminal("F"),
                new Terminal(TokenType.Identifier, "t"),
            }),
            new ProductionRule("G", new Symbol[]
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
