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

public class NodeParser
{
    private GeneratorOptions Options { get; }
    private ExpressionNodeParser ExpressionNodeParser { get; }

    public NodeParser(GeneratorOptions options)
    {
        Options = options;
        ExpressionNodeParser = new(options, this);
    }

    protected internal Expression Parse(Context context, Node node)
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
            throw new Exception();
        }

        return expression;
    }

    private Expression ParseExpression(Context context, ExpressionNode node)
    {
        return ExpressionNodeParser.Parse(context, node);
    }
}

public class ExpressionNodeParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }
    private ArithmeticOperatorsParser ArithmeticOperatorsParser { get; }
    private RelationalOperatorsParser RelationalOperatorsParser { get; }
    private LogicalOperatorsParser LogicalOperatorsParser { get; }
    private SemanticOperatorsParser SemanticOperatorsParser { get; }
    private QueryableOperatorsParser QueryableOperatorsParser { get; }

    public ExpressionNodeParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
        ArithmeticOperatorsParser = new(options, NodeParser);
        RelationalOperatorsParser = new(options, NodeParser);
        LogicalOperatorsParser = new(options, NodeParser);
        SemanticOperatorsParser = new(options, NodeParser);
        QueryableOperatorsParser = new(options, NodeParser);
    }

    public Expression Parse(Context context, ExpressionNode node)
    {
        var lhsValue = node.Lhs.Value;
        var isOperator = node.Lhs.IsOperator;

        if (!isOperator)
        {
            return ParseMemberAccess(context, node);
        }

        switch (ParseOperatorString(context, lhsValue))
        {
            // Arithmetic Operators
            case OperatorV2.Add:
                return ArithmeticOperatorsParser.ParseAdd(context, node);

            case OperatorV2.Subtract:
                return ArithmeticOperatorsParser.ParseSubtract(context, node);

            case OperatorV2.Divide:
                return ArithmeticOperatorsParser.ParseDivide(context, node);

            case OperatorV2.Multiply:
                return ArithmeticOperatorsParser.ParseMultiply(context, node);

            case OperatorV2.Modulo:
                return ArithmeticOperatorsParser.ParseModulo(context, node);

            // Relational Operators
            case OperatorV2.Equals:
                return RelationalOperatorsParser.ParseEquals(context, node);

            case OperatorV2.NotEquals:
                return RelationalOperatorsParser.ParseNotEquals(context, node);

            case OperatorV2.Less:
                return RelationalOperatorsParser.ParseLess(context, node);

            case OperatorV2.LessEquals:
                return RelationalOperatorsParser.ParseLessEquals(context, node);

            case OperatorV2.Greater:
                return RelationalOperatorsParser.ParseGreater(context, node);

            case OperatorV2.GreaterEquals:
                return RelationalOperatorsParser.ParseGreaterEquals(context, node);

            // Logical Operators
            case OperatorV2.Or:
                return LogicalOperatorsParser.ParseOr(context, node);

            case OperatorV2.And:
                return LogicalOperatorsParser.ParseAnd(context, node);

            case OperatorV2.Not:
                return LogicalOperatorsParser.ParseNot(context, node);

            // Semantic Operators
            case OperatorV2.Expr:
                break;

            case OperatorV2.Literal:
                return SemanticOperatorsParser.ParseLiteral(context, node);

            case OperatorV2.Select:
                return SemanticOperatorsParser.ParseSelect(context, node);

            // Queryable Operators
            case OperatorV2.Filter:
                return QueryableOperatorsParser.ParseFilter(context, node);

            case OperatorV2.Project:
                return QueryableOperatorsParser.ParseProject(context, node);

            case OperatorV2.Limit:
                return QueryableOperatorsParser.ParseLimit(context, node);

            case OperatorV2.Skip:
                return QueryableOperatorsParser.ParseSkip(context, node);

            case OperatorV2.Count:
                return QueryableOperatorsParser.ParseCount(context, node);

            case OperatorV2.Index:
                break;

            case OperatorV2.Any:
                return QueryableOperatorsParser.ParseAny(context, node);

            case OperatorV2.All:
                return QueryableOperatorsParser.ParseAll(context, node);

            // Aggregation Operators
            case OperatorV2.Min:
                return QueryableOperatorsParser.ParseMin(context, node);

            case OperatorV2.Max:
                break;
            case OperatorV2.Sum:
                break;
            case OperatorV2.Average:
                break;

            default:
                throw new Exception("Unknown or unsupported operator.");
        }

        throw new Exception("Unknown or unsupported operator.");
    }

    private string StringifyOperator(OperatorV2 op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    private OperatorV2 ParseOperatorString(Context context, string value)
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

    private Expression ParseMemberAccess(Context context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.AccessProperty(memberName);

        return NodeParser.Parse(subContext, node.Rhs.Value);
    }

}

