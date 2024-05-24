using ModularSystem.Core.TextAnalysis.Grammars;
using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Tools;

namespace ModularSystem.Core.TextAnalysis.Gdef;

/// <summary>
/// Reconizes and parses gdef files. (Grammar Definition Format)
/// </summary>
public static class GdefParser
{
    private static LR1Parser Parser { get; } = new LR1Parser(new GdefGrammar());

    private static string[] ReduceWhitelist { get; } = new string[]
    {
        // high order constructs
        "production",

        // terminal constructs
        //"terminal",
        "lexeme",
        "epsilon",

        // non-terminal constructs
        //"non_terminal",
        "grouping",
        "option",
        "repetition",
        "alternative",
        "semantic_action",
        "semantic_value"
    };

    public static CstRoot Parse(string text)
    {
        return Parser.Parse(text);
    }

    public static Grammar ParseGrammar(string text)
    {
        var cst = Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.ReduceCst();

        return new GdefGrammarBuilder(reducedCst)
            .Build();
    }
}
