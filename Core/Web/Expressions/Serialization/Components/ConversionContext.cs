using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Represents a context for managing expression references during a conversion process. <br/>
/// This class is designed to track and manage references to expressions, allowing for efficient reuse and identification.
/// </summary>
public class ConversionContext
{
    /// <summary>
    /// The parent context of the current context, allowing for nested conversions.
    /// </summary>
    private ConversionContext? Parent { get; }

    /// <summary>
    /// A table that maps expression references to their identifiers.
    /// </summary>
    private ReferenceTable ReferenceTable { get; }

    /// <summary>
    /// Initializes a new instance of the ConversionContext class.
    /// </summary>
    /// <param name="parent">The parent context, allowing this context to be nested within another (optional).</param>
    public ConversionContext(ConversionContext? parent = null)
    {
        Parent = parent;
        ReferenceTable = new();
    }

    /// <summary>
    /// Creates a child context based on the current context.
    /// </summary>
    /// <returns>A new ConversionContext instance that is a child of the current context.</returns>
    public ConversionContext CreateChild()
    {
        return new ConversionContext(this);
    }

    /// <summary>
    /// Retrieves a unique identifier for a given expression.
    /// </summary>
    /// <param name="expression">The expression for which to retrieve the reference identifier.</param>
    /// <returns>A string identifier unique to the given expression within this context.</returns>
    public string GetExpressionReferenceId(Expression expression)
    {
        return ReferenceTable.CreateReferenceId(expression);
    }

    /// <summary>
    /// Retrieves the reference expression for a given expression. <br/>
    /// If the expression does not exist in the reference table, it is added.
    /// </summary>
    /// <param name="expression">The expression for which to retrieve or add a reference.</param>
    /// <returns>The referenced expression, either retrieved or newly added.</returns>
    public Expression GetExpressionReference(Expression expression)
    {
        var id = GetExpressionReferenceId(expression);

        if (ReferenceTable.HashMap.TryGetValue(id, out var value))
        {
            return value;
        }

        ReferenceTable.HashMap.Add(id, expression);

        return expression;
    }
}

/// <summary>
/// Represents a table for storing and managing references to expressions.
/// This class provides a mechanism to create and retrieve unique identifiers for expressions.
/// </summary>
public class ReferenceTable
{
    /// <summary>
    /// A dictionary mapping string identifiers to expressions.
    /// </summary>
    public Dictionary<string, Expression> HashMap { get; } = new();

    /// <summary>
    /// Creates a unique identifier for a given expression.
    /// </summary>
    /// <param name="expression">The expression for which to create the identifier.</param>
    /// <returns>A string identifier unique to the given expression.</returns>
    public string CreateReferenceId(Expression expression)
    {
        return $"ref_{expression.GetHashCode()}";
    }
}