public class ArithmeticOperatorsParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public ArithmeticOperatorsParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
    }

    public Expression ParseAdd(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if(arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeParser.Parse(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeParser.Parse(elementTwoContext, arrayNode[1]);

            return Expression.Add(elementOne, elementTwo);
        }

        return Expression.Add(context.InputExpression, NodeParser.Parse(context, rhs));
    }

    public Expression ParseSubtract(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeParser.Parse(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeParser.Parse(elementTwoContext, arrayNode[1]);

            return Expression.Subtract(elementOne, elementTwo);
        }

        return Expression.Subtract(context.InputExpression, NodeParser.Parse(context, rhs));
    }

    public Expression ParseDivide(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeParser.Parse(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeParser.Parse(elementTwoContext, arrayNode[1]);

            return Expression.Divide(elementOne, elementTwo);
        }

        return Expression.Divide(context.InputExpression, NodeParser.Parse(context, rhs));
    }

    public Expression ParseMultiply(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeParser.Parse(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeParser.Parse(elementTwoContext, arrayNode[1]);

            return Expression.Multiply(elementOne, elementTwo);
        }

        return Expression.Multiply(context.InputExpression, NodeParser.Parse(context, rhs));
    }

    public Expression ParseModulo(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeParser.Parse(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeParser.Parse(elementTwoContext, arrayNode[1]);

            return Expression.Modulo(elementOne, elementTwo);
        }

        return Expression.Modulo(context.InputExpression, NodeParser.Parse(context, rhs));
    }
}

