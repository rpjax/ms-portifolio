using ModularSystem.Webql.Synthesis;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Analysis;

public class NodeTypeEvaluator : SemanticsVisitor
{
    public virtual Type Evaluate(SemanticContext context, Node node)
    {
        if (node is LiteralNode literalNode)
        {
            return Evaluate(context, literalNode);
        }
        if (node is ObjectNode objectNode)
        {
            return Evaluate(context, objectNode);
        }
        if (node is ArrayNode arrayNode)
        {
            return Evaluate(context, arrayNode);
        }
        if (node is ExpressionNode expression)
        {
            return Evaluate(context, expression);
        }

        throw new Exception();
    }

    protected Type Evaluate(SemanticContext context, LiteralNode node)
    {
        if(node.Value == null)
        {
            return context.Type;
        }
        if (node.Value.StartsWith('$'))
        {
            return EvaluateLiteralReference(context, node);
        }
    }

    protected Type EvaluateLiteralReference(SemanticContext context, LiteralNode node)
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

        var subContext = context.CreateSubContext(rootPropertyName, $".{rootPropertyName}");

        for (int i = 1; i < pathSplit.Length; i++)
        {
            subContext = subContext.CreateSubContext(pathSplit[i], $".{pathSplit[i]}");
        }

        return subContext.Type;
    }

    protected Type Evaluate(SemanticContext context, ObjectNode node)
    {
        var evaluatedType = context.Type;
        var subContext = context;
        
        foreach (var item in node)
        {
            evaluatedType = Evaluate(subContext, item);
            subContext = new SemanticContext(evaluatedType, subContext, $".{item.Lhs.Value}");
        }

        return evaluatedType;
    }

    protected Type Evaluate(SemanticContext context, ArrayNode node)
    {

    }

    protected Type Evaluate(SemanticContext context, ExpressionNode node)
    {

    }

    protected Expression ParseMemberAccess(Context context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.AccessProperty(memberName);

        return Translate(subContext, node.Rhs.Value);
    }
}