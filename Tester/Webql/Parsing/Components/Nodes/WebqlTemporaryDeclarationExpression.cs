using Webql.DocumentSyntax.Parsing.Tools;

namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlTemporaryDeclarationExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public string? Type { get; }
    public WebqlExpression Value { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlTemporaryDeclarationExpression(
        string identifier, 
        string? type, 
        WebqlExpression value, 
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes = null)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.TemporaryDeclaration;
        Identifier = identifier;
        Type = type;
        Value = value;
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    public override WebqlSyntaxNode Accept(SyntaxNodeVisitor visitor)
    {
        return visitor.VisitTemporaryDeclarationExpression(this);
    }
}

