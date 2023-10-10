using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

internal class ExpressionToSerializable : IConversion<Expression, SerializableExpression>
{
    private Configs Config { get; }
    private ITypeConverter TypeConverter => Config.TypeConverter;
    private IMethodInfoConverter MethodInfoConverter => Config.MethodInfoConverter;
    private IMemberInfoConverter MemberInfoConverter => Config.MemberInfoConverter;
    private IElementInitConverter ElementInitConverter => Config.ElementInitConverter;
    private IMemberBindingConverter MemberBindingConverter => Config.MemberBindingConverter;
    private ISerializer Serializer => Config.Serializer;

    public ExpressionToSerializable(Configs config)
    {
        Config = config;
    }

    public SerializableExpression Convert(Expression expression)
    {
        var nodeType = expression.NodeType;

        switch (nodeType)
        {
            case ExpressionType.Add: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.AddChecked: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.And: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.AndAlso: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.ArrayLength: return Convert(As<UnaryExpression>(expression));
            case ExpressionType.ArrayIndex: return Convert(As<MethodCallExpression>(expression));
            case ExpressionType.Call: return Convert(As<MethodCallExpression>(expression));
            case ExpressionType.Coalesce: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.Conditional: return Convert(As<ConditionalExpression>(expression));
            case ExpressionType.Constant: return Convert(As<ConstantExpression>(expression));
            case ExpressionType.Convert: return Convert(As<UnaryExpression>(expression));
            case ExpressionType.ConvertChecked: return Convert(As<UnaryExpression>(expression));
            case ExpressionType.Divide: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.Equal:return Convert(As<BinaryExpression>(expression));
            case ExpressionType.ExclusiveOr: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.GreaterThan: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.GreaterThanOrEqual: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.Invoke: return Convert(As<InvocationExpression>(expression));
            case ExpressionType.Lambda: return Convert(As<LambdaExpression>(expression));
            case ExpressionType.LeftShift: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.LessThan: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.LessThanOrEqual: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.ListInit: return Convert(As<ListInitExpression>(expression));
            case ExpressionType.MemberAccess:
                break;
            case ExpressionType.MemberInit:
                break;
            case ExpressionType.Modulo:
                break;
            case ExpressionType.Multiply:
                break;
            case ExpressionType.MultiplyChecked:
                break;
            case ExpressionType.Negate:
                break;
            case ExpressionType.UnaryPlus:
                break;
            case ExpressionType.NegateChecked:
                break;
            case ExpressionType.New:
                break;
            case ExpressionType.NewArrayInit:
                break;
            case ExpressionType.NewArrayBounds:
                break;
            case ExpressionType.Not:
                break;
            case ExpressionType.NotEqual:
                break;
            case ExpressionType.Or:
                break;
            case ExpressionType.OrElse:
                break;
            case ExpressionType.Parameter:
                break;
            case ExpressionType.Power:
                break;
            case ExpressionType.Quote:
                break;
            case ExpressionType.RightShift:
                break;
            case ExpressionType.Subtract:
                break;
            case ExpressionType.SubtractChecked:
                break;
            case ExpressionType.TypeAs:
                break;
            case ExpressionType.TypeIs:
                break;
            case ExpressionType.Assign:
                break;
            case ExpressionType.Block:
                break;
            case ExpressionType.DebugInfo:
                break;
            case ExpressionType.Decrement:
                break;
            case ExpressionType.Dynamic:
                break;
            case ExpressionType.Default:
                break;
            case ExpressionType.Extension:
                break;
            case ExpressionType.Goto:
                break;
            case ExpressionType.Increment:
                break;
            case ExpressionType.Index:
                break;
            case ExpressionType.Label:
                break;
            case ExpressionType.RuntimeVariables:
                break;
            case ExpressionType.Loop:
                break;
            case ExpressionType.Switch:
                break;
            case ExpressionType.Throw:
                break;
            case ExpressionType.Try:
                break;
            case ExpressionType.Unbox:
                break;
            case ExpressionType.AddAssign:
                break;
            case ExpressionType.AndAssign:
                break;
            case ExpressionType.DivideAssign:
                break;
            case ExpressionType.ExclusiveOrAssign:
                break;
            case ExpressionType.LeftShiftAssign:
                break;
            case ExpressionType.ModuloAssign:
                break;
            case ExpressionType.MultiplyAssign:
                break;
            case ExpressionType.OrAssign:
                break;
            case ExpressionType.PowerAssign:
                break;
            case ExpressionType.RightShiftAssign:
                break;
            case ExpressionType.SubtractAssign:
                break;
            case ExpressionType.AddAssignChecked:
                break;
            case ExpressionType.MultiplyAssignChecked:
                break;
            case ExpressionType.SubtractAssignChecked:
                break;
            case ExpressionType.PreIncrementAssign:
                break;
            case ExpressionType.PreDecrementAssign:
                break;
            case ExpressionType.PostIncrementAssign:
                break;
            case ExpressionType.PostDecrementAssign:
                break;
            case ExpressionType.TypeEqual:
                break;
            case ExpressionType.OnesComplement:
                break;
            case ExpressionType.IsTrue:
                break;
            case ExpressionType.IsFalse:
                break;
            default:
                throw new Exception();
        }
    }

