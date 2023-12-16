using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public interface IExpressionToSerializableConverter : IConverter<Expression, SerializableExpression, ConversionContext>
{

}

public class ExpressionToSerializable : ConverterBase, IExpressionToSerializableConverter
{
    private ITypeConverter TypeConverter { get; }
    private IMemberInfoConverter MemberInfoConverter { get; }
    private IMethodInfoConverter MethodInfoConverter { get; }
    private IPropertyInfoConverter PropertyInfoConverter { get; }
    private IConstructorInfoConverter ConstructorInfoConverter { get; }
    private IMemberBindingConverter MemberBindingConverter { get; }
    private IElementInitConverter ElementInitConverter { get; }
    private ISerializer Serializer { get; }

    public ExpressionToSerializable(
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

    public SerializableExpression Convert(ConversionContext context, Expression expression)
    {
        expression = Visit(expression);

        var nodeType = (ExtendedExpressionType)expression.NodeType;
        var serializable = null as SerializableExpression;

        switch (nodeType)
        {
            case ExtendedExpressionType.Add:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AddChecked:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.And:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AndAlso:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ArrayLength:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.ArrayIndex:
                serializable = ConvertMethodCallExpression(context, As<MethodCallExpression>(expression));
                break;

            case ExtendedExpressionType.Call:
                serializable = ConvertMethodCallExpression(context, As<MethodCallExpression>(expression));
                break;

            case ExtendedExpressionType.Coalesce:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Conditional:
                serializable = ConvertConditionalExpression(context, As<ConditionalExpression>(expression));
                break;

            case ExtendedExpressionType.Constant:
                serializable = ConvertConstantExpression(context, As<ConstantExpression>(expression));
                break;

            case ExtendedExpressionType.Convert:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.ConvertChecked:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.Divide:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Equal:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ExclusiveOr:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.GreaterThan:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.GreaterThanOrEqual:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Invoke:
                serializable = ConvertInvocationExpression(context, As<InvocationExpression>(expression));
                break;

            case ExtendedExpressionType.Lambda:
                serializable = ConvertLambdaExpression(context, As<LambdaExpression>(expression));
                break;

            case ExtendedExpressionType.LeftShift:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.LessThan:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.LessThanOrEqual:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ListInit:
                serializable = ConvertListInitExpression(context, As<ListInitExpression>(expression));
                break;

            case ExtendedExpressionType.MemberAccess:
                serializable = ConvertMemberExpression(context, As<MemberExpression>(expression));
                break;

            case ExtendedExpressionType.MemberInit:
                serializable = ConvertMemberInitExpression(context, As<MemberInitExpression>(expression));
                break;

            case ExtendedExpressionType.Modulo:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Multiply:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.MultiplyChecked:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Negate:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.UnaryPlus:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.NegateChecked:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.New:
                serializable = ConvertNewExpression(context, As<NewExpression>(expression));
                break;

            case ExtendedExpressionType.NewArrayInit:
                serializable = ConvertNewArrayExpression(context, As<NewArrayExpression>(expression));
                break;

            case ExtendedExpressionType.NewArrayBounds:
                serializable = ConvertNewArrayExpression(context, As<NewArrayExpression>(expression));
                break;

            case ExtendedExpressionType.Not:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.NotEqual:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Or:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.OrElse:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Parameter:
                serializable = ConvertParameterExpression(context, As<ParameterExpression>(expression));
                break;

            case ExtendedExpressionType.Power:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Quote:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.RightShift:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Subtract:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.SubtractChecked:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.TypeAs:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.TypeIs:
                serializable = ConvertTypeBinaryExpression(context, As<TypeBinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Assign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
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
                break; 

            case ExtendedExpressionType.AddAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AndAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.DivideAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ExclusiveOrAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.LeftShiftAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ModuloAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.MultiplyAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.OrAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.PowerAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.RightShiftAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.SubtractAssign:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AddAssignChecked:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.MultiplyAssignChecked:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.SubtractAssignChecked:
                serializable = ConvertBinaryExpression(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.PreIncrementAssign:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.PreDecrementAssign:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.PostIncrementAssign:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.PostDecrementAssign:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.TypeEqual:
                serializable = ConvertTypeBinaryExpression(context, As<TypeBinaryExpression>(expression));
                break;

            case ExtendedExpressionType.OnesComplement:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.IsTrue:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.IsFalse:
                serializable = ConvertUnaryExpression(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.UpdateSet:
                serializable = ConvertUpdateSetExpression(context, As<UpdateSetExpression>(expression));
                break;

            case ExtendedExpressionType.Ordering:
                serializable = ConvertOrderingExpression(context, As<OrderingExpression>(expression));
                break;

            case ExtendedExpressionType.ComplexOrdering:
                serializable = ConvertComplexOrderingExpression(context, As<ComplexOrderingExpression>(expression));
                break;

            default:
                break;
        }

        if (serializable == null)
        {
            throw ExpressionNotSupportedException(context, nodeType);
        }

        serializable.ReferenceId = context.GetExpressionReferenceId(expression);

        return serializable;
    }

    [return: NotNullIfNotNull("expression")]
    public SerializableExpression? NullableConvert(ConversionContext context, Expression? expression)
    {
        if (expression == null)
        {
            return null;
        }

        return Convert(context, expression);
    }

    private static Expression Visit(Expression expression)
    {
        if (IsAnnonymousClassMemberAccess(expression))
        {
            return VisitClosureExpression(expression.TypeCast<MemberExpression>());
        }

        return expression;
    }

    private static bool IsAnnonymousClassMemberAccess(Expression expression)
    {
        return expression is MemberExpression cast
            && cast.Expression is ConstantExpression
            && cast.Member.DeclaringType != null
            && cast.Member.DeclaringType.Name.StartsWith("<>");
    }

    private static T As<T>(Expression expression) where T : Expression
    {
        return expression.TypeCast<T>();
    }

    private static ConstantExpression VisitClosureExpression(MemberExpression expression)
    {
        if (expression.Expression == null)
        {
            throw new InvalidOperationException("The provided expression does not have a valid sub-expression.");
        }
        if (expression.Expression.NodeType != ExpressionType.Constant)
        {
            throw new InvalidOperationException("The sub-expression type is not of type 'Constant'.");
        }

        var constantExpression = expression.Expression.TryTypeCast<ConstantExpression>();

        if (constantExpression == null)
        {
            throw new InvalidOperationException("Failed to cast the sub-expression to 'ConstantExpression'.");
        }

        var annonymousObject = constantExpression.Value;

        if (annonymousObject == null)
        {
            throw new Exception("The value of the constant expression is null.");
        }

        var capturedValue = annonymousObject
            .GetType()
            .GetField(expression.Member.Name)
            ?.GetValue(annonymousObject);

        if (capturedValue == null)
        {
            throw new Exception($"Failed to retrieve the value of the property '{expression.Member.Name}' from the anonymous object.");
        }

        return Expression.Constant(capturedValue, expression.Type);
    }

    private SerializableBinaryExpression ConvertBinaryExpression(ConversionContext context, BinaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            IsLiftedToNull = expression.IsLiftedToNull,
            Left = Convert(context, expression.Left),
            Right = Convert(context, expression.Right)
        };
    }

    private SerializableUnaryExpression ConvertUnaryExpression(ConversionContext context, UnaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            Operand = Convert(context, expression.Operand)
        };
    }

    private SerializableMethodCallExpression ConvertMethodCallExpression(ConversionContext context, MethodCallExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            MethodInfo = MethodInfoConverter.Convert(context, expression.Method),
            Object =
                expression.Object != null
                ? Convert(context, expression.Object!)
                : null,
            Arguments = expression.Arguments.Transform(x => Convert(context, x)).ToArray()
        };
    }

    private SerializableConditionalExpression ConvertConditionalExpression(ConversionContext context, ConditionalExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Test = Convert(context, expression.Test),
            IfTrue = Convert(context, expression.IfTrue),
            IfFalse = Convert(context, expression.IfFalse)
        };
    }

