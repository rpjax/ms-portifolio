using System.Linq.Expressions;

namespace Webql.Parsing.Ast;

public class WebqlLiteralExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlLiteralType LiteralType { get; }
    public string Value { get; }

    public WebqlLiteralExpression(
        WebqlSyntaxNodeMetadata metadata, 
        Dictionary<string, object>? attributes,
        WebqlLiteralType literalType, 
        string value) 
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.Literal;
        LiteralType = literalType;
        Value = value;
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        return Enumerable.Empty<WebqlSyntaxNode>();
    }

    public override string ToString()
    {
        return Value;
    }

}

