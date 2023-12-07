using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

public class OperatorTranslator
{
    protected TranslatorOptions Options { get; }
    protected NodeTranslator NodeParser { get; }
    protected ArithmeticOperatorsTranslator ArithmeticOperatorsTranslator { get; }
    protected RelationalOperatorsTranslator RelationalOperatorsTranslator { get; }
    protected LogicalOperatorsTranslator LogicalOperatorTranslator { get; }
    protected SemanticOperatorsTranslator SemanticOperatorsTranslator { get; }
    protected QueryableOperatorsTranslator QueryableOperatorsParser { get; }

    public OperatorTranslator(TranslatorOptions options, NodeTranslator nodeParser)
    {
        Options = options;
        NodeParser = nodeParser;
        ArithmeticOperatorsTranslator = new(options, NodeParser);
        RelationalOperatorsTranslator = new(options, NodeParser);
        LogicalOperatorTranslator = new(options, NodeParser);
        SemanticOperatorsTranslator = new(options, NodeParser);
        QueryableOperatorsParser = new(options, NodeParser);
    }

    public Expression Translate(Context context, OperatorV2 @operator, Node node)
    {
        switch (@operator)
        {
            // Arithmetic Operators
            case OperatorV2.Add:
                return ArithmeticOperatorsTranslator.TranslateAdd(context, node);

            case OperatorV2.Subtract:
                return ArithmeticOperatorsTranslator.TranslateSubtract(context, node);

            case OperatorV2.Divide:
                return ArithmeticOperatorsTranslator.TranslateDivide(context, node);

            case OperatorV2.Multiply:
                return ArithmeticOperatorsTranslator.TranslateMultiply(context, node);

            case OperatorV2.Modulo:
                return ArithmeticOperatorsTranslator.TranslateModulo(context, node);

            // Relational Operators
            case OperatorV2.Equals:
                return RelationalOperatorsTranslator.TranslateEquals(context, node);

            case OperatorV2.NotEquals:
                return RelationalOperatorsTranslator.TranslateEquals(context, node);

            case OperatorV2.Less:
                return RelationalOperatorsTranslator.TranslateLess(context, node);

            case OperatorV2.LessEquals:
                return RelationalOperatorsTranslator.TranslateLessEquals(context, node);

            case OperatorV2.Greater:
                return RelationalOperatorsTranslator.TranslateGreater(context, node);

            case OperatorV2.GreaterEquals:
                return RelationalOperatorsTranslator.TranslateGreaterEquals(context, node);

            // Logical Operators
            case OperatorV2.Or:
                return LogicalOperatorTranslator.TranslateOr(context, node);

            case OperatorV2.And:
                return LogicalOperatorTranslator.TranslateAnd(context, node);

            case OperatorV2.Not:
                return LogicalOperatorTranslator.TranslateNot(context, node);

            // Semantic Operators
            case OperatorV2.Expr:
                break;

            case OperatorV2.Literal:
                return SemanticOperatorsTranslator.TranslateLiteral(context, node);

            case OperatorV2.Select:
                return SemanticOperatorsTranslator.TranslateSelect(context, node);

            // Queryable Operators
            case OperatorV2.Filter:
                return QueryableOperatorsParser.TranslateFilter(context, node);

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

}

public class ArithmeticOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public ArithmeticOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateAdd(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Add(elementOne, elementTwo);
        }

        return Expression.Add(context.InputExpression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateSubtract(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Subtract(elementOne, elementTwo);
        }

        return Expression.Subtract(context.InputExpression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateDivide(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Divide(elementOne, elementTwo);
        }

        return Expression.Divide(context.InputExpression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateMultiply(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Multiply(elementOne, elementTwo);
        }

        return Expression.Multiply(context.InputExpression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateModulo(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Modulo(elementOne, elementTwo);
        }

        return Expression.Modulo(context.InputExpression, NodeTranslator.Translate(context, node));
    }

}

public class RelationalOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public RelationalOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeParser)
    {
        Options = options;
        NodeTranslator = nodeParser;
    }

    public Expression TranslateEquals(Context context, Node node)
    {
        return Expression.Equal(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression TranslateNotEquals(Context context, Node node)
    {
        return Expression.NotEqual(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression TranslateLess(Context context, Node node)
    {
        return Expression.LessThan(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression TranslateLessEquals(Context context, Node node)
    {
        return Expression.LessThanOrEqual(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression TranslateGreater(Context context, Node node)
    {
        return Expression.GreaterThan(GetLeftSide(context, node), GetRightSide(context, node));
    }

    public Expression TranslateGreaterEquals(Context context, Node node)
    {
        return Expression.GreaterThanOrEqual(GetLeftSide(context, node), GetRightSide(context, node));
    }

    private Expression GetLeftSide(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
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

            return NodeTranslator.Translate(context, refNode);
        }

        return context.InputExpression;
    }

    private Expression GetRightSide(Context context, Node node)
    {
        if (node is ArrayNode arrayNode)
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

            var elementOne = NodeTranslator.Translate(context, refNode);
            var elementTwoContext = new Context(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, valueNode);

            return elementTwo;
        }

        return NodeTranslator.Translate(context, node);
    }


}

public class LogicalOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public LogicalOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateOr(Context context, Node node)
    {
        if (node is not ArrayNode array)
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
            return NodeTranslator.Translate(context, objects[0]);
        }

        var expression = null as Expression;

        foreach (var item in objects)
        {
            if (expression == null)
            {
                expression = NodeTranslator.Translate(context, item);
                continue;
            }

            expression = Expression.OrElse(expression, NodeTranslator.Translate(context, item));
        }

        if (expression == null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public Expression TranslateAnd(Context context, Node node)
    {
        if (node is not ArrayNode array)
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
            return NodeTranslator.Translate(context, objects[0]);
        }

        var expression = null as Expression;

        foreach (var item in objects)
        {
            if (expression == null)
            {
                expression = NodeTranslator.Translate(context, item);
                continue;
            }

            expression = Expression.AndAlso(expression, NodeTranslator.Translate(context, item));
        }

        if (expression == null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public Expression TranslateNot(Context context, Node node)
    {
        var expression = NodeTranslator.Translate(context, node);

        if (expression.Type != typeof(bool))
        {
            throw new Exception();
        }

        return Expression.Not(expression);
    }

}

public class SemanticOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public SemanticOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateLiteral(Context context, Node node)
    {
        var type = context.InputType;
        var value = JsonSerializerSingleton.Deserialize(node.ToString(), type);

        return Expression.Constant(value, type);
    }

    public Expression TranslateSelect(Context context, Node node)
    {
        if (node is not LiteralNode literal)
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

        return NodeTranslator.Translate(context, literal);
    }

}

public class QueryableOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public QueryableOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateFilter(Context context, Node node)
    {
        return Options.LinqProvider.TranslateWhereOperator(context, NodeTranslator, node);
    }

    public Expression ParseProject(Context context, Node node)
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
        if (node is not ObjectNode objectNode)
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
            var propertyExpression = NodeTranslator.Translate(subContext, projectionExpression.Rhs.Value);

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

    public Expression ParseLimit(Context context, Node node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node is not LiteralNode literalNode)
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

        var methodInfo = Options.TakeProvider
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(methodInfo, context.InputExpression, valueExpression);
    }

    public Expression ParseSkip(Context context, Node node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node is not LiteralNode literalNode)
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

    public Expression ParseCount(Context context, Node node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (node is not LiteralNode literalNode)
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

    public Expression ParseAny(Context context, Node node)
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
        var lambdaBody = NodeTranslator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var args = new Expression[]
        {
            context.InputExpression,
            lambda
        };
        var typeArgs = new Type[] { subContextType };

        return Expression.Call(typeof(Enumerable), "Any", typeArgs, args);
    }

    public Expression ParseAll(Context context, Node node)
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
        var lambdaBody = NodeTranslator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var args = new Expression[]
        {
            context.InputExpression,
            lambda
        };
        var typeArgs = new Type[] { subContextType };

        return Expression.Call(typeof(Enumerable), "All", typeArgs, args);
    }

    public Expression ParseMin(Context context, Node node)
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
        var lambdaBody = NodeTranslator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var methodInfo = Options.MaxProvider.MakeGenericMethod(subContextType, lambdaBody.Type);

        if (methodInfo == null)
        {
            throw new InvalidOperationException();
        }

        var methodArgs = new Expression[] { context.InputExpression, lambda };

        return Expression.Call(null, methodInfo, methodArgs);
    }

}
