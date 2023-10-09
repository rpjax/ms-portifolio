using ModularSystem.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides context for serializing expressions, storing the unique parameters encountered during serialization.
/// </summary>
public class SerializationContext
{
    /// <summary>
    /// Gets or sets a list of unique parameter expressions encountered during serialization.
    /// </summary>
    private List<ParameterExpression> Parameters { get; set; } = new();

    /// <summary>
    /// Determines whether the given parameter exists in the current serialization context.
    /// </summary>
    /// <param name="parameter">The parameter to search for.</param>
    /// <returns><c>true</c> if the parameter exists in the context; otherwise, <c>false</c>.</returns>
    public bool ContainsParameter(ParameterExpression parameter)
    {
        return Parameters.Where(x => x.Name == parameter.Name && x.Type.Name == parameter.Type.Name).IsNotEmpty();
    }

    /// <summary>
    /// Retrieves an existing parameter from the context that matches the given parameter's name and type.
    /// </summary>
    /// <param name="parameter">The parameter to match against.</param>
    /// <returns>A matching <see cref="ParameterExpression"/> from the context, or the provided parameter if no match is found.</returns>
    public ParameterExpression GetParameter(ParameterExpression parameter)
    {
        var fullMatch = Parameters.Where(x => x.Name == parameter.Name && x.Type.Name == parameter.Type.Name);

        if (fullMatch.IsNotEmpty())
        {
            return fullMatch.First();
        }

        var nameMatch = Parameters.Where(x => x.Name == parameter.Name);

        if (nameMatch.IsNotEmpty())
        {
            return nameMatch.First();
        }

        var typeMatch = Parameters.Where(x => x.Type.Name == parameter.Type.Name);

        if (typeMatch.IsNotEmpty())
        {
            return typeMatch.First();
        }

        return parameter;
    }

    /// <summary>
    /// Adds a parameter to the context, ensuring no duplicates are added.
    /// </summary>
    /// <param name="parameterExpression">The parameter to add.</param>
    public void AddParameter(ParameterExpression parameterExpression)
    {
        if (!ContainsParameter(parameterExpression))
        {
            Parameters.Add(parameterExpression);
        }
    }
}

/// <summary>
/// Handles the serialization and deserialization of <see cref="Expression"/> nodes.
/// This class provides methods to convert <see cref="Expression"/> objects to and from <see cref="ExpressionNode"/> objects.
/// </summary>
public partial class ExpressionSerializer
{
    /// <summary>
    /// Serializer for dealing with type information.
    /// </summary>
    TypeSerializer TypeSerializer { get; }

    /// <summary>
    /// Serializer for handling member information.
    /// </summary>
    MemberInfoSerializer MemberInfoSerializer { get; }

    /// <summary>
    /// Serializer for processing method information.
    /// </summary>
    MethodInfoSerializer MethodInfoSerializer { get; }

    /// <summary>
    /// Default serializer for converting objects to and from JSON.
    /// </summary>
    JsonSerializer JsonSerializer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionSerializer"/> class with optional configurations.
    /// </summary>
    /// <param name="options">The configuration options to use, or null to use default configurations.</param>
    public ExpressionSerializer(Configs? options = null)
    {
        options ??= DefaultConfigs();

        TypeSerializer = new TypeSerializer(options.TypeSerializerOptions);
        MemberInfoSerializer = new MemberInfoSerializer(TypeSerializer);
        MethodInfoSerializer = new MethodInfoSerializer(TypeSerializer, options.MethodInfoSerializerOptions);
        JsonSerializer = options.JsonSerializer ?? new JsonSerializer();

        // Explanation provided in your initial description.
        JsonSerializer.Converters.Add(new ExpressionJsonConverter());
    }

    //*
    // static members
    //*

    public static Configs DefaultConfigs()
    {
        return new Configs();
    }

    public static string ToJson(ExpressionNode node, Configs? config = null)
    {
        return new ExpressionSerializer(config).ToJson(node);
    }

    public static string ToJson(Expression expression, Configs? config = null)
    {
        return new ExpressionSerializer(config).ToJson(expression);
    }

    public static ExpressionNode FromJson(string json, Configs? config = null)
    {
        return new ExpressionSerializer(config).FromJson(json);
    }

    public static Expression<T> ToLambdaExpression<T>(Expression expression, Configs? config = null)
    {
        return new ExpressionSerializer(config).ToLambdaExpression<T>(expression);
    }

