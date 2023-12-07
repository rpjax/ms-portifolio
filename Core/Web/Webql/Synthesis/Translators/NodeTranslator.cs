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
/// The main translator for webql nodes.
/// </summary>
public class NodeTranslator
{
    private TranslatorOptions Options { get; }
    private OperatorTranslator OperatorTranslator { get; }

    public NodeTranslator(TranslatorOptions options)
    {
        Options = options;
        OperatorTranslator = new (options, this);
    }

    protected internal Expression Translate(Context context, Node node)
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

    protected internal Expression ParseLiteralReference(Context context, LiteralNode node)
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

    private Expression ParseLiteral(Context context, LiteralNode node)
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

    private Expression ParseObject(Context context, ObjectNode node)
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

    private Expression ParseExpression(Context context, ExpressionNode node)
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

    private Expression ParseMemberAccess(Context context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.AccessProperty(memberName);

        return Translate(subContext, node.Rhs.Value);
    }

    private OperatorV2 ParseOperatorString(string value)
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

    private string StringifyOperator(OperatorV2 op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

}
