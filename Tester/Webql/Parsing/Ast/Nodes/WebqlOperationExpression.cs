using Webql.Core;
using Webql.Semantics.Attributes;

namespace Webql.Parsing.Ast;

public class WebqlOperationExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlOperatorType Operator { get; }
    public WebqlExpression[] Operands { get; }

    public WebqlOperationExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        WebqlOperatorType @operator, 
        IEnumerable<WebqlExpression> operands)
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.Operation; 
        Operator = @operator;
        Operands = operands.ToArray();

        foreach (var operand in Operands)
        {
            operand.Parent = this;
        }
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        return Operands;
    }

    public override string ToString()
    {
        var operatorStr = OperatorHelper.ToCamelCase(Operator);
        var operandsStr = string.Join(", ", Operands.Select(x => x.ToString()));

        return $"${operatorStr}: [{operandsStr}]";
    }
}
