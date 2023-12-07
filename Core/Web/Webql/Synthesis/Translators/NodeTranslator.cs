using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

//*
// WebQL Notes:
//
// Pipeline Overview: The root is an object where key-value pairs represent expressions. Each root expression must
// resolve itself to the root queryable context (e.g., IQueryable in C#). This context supports various operations
// such as filtering, ordering, projection, limit, skip, etc.
// Sub-expressions, expressions at the child-to-root level, can resolve themselves
// to different values, such as bool, numbers, strings, etc.
// All expressions ultimately resolve to a value with a known type. The root type corresponds to the query itself.
//
// Filter Pipeline:
//  Arithmetic operators:
//      $add - { "$add": [args...] }.
//      $subtract - { "$subtract": [args...] }.
//      $divide - { "$divide": [args...] }.
//      $multiply - { "$multiply": [args...] }.
//      $modulo - { "$modulo": "$prop" }.
//
//  Relational operators:
//      syntax: <relational-expression> ::= <lhs> : (<literal> | <array> | <expression>)
//
//      $equals - { "prop": { "$equals: "foobar" } }.
//      $notEquals - { "prop": { "$notEquals: "foobar" } }.
//      $greater - { "prop": { "$greater: 5 } }.
//      $greaterEquals - { "prop": { "$greaterEquals: 5 } }.
//      $less - { "prop": { "$less: 5 } }.
//      $lessEquals - { "prop": { "$lessEquals: 5 } }.
//
//  Logical operators:
//      $and - { "$and": [{ }, { }] }.
//      $or - { "$or": [{ }, { }] }.
//      $not: { "$not": { } }.
//
//  String operators:
//      $like
//
//  Queryable operators:
//      $filter
//      $project
//      $limit
//      $skip
//      $size: - { "$size: "$arrayField" }.
//      $index:{int} - { "array": { "$index[0]": "foobar" } }.
//      
//  Queryable iteration operators:
//      $any - { "$any": [{ }, { }] }.
//      $all - { "$any": [{ }, { }] }.
//
//  Semantic operators:
//      $expr - { "$literal": "$text string..." }
//      $literal - { "$literal": "$text string..." }
//      $select - { "$select": "$property" }
//
//  Aggregation operators:    
//      $count
//      $min - { "$min": "$prop" } | { "$min": "$" }.
//      $max.
//      $sum.
//      $average.
//
//
// Filter Semantics:
// Arithmetic operators: ($add, $subtract, $divide, $multiply) expect an ARRAY RHS.
// Arithmetic operators: ($modulo) expect an ARRAY RHS.
//
// Projection Pipeline:
//
//*

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

        return OperatorTranslator.Translate(context, ParseOperatorString(lhs), rhs);
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

    /// <summary>
    /// Converts an <see cref="OperatorV2"/> enum value into a string representation.
    /// </summary>
    /// <param name="op">The OperatorV2 enum value.</param>
    /// <returns>The string representation of the operator.</returns>
    protected string StringifyOperator(OperatorV2 op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    /// <summary>
    /// Converts a string representation of an operator into its corresponding <see cref="OperatorV2"/> enum value.
    /// </summary>
    /// <param name="value">The string representation of the operator.</param>
    /// <returns>The OperatorV2 enum value.</returns>
    /// <exception cref="GeneratorException">Thrown when the operator string is not recognized.</exception>
    protected OperatorV2 ParseOperatorString(string value)
    {
        var operators = Enum.GetValues(typeof(OperatorV2));

        foreach (OperatorV2 op in operators)
        {
            if (StringifyOperator(op) == value)
            {
                return op.TypeCast<OperatorV2>();
            }
        }

        throw new GeneratorException($"The operator '{value}' is not recognized or supported. Please ensure it is a valid operator.", null);
    }

}
