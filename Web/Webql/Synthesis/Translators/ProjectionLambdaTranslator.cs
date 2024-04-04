using ModularSystem.Web.Webql.Synthesis.Productions;
using ModularSystem.Webql.Analysis;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

//public class ProjectionLambdaTranslator : TranslatorBase
//{
//    public ProjectionLambdaTranslator(TranslationOptions options) : base(options)
//    {
//    }

//    /// <summary>
//    /// Production: <br/>
//    /// &lt;projection-lambda&gt; ::= '[' &lt;param-array&gt; ',' &lt;projection-object&gt; ']'<br/>
//    /// </summary>
//    /// <returns></returns>
//    /// <exception cref="TranslationException"></exception>
//    public Expression TranslateProjectionLambda(TranslationContext context, ArrayNode node, IEnumerable<Type> @params)
//    {
//        //*
//        //*
//        // DEV NOTES:
//        //      call expression (IQueryable<T>.Select()) arguments:
//        //          constant expression (IEnumerable<T>)
//        //          quoat expression, operand:
//        //              lambda expression (Func<T, projectedT>), body:
//        //                  new expression:
//        //                      members: in order, the lhs of the assignments.
//        //                      arguments: in order, the rhs of the assignments.
//        //*
//        //*

//        context = context.CreateTranslationContext(new ProjectionLambdaProduction());

//        var translator = new ArrayTranslator(Options, node);

//        //* reads and translates the '<param-array>' symbol.
//        var parameters = translator.TranslateNextLambdaArgs(context, @params);

//        //* reads the '<projection-object>' symbol.
//        var projectionObject = translator.ConsumeNextNode<ObjectNode>(context);

//        //*
//        // projection object translation.
//        //*

//        //* creates the lambda arguments as symbols, in the symbol table.
//        foreach (var item in parameters)
//        {
//            if (item.Name is null)
//            {
//                throw new TranslationException(NullArgumentError().ToString(), context);
//            }

//            context.SetSymbol(item.Name, item, false);
//        }

//        //* translates the '<object>' symbol as a projection object.
//        var projection = new ProjectionObjectTranslator(Options)
//            .TranslateProjectionObject(context, projectionObject);

//        var projectedType = projection.Type;
//        var projectionProperties = projection.PropertyDefinitions;
//        var projectedExpressions = projection.Expressions;

//        //var elementType = arg.GetElementType(context);
//        //var parameterExpr = Expression.Parameter(elementType, elementType.Name.ToCamelCase());
//        var propertyBindings = new List<MemberBinding>();

//        if (projectedType == null)
//        {
//            throw TranslationThrowHelper.ErrorInternalUnknown(context, "Projection builder failed to construct a valid projected type. This may indicate an issue with the projection expressions or types involved.");
//        }

//        for (int i = 0; i < projectionTranslator.Properties.Count; i++)
//        {
//            var propDefinition = projectionTranslator.Properties[i];
//            var propertyExpression = projectionTranslator.Expressions[i];

//            var propertyInfo = projectedType.GetProperty(propDefinition.Name);

//            if (propertyInfo == null)
//            {
//                throw new TranslationException($"Property '{propDefinition.Name}' not found in the projected type '{projectedType.Name}'. Ensure the property name is correctly defined in the projection.", context);
//            }

//            // Cria um binding para a propriedade do novo tipo
//            propertyBindings.Add(Expression.Bind(propertyInfo, propertyExpression));
//        }

//        // Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
//        var newExpression = Expression.MemberInit(Expression.New(projectedType), propertyBindings);

//        // Cria a expressão lambda 'x => new projectedType { Prop1 = ..., Prop2 = ..., ... }'
//        return Expression.Lambda(newExpression, parameters);
//    }
//}
