using ModularSystem.Core.TextAnalysis.Language;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class MyGrammar : GrammarDefinition
{
    public MyGrammar() : base(productions: GetProductions(), start: GetStart())
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

public class MyComplexGrammar : GrammarDefinition
{
    public MyComplexGrammar() : base(productions: CreateProductions(), start: GetStartSymbol())
    {
    }

    private static ProductionRule[] CreateProductions()
    {
        return new[]
        {
            new ProductionRule("S", new NonTerminal("A"), new Terminal(TokenType.Identifier, "a"), new NonTerminal("X")),
            new ProductionRule("S", new Terminal(TokenType.Identifier, "b"), new NonTerminal("Y"), new Terminal(TokenType.Identifier, "c")),
            new ProductionRule("S", new NonTerminal("D"), new Terminal(TokenType.Identifier, "d"), new NonTerminal("Z")),
            new ProductionRule("A", new NonTerminal("B"), new NonTerminal("C"), new Terminal(TokenType.Identifier, "e")),
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

public class MacroTesterGrammar : GrammarDefinition
{
    public MacroTesterGrammar() : base(GetProductions(), GetStart())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new[]
        {
            new ProductionRule("S", new NonTerminal("A"), new OptionMacro(new Terminal(TokenType.Identifier, "b")), new NonTerminal("C")),
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("S");
    }
}

public class C_Grammar : GrammarDefinition
{
    public C_Grammar() : base(GetProductions(), GetStart())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            // translation-unit:
            new ProductionRule("translation-unit", new NonTerminal("external-declaration")),

            //external-declaration:
            new ProductionRule("external-declaration", new NonTerminal("function-definition")),

            new ProductionRule("external-declaration", new NonTerminal("declaration")),

            //function-definition:
            new ProductionRule("function-definition", new NonTerminal("declaration-specifiers"), new NonTerminal("declarator"), new NonTerminal("declaration-list"), new NonTerminal("compound-statement")),

            new ProductionRule("function-definition", new NonTerminal("declaration-specifiers"), new NonTerminal("declarator"), new NonTerminal("compound-statement")),

            //declaration:
            new ProductionRule("declaration", new NonTerminal("declaration-specifiers"), new NonTerminal("init-declarator-list"), new Terminal(TokenType.Punctuation, ";")),

            new ProductionRule("declaration", new NonTerminal("declaration-specifiers"), new Terminal(TokenType.Punctuation, ";")),

            new ProductionRule("declaration", new NonTerminal("static-assert-declaration")),

            new ProductionRule("declaration", new Terminal(TokenType.Punctuation, ";")),

            //declaration-specifiers:
            new ProductionRule("declaration-specifiers", new NonTerminal("declaration-specifier"), new NonTerminal("declaration-specifiers")),

            new ProductionRule("declaration-specifiers", new Epsilon()),

            //declaration-specifier:
            new ProductionRule("declaration-specifier", new NonTerminal("storage-class-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("type-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("type-qualifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("function-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("alignment-specifier")),

        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("translation-unit");
    }
}
