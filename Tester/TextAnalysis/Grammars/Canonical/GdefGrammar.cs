using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Grammars;

public class GdefGrammar : LR1Grammar
{
    public GdefGrammar() : base(GetStart(), GetProductions())
    {
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("grammar");
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            new ProductionRule(
                "grammar",

                new OptionMacro(
                    new NonTerminal("lexer_settings")
                ),
                new NonTerminal("production_list")
            ),

            new ProductionRule(
                "lexer_settings",
                new Terminal(TokenType.Punctuation, "<"),
                new Terminal(TokenType.Identifier, "lexer"),
                new Terminal(TokenType.Punctuation, ">"),

                new RepetitionMacro(
                    new NonTerminal("lexer_statement")
                ),

                new Terminal(TokenType.Punctuation, "<"),
                new Terminal(TokenType.Punctuation, "/"),
                new Terminal(TokenType.Identifier, "lexer"),
                new Terminal(TokenType.Punctuation, ">")
            ),

            new ProductionRule(
                "lexer_statement",
                new Terminal(TokenType.Identifier, "use"),
                new Terminal(TokenType.Identifier),
                new Terminal(TokenType.Punctuation, ";"),
                new AlternativeMacro(),
                new Terminal(TokenType.Identifier, "lexeme"),
                new Terminal(TokenType.Identifier),
                new NonTerminal("regex"),
                new Terminal(TokenType.Punctuation, ";")
            ),

            new ProductionRule(
                "regex",
                new Terminal(TokenType.String)
            ),

            new ProductionRule(
                "production_list",
                new NonTerminal("production"),
                new RepetitionMacro(
                    new NonTerminal("production")
                )
            ),

            new ProductionRule(
                "production",
                new Terminal(TokenType.Identifier),
                new Terminal(TokenType.Punctuation, ":"),
                new NonTerminal("production_body"),
                new Terminal(TokenType.Punctuation, ";")
            ),

            new ProductionRule(
                "production_body",
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new OptionMacro(
                    new NonTerminal("semantic_action")
                )
            ),

            new ProductionRule(
                "symbol",
                new NonTerminal("terminal"),
                new AlternativeMacro(),
                new NonTerminal("non_terminal"),
                new AlternativeMacro(),
                new NonTerminal("macro")
            ),

            new ProductionRule(
                "terminal",
                new Terminal(TokenType.String),
                new AlternativeMacro(),
                new NonTerminal("lexeme"),
                new AlternativeMacro(),
                new NonTerminal("epsilon")
            ),

            new ProductionRule(
                "non_terminal",
                new Terminal(TokenType.Identifier)
            ),

            new ProductionRule(
                "epsilon",
                new Terminal(TokenType.Identifier, "ε")
            ),

            new ProductionRule(
                "macro",
                new NonTerminal("group"),
                new AlternativeMacro(),
                new NonTerminal("option"),
                new AlternativeMacro(),
                new NonTerminal("repetition"),
                new AlternativeMacro(),
                new NonTerminal("alternative")
            ),

            new ProductionRule(
                "group",
                new Terminal(TokenType.Punctuation, "("),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(TokenType.Punctuation, ")")
            ),

            new ProductionRule(
                "option",
                new Terminal(TokenType.Punctuation, "["),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(TokenType.Punctuation, "]")
            ),

            new ProductionRule(
                "repetition",
                new Terminal(TokenType.Punctuation, "{"),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(TokenType.Punctuation, "}")
            ),

            new ProductionRule(
                "alternative",
                new Terminal(TokenType.Punctuation, "|")
            ),

            new ProductionRule(
                "lexeme",
                new Terminal(TokenType.Punctuation, "$"),
                new Terminal(TokenType.Identifier)
            ),

            new ProductionRule(
                "semantic_action",
                new Terminal(TokenType.Punctuation, "{"),
                new Terminal(TokenType.Punctuation, "$"),
                new NonTerminal("semantic_value"),
                new Terminal(TokenType.Punctuation, "}")
            ),

            new ProductionRule(
                "semantic_value",
                new Terminal(TokenType.Punctuation, "$"),
                new Terminal(TokenType.Integer)
            )
        };
    }
}