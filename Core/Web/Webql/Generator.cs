using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql;

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
//  Array operators:
//      $size: - { "$size: "$arrayField" }.
//      $index:{int} - { "array": { "$index[0]": "foobar" } }.
//      
//  Array iteration operators:
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
// Filter Semantics:
// Arithmetic operators: ($add, $subtract, $divide, $multiply) expect an ARRAY RHS.
// Arithmetic operators: ($modulo) expect an ARRAY RHS.
//
// Projection Pipeline:
//
//*

public class GeneratorOptions
{
    public MethodInfo WhereProvider { get; set; } 
    public MethodInfo ProjectProvider { get; set; }

    public GeneratorOptions()
    {
        WhereProvider = DefaultWhere();
        ProjectProvider = DefaultSelect();
    }

    private static MethodInfo DefaultWhere()
    {
        return typeof(Enumerable)
            .GetMethods()
            .Where(x => x.Name == "Where")
            .Where(x => x.GetParameters().Length == 2)
            .Where(x => x.GetParameters().ElementAt(1).ParameterType.IsGenericType)
            .FirstOrDefault()!;
    }

    private static MethodInfo DefaultSelect()
    {
        return typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Select" && m.IsGenericMethod)!;
    }
}

public class Generator
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public Generator(GeneratorOptions? options = null)
    {
        Options = options ?? new();
        NodeParser = new(Options);
    }

    public Expression CreateExpression(Node node, Type type, ParameterExpression? parameter = null)
    {
        var queryableType = typeof(IEnumerable<>).MakeGenericType(type);
        parameter ??= Expression.Parameter(queryableType, "root");
        var context = new Context(queryableType, parameter);

        return NodeParser.Parse(context, node);
    }

    public TranslatedEnumerable CreateEnumerable(Node node, Type type, IEnumerable queryable)
    {
        var inputType = typeof(IEnumerable<>).MakeGenericType(type);
        var parameter = Expression.Parameter(inputType, "root");
        var context = new Context(inputType, parameter);
        var expression = NodeParser.Parse(context, node);

        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = typeof(IEnumerable<>).MakeGenericType(projectedType);
        var lambdaExpressionType = typeof(Func<,>).MakeGenericType(inputType, outputType);

        var lambdaExpression = Expression.Lambda(lambdaExpressionType, expression, parameter);
        var lambda = lambdaExpression.Compile();

        var transformedQueryable = lambda.DynamicInvoke(queryable);

        if (transformedQueryable == null)
        {
            throw new Exception();
        }

        return new TranslatedEnumerable(inputType.GenericTypeArguments.First(), outputType.GenericTypeArguments.Last(), transformedQueryable);
    }

    public TranslatedEnumerable CreateEnumerable<T>(Node node, IEnumerable<T> queryable)
    {
        return CreateEnumerable(node, typeof(T), queryable);
    }

}
