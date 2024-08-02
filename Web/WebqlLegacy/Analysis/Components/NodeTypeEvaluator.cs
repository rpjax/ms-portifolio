namespace Aidan.Webql.Analysis;

public class NodeTypeEvaluator : SemanticsVisitor
{
    //*
    // { $filter: { } } => queryable type
    // { $project: { } }
    //*

    public Type Evaluate(SemanticContextOld context, Node node)
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

    protected Type EvaluateLiteral(SemanticContextOld context, LiteralNode node)
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

    protected Type EvaluateReference(SemanticContextOld context, LiteralNode node)
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

    protected Type EvaluateObject(SemanticContextOld context, ObjectNode node)
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
    protected Type EvaluateExpression(SemanticContextOld context, ExpressionNode node)
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
            case OperatorOld.Add:
            case OperatorOld.Subtract:
            case OperatorOld.Divide:
            case OperatorOld.Multiply:
            case OperatorOld.Modulo:
                return typeof(decimal);

            case OperatorOld.Equals:
            case OperatorOld.NotEquals:
            case OperatorOld.Less:
            case OperatorOld.LessEquals:
            case OperatorOld.Greater:
            case OperatorOld.GreaterEquals:
            case OperatorOld.Or:
            case OperatorOld.And:
            case OperatorOld.Not:
                return typeof(bool);

            case OperatorOld.Expr:
                return Evaluate(context, rhs);

            case OperatorOld.Literal:
                return context.Type;

            case OperatorOld.Select:
            case OperatorOld.Filter:
            case OperatorOld.Project:
            case OperatorOld.Limit:
            case OperatorOld.Skip:
                return context.Type;

            case OperatorOld.Count:
                return typeof(int);

            case OperatorOld.Index:
                return context.GetElementType()!;

            case OperatorOld.Any:
            case OperatorOld.All:
                return typeof(bool);

            case OperatorOld.Min:
            case OperatorOld.Max:
            case OperatorOld.Sum:
            case OperatorOld.Average:
                return typeof(void);

            case OperatorOld.Like:
            case OperatorOld.RegexMatch:
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
    protected Type EvaluateMemberAccess(SemanticContextOld context, ExpressionNode node)
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


