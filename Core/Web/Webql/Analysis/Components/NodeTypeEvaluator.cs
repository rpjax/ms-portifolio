namespace ModularSystem.Webql.Analysis;

public class NodeTypeEvaluator : SemanticsVisitor
{
    //*
    // { $filter: { } } => queryable type
    // { $project: { } }
    //*

    public Type Evaluate(SemanticContext context, Node node)
    {
        if (node is LiteralNode literal)
        {
            return EvaluateLiteral(context, literal);
        }
        if (node is ObjectNode objectNode)
        {
            return EvaluateObject(context, objectNode);
        }
        if (node is ExpressionNode expression)
        {
            return EvaluateExpression(context, expression);
        }

        throw new Exception();
    }

    protected Type EvaluateLiteral(SemanticContext context, LiteralNode node)
    {
        var type = context.Type;

        if (node.Value == null)
        {
            return typeof(Nullable);
        }

        if (node.IsOperator)
        {
            return EvaluateReference(context, node);
        }

        return type;
    }

    protected Type EvaluateReference(SemanticContext context, LiteralNode node)
    {
        var propPath = node.Value;

        if (propPath == null)
        {
            throw new Exception();
        }
        if (propPath.Length == 0)
        {
            throw new Exception();
        }
        if (propPath == "$")
        {
            return context.Type;
        }
        if (propPath.StartsWith('"') && propPath.EndsWith('"'))
        {
            propPath = propPath[2..^1];
        }

        var pathSplit = propPath.Split('.');
        var rootPropertyName = propPath.Contains('.')
            ? pathSplit.First()
            : propPath;

        var subContext = context.CreateSubContext(rootPropertyName, rootPropertyName);

        for (int i = 1; i < pathSplit.Length; i++)
        {
            subContext = subContext.CreateSubContext(pathSplit[i], pathSplit[i], false);
        }

        return subContext.Type;
    }

    protected Type EvaluateObject(SemanticContext context, ObjectNode node)
    {
        var type = null as Type;

        foreach (var item in node.Expressions)
        {
            type = Evaluate(context, item);
            context = new SemanticContext(type, context, $".{item.Lhs.Value}");
        }

        if (type == null)
        {
            type = context.Type;
        }

        return type;
    }

    /// <summary>
    /// Parses an expression node to a corresponding Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The expression node to parse.</param>
    /// <returns>An Expression representing the expression node.</returns>
    protected Type EvaluateExpression(SemanticContext context, ExpressionNode node)
    {
        var lhs = node.Lhs.Value;
        var rhs = node.Rhs.Value;
        var isOperator = node.Lhs.IsOperator;

        if (!isOperator)
        {
            return EvaluateMemberAccess(context, node);
        }

        switch (HelperTools.ParseOperatorString(lhs))
        {
            case OperatorV2.Add:
            case OperatorV2.Subtract:
            case OperatorV2.Divide:
            case OperatorV2.Multiply:
            case OperatorV2.Modulo:
                return typeof(decimal);

            case OperatorV2.Equals:
            case OperatorV2.NotEquals:
            case OperatorV2.Less:
            case OperatorV2.LessEquals:
            case OperatorV2.Greater:
            case OperatorV2.GreaterEquals:
            case OperatorV2.Or:
            case OperatorV2.And:
            case OperatorV2.Not:
                return typeof(bool);

            case OperatorV2.Expr:
                return Evaluate(context, rhs);

            case OperatorV2.Literal:
                return context.Type;

            case OperatorV2.Select:
            case OperatorV2.Filter:
            case OperatorV2.Project:
            case OperatorV2.Limit:
            case OperatorV2.Skip:
                return context.Type;

            case OperatorV2.Count:
                return typeof(int);

            case OperatorV2.Index:
                return context.GetQueryableType()!;

            case OperatorV2.Any:
            case OperatorV2.All:
                return typeof(bool);

            case OperatorV2.Min:
            case OperatorV2.Max:
            case OperatorV2.Sum:
            case OperatorV2.Average:
                return typeof(void);
        }

        throw new Exception();
    }

    /// <summary>
    /// Parses member access within an expression node.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The expression node representing member access.</param>
    /// <returns>An Expression representing the member access.</returns>
    protected Type EvaluateMemberAccess(SemanticContext context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;

        if (context.IsQueryable())
        {
            context = new SemanticContext(context.GetQueryableType()!, context, context.Stack);
        }

        context = context.CreateSubContext(memberName, $".{memberName}");

        return Evaluate(context, node.Rhs.Value);
    }

}


