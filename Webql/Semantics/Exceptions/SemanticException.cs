using ModularSystem.TextAnalysis.Parsing.Components;
using Webql.Exceptions;
using Webql.Parsing.Ast;

namespace Webql.Semantics.Exceptions;

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
        : base(message, node.Metadata.Position)
    { 
    }

    public override string ToString()
    {
        return $"Webql Semantic Error: {base.ToString()}";
    }
}