    private SerializableConstantExpression ConvertConstantExpression(ConversionContext context, ConstantExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            Value = Serializer.Serialize(expression.Value)
        };
    }

    private SerializableInvocationExpression ConvertInvocationExpression(ConversionContext context, InvocationExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Expression = Convert(context, expression.Expression),
            Arguments = expression.Arguments.Transform(x => Convert(context, x)).ToArray()
        };
    }

    private SerializableLambdaExpression ConvertLambdaExpression(ConversionContext context, LambdaExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            DelegateType = TypeConverter.Convert(context, expression.Type),
            Parameters = expression.Parameters
                .Transform(x => Convert(context, x).TypeCast<SerializableParameterExpression>())
                .ToArray(),
            Body = Convert(context, expression.Body)
        };
    }

    private SerializableListInitExpression ConvertListInitExpression(ConversionContext context, ListInitExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            NewExpression = Convert(context, expression.NewExpression).TypeCast<SerializableNewExpression>(),
            Initializers = expression.Initializers.Transform(x => ElementInitConverter.Convert(context, x)).ToArray()
        };
    }

    private SerializableMemberExpression ConvertMemberExpression(ConversionContext context, MemberExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            MemberInfo = MemberInfoConverter.Convert(context, expression.Member),
            Expression = NullableConvert(context, expression.Expression)
        };
    }

    private SerializableMemberInitExpression ConvertMemberInitExpression(ConversionContext context, MemberInitExpression expression)
    {
        var debug = expression.Bindings
            .Select(x => x.GetType().FullName)
            .ToArray(); // MemberAssignment

        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            NewExpression = Convert(context, expression.NewExpression)
                .TypeCast<SerializableNewExpression>(),
            Bindings = expression.Bindings
                .Transform(x => MemberBindingConverter.Convert(context, x))
                .ToArray()
        };
    }

    private SerializableNewExpression ConvertNewExpression(ConversionContext context, NewExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            ConstructorInfo = ConstructorInfoConverter.Convert(context, expression.Constructor!)
        };
    }

    private SerializableNewArrayExpression ConvertNewArrayExpression(ConversionContext context, NewArrayExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type!),
            Initializers = expression.Expressions.Transform(x => Convert(context, x)).ToArray(),
        };
    }

    private SerializableParameterExpression ConvertParameterExpression(ConversionContext context, ParameterExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            IsByRef = expression.IsByRef,
            Name = context.GetExpressionReferenceId(expression),
            Type = TypeConverter.Convert(context, expression.Type)
        };
    }

    private SerializableTypeBinaryExpression ConvertTypeBinaryExpression(ConversionContext context, TypeBinaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            Expression = Convert(context, expression.Expression),
        };
    }

    private SerializableIndexExpression ConvertIndexExpression(ConversionContext context, IndexExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Object = Convert(context, expression.Object!),
            Indexer = PropertyInfoConverter.Convert(context, expression.Indexer!),
            Arguments = expression.Arguments.Transform(x => Convert(context, x)).ToArray()
        };
    }

    private SerializableUpdateSetExpression ConvertUpdateSetExpression(ConversionContext context, UpdateSetExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.UpdateSet,
            FieldName = expression.FieldName,
            FieldType = TypeConverter.Convert(context, expression.FieldType),
            FieldSelector = Convert(context, expression.FieldSelector),
            Value = Convert(context, expression.Value)
        };
    }

    private SerializableOrderingExpression ConvertOrderingExpression(ConversionContext context, OrderingExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.UpdateSet,
            FieldName = expression.FieldName,
            FieldType = TypeConverter.Convert(context, expression.FieldType),
            FieldSelector = Convert(context, expression.FieldSelector),
            Direction = expression.Direction
        };
    }

    private SerializableComplexOrderingExpression ConvertComplexOrderingExpression(ConversionContext context, ComplexOrderingExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.ComplexOrdering,
            EntityType = TypeConverter.Convert(context, expression.EntityType),
            Expressions = expression.Expressions.Transform(x => Convert(context, x)).ToArray(),
        };
    }

}
