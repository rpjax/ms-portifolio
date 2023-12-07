using ModularSystem.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

public class LinqProvider
{
    public virtual Type GetQueryableType()
    {
        return typeof(IEnumerable<>);
    }

    public virtual Expression TranslateWhereOperator(Context context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
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

        var methodInfo = GetWhereMethodInfo().MakeGenericMethod(subEntityType);

        if (methodInfo == null)
        {
            throw new InvalidOperationException();
        }

        var subExpressionParameter = Expression.Parameter(subEntityType, "x");
        var subContext = new Context(subEntityType, subExpressionParameter, context);
        var subExpressionBody = translator.Translate(subContext, node);
        var subExpression = Expression.Lambda(subExpressionBody, subExpressionParameter);

        var methodArgs = new Expression[] { context.InputExpression, subExpression };

        return Expression.Call(null, methodInfo, methodArgs);
    }

    public virtual Expression TranslateProjectOperator(Context context, NodeTranslator translator, Node node)
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
            var propertyExpression = translator.Translate(subContext, projectionExpression.Rhs.Value);

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
        var selectMethod = GetSelectMethodInfo()
            .MakeGenericMethod(new[] { queryableType, projectedType });

        return Expression.Call(selectMethod, context.InputExpression, lambda);
    }

    private static MethodInfo GetWhereMethodInfo()
    {
        return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
    }

    private static MethodInfo GetSelectMethodInfo()
    {
        return typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                            && x.Args.Length == 2
                            && x.Params[0].ParameterType == typeof(IQueryable<>).MakeGenericType(x.Args[0])
                            && x.Params[1].ParameterType == typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(x.Args[0], x.Args[1])))
                .Select(x => x.Method)
                .Single();
    }

    public MethodInfo TakeProvider { get; set; }
    public MethodInfo SkipProvider { get; set; }
    public MethodInfo CountProvider { get; set; }
    public MethodInfo MinProvider { get; set; }
    public MethodInfo MaxProvider { get; set; }
}
