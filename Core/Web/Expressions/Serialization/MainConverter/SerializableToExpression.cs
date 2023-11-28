using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

internal class SerializableToExpression : ConverterBase, IConversion<SerializableExpression, Expression>
{
    private ITypeConverter TypeConverter { get; }
    private IMemberInfoConverter MemberInfoConverter { get; }
    private IMethodInfoConverter MethodInfoConverter { get; }
    private IPropertyInfoConverter PropertyInfoConverter { get; }
    private IConstructorInfoConverter ConstructorInfoConverter { get; }
    private IMemberBindingConverter MemberBindingConverter { get; }
    private IElementInitConverter ElementInitConverter { get; }
    private ISerializer Serializer { get; }
    protected override ConversionContext Context { get; }

    public SerializableToExpression(ConversionContext parentContext)
    {
        Context = parentContext.CreateChild("Serializable To Expression Conversion");
        TypeConverter = Context.GetDependency<ITypeConverter>();
        MemberInfoConverter = Context.GetDependency<IMemberInfoConverter>();
        MethodInfoConverter = Context.GetDependency<IMethodInfoConverter>();
        PropertyInfoConverter = Context.GetDependency<IPropertyInfoConverter>();
        ConstructorInfoConverter = Context.GetDependency<IConstructorInfoConverter>();
        MemberBindingConverter = Context.GetDependency<IMemberBindingConverter>();
        ElementInitConverter = Context.GetDependency<IElementInitConverter>();
        Serializer = Context.GetDependency<ISerializer>();
    }

    public Expression Convert(SerializableExpression sExpression)
    {
        var nodeType = sExpression.NodeType;

        switch (nodeType)
        {
            case ExtendedExpressionType.Add: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.AddChecked: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.And: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.AndAlso:
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.ArrayLength: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.ArrayIndex: 
                return Convert(As<SerializableMethodCallExpression>(sExpression));
            case ExtendedExpressionType.Call: 
                return Convert(As<SerializableMethodCallExpression>(sExpression));
            case ExtendedExpressionType.Coalesce: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Conditional: 
                return Convert(As<SerializableConditionalExpression>(sExpression));
            case ExtendedExpressionType.Constant: 
                return Convert(As<SerializableConstantExpression>(sExpression));
            case ExtendedExpressionType.Convert: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.ConvertChecked: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.Divide: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Equal: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.ExclusiveOr: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.GreaterThan: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.GreaterThanOrEqual: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Invoke: 
                return Convert(As<SerializableInvocationExpression>(sExpression));
            case ExtendedExpressionType.Lambda: 
                return Convert(As<SerializableLambdaExpression>(sExpression));
            case ExtendedExpressionType.LeftShift: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.LessThan: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.LessThanOrEqual: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.ListInit: 
                return Convert(As<SerializableListInitExpression>(sExpression));
            case ExtendedExpressionType.MemberAccess: 
                return Convert(As<SerializableMemberExpression>(sExpression));
            case ExtendedExpressionType.MemberInit: 
                return Convert(As<SerializableMemberInitExpression>(sExpression));
            case ExtendedExpressionType.Modulo: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Multiply: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.MultiplyChecked: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Negate: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.UnaryPlus: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.NegateChecked: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.New: 
                return Convert(As<SerializableNewExpression>(sExpression));
            case ExtendedExpressionType.NewArrayInit: 
                return Convert(As<SerializableNewArrayExpression>(sExpression));
            case ExtendedExpressionType.NewArrayBounds: 
                return Convert(As<SerializableNewArrayExpression>(sExpression));
            case ExtendedExpressionType.Not: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.NotEqual: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Or: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.OrElse: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Parameter: 
                return Convert(As<SerializableParameterExpression>(sExpression));
            case ExtendedExpressionType.Power: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Quote: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.RightShift: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Subtract: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.SubtractChecked: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.TypeAs: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.TypeIs: 
                return Convert(As<SerializableTypeBinaryExpression>(sExpression));
            case ExtendedExpressionType.Assign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.Block:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.DebugInfo:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Decrement: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.Dynamic:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Default:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Extension:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Goto:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Increment: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.Index: 
                return Convert(As<SerializableIndexExpression>(sExpression));
            case ExtendedExpressionType.Label:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.RuntimeVariables:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Loop:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Switch:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Throw:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Try:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.Unbox:
                throw ExpressionNotSupportedException(nodeType);
            case ExtendedExpressionType.AddAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.AndAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.DivideAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.ExclusiveOrAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.LeftShiftAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.ModuloAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.MultiplyAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.OrAssign:
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.PowerAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.RightShiftAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.SubtractAssign: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.AddAssignChecked: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.MultiplyAssignChecked:
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.SubtractAssignChecked: 
                return Convert(As<SerializableBinaryExpression>(sExpression));
            case ExtendedExpressionType.PreIncrementAssign:
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.PreDecrementAssign: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.PostIncrementAssign:
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.PostDecrementAssign: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.TypeEqual: 
                return Convert(As<SerializableTypeBinaryExpression>(sExpression));
            case ExtendedExpressionType.OnesComplement: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.IsTrue: 
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.IsFalse:
                return Convert(As<SerializableUnaryExpression>(sExpression));
            case ExtendedExpressionType.UpdateSet:
                return Convert(As<SerializableUpdateSetExpression>(sExpression));
            case ExtendedExpressionType.Ordering: 
                return Convert(As<SerializableOrderingExpression>(sExpression));
            case ExtendedExpressionType.ComplexOrdering:
                return Convert(As<SerializableComplexOrderingExpression>(sExpression));
        }

        throw ExpressionNotSupportedException(nodeType);
    }

