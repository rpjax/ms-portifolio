using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public interface ISerializableToExpressionConverter : IConverter<SerializableExpression, Expression, ConversionContext>
{

}

internal class SerializableToExpression : ConverterBase, ISerializableToExpressionConverter
{
    private ITypeConverter TypeConverter { get; }
    private IMemberInfoConverter MemberInfoConverter { get; }
    private IMethodInfoConverter MethodInfoConverter { get; }
    private IPropertyInfoConverter PropertyInfoConverter { get; }
    private IConstructorInfoConverter ConstructorInfoConverter { get; }
    private IMemberBindingConverter MemberBindingConverter { get; }
    private IElementInitConverter ElementInitConverter { get; }
    private ISerializer Serializer { get; }

    public SerializableToExpression(
        ITypeConverter typeConverter,
        IMemberInfoConverter memberInfoConverter, 
        IMethodInfoConverter methodInfoConverter, 
        IPropertyInfoConverter propertyInfoConverter, 
        IConstructorInfoConverter constructorInfoConverter,
        IMemberBindingConverter memberBindingConverter, 
        IElementInitConverter elementInitConverter,
        ISerializer serializer)
    {
        TypeConverter = typeConverter;
        MemberInfoConverter = memberInfoConverter;
        MethodInfoConverter = methodInfoConverter;
        PropertyInfoConverter = propertyInfoConverter;
        ConstructorInfoConverter = constructorInfoConverter;
        MemberBindingConverter = memberBindingConverter;
        ElementInitConverter = elementInitConverter;
        Serializer = serializer;
    }

