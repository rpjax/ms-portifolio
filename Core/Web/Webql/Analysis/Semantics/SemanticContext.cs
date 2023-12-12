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
    public string Name { get; }

    /// <summary>
    /// Indicates whether navigation through nested properties and contexts is enabled in the semantic context. <br/>
    /// When true, the analysis process can navigate through properties of the objects within the context.
    /// </summary>
    public bool EnableNavigation { get; private set; }

    /// <summary>
    /// Indicates whether the implicit 'and' syntax is enabled in the semantic context. <br/>
    /// When true, the analysis process considers implicit logical 'and' operations within the syntax tree.
    /// </summary>
    public bool EnableImplicitAndSyntax { get; private set; }

    /// <summary>
    /// Initializes a new instance of the SemanticContext class.
    /// </summary>
    /// <param name="type">The type associated with this context.</param>
    /// <param name="parentContext">The parent semantic context, if any.</param>
    /// <param name="name">The stack trace for the context.</param>
    public SemanticContext(Type type, SemanticContext? parentContext = null, string? name = null)
    {
        Type = type;
        ParentContext = parentContext;
        Name = name ?? "$";
        EnableNavigation = parentContext?.EnableNavigation ?? true;
        EnableImplicitAndSyntax = parentContext?.EnableImplicitAndSyntax ?? true;
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
    /// Enables navigation within the semantic context. 
    /// When enabled, navigation through nested properties and contexts is allowed.
    /// </summary>
    /// <returns>The current SemanticContext instance with navigation enabled.</returns>
    public SemanticContext SetNavigationEnabled()
    {
        EnableNavigation = true;
        return this;
    }

    /// <summary>
    /// Disables navigation within the semantic context.
    /// When disabled, navigation through nested properties and contexts is restricted.
    /// </summary>
    /// <returns>The current SemanticContext instance with navigation disabled.</returns>
    public SemanticContext SetNavigationDisabled()
    {
        EnableNavigation = false;
        return this;
    }

    /// <summary>
    /// Enables the implicit 'and' syntax within the semantic context. 
    /// When enabled, implicit logical 'and' operations are considered in the analysis.
    /// </summary>
    /// <returns>The current SemanticContext instance with implicit 'and' syntax enabled.</returns>
    public SemanticContext SetImplicitAndSyntaxEnabled()
    {
        EnableImplicitAndSyntax = true;
        return this;
    }

    /// <summary>
    /// Disables the implicit 'and' syntax within the semantic context.
    /// When disabled, implicit logical 'and' operations are not considered in the analysis.
    /// </summary>
    /// <returns>The current SemanticContext instance with implicit 'and' syntax disabled.</returns>
    public SemanticContext SetImplicitAndSyntaxDisabled()
    {
        EnableImplicitAndSyntax = false;
        return this;
    }

    /// <summary>
    /// Sets the semantic context to projection semantics.
    /// This method disables navigation and implicit 'and' syntax, adapting the context for projection operations.
    /// </summary>
    /// <returns>The current SemanticContext instance set to projection semantics.</returns>
    public SemanticContext SetToProjectionSematics()
    {
        SetNavigationDisabled();
        SetImplicitAndSyntaxDisabled();
        return this;
    }

    /// <summary>
    /// Creates a sub-context based on a specified property name and a sub-stack trace.
    /// This method is used for navigating deeper into the semantic structure of a context, allowing targeted analysis or modification.
    /// </summary>
    /// <param name="identifier">The property name to create a sub-context for.</param>
    /// <param name="name">The sub-stack trace for the new context, providing additional context for error reporting and analysis.</param>
    /// <param name="useParents">Indicates whether to use parent contexts to find the property if it's not present in the current context.</param>
    /// <returns>A new SemanticContext instance representing the sub-context.</returns>
    /// <exception cref="SemanticException">Thrown if the specified property is not found within the context hierarchy.</exception>
    public SemanticContext GetReference(string identifier, string name, bool useParents = true)
    {
        var propertyInfo = GetPropertyInfo(identifier, useParents);

        if (propertyInfo == null)
        {
            throw new SemanticException($"Reference '{identifier}' not found in the current context. Ensure the reference name is correct and exists in the context type {Name}.", this);
        }

        return new SemanticContext(propertyInfo.PropertyType, this, $"{Name}{name}");
    }

}
