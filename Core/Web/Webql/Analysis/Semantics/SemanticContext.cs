using ModularSystem.Core;
using System.Collections;
using System.Reflection;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Represents the semantic context within the analysis phase of a WebQL query. <br/>
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
    /// Determines if the type of the current context is 'void'.
    /// </summary>
    /// <returns>True if the type is void; otherwise, false.</returns>
    public bool IsVoid()
    {
        return Type == typeof(void);
    }

    /// <summary>
    /// Determines if the type of the current context is a form of IEnumerable, indicating a queryable type.
    /// </summary>
    /// <returns>True if the type is queryable; otherwise, false.</returns>
    public bool IsQueryable()
    {
        return
            typeof(IEnumerable).IsAssignableFrom(Type)
            || Type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Retrieves the element type of the queryable type of the current context, if applicable.
    /// </summary>
    /// <returns>The element type of the queryable, or null if the context is not a queryable type.</returns>
    public Type? GetQueryableType()
    {
        if (IsQueryable())
        {
            if (Type!.IsArray)
            {
                return Type.GetElementType();
            }
            else
            {
                return Type.GetGenericArguments().FirstOrDefault();
            }
        }

        return null;
    }

    /// <summary>
    /// Retrieves the PropertyInfo for a given property name in the current context's type.
    /// This method searches the type's properties, considering the case-insensitivity of the name.
    /// </summary>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <param name="useParentContexts">Indicates whether to search in parent contexts if the property is not found in the current context.</param>
    /// <returns>The PropertyInfo of the specified property, if found.</returns>
    /// <exception cref="Exception">Thrown if the property is not found in the current context and parent contexts.</exception>
    public PropertyInfo? GetPropertyInfo(string name, bool useParentContexts = true)
    {
        var propertyInfo = null as PropertyInfo;
        var context = this;

        while (true)
        {
            propertyInfo = context.Type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name == name || x.Name.ToLower() == name.ToLower())
                .FirstOrDefault();

            if (propertyInfo != null)
            {
                break;
            }
            if (useParentContexts)
            {
                context = context.ParentContext;
            }
            if (context == null)
            {
                break;
            }
        }

        return propertyInfo;
    }

    /// <summary>
    /// Creates a sub-context based on a specified property name and a sub-stack trace.
    /// This method is used for navigating deeper into the semantic structure of a context, allowing targeted analysis or modification.
    /// </summary>
    /// <param name="propertyName">The property name to create a sub-context for.</param>
    /// <param name="subStack">The sub-stack trace for the new context, providing additional context for error reporting and analysis.</param>
    /// <param name="useParents">Indicates whether to use parent contexts to find the property if it's not present in the current context.</param>
    /// <returns>A new SemanticContext instance representing the sub-context.</returns>
    /// <exception cref="SemanticException">Thrown if the specified property is not found within the context hierarchy.</exception>
    public SemanticContext CreateSubContext(string propertyName, string subStack, bool useParents = true)
    {
        var propertyInfo = GetPropertyInfo(propertyName, useParents);

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
        var operators = Enum.GetValues(typeof(OperatorV2));

        foreach (OperatorV2 op in operators)
        {
            if (HelperTools.StringifyOperator(op) == node.Value.ToCamelCase())
            {
                return op.TypeCast<Operator>();
            }
        }

        throw new SemanticException($"The operator '{node.Value}' is not recognized or supported. Please ensure it is a valid operator.", this);
    }

    public string GetStack(string subStack)
    {
        return Stack + subStack;
    }

}