    public Expression Convert(ConversionContext context, SerializableExpression sExpression)
    {
        var nodeType = sExpression.NodeType;
        var expression = null as Expression;

        switch (nodeType)
        {
            case ExtendedExpressionType.Add:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.AddChecked:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.And:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.AndAlso:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ArrayLength:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ArrayIndex:
                expression = ConvertMethodCallExpression(context, As<SerializableMethodCallExpression>(sExpression));
                break;

            case ExtendedExpressionType.Call:
                expression = ConvertMethodCallExpression(context, As<SerializableMethodCallExpression>(sExpression));
                break;

            case ExtendedExpressionType.Coalesce:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Conditional:
                expression = ConvertConditionalExpression(context, As<SerializableConditionalExpression>(sExpression));
                break;

            case ExtendedExpressionType.Constant:
                expression = ConvertConstantExpression(context, As<SerializableConstantExpression>(sExpression));
                break;

            case ExtendedExpressionType.Convert:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ConvertChecked:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Divide:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Equal:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ExclusiveOr:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.GreaterThan:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.GreaterThanOrEqual:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Invoke:
                expression = ConvertInvocationExpression(context, As<SerializableInvocationExpression>(sExpression));
                break;

            case ExtendedExpressionType.Lambda:
                expression = ConvertLambdaExpression(context, As<SerializableLambdaExpression>(sExpression));
                break;

            case ExtendedExpressionType.LeftShift:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.LessThan:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.LessThanOrEqual:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ListInit:
                expression = ConvertListInitExpression(context, As<SerializableListInitExpression>(sExpression));
                break;

            case ExtendedExpressionType.MemberAccess:
                expression = ConvertMemberExpression(context, As<SerializableMemberExpression>(sExpression));
                break;

            case ExtendedExpressionType.MemberInit:
                expression = ConvertMemberInitExpression(context, As<SerializableMemberInitExpression>(sExpression));
                break;

            case ExtendedExpressionType.Modulo:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Multiply:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.MultiplyChecked:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Negate:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.UnaryPlus:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.NegateChecked:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.New:
                expression = ConvertNewExpression(context, As<SerializableNewExpression>(sExpression));
                break;

            case ExtendedExpressionType.NewArrayInit:
                expression = ConvertNewArrayExpression(context, As<SerializableNewArrayExpression>(sExpression));
                break;

            case ExtendedExpressionType.NewArrayBounds:
                expression = ConvertNewArrayExpression(context, As<SerializableNewArrayExpression>(sExpression));
                break;

            case ExtendedExpressionType.Not:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.NotEqual:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Or:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.OrElse:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Parameter:
                expression = ConvertParameterExpression(context, As<SerializableParameterExpression>(sExpression));
                break;

            case ExtendedExpressionType.Power:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Quote:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.RightShift:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Subtract:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.SubtractChecked:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.TypeAs:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.TypeIs:
                expression = ConvertTypeBinaryExpression(context, As<SerializableTypeBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Assign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.Block:
            case ExtendedExpressionType.DebugInfo:
            case ExtendedExpressionType.Dynamic:
            case ExtendedExpressionType.Default:
            case ExtendedExpressionType.Extension:
            case ExtendedExpressionType.Goto:
            case ExtendedExpressionType.Label:
            case ExtendedExpressionType.RuntimeVariables:
            case ExtendedExpressionType.Loop:
            case ExtendedExpressionType.Switch:
            case ExtendedExpressionType.Throw:
            case ExtendedExpressionType.Try:
            case ExtendedExpressionType.Unbox:
                break; // Substituído por um break

            case ExtendedExpressionType.AddAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.AndAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.DivideAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ExclusiveOrAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.LeftShiftAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.ModuloAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.MultiplyAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.OrAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.PowerAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.RightShiftAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.SubtractAssign:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.AddAssignChecked:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.MultiplyAssignChecked:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.SubtractAssignChecked:
                expression = ConvertBinaryExpression(context, As<SerializableBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.PreIncrementAssign:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.TypeEqual:
                expression = ConvertTypeBinaryExpression(context, As<SerializableTypeBinaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.OnesComplement:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.IsTrue:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.IsFalse:
                expression = ConvertUnaryExpression(context, As<SerializableUnaryExpression>(sExpression));
                break;

            case ExtendedExpressionType.UpdateSet:
                expression = ConvertUpdateSetExpression(context, As<SerializableUpdateSetExpression>(sExpression));
                break;

            case ExtendedExpressionType.Ordering:
                expression = ConvertOrderingExpression(context, As<SerializableOrderingExpression>(sExpression));
                break;

            case ExtendedExpressionType.ComplexOrdering:
                expression = ConvertComplexOrderingExpression(context, As<SerializableComplexOrderingExpression>(sExpression));
                break;
        }

        if(expression == null)
        {
            throw ExpressionNotSupportedException(context, nodeType);
        }

        return context.GetExpressionReference(expression);
    }

    private static T As<T>(SerializableExpression sExpression) where T : SerializableExpression
    {
        return sExpression.TypeCast<T>();
    }

    [return: NotNullIfNotNull("sExpression")]
    public Expression? NullableConvert(ConversionContext context, SerializableExpression? sExpression)
    {
        if (sExpression == null)
        {
            return null;
        }

        return Convert(context, sExpression);
    }

    private BinaryExpression ConvertBinaryExpression(ConversionContext context, SerializableBinaryExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Type));
        }
        if (sExpression.Left == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Left));
        }
        if (sExpression.Right == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Right));
        }

        var nodeType = (ExpressionType)sExpression.NodeType;
        var left = Convert(context, sExpression.Left);
        var right = Convert(context, sExpression.Right);

        return Expression.MakeBinary(nodeType, left, right, sExpression.IsLiftedToNull, null);
    }

    private UnaryExpression ConvertUnaryExpression(ConversionContext context, SerializableUnaryExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Type));
        }
        if (sExpression.Operand == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Operand));
        }

        var nodeType = (ExpressionType)sExpression.NodeType;
        var type = TypeConverter.Convert(context, sExpression.Type);
        var operand = Convert(context, sExpression.Operand);

        return Expression.MakeUnary(nodeType, operand, type);
    }

    private MethodCallExpression ConvertMethodCallExpression(ConversionContext context, SerializableMethodCallExpression sExpression)
    {
        if (sExpression.MethodInfo == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.MethodInfo));
        }

        var obj = NullableConvert(context, sExpression.Object);
        var methodInfo = MethodInfoConverter.Convert(context, sExpression.MethodInfo);
        var arguments = sExpression.Arguments.Transform(x => Convert(context, x)).ToArray();

        return Expression.Call(obj, methodInfo, arguments);
    }

    private ConditionalExpression ConvertConditionalExpression(ConversionContext context, SerializableConditionalExpression sExpression)
    {
        if (sExpression.Test == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Test));
        }
        if (sExpression.IfTrue == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.IfTrue));
        }
        if (sExpression.IfFalse == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.IfFalse));
        }

        var test = Convert(context, sExpression.Test);
        var ifTrue = Convert(context, sExpression.IfTrue);
        var ifFalse = Convert(context, sExpression.IfFalse);

        return Expression.Condition(test, ifTrue, ifFalse);
    }

    private ConstantExpression ConvertConstantExpression(ConversionContext context, SerializableConstantExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Type));
        }

        var type = TypeConverter.Convert(context, sExpression.Type);

        if (sExpression.Value == null)
        {
            if (!type.IsNullable())
            {
                throw MissingArgumentException(context, nameof(sExpression.Value));
            }

            return Expression.Constant(null, type);
        }

        var value = Serializer.Deserialize(sExpression.Value, type);

        return Expression.Constant(value, type);
    }

    private InvocationExpression ConvertInvocationExpression(ConversionContext context, SerializableInvocationExpression sExpression)
    {
        if (sExpression.Expression == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Expression));
        }

        var expression = Convert(context, sExpression.Expression);
        var arguments = sExpression.Arguments.Transform(x => Convert(context, sExpression));

        return Expression.Invoke(expression, arguments);
    }

    private LambdaExpression ConvertLambdaExpression(ConversionContext context, SerializableLambdaExpression sExpression)
    {
        if (sExpression.DelegateType == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.DelegateType));
        }
        if (sExpression.Body == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Body));
        }

        var type = TypeConverter.Convert(context, sExpression.DelegateType);
        var parameters = sExpression.Parameters.Transform(x => Convert(context, x).TypeCast<ParameterExpression>());
        var body = Convert(context, sExpression.Body);

        return Expression.Lambda(type, body, parameters);
    }

    private ListInitExpression ConvertListInitExpression(ConversionContext context, SerializableListInitExpression sExpression)
    {
        if (sExpression.NewExpression == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.NewExpression));
        }

        var newExpression = Convert(context, sExpression.NewExpression).TypeCast<NewExpression>();
        var initializers = sExpression.Initializers.Transform(x => ElementInitConverter.Convert(context, x));

        return Expression.ListInit(newExpression, initializers);
    }

    private MemberExpression ConvertMemberExpression(ConversionContext context, SerializableMemberExpression sExpression)
    {
        if (sExpression.MemberInfo == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.MemberInfo));
        }
        if (sExpression.Expression == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Expression));
        }

        var memberInfo = MemberInfoConverter.Convert(context, sExpression.MemberInfo);
        var expression = Convert(context, sExpression.Expression);

        return Expression.MakeMemberAccess(expression, memberInfo);
    }

    private MemberInitExpression ConvertMemberInitExpression(ConversionContext context, SerializableMemberInitExpression sExpression)
    {
        if (sExpression.NewExpression == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.NewExpression));
        }

        var newExpression = Convert(context, sExpression.NewExpression).TypeCast<NewExpression>();
        var bindings = sExpression.Bindings
            .Transform(x => MemberBindingConverter.Convert(context, x))
            .ToArray();

        return Expression.MemberInit(newExpression, bindings);
    }

    private NewExpression ConvertNewExpression(ConversionContext context, SerializableNewExpression sExpression)
    {
        if (sExpression.ConstructorInfo == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.ConstructorInfo));
        }

        var constructorInfo = ConstructorInfoConverter.Convert(context, sExpression.ConstructorInfo);

        return Expression.New(constructorInfo);
    }

    private NewArrayExpression ConvertNewArrayExpression(ConversionContext context, SerializableNewArrayExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Type));
        }

        var type = TypeConverter.Convert(context, sExpression.Type);
        var initializers = sExpression.Initializers.Transform(x => Convert(context, x));

        if (sExpression.NodeType == ExtendedExpressionType.NewArrayInit)
        {
            return Expression.NewArrayInit(type, initializers);
        }
        else if (sExpression.NodeType == ExtendedExpressionType.NewArrayBounds)
        {
            return Expression.NewArrayBounds(type, initializers);
        }

        throw new Exception();
    }

    private ParameterExpression ConvertParameterExpression(ConversionContext context, SerializableParameterExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Type));
        }

        var name = sExpression.Name;
        var type = TypeConverter.Convert(context, sExpression.Type);

        return Expression.Parameter(type, name);
    }

    private TypeBinaryExpression ConvertTypeBinaryExpression(ConversionContext context, SerializableTypeBinaryExpression sExpression)
    {
        throw ExpressionNotSupportedException(context, sExpression.NodeType);
    }

    private IndexExpression ConvertIndexExpression(ConversionContext context, SerializableIndexExpression sExpression)
    {
        if (sExpression.Object == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Object));
        }
        if (sExpression.Indexer == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Indexer));
        }

        var instance = Convert(context, sExpression.Object);
        var indexer = PropertyInfoConverter.Convert(context, sExpression.Indexer);
        var arguments = sExpression.Arguments.Transform(x => Convert(context, x));

        return Expression.MakeIndex(instance, indexer, arguments);
    }

    private UpdateSetExpression ConvertUpdateSetExpression(ConversionContext context, SerializableUpdateSetExpression sExpression)
    {
        if (sExpression.FieldName == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.FieldName));
        }
        if (sExpression.FieldType == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.FieldType));
        }
        if (sExpression.FieldSelector == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.FieldSelector));
        }
        if (sExpression.Value == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.Value));
        }

        var name = sExpression.FieldName;
        var type = TypeConverter.Convert(context, sExpression.FieldType);
        var selectorExpr = Convert(context, sExpression.FieldSelector);
        var valueExpr = Convert(context, sExpression.Value);

        return new UpdateSetExpression(name, type, selectorExpr, valueExpr);
    }

    private OrderingExpression ConvertOrderingExpression(ConversionContext context, SerializableOrderingExpression sExpression)
    {
        if (sExpression.FieldType == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.FieldType));
        }
        if (sExpression.FieldSelector == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.FieldSelector));
        }

        var type = TypeConverter.Convert(context, sExpression.FieldType);
        var selector = Convert(context, sExpression.FieldSelector);
        var direction = sExpression.Direction;

        return new OrderingExpression(type, selector, direction);
    }

    private ComplexOrderingExpression ConvertComplexOrderingExpression(ConversionContext context, SerializableComplexOrderingExpression sExpression)
    {
        if (sExpression.EntityType == null)
        {
            throw MissingArgumentException(context, nameof(sExpression.EntityType));
        }

        var type = TypeConverter.Convert(context, sExpression.EntityType);
        var expressions = sExpression.Expressions.Transform(x => Convert(context, x));

        return new ComplexOrderingExpression(type, expressions);
    }
}
