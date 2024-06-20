using ModularSystem.Core.TextAnalysis.Parsing.Components;
using Webql.Components;
using Webql.Parsing.Components;

namespace Webql.Semantics.Components;

public class SemanticException : WebqlCompilationException
{
    public SemanticException(
        string message, 
        SyntaxElementPosition? position) 
        : base(message, position)
    {
    }

    public SemanticException(
        string message, 
        WebqlSyntaxNode node)
        : base(message, node.Metadata.StartPosition)
    { 
    }

    public override string ToString()
    {
        return $"Webql Semantic Error: {base.ToString()}";
    }
}
