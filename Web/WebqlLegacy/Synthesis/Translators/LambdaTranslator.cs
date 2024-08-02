using Aidan.Web.Webql.Synthesis.Productions;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Translates the &lt;lambda&gt; symbol to an Expression.
/// </summary>
public class LambdaTranslator : TranslatorBase
{
    public LambdaTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;lambda&gt; ::= [ &lt;lambda_args&gt;, &lt;object&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public LambdaExpression TranslateLambda(TranslationContextOld context, ArrayNode node, IEnumerable<Type> paramTypes)
    {
        context = context.CreateTranslationContext(new LambdaProduction());

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the '<lambda_args>' symbol.
        var parameters = translator.TranslateNextLambdaArgs(context, paramTypes);

        foreach (var param in parameters)
        {
            if(param.Name is null)
            {
                throw new TranslationException(NullArgumentError().ToString(), context);
            }

            context.SetSymbol(param.Name, param, false);
        }

        //* reads and translates the '<object>' symbol.
        var body = translator.TranslateNextObject(context);

        return Expression.Lambda(body, parameters);
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;unary_lambda&gt; ::= [ [ &lt;string&gt; ] , &lt;object&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public LambdaExpression TranslateUnaryLambda(TranslationContextOld context, ArrayNode node, Type paramType)
    {
        context = context.CreateTranslationContext(new LambdaProduction());

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the '<lambda_args>' symbol.
        var parameters = translator.TranslateNextLambdaArgs(context, new Type[] { paramType });

        foreach (var param in parameters)
        {
            if (param.Name is null)
            {
                throw new TranslationException(NullArgumentError().ToString(), context);
            }

            context.SetSymbol(param.Name, param, false);
        }

        //* reads and translates the '<object>' symbol.
        var body = translator.TranslateNextObject(context);

        return Expression.Lambda(body, parameters);
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;predicate_lambda&gt; ::= &lt;unary_lambda&gt; <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public LambdaExpression TranslatePredicateLambda(TranslationContextOld context, ArrayNode node, Type paramType)
    {
        var expression = TranslateUnaryLambda(context, node, paramType);

        if(expression.ReturnType != typeof(bool))
        {
            throw TranslationThrowHelper.IncorrectReturnType(
                context: context,
                expectedType: typeof(bool),
                encounteredType: expression.ReturnType
            );
        }

        return expression;
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;projection_lambda&gt; ::= '[' &lt;unary_lambda_args&gt; ',' &lt;projection_object&gt; ']'<br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public LambdaExpression TranslateProjectionLambda(TranslationContextOld context, ArrayNode node, Type paramType)
    {
        //*
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
        //*

        context = context.CreateTranslationContext(new ProjectionLambdaProduction());

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the '<unary_lambda_args>' symbol.
        _ = translator.TranslateNextUnaryLambdaArgs(context, paramType);

        //* reads the '<projection_object>' symbol.
        var projectionObject = translator.ConsumeNextObject(context);

        var projectionExpr = translator.TranslateNextProjectionObject(context);

        //*
        // projection object translation.
        //*

        throw new NotImplementedException();

        //* creates the lambda arguments as symbols, in the symbol table.
        //foreach (var item in parameters)
        //{
        //    if (item.Name is null)
        //    {
        //        throw new TranslationException(NullArgumentError().ToString(), context);
        //    }

        //    context.SetSymbol(item.Name, item, false);
        //}

        ////* translates the '<object>' symbol as a projection object.
        //var projection = new ProjectionObjectTranslator(Options)
        //    .TranslateProjectionObject(context, projectionObject);

        //var projectedType = projection.Type;
        //var projectionProperties = projection.PropertyDefinitions;
        //var projectedExpressions = projection.Expressions;

        ////var elementType = arg.GetElementType(context);
        ////var parameterExpr = Expression.Parameter(elementType, elementType.Name.ToCamelCase());
        //var propertyBindings = new List<MemberBinding>();

        //if (projectedType == null)
        //{
        //    throw TranslationThrowHelper.ErrorInternalUnknown(context, "Projection builder failed to construct a valid projected type. This may indicate an issue with the projection expressions or types involved.");
        //}

        //for (int i = 0; i < projectionTranslator.Properties.Count; i++)
        //{
        //    var propDefinition = projectionTranslator.Properties[i];
        //    var propertyExpression = projectionTranslator.Expressions[i];

        //    var propertyInfo = projectedType.GetProperty(propDefinition.Name);

        //    if (propertyInfo == null)
        //    {
        //        throw new TranslationException($"Property '{propDefinition.Name}' not found in the projected type '{projectedType.Name}'. Ensure the property name is correctly defined in the projection.", context);
        //    }

        //    // Cria um binding para a propriedade do novo tipo
        //    propertyBindings.Add(Expression.Bind(propertyInfo, propertyExpression));
        //}

        //// Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
        //var newExpression = Expression.MemberInit(Expression.New(projectedType), propertyBindings);

        //// Cria a expressão lambda 'x => new projectedType { Prop1 = ..., Prop2 = ..., ... }'
        //return Expression.Lambda(newExpression, parameters);
    }
}