    public static Exception? Evaluate(ExpressionNode node, Configs? config = null)
    {
        return new ExpressionSerializer(config).Evaluate(node, out _);
    }

    public static Exception? LambdaEvaluate<T>(ExpressionNode node, Configs? config = null)
    {
        return new ExpressionSerializer(config).LambdaEvaluate<T>(node, out var expression);
    }

    static T CastNode<T>(object obj) where T : ExpressionNode
    {
        var cast = obj.TryTypeCast<T>();

        if (cast == null)
        {
            throw new InvalidOperationException("Could not cast ExpressionNode.");
        }

        return cast;
    }

    static T CastExpression<T>(Expression expression) where T : Expression
    {
        var cast = expression.TryTypeCast<T>();

        if (cast == null)
        {
            throw new InvalidOperationException("Could not cast Expression.");
        }

        return cast;
    }

    //*
    // instance members
    //*

    /// <summary>
    /// Serializes the given <see cref="Expression"/> into its corresponding <see cref="ExpressionNode"/> representation.
    /// </summary>
    /// <param name="expression">The expression to serialize.</param>
    /// <returns>The serialized <see cref="ExpressionNode"/> representation of the provided expression.</returns>
    public ExpressionNode ToNode(Expression expression)
    {
        if (IsAnnonymousClassMemberAccess(expression))
        {
            return ToAnnonymousClassConstantNode(expression);
        }

        switch (expression.NodeType)
        {
            case ExpressionType.Add:
                return ToBinaryNode(expression);
            case ExpressionType.AddChecked:
                return ToBinaryNode(expression);
            case ExpressionType.And:
                return ToBinaryNode(expression);
            case ExpressionType.AndAlso:
                return ToBinaryNode(expression);
            case ExpressionType.ArrayLength:
                break;
            case ExpressionType.ArrayIndex:
                break;
            case ExpressionType.Call:
                return ToMethodCallNode(expression);
            case ExpressionType.Coalesce:
                break;
            case ExpressionType.Conditional:
                break;
            case ExpressionType.Constant:
                return ToConstantNode(expression);
            case ExpressionType.Convert:
                return ToUnaryNode(expression);
            case ExpressionType.ConvertChecked:
                break;
            case ExpressionType.Divide:
                return ToBinaryNode(expression);
            case ExpressionType.Equal:
                return ToBinaryNode(expression);
            case ExpressionType.ExclusiveOr:
                return ToBinaryNode(expression);
            case ExpressionType.GreaterThan:
                return ToBinaryNode(expression);
            case ExpressionType.GreaterThanOrEqual:
                return ToBinaryNode(expression);
            case ExpressionType.Invoke:
                break;
            case ExpressionType.Lambda:
                return ToLambdaNode(expression);
            case ExpressionType.LeftShift:
                return ToBinaryNode(expression);
            case ExpressionType.LessThan:
                return ToBinaryNode(expression);
            case ExpressionType.LessThanOrEqual:
                return ToBinaryNode(expression);
            case ExpressionType.ListInit:
                break;
            case ExpressionType.MemberAccess:
                return ToMemberAccessNode(expression);
            case ExpressionType.MemberInit:
                break;
            case ExpressionType.Modulo:
                break;
            case ExpressionType.Multiply:
                return ToBinaryNode(expression);
            case ExpressionType.MultiplyChecked:
                return ToBinaryNode(expression);
            case ExpressionType.Negate:
                return ToUnaryNode(expression);
            case ExpressionType.UnaryPlus:
                break;
            case ExpressionType.NegateChecked:
                return ToUnaryNode(expression);
            case ExpressionType.New:
                break;
            case ExpressionType.NewArrayInit:
                break;
            case ExpressionType.NewArrayBounds:
                break;
            case ExpressionType.Not:
                return ToUnaryNode(expression);
            case ExpressionType.NotEqual:
                return ToBinaryNode(expression);
            case ExpressionType.Or:
                return ToBinaryNode(expression);
            case ExpressionType.OrElse:
                return ToBinaryNode(expression);
            case ExpressionType.Parameter:
                return ToParameterNode(expression);
            case ExpressionType.Power:
                break;
            case ExpressionType.Quote:
                break;
            case ExpressionType.RightShift:
                return ToBinaryNode(expression);
            case ExpressionType.Subtract:
                return ToBinaryNode(expression);
            case ExpressionType.SubtractChecked:
                return ToBinaryNode(expression);
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
                return ToBinaryNode(expression);
            case ExpressionType.AndAssign:
                return ToBinaryNode(expression);
            case ExpressionType.DivideAssign:
                return ToBinaryNode(expression);
            case ExpressionType.ExclusiveOrAssign:
                return ToBinaryNode(expression);
            case ExpressionType.LeftShiftAssign:
                return ToBinaryNode(expression);
            case ExpressionType.ModuloAssign:
                break;
            case ExpressionType.MultiplyAssign:
                return ToBinaryNode(expression);
            case ExpressionType.OrAssign:
                return ToBinaryNode(expression);
            case ExpressionType.PowerAssign:
                break;
            case ExpressionType.RightShiftAssign:
                return ToBinaryNode(expression);
            case ExpressionType.SubtractAssign:
                return ToBinaryNode(expression);
            case ExpressionType.AddAssignChecked:
                return ToBinaryNode(expression);
            case ExpressionType.MultiplyAssignChecked:
                return ToBinaryNode(expression);
            case ExpressionType.SubtractAssignChecked:
                return ToBinaryNode(expression);
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
                return ToUnaryNode(expression);
            case ExpressionType.IsFalse:
                return ToUnaryNode(expression);
            default:
                break;
        }

        throw new InvalidOperationException($"Could not serialize Expression. The node type '{expression.NodeType}' is not supported.", null);
    }

