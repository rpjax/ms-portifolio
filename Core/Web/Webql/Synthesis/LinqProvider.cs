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
    protected enum LinqTypeSource
    {
        IEnumerable,
        IQueryable
    }

    /// <summary>
    /// Retrieves the queryable type associated with the provider.
    /// </summary>
    /// <returns>Type representing a queryable data source.</returns>
    public virtual Type GetQueryableType()
    {
        return typeof(IQueryable<>);
    }

    /// <summary>
    /// Translates a '$like' operator node into a LINQ 'string.ToLower().Contains($value)' expression.
    /// </summary>
    /// <param name="context">Context in which the expression is being translated.</param>
    /// <param name="translator">Translator for handling node transformations.</param>
    /// <param name="node">Node representing the filter operation.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if context is not queryable or if queryable type is null.</exception>
    public virtual Expression TranslateLikeExpression(TranslationContext context, NodeTranslator translator, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = translator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = translator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = translator.Translate(rhsContext, arrayNode[1]);
        }

        if (lhs.Type != typeof(string))
        {
            throw TranslationThrowHelper.WrongArgumentType(context, "Left-hand side (LHS) expression is expected to be of type 'string'. Found type: " + lhs.Type);
        }
        if (rhs.Type != typeof(string))
        {
            throw TranslationThrowHelper.WrongArgumentType(context, "Right-hand side (RHS) expression is expected to be of type 'string'. Found type: " + rhs.Type);
        }

        var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { })!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var lhsTolowerExpression = Expression.Call(lhs, toLowerMethod);
        var rhsToLowerExpression = Expression.Call(rhs, toLowerMethod);

        var containsArgs = new[] { rhsToLowerExpression };

        return Expression.Call(lhsTolowerExpression, containsMethod!, containsArgs);
    }

    /// <summary>
    /// Translates a '$filter' operator node into a LINQ 'Where' expression.
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
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Filter);
        }

        var queryableType = context.GetQueryableElementType();
        var methodInfo = GetWhereMethodInfo().MakeGenericMethod(queryableType);
        var subExpressionParameter = Expression.Parameter(queryableType, "x");
        var subContext = new TranslationContext(queryableType, subExpressionParameter, context);
        var subExpressionBody = translator.Translate(subContext, node);
        var subExpression = Expression.Lambda(subExpressionBody, subExpressionParameter);

        var methodArgs = new Expression[] { context.Expression, subExpression };

        return Expression.Call(null, methodInfo, methodArgs);
    }

    /// <summary>
    /// Translates a '$project' operator node into a LINQ 'Select' expression.
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
        //          quoat expression, operand:
        //              lambda expression (Func<T, projectedT>), body:
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

        var elementType = context.GetQueryableElementType();
        var subContextParameter = Expression.Parameter(elementType, context.CreateParameterName());
        var subContext = new ProjectionTranslationContext(elementType, subContextParameter, context);

        // Cria uma lista para armazenar as associações de propriedades do tipo projetado
        var propertyBindings = new List<MemberBinding>();

        var projectionBuilder = new ProjectionBuilder(translator);
        var projectedType = projectionBuilder.Run(subContext, objectNode);

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

        //*
        // Ensures '.Select' LINQ operations target the correct extension method within data structures.
        //
        // In the context of translating projection operations, it's crucial to specify the source of the '.Select' method invocation accurately.
        // This implementation guarantees that the '.Select' calls are bound to the 'Enumerable.Select' method rather than 'Queryable.Select'.
        // This distinction is important because:
        //   - The translation context operates on collections that are expected to implement IEnumerable<T>, not necessarily IQueryable<T>.
        //   - Using 'Enumerable.Select' ensures compatibility with a wider range of collection types by focusing on the IEnumerable<T> interface,
        //     which is a common denominator for both in-memory and queryable data collections.
        //   - It prevents the automatic resolution to 'Queryable.Select', which requires an IQueryable<T> source and can lead to incorrect
        //     behavior or errors if the source collection does not support IQueryable<T> operations.
        //
        // This approach is designed to maintain consistency and predictability in how projection operations are applied to the data structure,
        // ensuring that the translations align with the intended collection interfaces and behaviors.
        //*
        // Cria a expressão de chamada ao método 'Select'
        var selectMethod = context.IsProjectionContext 
            ? GetEnumerableSelectMethodInfo()
            : GetQueryableSelectMethodInfo();

        selectMethod = selectMethod.MakeGenericMethod(new[] { elementType, projectedType });

        return Expression.Call(selectMethod, context.Expression, lambda);
    }

    public virtual Expression TranslateTransformOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.SelectMany);
        }
        if (node is not ObjectNode objectNode)
        {
            throw TranslationThrowHelper.WrongNodeType(context, "Expected an ObjectNode for transform operation. Received a node of a different type.");
        }

        var elementType = context.GetQueryableElementType();

        var lambdaContextParameter = Expression.Parameter(elementType, context.CreateParameterName());
        var lambdaContext = new ProjectionTranslationContext(elementType, lambdaContextParameter, context);
        var lambdaBody = translator.Translate(lambdaContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaContextParameter);

        var selectMethod = context.IsProjectionContext
            ? GetEnumerableSelectMethodInfo()
            : GetQueryableSelectMethodInfo();

        var selectedElementType = lambdaBody.Type;
        var selectedType = selectedElementType;

        selectMethod = selectMethod.MakeGenericMethod(new[] { elementType, selectedType });

        return Expression.Call(selectMethod, context.Expression, lambda);
    }

    public virtual Expression TranslateSelectManyOperator(TranslationContext context, NodeTranslator translator, Node node)
    {
        throw TranslationThrowHelper.UnknownOrUnsupportedOperator(context, Operator.SelectMany);

        if (!context.IsQueryable())
        {
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.SelectMany);
        }

        var elementType = context.GetQueryableElementType();

        var lambdaContextParameter = Expression.Parameter(elementType, context.CreateParameterName());
        var lambdaContext = new ProjectionTranslationContext(elementType, lambdaContextParameter, context);
        var lambdaBody = translator.Translate(lambdaContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaContextParameter);

        //*
        // Ensures '.Select' LINQ operations target the correct extension method within data structures.
        //
        // In the context of translating projection operations, it's crucial to specify the source of the '.Select' method invocation accurately.
        // This implementation guarantees that the '.Select' calls are bound to the 'Enumerable.Select' method rather than 'Queryable.Select'.
        // This distinction is important because:
        //   - The translation context operates on collections that are expected to implement IEnumerable<T>, not necessarily IQueryable<T>.
        //   - Using 'Enumerable.Select' ensures compatibility with a wider range of collection types by focusing on the IEnumerable<T> interface,
        //     which is a common denominator for both in-memory and queryable data collections.
        //   - It prevents the automatic resolution to 'Queryable.Select', which requires an IQueryable<T> source and can lead to incorrect
        //     behavior or errors if the source collection does not support IQueryable<T> operations.
        //
        // This approach is designed to maintain consistency and predictability in how projection operations are applied to the data structure,
        // ensuring that the translations align with the intended collection interfaces and behaviors.
        //*
        var selectManyMethod = context.IsProjectionContext
            ? GetEnumerableSelectManyMethodInfo()
            : GetSelectManyMethodInfo();

        var selectedElementType = lambdaBody.Type;
        var selectedType = context.IsProjectionContext
            ? typeof(IEnumerable<>).MakeGenericType(selectedElementType)
            : typeof(IQueryable<>).MakeGenericType(selectedElementType);

        selectManyMethod = selectManyMethod.MakeGenericMethod(new[] { elementType, selectedType });

        return Expression.Call(selectManyMethod, context.Expression, lambda);
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
            throw TranslationThrowHelper.QueryableExclusiveOperator(context, Operator.Limit);
        }
        if (node is not LiteralNode literalNode)
        {
            throw TranslationThrowHelper.WrongNodeType(context, "Expected a LiteralNode for limit operation. Received a node of a different type.");
        }

        var queryableType = context.GetQueryableElementType();
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
    /// Translates a '$skip' operator node into a LINQ 'Skip' expression.
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
            throw TranslationThrowHelper.WrongNodeType(context, "Expected a LiteralNode for skip operation. Received a node of a different type.");
        }

        var queryableType = context.GetQueryableElementType();
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
    /// Translates a '$count' operator node into a LINQ 'Count' expression.
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

        var queryableType = context.GetQueryableElementType();

        var methodSource = GetLinqTypeSource(context);

        var methodInfo = GetCountMethodInfo(methodSource)
            .MakeGenericMethod(new[] { queryableType });

        var countExpression = Expression.Call(null, methodInfo, context.Expression);

        if (node is not ObjectNode objectNode)
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
    /// Translates an '$any' operator node into a LINQ 'Any' expression.
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

        var queryableType = context.GetQueryableElementType();
        var subContextExpression = Expression.Parameter(queryableType, "x");
        var subContext = new TranslationContext(queryableType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = translator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var methodSource = GetLinqTypeSource(context);

        var methodInfo = GetAnyMethodInfo(methodSource)
            .MakeGenericMethod(new[] { queryableType });

        var args = new Expression[] { context.Expression, lambda };

        return Expression.Call(null, methodInfo, args);
    }

    /// <summary>
    /// Translates an '$all' operator node into a LINQ 'All' expression.
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

        var queryableType = context.GetQueryableElementType();
        var subContextExpression = Expression.Parameter(queryableType, "x");
        var subContext = new TranslationContext(queryableType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = translator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var methodSource = GetLinqTypeSource(context);

        var methodInfo = GetAllMethodInfo(methodSource)
            .MakeGenericMethod(new[] { queryableType });

        var args = new Expression[] { context.Expression, lambda };

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

        var subContextType = context.GetQueryableElementType();
        var subContextExpression = Expression.Parameter(subContextType, "x");
        var subContext = new TranslationContext(subContextType, subContextExpression, context);
        var lambdaParameter = subContextExpression;
        var lambdaBody = translator.Translate(subContext, node);
        var lambda = Expression.Lambda(lambdaBody, lambdaParameter);

        var methodSource = GetLinqTypeSource(context);

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
    // TODO: add methods for translating the missing operators.
    //*

    //*
    // Helpers section. 
    //*

    protected MethodInfo GetAsQueryableMethod()
    {
        return typeof(Queryable)
            .GetMethods()
            .Where(x => x.Name == "AsQueryable")
            .Where(x => x.GetParameters().Length == 1)
            .Where(x => x.GetParameters().First().ParameterType.IsGenericType)
            .Where(x => x.GetParameters().First().ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .First();
    }

    protected LinqTypeSource GetLinqTypeSource(TranslationContext context)
    {
        if (context.Expression.Type.IsArray)
        {
            return LinqTypeSource.IEnumerable;
        }

        var genericDefinition = context.Expression.Type.TryGetGenericTypeDefinition();

        if(genericDefinition == typeof(List<>))
        {
            return LinqTypeSource.IEnumerable;
        }

        return LinqTypeSource.IQueryable;
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
        return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Select' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Select' method.</returns>
    protected virtual MethodInfo GetQueryableSelectMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length == 2
                && x.Args.Length == 2
                && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
            .Select(x => x.Method)
            .First(m => m != null);
    }

    protected virtual MethodInfo GetEnumerableSelectMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length == 2
                && x.Args.Length == 2
                && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
            .Select(x => x.Method)
            .First(m => m != null);
    }

    protected virtual MethodInfo GetSelectManyMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .Where(m => m.Name == "SelectMany" && m.IsGenericMethodDefinition)
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length >= 2
                && x.Args.Length == 2
                && x.Params[0].ParameterType.IsGenericType
                && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                && x.Params[1].ParameterType.IsGenericType
                && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                && x.Params[1].ParameterType.GetGenericArguments()[0].IsGenericType
                && x.Params[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>))
            .Select(x => x.Method)
            .First(m => m.GetParameters().Length == 2 && m != null);
    }

    protected virtual MethodInfo GetEnumerableSelectManyMethodInfo()
    {

        return typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(x => new
            {
                Name = x.Name,
                Parameters = x.GetParameters(),
                GenericArguments = x.GetGenericArguments(),
                MethodInfo = x
            })
            .Where(x => x.Name == "SelectMany")
            .Where(x => x.Parameters.Length == 2)
            .Where(x => x.GenericArguments.Length == 2)
            .Select(x => x.MethodInfo)
            .First();

        return typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "SelectMany"
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 2
                && m.GetParameters().Length == 2
                && m.GetParameters()[0].ParameterType.IsGenericType
                && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && m.GetParameters()[1].ParameterType.IsGenericType
                && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType
                && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Take' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Take' method.</returns>
    protected virtual MethodInfo GetTakeMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == "Take" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType == typeof(int));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Skip' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Skip' method.</returns>
    protected virtual MethodInfo GetSkipMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == "Skip" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType == typeof(int));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Count' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Count' method.</returns>
    protected virtual MethodInfo GetCountMethodInfo(LinqTypeSource source)
    {
        if(source == LinqTypeSource.IEnumerable)
        {
            return typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == "Count" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                );
        }

        return typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == "Count" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Any' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Any' method.</returns>
    protected virtual MethodInfo GetAnyMethodInfo(LinqTypeSource source)
    {
        if (source == LinqTypeSource.IEnumerable)
        {
            return typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Any" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[1].ParameterType.GetGenericArguments()[1] == typeof(bool)
                );
        }

        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Any" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'All' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'All' method.</returns>
    protected virtual MethodInfo GetAllMethodInfo(LinqTypeSource source)
    {
        if (source == LinqTypeSource.IEnumerable)
        {
            return typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "All" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[1].ParameterType.GetGenericArguments()[1] == typeof(bool)
                );
        }

        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "All" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Min' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Min' method.</returns>
    protected virtual MethodInfo GetMinMethodInfo()
    {
        return typeof(Queryable).GetMethods()
            .First(m => m.Name == "Min" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>));
    }

    /// <summary>
    /// Retrieves the MethodInfo for the 'Max' LINQ method.
    /// </summary>
    /// <returns>MethodInfo for the 'Max' method.</returns>
    protected virtual MethodInfo GetMaxMethodInfo()
    {
        return typeof(Queryable).GetMethods()
            .First(m => m.Name == "Max" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>));
    }

    /// <summary>
    /// Finds the MethodInfo for a 'Sum' LINQ method based on the return type.
    /// </summary>
    /// <param name="returnType">The return type of the 'Sum' method.</param>
    /// <returns>MethodInfo for the 'Sum' method with the specified return type.</returns>
    protected virtual MethodInfo FindSumMethod(Type returnType)
    {
        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Sum" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.IsGenericType &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == returnType);
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
        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Average" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.IsGenericType &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == returnType);
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