    [return: NotNullIfNotNull("expression")]
    public SerializableExpression? NullableConvert(Expression? expression)
    {
        if(expression == null)
        {
            return null;
        }

        return Convert(expression);
    }

    private static T As<T>(Expression expression) where T : Expression
    {
        return expression.TypeCast<T>();
    }

    private SerializableBinaryExpression Convert(BinaryExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
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
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            IsLiftedToNull = expression.IsLiftedToNull,
            Operand = Convert(expression.Operand)
        };
    }

    private SerializableMethodCallExpression Convert(MethodCallExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            MethodInfo = MethodInfoConverter.Convert(expression.Method),
            Arguments = expression.Arguments.Transform(x => Convert(x)).ToArray()
        };
    }

    private SerializableConditionalExpression Convert(ConditionalExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Test = Convert(expression.Test),
            IfTrue = Convert(expression.IfTrue),
            IfFalse = Convert(expression.IfFalse)
        };
    }

    private SerializableConstantExpression Convert(ConstantExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            Value = Serializer.Serialize(expression.Value)
        };
    }

    private SerializableInvocationExpression Convert(InvocationExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Expression = Convert(expression.Expression),
            Arguments = expression.Arguments.Transform(x => Convert(x)).ToArray()
        };
    }

    private SerializableLambdaExpression Convert(LambdaExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            ReturnType = TypeConverter.Convert(expression.ReturnType),
            Parameters = expression.Parameters.Transform(x => Convert(x)).ToArray(),
            Body = Convert(expression.Body)
        };
    }

    private SerializableListInitExpression Convert(ListInitExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            NewExpression = Convert(expression.NewExpression),
            Initializers = expression.Initializers.Transform(x => ElementInitConverter.Convert(x)).ToArray()
        };
    }

    private SerializableMemberExpression Convert(MemberExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),  
            MemberInfo = MemberInfoConverter.Convert(expression.Member),
            Expression = NullableConvert(expression.Expression)
        };
    }

    private SerializableMemberInitExpression Convert(MemberInitExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            NewExpression = Convert(expression.NewExpression),
            Bindings = expression.Bindings.Transform(x => MemberBindingConverter.Convert(x)).ToArray()
        };
    }

    public class Configs
    {
        public ITypeConverter TypeConverter { get; set; }
        public IMethodInfoConverter MethodInfoConverter { get; set; }
        public IMemberInfoConverter MemberInfoConverter { get; set; }
        public IElementInitConverter ElementInitConverter { get; set; }
        public IMemberBindingConverter MemberBindingConverter { get; set; }
        public IConstructorInfoConverter ConstructorInfoConverter { get; set; }
        public ISerializer Serializer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {
            TypeConverter ??= dependencyContainer.GetInterface<ITypeConverter>();
            MethodInfoConverter ??= dependencyContainer.GetInterface<IMethodInfoConverter>();
            MemberInfoConverter ??= dependencyContainer.GetInterface<IMemberInfoConverter>();
            ElementInitConverter ??= dependencyContainer.GetInterface<IElementInitConverter>();
            MemberBindingConverter ??= dependencyContainer.GetInterface<IMemberBindingConverter>();
            ConstructorInfoConverter ??= dependencyContainer.GetInterface<IConstructorInfoConverter>();
            Serializer ??= dependencyContainer.GetInterface<ISerializer>();
        }
    }
}