    /// <summary>
    /// Deserializes the given <see cref="ExpressionNode"/> back into its original <see cref="Expression"/> representation.
    /// </summary>
    /// <param name="node">The serialized node to deserialize.</param>
    /// <param name="context">An optional context to use during deserialization. If not provided, a new context will be created.</param>
    /// <returns>The original <see cref="Expression"/> that the node represents.</returns>
    public Expression FromNode(ExpressionNode node, SerializationContext? context = null)
    {
        context ??= new SerializationContext();

        switch (node.NodeType)
        {
            case ExpressionType.Add:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.AddChecked:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.And:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.AndAlso:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.ArrayLength:
                break;
            case ExpressionType.ArrayIndex:
                break;
            case ExpressionType.Call:
                return FromMethodCallNode(CastNode<MethodCallNode>(node), context);
            case ExpressionType.Coalesce:
                break;
            case ExpressionType.Conditional:
                break;
            case ExpressionType.Constant:
                return FromConstantNode(CastNode<ConstantNode>(node));
            case ExpressionType.Convert:
                return FromUnaryNode(CastNode<UnaryNode>(node), context);
            case ExpressionType.ConvertChecked:
                break;
            case ExpressionType.Divide:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.Equal:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.ExclusiveOr:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.GreaterThan:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.GreaterThanOrEqual:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.Invoke:
                break;
            case ExpressionType.Lambda:
                return FromLambdaNode(CastNode<LambdaNode>(node), context);
            case ExpressionType.LeftShift:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.LessThan:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.LessThanOrEqual:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.ListInit:
                break;
            case ExpressionType.MemberAccess:
                return FromMemberAccessNode(CastNode<MemberAccessNode>(node), context);
            case ExpressionType.MemberInit:
                break;
            case ExpressionType.Modulo:
                break;
            case ExpressionType.Multiply:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.MultiplyChecked:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.Negate:
                return FromUnaryNode(CastNode<UnaryNode>(node), context);
            case ExpressionType.UnaryPlus:
                break;
            case ExpressionType.NegateChecked:
                return FromUnaryNode(CastNode<UnaryNode>(node), context);
            case ExpressionType.New:
                break;
            case ExpressionType.NewArrayInit:
                break;
            case ExpressionType.NewArrayBounds:
                break;
            case ExpressionType.Not:
                return FromUnaryNode(CastNode<UnaryNode>(node), context);
            case ExpressionType.NotEqual:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.Or:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.OrElse:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.Parameter:
                return FromParameterNode(CastNode<ParameterNode>(node), context);
            case ExpressionType.Power:
                break;
            case ExpressionType.Quote:
                break;
            case ExpressionType.RightShift:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.Subtract:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.SubtractChecked:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
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
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.AndAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.DivideAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.ExclusiveOrAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.LeftShiftAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.ModuloAssign:
                break;
            case ExpressionType.MultiplyAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.OrAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.PowerAssign:
                break;
            case ExpressionType.RightShiftAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.SubtractAssign:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.AddAssignChecked:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.MultiplyAssignChecked:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
            case ExpressionType.SubtractAssignChecked:
                return FromBinaryNode(CastNode<BinaryNode>(node), context);
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
                return FromUnaryNode(CastNode<UnaryNode>(node), context);
            case ExpressionType.IsFalse:
                return FromUnaryNode(CastNode<UnaryNode>(node), context);
            default:
                break;
        }

        throw new InvalidOperationException($"Could not deserialize ExpressionNode. The node type '{node.NodeType}' is not supported.", null);
    }

