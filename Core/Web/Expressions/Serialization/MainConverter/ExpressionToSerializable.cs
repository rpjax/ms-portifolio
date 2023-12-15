using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

internal class ExpressionToSerializable : ConverterBase, IConversion<Expression, SerializableExpression>
{
    protected override ConversionContext Context { get; }

    private ITypeConverter TypeConverter { get; }
    private IMemberInfoConverter MemberInfoConverter { get; }
    private IMethodInfoConverter MethodInfoConverter { get; }
    private IPropertyInfoConverter PropertyInfoConverter { get; }
    private IConstructorInfoConverter ConstructorInfoConverter { get; }
    private IMemberBindingConverter MemberBindingConverter { get; }
    private IElementInitConverter ElementInitConverter { get; }
    private ISerializer Serializer { get; }

    public ExpressionToSerializable(ConversionContext context)
    {
        Context = context;
        TypeConverter = Context.TypeConverter;
        MemberInfoConverter = Context.MemberInfoConverter;
        MethodInfoConverter = Context.MethodInfoConverter;
        PropertyInfoConverter = Context.PropertyInfoConverter;
        ConstructorInfoConverter = Context.ConstructorInfoConverter;
        MemberBindingConverter = Context.MemberBindingConverter;
        ElementInitConverter = Context.ElementInitConverter;
        Serializer = Context.Serializer;
    }

    public SerializableExpression Convert(Expression expression)
    {
        expression = Visit(expression);

        var nodeType = (ExtendedExpressionType)expression.NodeType;

        switch (nodeType)
        {
            case ExtendedExpressionType.Add: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.AddChecked: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.And: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.AndAlso: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.ArrayLength: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.ArrayIndex: return Convert(As<MethodCallExpression>(expression));
            case ExtendedExpressionType.Call: return Convert(As<MethodCallExpression>(expression));
            case ExtendedExpressionType.Coalesce: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Conditional: return Convert(As<ConditionalExpression>(expression));
            case ExtendedExpressionType.Constant: return Convert(As<ConstantExpression>(expression));
            case ExtendedExpressionType.Convert: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.ConvertChecked: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.Divide: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Equal: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.ExclusiveOr: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.GreaterThan: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.GreaterThanOrEqual: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Invoke: return Convert(As<InvocationExpression>(expression));
            case ExtendedExpressionType.Lambda: return Convert(As<LambdaExpression>(expression));
            case ExtendedExpressionType.LeftShift: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.LessThan: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.LessThanOrEqual: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.ListInit: return Convert(As<ListInitExpression>(expression));
            case ExtendedExpressionType.MemberAccess: return Convert(As<MemberExpression>(expression));
            case ExtendedExpressionType.MemberInit: return Convert(As<MemberInitExpression>(expression));
            case ExtendedExpressionType.Modulo: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Multiply: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.MultiplyChecked: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Negate: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.UnaryPlus: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.NegateChecked: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.New: return Convert(As<NewExpression>(expression));
            case ExtendedExpressionType.NewArrayInit: return Convert(As<NewArrayExpression>(expression));
            case ExtendedExpressionType.NewArrayBounds: return Convert(As<NewArrayExpression>(expression));
            case ExtendedExpressionType.Not: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.NotEqual: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Or: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.OrElse: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Parameter: return Convert(As<ParameterExpression>(expression));
            case ExtendedExpressionType.Power: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Quote: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.RightShift: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Subtract: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.SubtractChecked: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.TypeAs: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.TypeIs: return Convert(As<TypeBinaryExpression>(expression));
            case ExtendedExpressionType.Assign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.Block: throw new NotImplementedException();
            case ExtendedExpressionType.DebugInfo: throw new NotImplementedException();
            case ExtendedExpressionType.Decrement: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.Dynamic: throw new NotImplementedException();
            case ExtendedExpressionType.Default: throw new NotImplementedException();
            case ExtendedExpressionType.Extension: throw new NotImplementedException();
            case ExtendedExpressionType.Goto: throw new NotImplementedException();
            case ExtendedExpressionType.Increment: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.Index: return Convert(As<IndexExpression>(expression));
            case ExtendedExpressionType.Label: throw new NotImplementedException();
            case ExtendedExpressionType.RuntimeVariables: throw new NotImplementedException();
            case ExtendedExpressionType.Loop: throw new NotImplementedException();
            case ExtendedExpressionType.Switch: throw new NotImplementedException();
            case ExtendedExpressionType.Throw: throw new NotImplementedException();
            case ExtendedExpressionType.Try: throw new NotImplementedException();
            case ExtendedExpressionType.Unbox: throw new NotImplementedException();
            case ExtendedExpressionType.AddAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.AndAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.DivideAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.ExclusiveOrAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.LeftShiftAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.ModuloAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.MultiplyAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.OrAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.PowerAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.RightShiftAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.SubtractAssign: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.AddAssignChecked: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.MultiplyAssignChecked: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.SubtractAssignChecked: return Convert(As<BinaryExpression>(expression));
            case ExtendedExpressionType.PreIncrementAssign: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.PreDecrementAssign: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.PostIncrementAssign: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.PostDecrementAssign: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.TypeEqual: return Convert(As<TypeBinaryExpression>(expression));
            case ExtendedExpressionType.OnesComplement: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.IsTrue: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.IsFalse: return Convert(As<UnaryExpression>(expression));
            case ExtendedExpressionType.UpdateSet: return Convert(As<UpdateSetExpression>(expression));
            case ExtendedExpressionType.Ordering: return Convert(As<OrderingExpression>(expression));
            case ExtendedExpressionType.ComplexOrdering: return Convert(As<ComplexOrderingExpression>(expression));
        }

        throw ExpressionNotSupportedException(nodeType);
    }

