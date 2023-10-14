using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="Expression"/> and <see cref="SerializableExpression"/>.
/// </summary>
public interface IExpressionConverter : IBidirectionalConverter<Expression, SerializableExpression>
{
}

/// <summary>
/// Provides functionality to convert between <see cref="Expression"/> and <see cref="SerializableExpression"/>.
/// </summary>
public class ExpressionConverter : ConverterBase, IExpressionConverter
{
    /// <summary>
    /// Gets the parsing context associated with this converter.
    /// </summary>
    protected override ConversionContext Context { get; }

    /// <summary>
    /// Provides functionality to convert from <see cref="Expression"/> to <see cref="SerializableExpression"/>.
    /// </summary>
    private ExpressionToSerializable ExpressionToNodeConversion { get; }

    /// <summary>
    /// Provides functionality to convert from <see cref="SerializableExpression"/> to <see cref="Expression"/>.
    /// </summary>
    private SerializableToExpression NodeToExpressionConversion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    public ExpressionConverter(ConversionContext context)
    {
        Context = context;
        ExpressionToNodeConversion = new ExpressionToSerializable(context);
        NodeToExpressionConversion = new SerializableToExpression(context);
    }

    /// <summary>
    /// Converts an <see cref="Expression"/> instance to its serializable counterpart.
    /// </summary>
    /// <param name="instance">The expression instance to convert.</param>
    /// <returns>The serializable representation of the expression.</returns>
    public SerializableExpression Convert(Expression instance)
    {
        return ExpressionToNodeConversion.Convert(instance);
    }

    /// <summary>
    /// Converts a <see cref="SerializableExpression"/> instance back to its expression form.
    /// </summary>
    /// <param name="instance">The serializable expression to convert.</param>
    /// <returns>The deserialized expression.</returns>
    public Expression Convert(SerializableExpression instance)
    {
        return NodeToExpressionConversion.Convert(instance);
    }

}