    public void AddJsonConverter(JsonConverter converter)
    {
        var isConverterAlreadyAdded = JsonSerializer.Converters.Where(x => x.GetType() == converter.GetType()).IsNotEmpty();

        if (!isConverterAlreadyAdded)
        {
            JsonSerializer.Converters.Add(converter);
        }
    }

    public string ToJson(ExpressionNode node)
    {
        var writer = new StringWriter();

        JsonSerializer.Serialize(writer, node);
        return writer.ToString();
    }

    public string ToJson(Expression expression)
    {
        return ToJson(ToNode(expression));
    }

    public ExpressionNode FromJson(string json)
    {
        try
        {
            var reader = new StringReader(json);
            var rawNode = JsonSerializer.Deserialize(reader, typeof(ExpressionNode));
            var node = rawNode as ExpressionNode;

            if (node == null)
            {
                throw new InvalidOperationException("The JSON provided could not be deserialized into an ExpressionNode.");
            }

            return node;
        }
        catch (Exception e)
        {
            throw new AppException("Failed to parse serialized expression node.", ExceptionCode.Internal, e, new { json });
        }
    }

    public Expression<T> ToLambdaExpression<T>(Expression expression)
    {
        if (expression.NodeType != ExpressionType.Lambda)
        {
            throw new InvalidOperationException("Cannot convert Expression to LambdaExpression because the 'ExpressionType' is not 'Lambda'.");
        }

        var cast = CastExpression<LambdaExpression>(expression);
        return Expression.Lambda<T>(cast.Body, cast.Parameters);
    }

    public Exception? Evaluate(ExpressionNode node, out Expression? expression)
    {
        try
        {
            expression = FromNode(node);

            if (expression.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)expression;
                _ = lambda.Compile();
                expression = lambda;
            }

            return null;
        }
        catch (Exception e)
        {
            expression = null;
            return e;
        }
    }

    public Exception? LambdaEvaluate<T>(ExpressionNode node, out Expression<T>? lambdaExpression)
    {
        try
        {
            var e = Evaluate(node);

            if (e != null)
            {
                lambdaExpression = null;
                return e;
            }

            var expression = FromNode(node);
            lambdaExpression = ToLambdaExpression<T>(expression);
            _ = lambdaExpression.Compile();

            return null;
        }
        catch (Exception e)
        {
            lambdaExpression = null;
            return e;
        }
    }

    bool IsAnnonymousClassMemberAccess(Expression expression)
    {
        var cast = expression.TryTypeCast<MemberExpression>();

        if (cast != null)
        {
            return cast.Expression is ConstantExpression && cast.Member.DeclaringType.Name.StartsWith("<>");
        }

        return false;
    }

    bool IsAnnonymousClassMemberAccess(ExpressionNode node)
    {
        var cast = node.TryTypeCast<MemberAccessNode>();

        if (cast != null)
        {
            return cast.Expression is ConstantNode && cast.MemberInfo.DeclaringType.Name.StartsWith("<>");
        }

        return false;
    }

    // > NODE SERIALIZATION / DESERIALIZATION < //

    public class Configs
    {
        public TypeSerializer.Options? TypeSerializerOptions { get; set; }
        public MethodInfoSerializer.Options? MethodInfoSerializerOptions { get; set; }
        public JsonSerializer? JsonSerializer { get; set; }
    }
}

// node convertion
public partial class ExpressionSerializer
{
    ConstantNode ToConstantNode(Expression expression)
    {
        var cast = CastExpression<ConstantExpression>(expression);
        var writer = new StringWriter();
        var type = cast.Type;
        var value = cast.Value;

        JsonSerializer.Serialize(writer, value);

        return new ConstantNode()
        {
            NodeType = ExpressionType.Constant,
            Type = TypeSerializer.Serialize(type),
            Value = writer.ToString()
        };
    }

    Expression FromConstantNode(ConstantNode node)
    {
        if (node.Type == null)
        {
            throw new InvalidOperationException();
        }
        if (node.Value == null)
        {
            throw new InvalidOperationException();
        }

        var reader = new StringReader(node.Value);
        var type = TypeSerializer.Deserialize(node.Type);
        var value = JsonSerializer.Deserialize(reader, type);

        return Expression.Constant(value, type);
    }

