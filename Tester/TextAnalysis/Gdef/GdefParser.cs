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

        /*
         * symbols
         */
        "terminal",
        "non_terminal",
        "macro",

        // terminal constructs
        "lexeme",
        "epsilon",

        // non-terminal constructs
        "grouping",
        "option",
        "repetition",
        "alternative",

        // semantic stuff, not working right now
        "semantic_action",
        "semantic_value",
        "semantic_statement",
        "reduction",
        "assignment",
        "expression",
        "literal",
        "reference",
        "index_expression",
        "function_call",
        "parameter"
    };

    /// <summary>
    /// Parses a gdef file and returns a CST.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static CstRoot Parse(string text)
    {
        return Parser.Parse(text);
    }

    /// <summary>
    /// Parses a gdef file and returns a grammar object.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Grammar ParseGrammar(string text)
    {
        var cst = Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.ReduceCst();

        return GdefTranslator.TranslateGrammar(reducedCst);
    }

}
