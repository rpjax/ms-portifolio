using ModularSystem.Core;
using ModularSystem.Webql.Analysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Provides a mechanism for translating WebQL query nodes into LINQ expressions. <br/>
/// This class serves as a central component for converting various query operations <br/>
/// such as filtering, projection, and aggregation into corresponding LINQ expressions.
/// </summary>
public class LinqProvider
{
    /// <summary>
    /// Retrieves the queryable type associated with the provider.
    /// </summary>
    /// <returns>Type representing a queryable data source.</returns>
    public virtual Type GetQueryableType()
    {
        return typeof(IEnumerable<>);
    }

    /// <summary>
    /// Translates a filter operator node into a LINQ 'Where' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the filter operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateFilterOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }
        if (context.Expression == null)
        {
            throw new Exception();
        }

        var queryableType = context.GetQueryableType();
        var methodInfo = GetWhereMethodInfo().MakeGenericMethod(queryableType);
        var subExpressionParameter = Expression.Parameter(queryableType, "x");
        var subContext = new TranslationContext(queryableType, subExpressionParameter, context);
        var subExpressionBody = translator.Translate(subContext, node);
        var subExpression = Expression.Lambda(subExpressionBody, subExpressionParameter);

        var methodArgs = new Expression[] { context.Expression, subExpression };

