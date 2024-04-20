using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

public class JsonGrammar : GrammarDefinition
{
    public JsonGrammar() : base(GetProductions(), GetStart())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            // json:
            new ProductionRule(
                "json",
                new NonTerminal("object"),
                new PipeMacro(),
                new NonTerminal("array")
            ),

            // object:
            new ProductionRule(
                "object",
                new Terminal(TokenType.Punctuation, "{"),
                new OptionMacro(
                    new NonTerminal("members")
                ),
                new Terminal(TokenType.Punctuation, "}")
            ),

            // members:
            new ProductionRule(
                "members",
                new NonTerminal("pair"),
                new RepetitionMacro(
                    new Terminal(TokenType.Punctuation, ","),
                    new NonTerminal("pair")
                )
            ),

            // pair:
            new ProductionRule(
                "pair",
                new Terminal(TokenType.String),
                new Terminal(TokenType.Punctuation, ":"),
                new NonTerminal("value")
            ),

            // array:
            new ProductionRule(
                "array",
                new Terminal(TokenType.Punctuation, "["),
                new OptionMacro(
                    new NonTerminal("elements")
                ),
                new Terminal(TokenType.Punctuation, "]")
            ),

            // elements:
            new ProductionRule(
                "elements",
                new NonTerminal("value"),
                new RepetitionMacro(
                    new Terminal(TokenType.Punctuation, ","),
                    new NonTerminal("value")
                )
            ),

            // value:
            new ProductionRule(
                "value",
                new Terminal(TokenType.String),
                new PipeMacro(),
                new NonTerminal("number"),
                new PipeMacro(),
                new NonTerminal("object"),
                new PipeMacro(),
                new NonTerminal("array"),
                new PipeMacro(),
                new Terminal(TokenType.Identifier, "true"),
                new PipeMacro(),
                new Terminal(TokenType.Identifier, "false"),
                new PipeMacro(),
                new Terminal(TokenType.Identifier, "null")
            ),

            // number:
            new ProductionRule(
                "number",
                new Terminal(TokenType.Integer),
                new PipeMacro(),
                new Terminal(TokenType.Float),
                new PipeMacro(),
                new Terminal(TokenType.Hexadecimal)
            )
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("json");
    }
}