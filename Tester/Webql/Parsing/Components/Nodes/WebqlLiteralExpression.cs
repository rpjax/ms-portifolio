using Webql.Parsing.Tools;

namespace Webql.Parsing.Components;

public class WebqlLiteralExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlLiteralType LiteralType { get; }
    public string Value { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlLiteralExpression(
        WebqlLiteralType literalType, 
        string value, 
        WebqlSyntaxNodeMetadata metadata, 
        Dictionary<string, object>? attributes = null)
    {
        ExpressionType = WebqlExpressionType.Literal;
        LiteralType = literalType;
        Value = value;
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    public override WebqlSyntaxNode Accept(SyntaxTreeVisitor visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}

