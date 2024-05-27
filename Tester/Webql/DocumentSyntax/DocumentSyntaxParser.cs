using ModularSystem.Core.TextAnalysis.Gdef;
using ModularSystem.Core.TextAnalysis.Parsing;

namespace Webql.DocumentSyntax.Parsing;

public static class DocumentSyntaxParser
{
    const string RawGrammarText = @"

";

    private static LR1Parser? Parser { get; set; }

    public static void Initialize()
    {
        if(Parser is not null)
        {
            return;
        }

        Parser = new LR1Parser(
            grammar: GdefParser.ParseGrammar(RawGrammarText)
        );
    }

}

public abstract class WebqlNode
{

}

public class WebqlDocument : WebqlNode
{
    public List<WebqlNode> Nodes { get; }

    public WebqlDocument(List<WebqlNode> nodes)
    {
        Nodes = nodes;
    }
}