public class RelationalOperatorsParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public RelationalOperatorsParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
    }

    public Expression ParseEquals(Context context, ExpressionNode node)
    {
        return Expression.Equal(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression ParseNotEquals(Context context, ExpressionNode node)
    {
        return Expression.NotEqual(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression ParseLess(Context context, ExpressionNode node)
    {
        return Expression.LessThan(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression ParseLessEquals(Context context, ExpressionNode node)
    {
        return Expression.LessThanOrEqual(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression ParseGreater(Context context, ExpressionNode node)
    {
        return Expression.GreaterThan(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression ParseGreaterEquals(Context context, ExpressionNode node)
    {
        return Expression.GreaterThanOrEqual(GetLeftSide(context, node), GetRightSide(context, node));
    }

    private Expression GetLeftSide(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var nodeOne = arrayNode[0];
            var nodeTwo = arrayNode[1];
            var refNode = null as Node;
            var valueNode = null as Node;

            if (refNode == null && nodeOne is LiteralNode nodeOneLiteral)
            {
                if (nodeOneLiteral.IsOperator)
                {
                    refNode = nodeOne;
                    valueNode = nodeTwo;
                }
            }
            if (refNode == null && nodeTwo is LiteralNode nodeTwoLiteral)
            {
                if (nodeTwoLiteral.IsOperator)
                {
                    refNode = nodeTwo;
                    valueNode = nodeOne;
                }
            }

            if (refNode == null)
            {
                throw new Exception();
            }
            if (valueNode == null)
            {
                throw new Exception();
            }

            return NodeParser.Parse(context, refNode);
        }

        return context.InputExpression;
    }

    private Expression GetRightSide(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var nodeOne = arrayNode[0];
            var nodeTwo = arrayNode[1];
            var refNode = null as Node;
            var valueNode = null as Node;

            if (refNode == null && nodeOne is LiteralNode nodeOneLiteral)
            {
                if (nodeOneLiteral.IsOperator)
                {
                    refNode = nodeOne;
                    valueNode = nodeTwo;
                }
            }
            if (refNode == null && nodeTwo is LiteralNode nodeTwoLiteral)
            {
                if (nodeTwoLiteral.IsOperator)
                {
                    refNode = nodeTwo;
                    valueNode = nodeOne;
                }
            }

            if (refNode == null)
            {
                throw new Exception();
            }
            if (valueNode == null)
            {
                throw new Exception();
            }

            var elementOne = NodeParser.Parse(context, refNode);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeParser.Parse(elementTwoContext, valueNode);

            return elementTwo;
        }

        return NodeParser.Parse(context, rhs);
    }


}

public class LogicalOperatorsParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public LogicalOperatorsParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
    }

    public Expression ParseOr(Context context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new Exception();
        }

        var values = array.Values;
        var objects = new List<ObjectNode>();

        for (int i = 0; i < values.Length; i++)
        {
            var item = values[i];

            if (item is not ObjectNode objectNode)
            {
                throw new Exception();
            }

            objects.Add(objectNode);
        }

        if (objects.Count == 0)
        {
            return Expression.Constant(true, typeof(bool));
        }

        if (objects.Count == 1)
        {
            return NodeParser.Parse(context, objects[0]);
        }

        var expression = null as Expression;

        foreach (var item in objects)
        {
            if (expression == null)
            {
                expression = NodeParser.Parse(context, item);
                continue;
            }

            expression = Expression.OrElse(expression, NodeParser.Parse(context, item));
        }

        if (expression == null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public Expression ParseAnd(Context context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new Exception();
        }

        var values = array.Values;
        var objects = new List<ObjectNode>();

        for (int i = 0; i < values.Length; i++)
        {
            var item = values[i];

            if (item is not ObjectNode objectNode)
            {
                throw new Exception();
            }

            objects.Add(objectNode);
        }

        if (objects.Count == 0)
        {
            return Expression.Constant(true, typeof(bool));
        }

        if (objects.Count == 1)
        {
            return NodeParser.Parse(context, objects[0]);
        }

        var expression = null as Expression;

        foreach (var item in objects)
        {
            if (expression == null)
            {
                expression = NodeParser.Parse(context, item);
                continue;
            }

            expression = Expression.AndAlso(expression, NodeParser.Parse(context, item));
        }

        if (expression == null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public Expression ParseNot(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;
        var expression = NodeParser.Parse(context, rhs);

        if(expression.Type != typeof(bool))
        {
            throw new Exception();
        }

        return Expression.Not(expression);
    }

}

public class SemanticOperatorsParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public SemanticOperatorsParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
    }

    public Expression ParseLiteral(Context context, ExpressionNode node)
    {
        var type = context.InputType;
        var value = JsonSerializerSingleton.Deserialize(node.Rhs.ToString(), type);

        return Expression.Constant(value, type);
    }

    public Expression ParseSelect(Context context, ExpressionNode node)
    {
        if (node.Rhs.Value is not LiteralNode literal)
        {
            throw new Exception();
        }

        if (literal.Value == null)
        {
            throw new Exception();
        }
        if (!literal.IsOperator)
        {
            throw new Exception();
        }

        return NodeParser.Parse(context, literal);
    }

}

public class QueryableOperatorsParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public QueryableOperatorsParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
    }

    public Expression ParseFilter(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception();
        }
        if (context.InputExpression == null)
        {
            throw new Exception();
        }

        var subEntityType = context.GetQueryableType();

        if (subEntityType == null)
        {
            throw new Exception();
        }

        var methodInfo = Options.WhereProvider.MakeGenericMethod(subEntityType);

        if (methodInfo == null)
        {
            throw new InvalidOperationException();
        }

        var subExpressionParameter = Expression.Parameter(subEntityType, "x");
        var subContext = new Context(subEntityType, subExpressionParameter, context);
        var subExpressionBody = NodeParser.Parse(subContext, node.Rhs.Value);
        var subExpression = Expression.Lambda(subExpressionBody, subExpressionParameter);

        var methodArgs = new Expression[] { context.InputExpression, subExpression };
        var callExpression = Expression.Call(null, methodInfo, methodArgs);

        return callExpression;
    }

    public Expression ParseProject(Context context, ExpressionNode node)
    {
        //      call expression (IQueryable<T>.Select()) arguments:
        //          constant expression (IEnumerable<T>)
        //          quoat expression operand:
        //              lambda expression (Func<T, projectedT>) body:
        //                  new expression:
        //                      members: in order, the lhs of the assignments.
        //                      arguments: in order, the rhs of the assignments.

        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node.Rhs.Value is not ObjectNode objectNode)
        {
            throw new Exception("");
        }

        var queryableType = context.GetQueryableType();

        if (queryableType == null)
        {
            throw new Exception();
        }

        var subContextParameter = Expression.Parameter(queryableType, context.CreateParameterName());
        var subContext = new Context(queryableType, subContextParameter, context);

        // Cria uma lista para armazenar as associações de propriedades do tipo projetado
        var propertyBindings = new List<MemberBinding>();

        var anonymousTypeProperties = new List<AnonymousPropertyDefinition>(objectNode.Expressions.Length);
        var propertySelectorExpressions = new List<Expression>(objectNode.Expressions.Length);

        // Itera sobre cada propriedade na expressão de projeção
        foreach (var projectionExpression in objectNode.Expressions)
        {
            // Obtém o nome da propriedade e a expressão associada
            var propertyName = projectionExpression.Lhs.Value;
            var propertyExpression = NodeParser.Parse(subContext, projectionExpression.Rhs.Value);

            anonymousTypeProperties.Add(new(propertyName, propertyExpression.Type));
            propertySelectorExpressions.Add(propertyExpression);
        }

        var typeCreationOptions = new AnonymousTypeCreationOptions()
        {
            CreateDefaultConstructor = true,
            CreateSetters = true
        };
        var projectedType = TypeHelper.CreateAnonymousType(anonymousTypeProperties.ToArray(), typeCreationOptions);

        if (projectedType == null)
        {
            throw new Exception();
        }

        for (int i = 0; i < anonymousTypeProperties.Count; i++)
        {
            var propDefinition = anonymousTypeProperties[i];
            var propertyExpression = propertySelectorExpressions[i];

            var propertyInfo = projectedType.GetProperty(propDefinition.Name);

            if (propertyInfo == null)
            {
                throw new Exception();
            }

            // Cria um binding para a propriedade do novo tipo
            propertyBindings.Add(Expression.Bind(propertyInfo, propertyExpression));
        }

        // Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
        var newExpression = Expression.MemberInit(Expression.New(projectedType), propertyBindings);

        // Cria a expressão lambda 'x => new projectedType { Prop1 = ..., Prop2 = ..., ... }'
        var lambda = Expression.Lambda(newExpression, subContextParameter);

        // Cria a expressão de chamada ao método 'Select'
        var selectMethod = Options.ProjectProvider
            .MakeGenericMethod(new[] { queryableType, projectedType });

        return Expression.Call(selectMethod, context.InputExpression, lambda);
    }

    public Expression ParseLimit(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node.Rhs.Value is not LiteralNode literalNode)
        {
            throw new Exception("");
        }

        var queryableType = context.GetQueryableType();

        if (queryableType == null)
        {
            throw new Exception();
        }

        var valueExpression = null as Expression;

        if (Options.TakeSupportsInt64)
        {
            if(!long.TryParse(literalNode.Value, out long longValue))
            {
                throw new Exception();
            }

            valueExpression = Expression.Constant(longValue, typeof(long));
        }
        else
        {
            if (!int.TryParse(literalNode.Value, out int intValue))
            {
                throw new Exception();
            }

            valueExpression = Expression.Constant(intValue, typeof(int));
        }

        var methodInfo = Options.TakeProvider
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(methodInfo, context.InputExpression, valueExpression);
    }

    public Expression ParseSkip(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node.Rhs.Value is not LiteralNode literalNode)
        {
            throw new Exception("");
        }

        var queryableType = context.GetQueryableType();

        if (queryableType == null)
        {
            throw new Exception();
        }

        var valueExpression = null as Expression;

        if (Options.SkipSupportsInt64)
        {
            if (!long.TryParse(literalNode.Value, out long longValue))
            {
                throw new Exception();
            }

            valueExpression = Expression.Constant(longValue, typeof(long));
        }
        else
        {
            if (!int.TryParse(literalNode.Value, out int intValue))
            {
                throw new Exception();
            }

            valueExpression = Expression.Constant(intValue, typeof(int));
        }

        var methodInfo = Options.SkipProvider
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(methodInfo, context.InputExpression, valueExpression);
    }

    public Expression ParseCount(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node.Rhs.Value is not LiteralNode literalNode)
        {
            throw new Exception("");
        }

        var queryableType = context.GetQueryableType();

        if (queryableType == null)
        {
            throw new Exception();
        }

        var methodInfo = Options.CountProvider
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(methodInfo, context.InputExpression);
    }

    public Expression ParseAny(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception();
        }

        var subContextType = context.GetQueryableType();

        if (subContextType == null)
        {
            throw new Exception();
        }

        var subContextExpression = Expression.Parameter(subContextType, "x");
        var subContext = new Context(subContextType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = NodeParser.Parse(subContext, node.Rhs.Value);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var args = new Expression[]
        {
            context.InputExpression,
            lambda
        };
        var typeArgs = new Type[] { subContextType };

        return Expression.Call(typeof(Enumerable), "Any", typeArgs, args);
    }

    public Expression ParseAll(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception();
        }

        var subContextType = context.GetQueryableType();

        if (subContextType == null)
        {
            throw new Exception();
        }

        var subContextExpression = Expression.Parameter(subContextType, "x");
        var subContext = new Context(subContextType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = NodeParser.Parse(subContext, node.Rhs.Value);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var args = new Expression[]
        {
            context.InputExpression,
            lambda
        };
        var typeArgs = new Type[] { subContextType };

        return Expression.Call(typeof(Enumerable), "All", typeArgs, args);
    }

    public Expression ParseMin(Context context, ExpressionNode node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception();
        }
        if (context.InputExpression == null)
        {
            throw new Exception();
        }

        var subContextType = context.GetQueryableType();

        if (subContextType == null)
        {
            throw new Exception();
        }

        var subContextExpression = Expression.Parameter(subContextType, "x");
        var subContext = new Context(subContextType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = NodeParser.Parse(subContext, node.Rhs.Value);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var methodInfo = Options.MinProvider.MakeGenericMethod(subContextType);

        if (methodInfo == null)
        {
            throw new InvalidOperationException();
        }

        var methodArgs = new Expression[] { context.InputExpression, lambda };

        return Expression.Call(null, methodInfo, methodArgs);
    }

}

public class AggregationOperatorsParser
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public AggregationOperatorsParser(GeneratorOptions options, NodeParser nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
    }

}

