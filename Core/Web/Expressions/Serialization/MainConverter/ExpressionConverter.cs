﻿using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="Expression"/> and <see cref="SerializableExpression"/>. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IExpressionConverter : IBidirectionalConverter<Expression, SerializableExpression, ConversionContext>
{
}

/// <summary>
/// Provides functionality to convert between <see cref="Expression"/> and <see cref="SerializableExpression"/>. <br/>
/// Utilizes specialized conversion classes for converting to and from serializable expressions.
/// </summary>
public class ExpressionConverter : ConverterBase, IExpressionConverter
{
    /// <summary>
    /// Provides functionality to convert from <see cref="Expression"/> to <see cref="SerializableExpression"/>.
    /// </summary>
    private IExpressionToSerializableConverter ToSerializableConverter { get; }

    /// <summary>
    /// Provides functionality to convert from <see cref="SerializableExpression"/> to <see cref="Expression"/>.
    /// </summary>
    private ISerializableToExpressionConverter ToExpressionConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionConverter"/> class.
    /// </summary>
    public ExpressionConverter()
    {
        var typeConverter = new TypeConverter(TypeConversionStrategy.UseAssemblyName);
        var memberInfoConverter = new MemberInfoConverter(typeConverter);
        var methodInfoConverter = new MethodInfoConverter(typeConverter);
        var propertyInfoConverter = new PropertyInfoConverter(typeConverter);
        var constructorInfoConverter = new ConstructorInfoConverter(typeConverter);
        var elementInitConverter = new ElementInitConverter(methodInfoConverter, this);
        var memberBindingConverter = new MemberBindingConverter(this, memberInfoConverter, elementInitConverter);
        var serializer = new ExprJsonSerializer();

        ToSerializableConverter = new ExpressionToSerializable(
            typeConverter,
            memberInfoConverter,
            methodInfoConverter,
            propertyInfoConverter,
            constructorInfoConverter,
            memberBindingConverter,
            elementInitConverter,
            serializer
        );

        ToExpressionConverter = new SerializableToExpression(
            typeConverter,
            memberInfoConverter,
            methodInfoConverter,
            propertyInfoConverter,
            constructorInfoConverter,
            memberBindingConverter,
            elementInitConverter,
            serializer
        );
    }

    /// <summary>
    /// Converts an <see cref="Expression"/> instance to its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="expression">The expression instance to convert.</param>
    /// <returns>The serializable representation of the expression.</returns>
    public SerializableExpression Convert(ConversionContext context, Expression expression)
    {
        var serializable = ToSerializableConverter.Convert(context, expression);

        //*
        // Runs the visitor pipeline. Fix are required for webql.
        //*
        serializable = new AnonymousNewFix()
            .Visit(serializable);

        return serializable;
    }

    /// <summary>
    /// Converts a <see cref="SerializableExpression"/> instance back to its original expression form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="instance">The serializable expression to convert.</param>
    /// <returns>The deserialized expression.</returns>
    public Expression Convert(ConversionContext context, SerializableExpression instance)
    {
        return ToExpressionConverter.Convert(context, instance);
    }

    private IExpressionToSerializableConverter GetToSerializableConverter()
    {
        var typeConverter = new TypeConverter(TypeConversionStrategy.UseAssemblyName);
        var memberInfoConverter = new MemberInfoConverter(typeConverter);
        var methodInfoConverter = new MethodInfoConverter(typeConverter);
        var propertyInfoConverter = new PropertyInfoConverter(typeConverter);
        var constructorInfoConverter = new ConstructorInfoConverter(typeConverter);
        var elementInitConverter = new ElementInitConverter(methodInfoConverter, this);
        var memberBindingConverter = new MemberBindingConverter(this, memberInfoConverter, elementInitConverter);
        var serializer = new ExprJsonSerializer();

        return new ExpressionToSerializable(
            typeConverter,
            memberInfoConverter,
            methodInfoConverter,
            propertyInfoConverter,
            constructorInfoConverter,
            memberBindingConverter,
            elementInitConverter,
            serializer
        );
    }

    private ISerializableToExpressionConverter GetToExpressionConverter()
    {
        var typeConverter = new TypeConverter(TypeConversionStrategy.UseAssemblyName);
        var memberInfoConverter = new MemberInfoConverter(typeConverter);
        var methodInfoConverter = new MethodInfoConverter(typeConverter);
        var propertyInfoConverter = new PropertyInfoConverter(typeConverter);
        var constructorInfoConverter = new ConstructorInfoConverter(typeConverter);
        var elementInitConverter = new ElementInitConverter(methodInfoConverter, this);
        var memberBindingConverter = new MemberBindingConverter(this, memberInfoConverter, elementInitConverter);
        var serializer = new ExprJsonSerializer();

        return new SerializableToExpression(
            typeConverter,
            memberInfoConverter,
            methodInfoConverter,
            propertyInfoConverter,
            constructorInfoConverter,
            memberBindingConverter,
            elementInitConverter,
            serializer
        );
    }

}

internal class AnonymousNewFix : SerializableExpressionVisitor
{
    protected override SerializableExpression VisitMemberInit(SerializableMemberInitExpression node)
    {
        if(node.NewExpression != null)
        {
            node.NewExpression.IsChildToMemberInit = true;
        }

        return base.VisitMemberInit(node);
    }

    protected override SerializableExpression VisitNew(SerializableNewExpression node)
    {
        if (node.IsChildToMemberInit)
        {
            return node;
        }

        var constructorInfo = node.ConstructorInfo;

        if(constructorInfo == null)
        {
            return node;
        }

        var constructedType = constructorInfo.DeclaringType;

        if (constructedType == null || !constructedType.IsAnonymousType)
        {
            return node;
        }

        var constructorParams = constructorInfo.Parameters;
        var arguments = node.Arguments;
        var members = node.Members!;
        var propertyBindings = new List<SerializableMemberBinding>();

        for (int i = 0; i < constructorParams.Length; i++)
        {
            var memberInfo = members[i];
            var binding = new SerializableMemberBinding()
            {
                BindingType = MemberBindingType.Assignment,
                Expression = arguments[i],
                MemberInfo = memberInfo
            };

            propertyBindings.Add(binding);
        }

        var memberInit = new SerializableMemberInitExpression()
        {
            NewExpression = node,
            Bindings = propertyBindings.ToArray(),
        };

        node.IsChildToMemberInit = true;

        return memberInit;
    }

}