    ParameterNode ToParameterNode(Expression expression)
    {
        var cast = CastExpression<ParameterExpression>(expression);
        var type = TypeSerializer.Serialize(cast.Type);

        return new ParameterNode()
        {
            NodeType = ExpressionType.Parameter,
            IsByRef = cast.IsByRef,
            Name = cast.Name,
            Type = type,
        };
    }
    // This method is really important, a ParameterExpression is differed by the compiler by comparing the actual pointer value.
    // So if the ParameterExpression is used by nested expressions down the tree, the parameters must be the same object, meaning that 
    // a reference to every ParameterExpression created in the tree must be kept and then reused whenever an expression uses
    // the same parameter.
    //
    // Ex: (string x) => { x == "foo" }
    // In this example the equals operator is an expression where the left operand is the 'x' ParameterExpression, 
    // and the root expression is a LambdaExpression that should contain this ParameterExpression in the '.Parameters' property, 
    // if they are not the very same object, the same pointer value, the compiler won't know that they are the same, 
    // causing an exception to be thrown, sating that one of those parameters is out of scope.
    Expression FromParameterNode(ParameterNode node, SerializationContext context, bool useContext = true)
    {
        Type type;
        ParameterExpression? parameterExpression = null;

        if (node.Type == null)
        {
            throw new InvalidOperationException();
        }

        type = TypeSerializer.Deserialize(node.Type);

        if (useContext)
        {
            parameterExpression = context.GetParameter(Expression.Parameter(type, node.Name));
        }
        else
        {
            parameterExpression = Expression.Parameter(type, node.Name);
        }

        if (useContext)
        {
            context.AddParameter(parameterExpression);
        }

        return parameterExpression;
    }

    MemberAccessNode ToMemberAccessNode(Expression expression)
    {
        var cast = CastExpression<MemberExpression>(expression);

        if (cast.Expression == null)
        {
            throw new InvalidOperationException();
        }

        return new MemberAccessNode()
        {
            NodeType = ExpressionType.MemberAccess,
            Type = TypeSerializer.Serialize(cast.Type),
            MemberInfo = MemberInfoSerializer.Serialize(cast.Member),
            Expression = ToNode(cast.Expression),
        };
    }

    Expression FromMemberAccessNode(MemberAccessNode node, SerializationContext context)
    {
        if (node.Expression == null)
        {
            throw new InvalidOperationException();
        }
        if (node.MemberInfo == null)
        {
            throw new InvalidOperationException();
        }
        // If this condition is matched it means that the member access is an annonymous class generated by the compiler
        // This could be the result a closure capture of local variables, like lambdas.
        // The annonymous class will not exist in the current assembly if the expression was generated by other application.
        // To prevent unknown Type exception the deserializer will skip the member access node
        // and directly use the constant expression, if available.
        if (IsAnnonymousClassMemberAccess(node))
        {
            return FromAnnonymousClassMemberAccess(node, context);
        }

        var memberInfo = MemberInfoSerializer.Deserialize(node.MemberInfo);
        var expression = FromNode(node.Expression, context);

        return Expression.MakeMemberAccess(expression, memberInfo);
    }

    LambdaNode ToLambdaNode(Expression expression)
    {
        var cast = CastExpression<LambdaExpression>(expression);

        return new LambdaNode()
        {
            NodeType = ExpressionType.Lambda,
            Type = TypeSerializer.Serialize(cast.Type),
            ReturnType = TypeSerializer.Serialize(cast.ReturnType),
            Parameters = cast.Parameters.ToList().ConvertAll(x => ToParameterNode(x)),
            Body = ToNode(cast.Body)
        };
    }

    Expression FromLambdaNode(LambdaNode node, SerializationContext context)
    {
        if (node.Body == null)
        {
            throw new InvalidOperationException();
        }
        if (node.Type == null)
        {
            throw new InvalidOperationException();
        }

        var type = TypeSerializer.Deserialize(node.Type);
        var body = FromNode(node.Body, context);
        var parameters = node.Parameters.ConvertAll(x => (FromParameterNode(x, context) as ParameterExpression)!);

        return Expression.Lambda(type, body, parameters);
    }

    BinaryNode ToBinaryNode(Expression expression)
    {
        var cast = CastExpression<BinaryExpression>(expression);

        return new BinaryNode()
        {
            IsLiftedToNull = cast.IsLiftedToNull,
            NodeType = cast.NodeType,
            Type = TypeSerializer.Serialize(cast.Type),
            Left = ToNode(cast.Left),
            Right = ToNode(cast.Right),
        };
    }

