using Webql.DocumentSyntax.Parsing.Tools;

namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlReferenceExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlReferenceExpression(
        string identifier, 
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes = null)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.Reference;
        Identifier = identifier;
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    public override WebqlSyntaxNode Accept(SyntaxNodeVisitor visitor)
    {
        return visitor.VisitReferenceExpression(this);
    }
}

