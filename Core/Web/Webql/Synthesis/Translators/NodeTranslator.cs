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
    public Expression Translate(Context context, Node node)
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

        throw new Exception();
    }

    /// <summary>
    /// Parses a literal reference within a WebQL node.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The literal node to parse.</param>
    /// <returns>An Expression representing the literal reference.</returns>
    /// <exception cref="Exception">Thrown if the literal reference is invalid.</exception>
    protected Expression ParseLiteralReference(Context context, LiteralNode node)
    {
        var propPath = node.Value;

        if(propPath == null)
        {
            throw new Exception();
        }
        if (propPath.Length == 0)
        {
            throw new Exception();
        }
        if (propPath == "$")
        {
            return context.InputExpression;
        }
        if (propPath.StartsWith('"') && propPath.EndsWith('"'))
        {
            propPath = propPath[2..^1];
        }

        var pathSplit = propPath.Split('.');
        var rootPropertyName = propPath.Contains('.')
            ? pathSplit.First()
            : propPath;

        var subContext = context.AccessProperty(rootPropertyName);

        for (int i = 1; i < pathSplit.Length; i++)
        {
            subContext = subContext.AccessProperty(pathSplit[i], false);
        }

        return subContext.InputExpression;
    }

    /// <summary>
    /// Parses a literal node to a corresponding Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The literal node to parse.</param>
    /// <returns>An Expression representing the literal.</returns>
    protected Expression ParseLiteral(Context context, LiteralNode node)
    {
        var type = context.InputType;

        if (node.Value == null)
        {
            return Expression.Constant(null, type);
        }

        if (node.IsOperator)
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
    protected Expression ParseObject(Context context, ObjectNode node)
    {
        var expression = null as Expression;

        foreach (var item in node.Expressions)
        {
            expression = ParseExpression(context, item);
            var resolvedType = expression.Type;
            context = new Context(resolvedType, expression, context);
        }

        if (expression == null)
        {
             expression = context.InputExpression;
        }

        return expression;
    }

    /// <summary>
    /// Parses an expression node to a corresponding Expression.
    /// </summary>
    /// <param name="context">The current translation context.</param>
    /// <param name="node">The expression node to parse.</param>
    /// <returns>An Expression representing the expression node.</returns>
    protected Expression ParseExpression(Context context, ExpressionNode node)
    {
        var lhs = node.Lhs.Value;
        var rhs = node.Rhs.Value;  
        var isOperator = node.Lhs.IsOperator;

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
    protected Expression ParseMemberAccess(Context context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.AccessProperty(memberName);

        return Translate(subContext, node.Rhs.Value);
    }

}