    Expression FromBinaryNode(BinaryNode node, SerializationContext context)
    {
        if (node.Left == null)
        {
            throw new InvalidOperationException();
        }
        if (node.Right == null)
        {
            throw new InvalidOperationException();
        }

        return Expression.MakeBinary(node.NodeType, FromNode(node.Left, context), FromNode(node.Right, context), node.IsLiftedToNull, null);
    }

    UnaryNode ToUnaryNode(Expression expression)
    {
        var cast = CastExpression<UnaryExpression>(expression);

        return new UnaryNode()
        {
            IsLiftedToNull = cast.IsLiftedToNull,
            NodeType = cast.NodeType,
            Type = TypeSerializer.Serialize(cast.Type),
            Operand = ToNode(cast.Operand),
        };
    }

    Expression FromUnaryNode(UnaryNode node, SerializationContext context)
    {
        if (node.Operand == null)
        {
            throw new InvalidOperationException();
        }
        if (node.Type == null)
        {
            throw new InvalidOperationException();
        }

        return Expression.MakeUnary(node.NodeType, FromNode(node.Operand, context), TypeSerializer.Deserialize(node.Type));
    }

    MethodCallNode ToMethodCallNode(Expression expression)
    {
        var cast = CastExpression<MethodCallExpression>(expression);

        return new MethodCallNode()
        {
            IsStatic = cast.Method.IsStatic,
            NodeType = ExpressionType.Call,
            MethodInfo = MethodInfoSerializer.Serialize(cast.Method),
            Target = cast.Object != null ? ToNode(cast.Object) : null,
            Arguments = cast.Arguments.ToList().ConvertAll(x => ToNode(x))
        };
    }

    Expression FromMethodCallNode(MethodCallNode node, SerializationContext context)
    {
        if (node.MethodInfo == null)
        {
            throw new InvalidOperationException();
        }

        var arguments = node.Arguments.ConvertAll(x => FromNode(x, context)).ToArray();

        if (node.Target != null)
        {
            return Expression.Call(FromNode(node.Target, context), MethodInfoSerializer.Deserialize(node.MethodInfo), arguments);
        }

        if (node.MethodInfo.DeclaringType == null)
        {
            throw new InvalidOperationException();
        }

        var methodInfo = MethodInfoSerializer.Deserialize(node.MethodInfo);

        if (methodInfo.DeclaringType == null)
        {
            throw new InvalidOperationException();
        }

        if (node.IsStatic)
        {
            var typeArgs = node.MethodInfo.GenericArguments.ConvertAll(x => TypeSerializer.Deserialize(x)).ToArray();
            return Expression.Call(methodInfo.DeclaringType, methodInfo.Name, typeArgs, arguments);
        }

        if (node.Target == null)
        {
            throw new InvalidOperationException();
        }

        return Expression.Call(FromNode(node.Target, context), MethodInfoSerializer.Deserialize(node.MethodInfo), arguments);
    }

    //

    ExpressionNode ToAnnonymousClassConstantNode(Expression expression)
    {
        var cast = CastExpression<MemberExpression>(expression);

        if (cast.Expression == null)
        {
            throw new InvalidOperationException();
        }
        if (cast.Expression.NodeType != ExpressionType.Constant)
        {
            throw new InvalidOperationException();
        }

        var originalConstantExpression = (ConstantExpression)cast.Expression;
        var jobject = JObject.FromObject(originalConstantExpression.Value, JsonSerializer);
        var jtoken = jobject.GetValue(cast.Member.Name);
        var value = jtoken.ToObject(cast.Type, JsonSerializer);
        var constantExpression = Expression.Constant(value, cast.Type);

        return ToConstantNode(constantExpression);
    }

    Expression FromAnnonymousClassMemberAccess(MemberAccessNode node, SerializationContext context)
    {
        if (node.Expression == null)
        {
            throw new InvalidOperationException();
        }
        if (node.MemberInfo == null)
        {
            throw new InvalidOperationException();
        }
        if (node.Type == null)
        {
            throw new InvalidOperationException();
        }

        var constantNode = (ConstantNode)node.Expression;
        var type = TypeSerializer.Deserialize(node.Type);
        var jobject = JObject.Parse(constantNode.Value);
        var jtoken = jobject.GetValue(node.MemberInfo.Name);
        var value = jtoken.ToObject(type, JsonSerializer);

        return Expression.Constant(value, type);
    }
}