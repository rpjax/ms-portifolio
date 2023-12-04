using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql;

public class NodeParser
{
    private GeneratorOptions Options { get; }
    private ExpressionNodeParser ExpressionNodeParser { get; }

    public NodeParser(GeneratorOptions options)
    {
        Options = options;
        ExpressionNodeParser = new(this, options);
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

        var rootPropertyInfo = context.GetPropertyInfo(rootPropertyName);

        if (rootPropertyInfo == null)
        {
            throw new Exception();
        }

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

    public ExpressionNodeParser(NodeParser nodeParser, GeneratorOptions options)
    {
        NodeParser = nodeParser;
        Options = options;
    }

    public Expression Parse(Context context, ExpressionNode node)
    {
        var lhsValue = node.Lhs.Value;
        var isOperator = node.Lhs.IsOperator;

        if (isOperator)
        {
            if (lhsValue == "$filter")
            {
                return ParseFilter(context, node);
            }
            if (lhsValue == "$project")
            {
                return ParseProject(context, node);
            }
            if (lhsValue == "$equals")
            {
                return ParseEquals(context, node);
            }
            if (lhsValue == "$notEquals")
            {
                return ParseNotEquals(context, node);
            }
            if (lhsValue == "$or")
            {
                return ParseOr(context, node);
            }
            if (lhsValue == "$and")
            {
                return ParseAnd(context, node);
            }
            if (lhsValue == "$any")
            {
                return ParseAny(context, node);
            }
            if (lhsValue == "$all")
            {
                return ParseAll(context, node);
            }
            if (lhsValue == "$literal")
            {
                return ParseLiteral(context, node);
            }
            if (lhsValue == "$select")
            {
                return ParseSelect(context, node);
            }

            throw new Exception("");
        }
        else
        {
            return ParseMemberAccess(context, node);
        }

        throw new Exception();
    }

    private Expression ParseMemberAccess(Context context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.AccessProperty(memberName);

        return NodeParser.Parse(subContext, node.Rhs.Value);
    }

    private Expression ParseFilter(Context context, ExpressionNode node)
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

    private Expression ParseProject(Context context, ExpressionNode node)
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

        var callExpression = Expression.Call(selectMethod, context.InputExpression, lambda);

        return callExpression;
    }

    private Expression ParseEquals(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            // todo resolve the syntax where the comparison occurs
            // using the array elements 0 and 1, such as: [0] == [1]
        }

        if (context.InputExpression == null)
        {
            throw new Exception();
        }

        return Expression.Equal(context.InputExpression, NodeParser.Parse(context, rhs));
    }

    private Expression ParseNotEquals(Context context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is ArrayNode arrayNode)
        {
            // todo resolve the syntax where the comparison occurs
            // using the array elements 0 and 1, such as: [0] == [1]
        }

        if (context.InputExpression == null)
        {
            throw new Exception();
        }

        return Expression.NotEqual(context.InputExpression, NodeParser.Parse(context, rhs));
    }

    private Expression ParseOr(Context context, ExpressionNode node)
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

    private Expression ParseAnd(Context context, ExpressionNode node)
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

    private Expression ParseAny(Context context, ExpressionNode node)
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

    private Expression ParseAll(Context context, ExpressionNode node)
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

    private Expression ParseLiteral(Context context, ExpressionNode node)
    {
        var type = context.InputType;
        var value = JsonSerializerSingleton.Deserialize(node.Rhs.ToString(), type);

        return Expression.Constant(value, type);
    }

    private Expression ParseSelect(Context context, ExpressionNode node)
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
