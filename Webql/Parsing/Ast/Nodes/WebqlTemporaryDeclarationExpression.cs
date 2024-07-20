namespace Webql.Parsing.Ast;

public class WebqlTemporaryDeclarationExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public string? Type { get; }
    public WebqlExpression Value { get; }
    public WebqlExpression Expression { get; }

    public WebqlTemporaryDeclarationExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        string identifier, 
        string? type, 
        WebqlExpression value,
        WebqlExpression expression)
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.TemporaryDeclaration;
        Identifier = identifier;
        Type = type;
        Value = value;
        Expression = expression;

        Value.Parent = this;
        Expression.Parent = this;
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        if(Value is not null)
        {
            yield return Value;
        }

        yield return Expression;
    }

    public override string ToString()
    {
        return $"[{Type} {Identifier} = {Value}] {Expression}";
    }

}
