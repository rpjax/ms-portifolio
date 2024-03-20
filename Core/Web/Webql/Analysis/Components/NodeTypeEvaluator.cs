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

        if (node.IsReference)
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

        var subContext = context.GetReferenceContext(rootPropertyName, rootPropertyName);

        for (int i = 1; i < pathSplit.Length; i++)
        {
            subContext = subContext.GetReferenceContext(pathSplit[i], pathSplit[i], false);
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

        switch (ParseOperatorString(context, lhs))
        {
            case Operator.Add:
            case Operator.Subtract:
            case Operator.Divide:
            case Operator.Multiply:
            case Operator.Modulo:
                return typeof(decimal);

            case Operator.Equals:
            case Operator.NotEquals:
            case Operator.Less:
            case Operator.LessEquals:
            case Operator.Greater:
            case Operator.GreaterEquals:
            case Operator.Or:
            case Operator.And:
            case Operator.Not:
                return typeof(bool);

            case Operator.Expr:
                return Evaluate(context, rhs);

            case Operator.Literal:
                return context.Type;

            case Operator.Select:
            case Operator.Filter:
            case Operator.Project:
            case Operator.Limit:
            case Operator.Skip:
                return context.Type;

            case Operator.Count:
                return typeof(int);

            case Operator.Index:
                return context.GetElementType()!;

            case Operator.Any:
            case Operator.All:
                return typeof(bool);

            case Operator.Min:
            case Operator.Max:
            case Operator.Sum:
            case Operator.Average:
                return typeof(void);

            case Operator.Like:
            case Operator.RegexMatch:
                return typeof(bool);
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
            context = new SemanticContext(context.GetElementType()!, context, context.Label);
        }

        context = context.GetReferenceContext(memberName, $".{memberName}");

        return Evaluate(context, node.Rhs.Value);
    }

}