        return Expression.Call(null, methodInfo, methodArgs);
    }

    /// <summary>
    /// Translates a projection operator node into a LINQ 'Select' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the projection operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateProjectOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        //*
        // DEV NOTES:
        //      call expression (IQueryable<T>.Select()) arguments:
        //          constant expression (IEnumerable<T>)
        //          quoat expression operand:
        //              lambda expression (Func<T, projectedT>) body:
        //                  new expression:
        //                      members: in order, the lhs of the assignments.
        //                      arguments: in order, the rhs of the assignments.
        //*

        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }

        if (node is not ObjectNode objectNode)
        {
            throw TranslationThrowHelper.WrongNodeType(context, "Expected an ObjectNode for projection operation. Received a node of a different type.");
        }

        var queryableType = context.GetQueryableType();
        var subContextParameter = Expression.Parameter(queryableType, context.CreateParameterName());
        var subContext = new TranslationContext(queryableType, subContextParameter, context);

        // Cria uma lista para armazenar as associações de propriedades do tipo projetado
        var propertyBindings = new List<MemberBinding>();

        var projectionBuilder = new ProjectionBuilder(translator)
            .Run(subContext, objectNode);

        var projectedType = projectionBuilder.ProjectedType;

        if (projectedType == null)
        {
            throw TranslationThrowHelper.ErrorInternalUnknown(context, "Projection builder failed to construct a valid projected type. This may indicate an issue with the projection expressions or types involved.");
        }

        for (int i = 0; i < projectionBuilder.Properties.Count; i++)
        {
            var propDefinition = projectionBuilder.Properties[i];
            var propertyExpression = projectionBuilder.Expressions[i];

            var propertyInfo = projectedType.GetProperty(propDefinition.Name);

            if (propertyInfo == null)
            {
                throw new TranslationException($"Property '{propDefinition.Name}' not found in the projected type '{projectedType.Name}'. Ensure the property name is correctly defined in the projection.", context);
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

        return Expression.Call(selectMethod, context.Expression, lambda);
    }

    /// <summary>
    /// Translates a limit operator node into a LINQ 'Take' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the limit operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateLimitOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
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

        if (translator.Options.TakeSupportsInt64)
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

        var methodInfo = GetTakeMethodInfo()
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(methodInfo, context.Expression, valueExpression);
    }

    /// <summary>
    /// Translates a skip operator node into a LINQ 'Skip' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the skip operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateSkipOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }
        if (node is not LiteralNode literalNode)
        {
            throw TranslationThrowHelper.WrongNodeType(context, "Expected n LiteralNode for skip operation. Received a node of a different type.");
        }

        var queryableType = context.GetQueryableType();
        var valueExpression = null as Expression;

        if (translator.Options.SkipSupportsInt64)
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

        var methodInfo = GetSkipMethodInfo()
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(methodInfo, context.Expression, valueExpression);
    }

    /// <summary>
    /// Translates a count operator node into a LINQ 'Count' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the count operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateCountOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }

        var queryableType = context.GetQueryableType();
        var methodInfo = GetCountMethodInfo()
            .MakeGenericMethod(new[] { queryableType });

        var countExpression = Expression.Call(null, methodInfo, context.Expression);

        if(node is not ObjectNode objectNode)
        {
            throw TranslationThrowHelper.WrongNodeType(context, "An object node was expected.");
        }

        if (objectNode.IsEmpty())
        {
            return countExpression;
        }

        var subContext = new TranslationContext(countExpression.Type, countExpression, context);

        return translator.Translate(subContext, objectNode);
    }

    /// <summary>
    /// Translates an 'any' operator node into a LINQ 'Any' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the 'any' operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateAnyOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }

        var queryableType = context.GetQueryableType();
        var subContextExpression = Expression.Parameter(queryableType, "x");
        var subContext = new TranslationContext(queryableType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = translator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var args = new Expression[] { context.Expression, lambda };

        var methodInfo = GetAnyMethodInfo()
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(null, methodInfo, args);
    }

    /// <summary>
    /// Translates an 'all' operator node into a LINQ 'All' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the 'all' operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateAllOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }

        var queryableType = context.GetQueryableType();
        var subContextExpression = Expression.Parameter(queryableType, "x");
        var subContext = new TranslationContext(queryableType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = translator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var args = new Expression[] { context.Expression, lambda };

        var methodInfo = GetAllMethodInfo()
            .MakeGenericMethod(new[] { queryableType });

        return Expression.Call(null, methodInfo, args);
    }

    /// <summary>
    /// Translates a 'min' operator node into a LINQ 'Min' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the 'min' operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateMinOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Project);
        }
        if (context.Expression == null)
        {
            throw new Exception();
        }

        var subContextType = context.GetQueryableType();
        var subContextExpression = Expression.Parameter(subContextType, "x");
        var subContext = new TranslationContext(subContextType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = translator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var methodInfo = GetMinMethodInfo()
            .MakeGenericMethod(subContextType, lambdaBody.Type);

        var methodArgs = new Expression[] { context.Expression, lambda };

        return Expression.Call(null, methodInfo, methodArgs);
    }

    /// <summary>
    /// Translates a 'max' operator node into a LINQ 'Max' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the 'max' operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateMaxOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        throw new NotImplementedException();
    }

    //*
    // MethodInfo section. 
    //*

    /// <summary>
    /// Retrieves the MethodInfo for the 'Where' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Where' method.</returns>
    protected virtual MethodInfo GetWhereMethodInfo()
    {
        return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Select' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Select' method.</returns>
    protected virtual MethodInfo GetSelectMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                            && x.Args.Length == 2  // 'Select' for IEnumerable<T> has two generic arguments
                            && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
                .Select(x => x.Method)
                .First(m => m != null);
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Take' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Take' method.</returns>
    protected virtual MethodInfo GetTakeMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Take" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                            m.GetParameters()[1].ParameterType == typeof(int));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Skip' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Skip' method.</returns>
    protected virtual MethodInfo GetSkipMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Skip" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                            m.GetParameters()[1].ParameterType == typeof(int));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Count' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Count' method.</returns>
    protected virtual MethodInfo GetCountMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Count" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Any' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Any' method.</returns>
    protected virtual MethodInfo GetAnyMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Any" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                            m.GetParameters()[1].ParameterType.GetGenericArguments()[1] == typeof(bool));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'All' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'All' method.</returns>
    protected virtual MethodInfo GetAllMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "All" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                            m.GetParameters()[1].ParameterType.GetGenericArguments()[1] == typeof(bool));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Min' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Min' method.</returns>
    protected virtual MethodInfo GetMinMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Min" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Max' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Max' method.</returns>
    protected virtual MethodInfo GetMaxMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Max" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Finds the MethodInfo for a 'Sum' LINQ method based on the return type.
    /// </summary>
    /// <param name="returnType">The return type of the 'Sum' method.</param>
    /// <returns>MethodInfo for the 'Sum' method with the specified return type.</returns>
    protected virtual MethodInfo FindSumMethod(Type returnType)
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Sum" &&
                            m.IsGenericMethodDefinition &&
                            m.ReturnType == returnType &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Sum' LINQ method for integer type.
    /// </summary>
    /// <returns>MethodInfo for the 'Sum' method for integers.</returns>
    protected virtual MethodInfo GetIntSumMethodInfo()
    {
        return FindSumMethod(typeof(int));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Sum' LINQ method for long integer type.
    /// </summary>
    /// <returns>MethodInfo for the 'Sum' method for long integers.</returns>
    protected virtual MethodInfo GetInt64SumMethodInfo()
    {
        return FindSumMethod(typeof(long));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Sum' LINQ method for float type.
    /// </summary>
    /// <returns>MethodInfo for the 'Sum' method for floats.</returns>
    protected virtual MethodInfo GetFloatSumMethodInfo()
    {
        return FindSumMethod(typeof(float));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Sum' LINQ method for double type.
    /// </summary>
    /// <returns>MethodInfo for the 'Sum' method for doubles.</returns>
    protected virtual MethodInfo GetDoubleSumMethodInfo()
    {
        return FindSumMethod(typeof(double));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Sum' LINQ method for decimal type.
    /// </summary>
    /// <returns>MethodInfo for the 'Sum' method for decimals.</returns>
    protected virtual MethodInfo GetDecimalSumMethodInfo()
    {
        return FindSumMethod(typeof(decimal));
    }

    /// <summary>
    /// Finds the MethodInfo for an 'Average' LINQ method based on the return type.
    /// </summary>
    /// <param name="returnType">The return type of the 'Average' method.</param>
    /// <returns>MethodInfo for the 'Average' method with the specified return type.</returns>
    protected virtual MethodInfo FindAverageMethod(Type returnType)
    {
        return typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Average" &&
                            m.IsGenericMethodDefinition &&
                            m.ReturnType == returnType &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Average' LINQ method for integer type.
    /// </summary>
    /// <returns>MethodInfo for the 'Average' method for integers.</returns>
    protected virtual MethodInfo GetIntAverageMethodInfo()
    {
        return FindAverageMethod(typeof(int));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Average' LINQ method for long integer type.
    /// </summary>
    /// <returns>MethodInfo for the 'Average' method for long integers.</returns>
    protected virtual MethodInfo GetInt64AverageMethodInfo()
    {
        return FindAverageMethod(typeof(long));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Average' LINQ method for float type.
    /// </summary>
    /// <returns>MethodInfo for the 'Average' method for floats.</returns>
    protected virtual MethodInfo GetFloatAverageMethodInfo()
    {
        return FindAverageMethod(typeof(float));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Average' LINQ method for double type.
    /// </summary>
    /// <returns>MethodInfo for the 'Average' method for doubles.</returns>
    protected virtual MethodInfo GetDoubleAverageMethodInfo()
    {
        return FindAverageMethod(typeof(double));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Average' LINQ method for decimal type.
    /// </summary>
    /// <returns>MethodInfo for the 'Average' method for decimals.</returns>
    protected virtual MethodInfo GetDecimalAverageMethodInfo()
    {
        return FindAverageMethod(typeof(decimal));
    }

}
