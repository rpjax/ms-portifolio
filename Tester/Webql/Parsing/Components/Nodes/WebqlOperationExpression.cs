using Webql.DocumentSyntax.Parsing.Tools;
using Webql.DocumentSyntax.Semantics.Components;

namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlOperationExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlOperatorType Operator { get; }
    public WebqlExpression[] Operands { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlOperationExpression(
        WebqlOperatorType @operator, 
        WebqlExpression[] operands, 
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes = null)
    {
        ExpressionType = WebqlExpressionType.Operation; 
        Operator = @operator;
        Operands = operands;
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();

        Attributes.TryAdd(SemanticsHelper.ScopeSourceAttribute, new object());
    }

    public WebqlOperationExpression(
        WebqlOperatorType @operator, 
        IEnumerable<WebqlExpression> operands, 
        WebqlSyntaxNodeMetadata metadata) 
        : this(@operator, operands.ToArray(), metadata)
    {

    }

    public override WebqlSyntaxNode Accept(SyntaxNodeVisitor visitor)
    {
        return visitor.VisitOperationExpression(this);
    }
}