    private static T As<T>(SerializableExpression sExpression) where T : SerializableExpression
    {
        return sExpression.TypeCast<T>();
    }

    [return: NotNullIfNotNull("sExpression")]
    public Expression? NullableConvert(SerializableExpression? sExpression)
    {
        if (sExpression == null)
        {
            return null;
        }

        return Convert(sExpression);
    }

    private BinaryExpression Convert(SerializableBinaryExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(nameof(sExpression.Type));
        }
        if (sExpression.Left == null)
        {
            throw MissingArgumentException(nameof(sExpression.Left));
        }
        if (sExpression.Right == null)
        {
            throw MissingArgumentException(nameof(sExpression.Right));
        }

        var nodeType = (ExpressionType)sExpression.NodeType;
        var left = Convert(sExpression.Left);
        var right = Convert(sExpression.Right);

        return Expression.MakeBinary(nodeType, left, right, sExpression.IsLiftedToNull, null);
    }

    private UnaryExpression Convert(SerializableUnaryExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(nameof(sExpression.Type));
        }
        if (sExpression.Operand == null)
        {
            throw MissingArgumentException(nameof(sExpression.Operand));
        }

        var nodeType = (ExpressionType)sExpression.NodeType;
        var type = TypeConverter.Convert(sExpression.Type);
        var operand = Convert(sExpression.Operand);

        return Expression.MakeUnary(nodeType, operand, type);
    }

    private MethodCallExpression Convert(SerializableMethodCallExpression sExpression)
    {
        if (sExpression.MethodInfo == null)
        {
            throw MissingArgumentException(nameof(sExpression.MethodInfo));
        }

        var obj = NullableConvert(sExpression.Object);
        var methodInfo = MethodInfoConverter.Convert(sExpression.MethodInfo);
        var arguments = sExpression.Arguments.Transform(x => Convert(x)).ToArray();

        return Expression.Call(obj, methodInfo, arguments);
    }

    private ConditionalExpression Convert(SerializableConditionalExpression sExpression)
    {
        if (sExpression.Test == null)
        {
            throw MissingArgumentException(nameof(sExpression.Test));
        }
        if (sExpression.IfTrue == null)
        {
            throw MissingArgumentException(nameof(sExpression.IfTrue));
        }
        if (sExpression.IfFalse == null)
        {
            throw MissingArgumentException(nameof(sExpression.IfFalse));
        }

        var test = Convert(sExpression.Test);
        var ifTrue = Convert(sExpression.IfTrue);
        var ifFalse = Convert(sExpression.IfFalse);

        return Expression.Condition(test, ifTrue, ifFalse);
    }

    private ConstantExpression Convert(SerializableConstantExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(nameof(sExpression.Type));
        }

        var type = TypeConverter.Convert(sExpression.Type);

        if (sExpression.Value == null)
        {
            if (!type.IsNullable())
            {
                throw MissingArgumentException(nameof(sExpression.Value));
            }
           
            return Expression.Constant(null, type);
        }

        var value = Serializer.Deserialize(sExpression.Value, type);

        return Expression.Constant(value, type);
    }

    private InvocationExpression Convert(SerializableInvocationExpression sExpression)
    {
        if (sExpression.Expression == null)
        {
            throw MissingArgumentException(nameof(sExpression.Expression));
        }

        var expression = Convert(sExpression.Expression);
        var arguments = sExpression.Arguments.Transform(x => Convert(sExpression));

        return Expression.Invoke(expression, arguments);
    }

    private LambdaExpression Convert(SerializableLambdaExpression sExpression)
    {
        if (sExpression.DelegateType == null)
        {
            throw MissingArgumentException(nameof(sExpression.DelegateType));
        }
        if (sExpression.Body == null)
        {
            throw MissingArgumentException(nameof(sExpression.Body));
        }

        var type = TypeConverter.Convert(sExpression.DelegateType);
        var parameters = sExpression.Parameters.Transform(x => Convert(x));
        var body = Convert(sExpression.Body);

        return Expression.Lambda(type, body, parameters);
    }

    private ListInitExpression Convert(SerializableListInitExpression sExpression)
    {
        if (sExpression.NewExpression == null)
        {
            throw MissingArgumentException(nameof(sExpression.NewExpression));
        }

        var newExpression = Convert(sExpression.NewExpression);
        var initializers = sExpression.Initializers.Transform(x => ElementInitConverter.Convert(x));

        return Expression.ListInit(newExpression, initializers);
    }

    private MemberExpression Convert(SerializableMemberExpression sExpression)
    {
        if (sExpression.MemberInfo == null)
        {
            throw MissingArgumentException(nameof(sExpression.MemberInfo));
        }
        if (sExpression.Expression == null)
        {
            throw MissingArgumentException(nameof(sExpression.Expression));
        }

        var memberInfo = MemberInfoConverter.Convert(sExpression.MemberInfo);
        var expression = Convert(sExpression.Expression);

        return Expression.MakeMemberAccess(expression, memberInfo);
    }

    private MemberInitExpression Convert(SerializableMemberInitExpression sExpression)
   {
        if (sExpression.NewExpression == null)
        {
            throw MissingArgumentException(nameof(sExpression.NewExpression));
        }

        var newExpression = Convert(sExpression.NewExpression);
        var bindings = sExpression.Bindings.Transform(x => MemberBindingConverter.Convert(x));

        return Expression.MemberInit(newExpression, bindings);
    }

    private NewExpression Convert(SerializableNewExpression sExpression)
    {
        if (sExpression.ConstructorInfo == null)
        {
            throw MissingArgumentException(nameof(sExpression.ConstructorInfo));
        }

        var constructorInfo = ConstructorInfoConverter.Convert(sExpression.ConstructorInfo);

        return Expression.New(constructorInfo);
    }

    private NewArrayExpression Convert(SerializableNewArrayExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(nameof(sExpression.Type));
        }

        var type = TypeConverter.Convert(sExpression.Type);
        var initializers = sExpression.Initializers.Transform(x => Convert(x));

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

    private ParameterExpression Convert(SerializableParameterExpression sExpression)
    {
        if (sExpression.Type == null)
        {
            throw MissingArgumentException(nameof(sExpression.Type));
        }

        var name = sExpression.Name;
        var type = TypeConverter.Convert(sExpression.Type);

        return Expression.Parameter(type, name);
    }

    private TypeBinaryExpression Convert(SerializableTypeBinaryExpression sExpression)
    {
        throw ExpressionNotSupportedException(sExpression.NodeType);
    }

    private IndexExpression Convert(SerializableIndexExpression sExpression)
    {
        if (sExpression.Object == null)
        {
            throw MissingArgumentException(nameof(sExpression.Object));
        }
        if (sExpression.Indexer == null)
        {
            throw MissingArgumentException(nameof(sExpression.Indexer));
        }

        var instance = Convert(sExpression.Object);
        var indexer = PropertyInfoConverter.Convert(sExpression.Indexer);
        var arguments = sExpression.Arguments.Transform(x => Convert(x));

        return Expression.MakeIndex(instance, indexer, arguments);
    }

    private UpdateSetExpression Convert(SerializableUpdateSetExpression sExpression)
    {
        if (sExpression.FieldName == null)
        {
            throw MissingArgumentException(nameof(sExpression.FieldName));
        }
        if (sExpression.FieldType == null)
        {
            throw MissingArgumentException(nameof(sExpression.FieldType));
        }
        if (sExpression.FieldSelector == null)
        {
            throw MissingArgumentException(nameof(sExpression.FieldSelector));
        }
        if (sExpression.Value == null)
        {
            throw MissingArgumentException(nameof(sExpression.Value));
        }

        var name = sExpression.FieldName;
        var type = TypeConverter.Convert(sExpression.FieldType);
        var selectorExpr = Convert(sExpression.FieldSelector);
        var valueExpr = Convert(sExpression.Value);

        return new UpdateSetExpression(name, type, selectorExpr, valueExpr);
    }

    private OrderingExpression Convert(SerializableOrderingExpression sExpression)
    {
        if (sExpression.FieldType == null)
        {
            throw MissingArgumentException(nameof(sExpression.FieldType));
        }
        if (sExpression.FieldSelector == null)
        {
            throw MissingArgumentException(nameof(sExpression.FieldSelector));
        }

        var type = TypeConverter.Convert(sExpression.FieldType);
        var selector = Convert(sExpression.FieldSelector);
        var direction = sExpression.Direction;

        return new OrderingExpression(type, selector, direction);
    }

    private ComplexOrderingExpression Convert(SerializableComplexOrderingExpression sExpression)
    {
        if (sExpression.EntityType == null)
        {
            throw MissingArgumentException(nameof(sExpression.EntityType));
        }

        var type = TypeConverter.Convert(sExpression.EntityType);
        var expressions = sExpression.Expressions.Transform(x => Convert(x));

        return new ComplexOrderingExpression(type, expressions);
    }
}
