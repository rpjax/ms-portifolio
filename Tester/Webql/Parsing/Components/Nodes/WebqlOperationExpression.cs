﻿namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlOperationExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlOperatorType Operator { get; }
    public WebqlExpression[] Operands { get; }

    public WebqlOperationExpression(WebqlOperatorType @operator, params WebqlExpression[] operands)
    {
        ExpressionType = WebqlExpressionType.Operation;
        Operator = @operator;
        Operands = operands;
    }

    public WebqlOperationExpression(WebqlOperatorType @operator, IEnumerable<WebqlExpression> operands) : this(@operator, operands.ToArray())
    {

    }
}

