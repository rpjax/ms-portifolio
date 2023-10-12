﻿using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides serialization and deserialization capabilities for LINQ Expressions.
/// </summary>
/// <remarks>
/// This serializer is designed to convert LINQ Expressions into a serializable format and vice versa.
/// It leverages a configurable converter and serializer to achieve this.
/// </remarks>
public class ExprSerializer : ISerializer<Expression>
{
    /// <summary>
    /// Gets the converter used to transform between Expression and its serializable counterpart.
    /// </summary>
    private IExpressionConverter Converter { get; }

    /// <summary>
    /// Gets the underlying serializer used for string serialization.
    /// </summary>
    private ISerializer Serializer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExprSerializer"/> class with optional configurations.
    /// </summary>
    /// <param name="config">The configurations for the serializer. If not provided, defaults will be used.</param>
    public ExprSerializer(Configs? config = null)
    {
        config ??= new();
        Converter = config.ExpressionConverter;
        Serializer = config.Serializer;
    }

    /// <summary>
    /// Converts a LINQ Expression into its serializable counterpart.
    /// </summary>
    /// <param name="expression">The LINQ Expression to convert.</param>
    /// <returns>The serializable representation of the expression.</returns>
    public SerializableExpression ToSerializable(Expression expression)
    {
        return Converter.Convert(expression);
    }

    /// <summary>
    /// Converts a serializable representation of an expression back into a LINQ Expression.
    /// </summary>
    /// <param name="serializedExpression">The serializable representation of the expression.</param>
    /// <returns>The LINQ Expression.</returns>
    public Expression FromSerializable(SerializableExpression serializedExpression)
    {
        return Converter.Convert(serializedExpression);
    }

    /// <summary>
    /// Serializes a LINQ Expression into a string representation.
    /// </summary>
    /// <param name="expression">The LINQ Expression to serialize.</param>
    /// <returns>The string representation of the serialized expression.</returns>
    public string Serialize(Expression expression)
    {
        return Serializer.Serialize(ToSerializable(expression));
    }

    /// <summary>
    /// Attempts to deserialize a string representation of an expression into a LINQ Expression.
    /// </summary>
    /// <param name="serializedExpression">The string representation of the serialized expression.</param>
    /// <returns>The LINQ Expression if deserialization is successful; otherwise, <c>null</c>.</returns>
    public Expression? TryDeserialize(string serializedExpression)
    {
        var node = Serializer.TryDeserialize<SerializableExpression>(serializedExpression);

        if (node == null)
        {
            return null;
        }

        return FromSerializable(node);
    }

    /// <summary>
    /// Deserializes a string representation of an expression into a LINQ Expression.
    /// </summary>
    /// <param name="serializedExpression">The string representation of the serialized expression.</param>
    /// <returns>The LINQ Expression.</returns>
    /// <exception cref="Exception">Thrown when the deserialization process encounters an error.</exception>
    public Expression Deserialize(string serializedExpression)
    {
        var expression = TryDeserialize(serializedExpression);

        if (expression == null)
        {
            throw new Exception("Failed to deserialize the provided string into a LINQ Expression.");
        }

        return expression;
    }

    private ParsingContext CreateContext()
    {
        return new DefaultParsingContext("Root Expression Serializer");
    }

    /// <summary>
    /// Represents the configuration settings for the <see cref="ExprSerializer"/>.
    /// </summary>
    public class Configs
    {
        public IExpressionConverter ExpressionConverter { get; set; } = new ExpressionConverter(new DefaultParsingContext("Root Expression Converter"));

        /// <summary>
        /// Gets the underlying serializer used for string serialization.
        /// </summary>
        public ISerializer Serializer { get; set; } = new ExprToUtf8Serializer();
    }

}
