using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Represents the semantic context within the analysis phase of a WebQL query.
/// This class holds the contextual information required to understand and interpret each part of the syntax tree.
/// </summary>
public class SemanticContext
{
    /// <summary>
    /// Gets the type associated with the current context.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the parent context of the current context, if any.
    /// </summary>
    public SemanticContext? ParentContext { get; }

    /// <summary>
    /// Gets the stack trace of the context for debugging and tracing purposes.
    /// </summary>
    public string Stack { get; }

    /// <summary>
    /// Initializes a new instance of the SemanticContext class.
    /// </summary>
    /// <param name="type">The type associated with this context.</param>
    /// <param name="parentContext">The parent semantic context, if any.</param>
    /// <param name="stack">The stack trace for the context.</param>
    public SemanticContext(Type type, SemanticContext? parentContext = null, string? stack = null)
    {
        Type = type;
        ParentContext = parentContext;
        Stack = stack ?? "$";
    }

    /// <summary>
    /// Retrieves the PropertyInfo for a given property name in the current context's type.
    /// </summary>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <returns>The PropertyInfo of the specified property.</returns>
    /// <exception cref="Exception">Thrown if the property is not found.</exception>
    public PropertyInfo? GetPropertyInfo(string name)
    {
        var propertyInfo = Type
            .GetProperties()
            .Where(x => x.Name == name || x.Name.ToLower() == name.ToLower())
            .FirstOrDefault();

        return propertyInfo;
    }

    /// <summary>
    /// Creates a sub-context based on a specified property name and a sub-stack trace.
    /// </summary>
    /// <param name="propertyName">The property name to create a sub-context for.</param>
    /// <param name="subStack">The sub-stack trace for the new context.</param>
    /// <returns>A new SemanticContext instance representing the sub-context.</returns>
    public SemanticContext CreateSubContext(string propertyName, string subStack)
    {
        var propertyInfo = GetPropertyInfo(propertyName);

        if (propertyInfo == null)
        {
            throw new SemanticException($"Property '{propertyName}' not found in the current context. Ensure the property name is correct and exists in the context type {Type.FullName}.", this);
        }

        return new SemanticContext(propertyInfo.PropertyType, this, Stack + subStack);
    }

    /// <summary>
    /// Gets the operator associated with the specified LhsNode.
    /// </summary>
    /// <param name="node">The LhsNode representing the operator.</param>
    /// <returns>The Operator corresponding to the LhsNode.</returns>
    /// <exception cref="SemanticException">Thrown if the operator is not recognized or supported.</exception>
    public Operator GetOperatorFromLhs(LhsNode node)
    {
        var operators = Enum.GetValues(typeof(Operator));

        foreach (Operator op in operators)
        {
            if (HelperTools.Stringify(op) == node.Value.ToCamelCase())
            {
                return op.TypeCast<Operator>();
            }
        }

        throw new SemanticException($"The operator '{node.Value}' is not recognized or supported. Please ensure it is a valid operator.", this);
    }
}
