using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Grammars;

public class DebugGrammars
{
    /*
        Reference: https://www.youtube.com/watch?v=s9N3_7ADZno&list=PLBlnK6fEyqRjT3oJxFXRgjPNzeS-LFY-q&index=27
    */
    public static GrammarDefinition CreateCommonFactorGrammar()
    {
        var set = new ProductionSet();
        var startSymbol = new NonTerminal("A");

        set.Add(
            head: "A",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "a"),
                new NonTerminal("A"),
                new NonTerminal("B")
            )
        );

        set.Add(
            head: "A",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "a"),
                new NonTerminal("B"),
                new Terminal(TokenType.Identifier, "c")
            )
        );

        set.Add(
            head: "A",
            body: new Sentence(
                new Terminal(TokenType.Identifier, "a"),
                new NonTerminal("A"),
                new Terminal(TokenType.Identifier, "c")
            )
        );

        return new GrammarDefinition(set, startSymbol);
    }

}