    [return: NotNullIfNotNull("expression")]
    public SerializableExpression? NullableConvert(Expression? expression)
    {
        if (expression == null)
        {
            return null;
        }

        return Convert(expression);
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

    private SerializableBinaryExpression Convert(BinaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            IsLiftedToNull = expression.IsLiftedToNull,
            Left = Convert(expression.Left),
            Right = Convert(expression.Right)
        };
    }

    private SerializableUnaryExpression Convert(UnaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            Operand = Convert(expression.Operand)
        };
    }

    private SerializableMethodCallExpression Convert(MethodCallExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            MethodInfo = MethodInfoConverter.Convert(expression.Method),
            Object =
                expression.Object != null
                ? Convert(expression.Object!)
                : null,
            Arguments = expression.Arguments.Transform(x => Convert(x)).ToArray()
        };
    }

    private SerializableConditionalExpression Convert(ConditionalExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Test = Convert(expression.Test),
            IfTrue = Convert(expression.IfTrue),
            IfFalse = Convert(expression.IfFalse)
        };
    }

    private SerializableConstantExpression Convert(ConstantExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            Value = Serializer.Serialize(expression.Value)
        };
    }

    private SerializableInvocationExpression Convert(InvocationExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Expression = Convert(expression.Expression),
            Arguments = expression.Arguments.Transform(x => Convert(x)).ToArray()
        };
    }

    private SerializableLambdaExpression Convert(LambdaExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            DelegateType = TypeConverter.Convert(expression.Type),
            Parameters = expression.Parameters.Transform(x => Convert(x)).ToArray(),
            Body = Convert(expression.Body)
        };
    }

    private SerializableListInitExpression Convert(ListInitExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            NewExpression = Convert(expression.NewExpression),
            Initializers = expression.Initializers.Transform(x => ElementInitConverter.Convert(x)).ToArray()
        };
    }

    private SerializableMemberExpression Convert(MemberExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            MemberInfo = MemberInfoConverter.Convert(expression.Member),
            Expression = NullableConvert(expression.Expression)
        };
    }

    private SerializableMemberInitExpression Convert(MemberInitExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            NewExpression = Convert(expression.NewExpression),
            Bindings = expression.Bindings
                .Where(x => x is MemberMemberBinding)
                .Select(x => (MemberMemberBinding)x)
                .Transform(x => MemberBindingConverter.Convert(x)).ToArray()
        };
    }

    private SerializableNewExpression Convert(NewExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            ConstructorInfo = ConstructorInfoConverter.Convert(expression.Constructor!)
        };
    }

    private SerializableNewArrayExpression Convert(NewArrayExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(expression.Type!),
            Initializers = expression.Expressions.Transform(x => Convert(x)).ToArray(),
        };
    }

    private SerializableParameterExpression Convert(ParameterExpression expression)
    {
        var refString = Context.GetExpressionReferenceString(expression);

        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            IsByRef = expression.IsByRef,
            Name = expression.Name,
            Type = TypeConverter.Convert(expression.Type)
        };
    }

    private SerializableTypeBinaryExpression Convert(TypeBinaryExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            Expression = Convert(expression.Expression),
        };
    }

    private SerializableIndexExpression Convert(IndexExpression expression)
    {
        return new()
        {
            NodeType = (ExtendedExpressionType)expression.NodeType,
            Object = Convert(expression.Object!),
            Indexer = PropertyInfoConverter.Convert(expression.Indexer!),
            Arguments = expression.Arguments.Transform(x => Convert(x)).ToArray()
        };
    }

    private SerializableUpdateSetExpression Convert(UpdateSetExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.UpdateSet,
            FieldName = expression.FieldName,
            FieldType = TypeConverter.Convert(expression.FieldType),
            FieldSelector = Convert(expression.FieldSelector),
            Value = Convert(expression.Value)
        };
    }

    private SerializableOrderingExpression Convert(OrderingExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.UpdateSet,
            FieldName = expression.FieldName,
            FieldType = TypeConverter.Convert(expression.FieldType),
            FieldSelector = Convert(expression.FieldSelector),
            Direction = expression.Direction
        };
    }

    private SerializableComplexOrderingExpression Convert(ComplexOrderingExpression expression)
    {
        return new()
        {
            NodeType = ExtendedExpressionType.ComplexOrdering,
            EntityType = TypeConverter.Convert(expression.EntityType),
            Expressions = expression.Expressions.Transform(x => Convert(x)).ToArray(),
        };
    }
}
