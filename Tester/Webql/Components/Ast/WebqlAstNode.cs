namespace Webql.Components.Ast;

public abstract class WebqlAstNode
{

}

public class WebqlDocument : WebqlAstNode
{
    public WebqlAstNode[] Nodes { get; }

    public WebqlDocument(IEnumerable<WebqlAstNode> nodes)
    {
        Nodes = nodes.ToArray();
    }
}

public enum WebqlExpressionType
{
    Literal,
    Reference,
    TemporaryDeclaration,
    Block,
    Operation
}

public abstract class WebqlExpression : WebqlAstNode
{
    public abstract WebqlExpressionType ExpressionType { get; }
}

public enum WebqlLiteralType
{
    Bool,
    Null,
    Int,
    Float,
    Hex,
    String
}

public class WebqlLiteralExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlLiteralType LiteralType { get; }
    public string Value { get; }

    public WebqlLiteralExpression(WebqlLiteralType literalType, string value)
    {
        ExpressionType = WebqlExpressionType.Literal;
        LiteralType = literalType;
        Value = value;
    }
}

public class WebqlReferenceExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }

    public WebqlReferenceExpression(string identifier)
    {
        ExpressionType = WebqlExpressionType.Reference;
        Identifier = identifier;
    }
}

public class WebqlTemporaryDeclarationExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public string? Type { get; }
    public WebqlExpression Value { get; }

    public WebqlTemporaryDeclarationExpression(string identifier, string? type, WebqlExpression value)
    {
        ExpressionType = WebqlExpressionType.TemporaryDeclaration;
        Identifier = identifier;
        Type = type;
        Value = value;
    }
}

public class WebqlBlockExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlExpression[] Expressions { get; }

    public WebqlBlockExpression(IEnumerable<WebqlExpression> expressions)
    {
        ExpressionType = WebqlExpressionType.Block;
        Expressions = expressions.ToArray();
    }
}

public enum WebqlOperatorType
{
    Equals,
    NotEquals,
    LessThan,
    LessThanOrEquals,
    GreaterThan,
    GreaterThanOrEquals,
    And,
    Or,
    Not
}

public class WebqlOperationExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlOperatorType Operator { get; }
    public WebqlExpression[] Operands { get; }

    public WebqlOperationExpression(WebqlOperatorType @operator, IEnumerable<WebqlExpression> operands)
    {
        ExpressionType = WebqlExpressionType.Operation;
        Operator = @operator;
        Operands = operands.ToArray();
    }
}
