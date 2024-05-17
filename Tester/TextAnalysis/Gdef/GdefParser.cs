using ModularSystem.Core.TextAnalysis.Grammars;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace ModularSystem.Core.TextAnalysis.Gdef;

/// <summary>
/// Reconizes and parses gdef files. (Grammar Definition Format)
/// </summary>
public static class GdefParser
{
    private static GdefGrammar Grammar { get; } = new GdefGrammar();
    private static LL1Parser Parser { get; } = new LL1Parser(Grammar.ToLL1());

    public static CstNode Parse(string text)
    {
        return Parser.Parse(text);
    }
}

public class GdefCstReducer
{
    private CstNode Root { get; }

    public GdefCstReducer(CstNode root)
    {
        Root = root;
    }

    public void Reduce()
    {
        
    }
}
