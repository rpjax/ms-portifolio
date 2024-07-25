using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines an interface for converting <see cref="Expression"/> objects to <see cref="SerializableExpression"/> objects within a given <see cref="ConversionContext"/>.
/// </summary>
public interface IExpressionToSerializableConverter : IConverter<Expression, SerializableExpression, ConversionContext>
{

}

/// <summary>
/// Converts <see cref="Expression"/> objects to <see cref="SerializableExpression"/> objects. <br/>
/// This class handles the conversion of various expression types, ensuring they are transformed into a format suitable for serialization.
/// </summary>
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

    /// <summary>
    /// Initializes a new instance of the ExpressionToSerializable class with converters and a serializer.
    /// </summary>
    /// <param name="typeConverter">Converter for types.</param>
    /// <param name="memberInfoConverter">Converter for member info.</param>
    /// <param name="methodInfoConverter">Converter for method info.</param>
    /// <param name="propertyInfoConverter">Converter for property info.</param>
    /// <param name="constructorInfoConverter">Converter for constructor info.</param>
    /// <param name="memberBindingConverter">Converter for member bindings.</param>
    /// <param name="elementInitConverter">Converter for element initializers.</param>
    /// <param name="serializer">Serializer for converting expressions to a serializable format.</param>
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

    /// <summary>
    /// Converts an Expression object to a SerializableExpression object within the provided ConversionContext.
    /// </summary>
    /// <param name="context">The context in which the conversion is taking place.</param>
    /// <param name="expression">The Expression object to convert.</param>
    /// <returns>A SerializableExpression object representing the converted expression.</returns>
    public SerializableExpression Convert(ConversionContext context, Expression expression)
    {
        expression = Visit(expression);

        var nodeType = (ExtendedExpressionType)expression.NodeType;
        var serializable = null as SerializableExpression;

        switch (nodeType)
        {
            case ExtendedExpressionType.Add:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AddChecked:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.And:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AndAlso:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ArrayLength:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.ArrayIndex:
                serializable = ConvertMethodCall(context, As<MethodCallExpression>(expression));
                break;

            case ExtendedExpressionType.Call:
                serializable = ConvertMethodCall(context, As<MethodCallExpression>(expression));
                break;

            case ExtendedExpressionType.Coalesce:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Conditional:
                serializable = ConvertConditional(context, As<ConditionalExpression>(expression));
                break;

            case ExtendedExpressionType.Constant:
                serializable = ConvertConstant(context, As<ConstantExpression>(expression));
                break;

            case ExtendedExpressionType.Convert:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.ConvertChecked:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.Divide:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Equal:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ExclusiveOr:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.GreaterThan:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.GreaterThanOrEqual:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Invoke:
                serializable = ConvertInvocation(context, As<InvocationExpression>(expression));
                break;

            case ExtendedExpressionType.Lambda:
                serializable = ConvertLambda(context, As<LambdaExpression>(expression));
                break;

            case ExtendedExpressionType.LeftShift:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.LessThan:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.LessThanOrEqual:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ListInit:
                serializable = ConvertListInit(context, As<ListInitExpression>(expression));
                break;

            case ExtendedExpressionType.MemberAccess:
                serializable = ConvertMember(context, As<MemberExpression>(expression));
                break;

            case ExtendedExpressionType.MemberInit:
                serializable = ConvertMemberInit(context, As<MemberInitExpression>(expression));
                break;

            case ExtendedExpressionType.Modulo:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Multiply:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.MultiplyChecked:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Negate:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.UnaryPlus:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.NegateChecked:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.New:
                serializable = ConvertNew(context, As<NewExpression>(expression));
                break;

            case ExtendedExpressionType.NewArrayInit:
                serializable = ConvertNewArray(context, As<NewArrayExpression>(expression));
                break;

            case ExtendedExpressionType.NewArrayBounds:
                serializable = ConvertNewArray(context, As<NewArrayExpression>(expression));
                break;

            case ExtendedExpressionType.Not:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.NotEqual:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Or:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.OrElse:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Parameter:
                serializable = ConvertParameter(context, As<ParameterExpression>(expression));
                break;

            case ExtendedExpressionType.Power:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Quote:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.RightShift:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Subtract:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.SubtractChecked:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.TypeAs:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.TypeIs:
                serializable = ConvertTypeBinary(context, As<TypeBinaryExpression>(expression));
                break;

            case ExtendedExpressionType.Assign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
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
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AndAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.DivideAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ExclusiveOrAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.LeftShiftAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.ModuloAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.MultiplyAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.OrAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.PowerAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.RightShiftAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.SubtractAssign:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.AddAssignChecked:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.MultiplyAssignChecked:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.SubtractAssignChecked:
                serializable = ConvertBinary(context, As<BinaryExpression>(expression));
                break;

            case ExtendedExpressionType.PreIncrementAssign:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.PreDecrementAssign:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.PostIncrementAssign:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.PostDecrementAssign:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.TypeEqual:
                serializable = ConvertTypeBinary(context, As<TypeBinaryExpression>(expression));
                break;

            case ExtendedExpressionType.OnesComplement:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.IsTrue:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.IsFalse:
                serializable = ConvertUnary(context, As<UnaryExpression>(expression));
                break;

            case ExtendedExpressionType.UpdateSet:
                serializable = ConvertUpdateSet(context, As<UpdateSetExpression>(expression));
                break;

            case ExtendedExpressionType.Ordering:
                serializable = ConvertOrdering(context, As<OrderingExpression>(expression));
                break;

            case ExtendedExpressionType.ComplexOrdering:
                serializable = ConvertComplexOrdering(context, As<ComplexOrderingExpression>(expression));
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
            return VisitClosure(expression.TypeCast<MemberExpression>());
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

    private static ConstantExpression VisitClosure(MemberExpression expression)
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

    private SerializableBinaryExpression ConvertBinary(ConversionContext context, BinaryExpression expression)
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

    private SerializableUnaryExpression ConvertUnary(ConversionContext context, UnaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            Operand = Convert(context, expression.Operand)
        };
    }

    private SerializableMethodCallExpression ConvertMethodCall(ConversionContext context, MethodCallExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            MethodInfo = MethodInfoConverter.Convert(context, expression.Method),
            Object =
                expression.Object != null
                ? Convert(context, expression.Object!)
                : null,
            Arguments = expression.Arguments
                .Transform(x => Convert(context, x))
                .ToArray()
        };
    }

    private SerializableConditionalExpression ConvertConditional(ConversionContext context, ConditionalExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Test = Convert(context, expression.Test),
            IfTrue = Convert(context, expression.IfTrue),
            IfFalse = Convert(context, expression.IfFalse)
        };
    }

    private SerializableConstantExpression ConvertConstant(ConversionContext context, ConstantExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            Value = Serializer.Serialize(expression.Value)
        };
    }

    private SerializableInvocationExpression ConvertInvocation(ConversionContext context, InvocationExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Expression = Convert(context, expression.Expression),
            Arguments = expression.Arguments.Transform(x => Convert(context, x)).ToArray()
        };
    }

    private SerializableLambdaExpression ConvertLambda(ConversionContext context, LambdaExpression expression)
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

    private SerializableListInitExpression ConvertListInit(ConversionContext context, ListInitExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            NewExpression = Convert(context, expression.NewExpression).TypeCast<SerializableNewExpression>(),
            Initializers = expression.Initializers.Transform(x => ElementInitConverter.Convert(context, x)).ToArray()
        };
    }

    private SerializableMemberExpression ConvertMember(ConversionContext context, MemberExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            MemberInfo = MemberInfoConverter.Convert(context, expression.Member),
            Expression = NullableConvert(context, expression.Expression)
        };
    }

    private SerializableMemberInitExpression ConvertMemberInit(ConversionContext context, MemberInitExpression expression)
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

    private SerializableNewExpression ConvertNew(ConversionContext context, NewExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            ConstructorInfo = ConstructorInfoConverter.Convert(context, expression.Constructor!),
            Arguments = expression.Arguments
                .Transform(x => Convert(context, x))
                .ToArray(),
            Members = expression.Members != null
                ? expression.Members
                    .Transform(x => MemberInfoConverter.Convert(context, x))
                    .ToArray()
                : Array.Empty<SerializableMemberInfo>()
        };
    }

    private SerializableNewArrayExpression ConvertNewArray(ConversionContext context, NewArrayExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type!),
            Initializers = expression.Expressions.Transform(x => Convert(context, x)).ToArray(),
        };
    }

    private SerializableParameterExpression ConvertParameter(ConversionContext context, ParameterExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            IsByRef = expression.IsByRef,
            Name = context.GetExpressionReferenceId(expression),
            Type = TypeConverter.Convert(context, expression.Type)
        };
    }

    private SerializableTypeBinaryExpression ConvertTypeBinary(ConversionContext context, TypeBinaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(context, expression.Type),
            Expression = Convert(context, expression.Expression),
        };
    }

    private SerializableIndexExpression ConvertIndex(ConversionContext context, IndexExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Object = Convert(context, expression.Object!),
            Indexer = PropertyInfoConverter.Convert(context, expression.Indexer!),
            Arguments = expression.Arguments.Transform(x => Convert(context, x)).ToArray()
        };
    }

    private SerializableUpdateSetExpression ConvertUpdateSet(ConversionContext context, UpdateSetExpression expression)
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

    private SerializableOrderingExpression ConvertOrdering(ConversionContext context, OrderingExpression expression)
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

    private SerializableComplexOrderingExpression ConvertComplexOrdering(ConversionContext context, ComplexOrderingExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.ComplexOrdering,
            EntityType = TypeConverter.Convert(context, expression.EntityType),
            Expressions = expression.Expressions.Transform(x => Convert(context, x)).ToArray(),
        };
    }

}
