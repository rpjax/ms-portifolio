using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

internal class SerializableToExpression : Parser, IConversion<SerializableExpression, Expression>
{
    private ITypeConverter TypeConverter { get; }
    private IMemberInfoConverter MemberInfoConverter { get; }
    private IMethodInfoConverter MethodInfoConverter { get; }
    private IPropertyInfoConverter PropertyInfoConverter { get; }
    private IConstructorInfoConverter ConstructorInfoConverter { get; }
    private IMemberBindingConverter MemberBindingConverter { get; }
    private IElementInitConverter ElementInitConverter { get; }
    private ISerializer Serializer { get; }
    protected override ParsingContext Context { get; }

    public SerializableToExpression(ParsingContext parentContext)
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
        }

        throw new Exception("");
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

}
