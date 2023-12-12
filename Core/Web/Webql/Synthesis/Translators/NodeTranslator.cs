using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// A central component for translating WebQL nodes into corresponding LINQ expressions. This class handles <br/>
/// the conversion of different types of nodes, such as literal, object, and expression nodes, into expressions <br/>
/// that can be executed in a .NET environment.
/// </summary>
public class NodeTranslator
{
    /// <summary>
    /// Provides access to translation options and configurations.
    /// </summary>
    public TranslatorOptions Options { get; }

    /// <summary>
    /// Manages the translation of individual operators within nodes.
    /// </summary>
    private OperatorTranslator OperatorTranslator { get; }

    /// <summary>
    /// Initializes a new instance of the NodeTranslator class with specified translation options.
    /// </summary>
    /// <param name="options">Configuration options for the translator.</param>
    public NodeTranslator(TranslatorOptions options)
    {
        Options = options;
        OperatorTranslator = new OperatorTranslator(options, this);
    }

    /// <summary>
    /// Translates a WebQL node into a LINQ Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The WebQL node to be translated.</param>
    /// <returns>The LINQ Expression equivalent of the given node.</returns>
    /// <exception cref="Exception">Thrown when the node type is unrecognized.</exception>
    public Expression Translate(TranslationContext context, Node node)
    {
        if (node is LiteralNode literal)
        {
            return ParseLiteral(context, literal);
        }
        if (node is ObjectNode objectNode)
        {
            return ParseObject(context, objectNode);
        }
        if (node is ExpressionNode expression)
        {
            return ParseExpression(context, expression);
        }

        throw new InvalidOperationException($"Unsupported node type encountered: {node.GetType()}. NodeTranslator can only handle LiteralNode, ObjectNode, and ExpressionNode types.");
    }

    /// <summary>
    /// Parses a literal reference within a WebQL node.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The literal node to parse.</param>
    /// <returns>An Expression representing the literal reference.</returns>
    /// <exception cref="Exception">Thrown if the literal reference is invalid.</exception>
    protected Expression ParseLiteralReference(TranslationContext context, LiteralNode node)
    {
        var propPath = node.Value;

        if (propPath == null)
        {
            throw new ArgumentNullException(nameof(node.Value), "Literal node value cannot be null.");
        }
        if (propPath.Length <= 0)
        {
            throw new ArgumentException("Literal node value cannot be an empty string.", nameof(node.Value));
        }
        if (propPath.StartsWith('"') && propPath.EndsWith('"'))
        {
            propPath = propPath[2..^1];
        }

        if (propPath == "$")
        {
            return context.Expression;
        }

        var pathSplit = propPath.Split('.');
        var rootPropertyName = propPath.Contains('.')
            ? pathSplit.First()
            : propPath;

        var subContext = context.CreateSubTranslationContext(rootPropertyName);

        for (int i = 1; i < pathSplit.Length; i++)
        {
            subContext = subContext.CreateSubTranslationContext(pathSplit[i], false);
        }

        return subContext.Expression;
    }

    /// <summary>
    /// Parses a literal node to a corresponding Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The literal node to parse.</param>
    /// <returns>An Expression representing the literal.</returns>
    protected Expression ParseLiteral(TranslationContext context, LiteralNode node)
    {
        var type = context.Type;

        if (node.Value == null)
        {
            return Expression.Constant(null, type);
        }

        if (node.IsReference)
        {
            return ParseLiteralReference(context, node);
        }

        var value = JsonSerializerSingleton.Deserialize(node.Value, type);

        return Expression.Constant(value, type);
    }

    /// <summary>
    /// Parses an object node to a corresponding Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The object node to parse.</param>
    /// <returns>An Expression representing the object.</returns>
    protected Expression ParseObject(TranslationContext context, ObjectNode node)
    {
        var expression = null as Expression;

        foreach (var item in node.Expressions)
        {
            expression = ParseExpression(context, item);
            var resolvedType = expression.Type;
            context = new TranslationContext(resolvedType, expression, context);
        }

        if (expression == null)
        {
            throw TranslationThrowHelper.ErrorInternalUnknown(context, "Could not evaluate the query.");
        }

        return expression;
    }

    /// <summary>
    /// Parses an expression node to a corresponding Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The expression node to parse.</param>
    /// <returns>An Expression representing the expression node.</returns>
    protected Expression ParseExpression(TranslationContext context, ExpressionNode node)
    {
        var lhs = node.Lhs.Value;
        var rhs = node.Rhs.Value;
        var isOperator = node.Lhs.IsOperator;

        if(rhs is ObjectNode objectNode && objectNode.IsEmpty())
        {
            return context.Expression;
        }

        if (!isOperator)
        {
            return ParseMemberAccess(context, node);
        }

        return OperatorTranslator.Translate(context, HelperTools.ParseOperatorString(lhs), rhs);
    }

    /// <summary>
    /// Parses member access within an expression node.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The expression node representing member access.</param>
    /// <returns>An Expression representing the member access.</returns>
    protected Expression ParseMemberAccess(TranslationContext context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.CreateSubTranslationContext(memberName);

        return Translate(subContext, node.Rhs.Value);
    }

}
