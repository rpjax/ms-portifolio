using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql;

/// <summary>
/// Represents the context for generating WebQL queries.
/// </summary>
public class GeneratorContext
{
    /// <summary>
    /// Gets the type associated with the context.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the expression associated with the context.
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// Gets the parent context, if any.
    /// </summary>
    public GeneratorContext? ParentContext { get; }

    /// <summary>
    /// Gets the stack information for the context.
    /// </summary>
    public string Stack { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorContext"/> class.
    /// </summary>
    /// <param name="type">The type associated with the context.</param>
    /// <param name="expression">The expression associated with the context.</param>
    /// <param name="parentContext">The parent context, if any.</param>
    /// <param name="stack">The stack information for the context.</param>
    public GeneratorContext(Type type, Expression expression, GeneratorContext? parentContext = null, string stack = null)
    {
        Type = type;
        Expression = expression;
        ParentContext = parentContext;
        Stack = stack ?? "$root";
    }

    /// <summary>
    /// Gets the PropertyInfo for a specified property name.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <returns>The PropertyInfo for the specified property name.</returns>
    public PropertyInfo? GetPropertyInfo(string name)
    {
        var propertyInfo = Type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == name || x.Name.ToLower() == name.ToLower())
            .FirstOrDefault();

        return propertyInfo;
    }

    /// <summary>
    /// Creates a subcontext for a specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The subcontext for the specified property.</returns>
    public GeneratorContext CreateSubContext(string propertyName)
    {
        var propertyInfo = GetPropertyInfo(propertyName);

        if (propertyInfo == null)
        {
            throw new GeneratorException($"Property '{propertyName}' not found in the type '{Type.FullName}'. Ensure the property name is correct and exists in the specified type.", null);
        }

        var subType = propertyInfo.PropertyType;
        var expression = Expression.MakeMemberAccess(Expression, propertyInfo);
        var subStack = $"{Stack}->{propertyName}";

        return new GeneratorContext(subType, expression, this, subStack);
    }

    /// <summary>
    /// Creates a subcontext with a specified substack information.
    /// </summary>
    /// <param name="subStack">The substack information.</param>
    /// <returns>The subcontext with the specified substack.</returns>
    public GeneratorContext CreateSubStackContext(string subStack)
    {
        return new GeneratorContext(Type, Expression, this, $"{Stack}->{subStack}");
    }
}
