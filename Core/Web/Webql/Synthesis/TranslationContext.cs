using ModularSystem.Webql.Analysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Represents the context for a translation process within the WebQL framework.
/// This class encapsulates information about the current state of translation, including
/// the type being translated, the current expression, and the hierarchy of translation contexts.
/// </summary>
public class TranslationContext : SemanticContext
{
    /// <summary>
    /// Gets the current expression in the translation process.
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationContext"/> class.
    /// </summary>
    /// <param name="type">The type associated with this context.</param>
    /// <param name="inputExpression">The current expression in the translation process.</param>
    /// <param name="parentContext">The parent translation context, if any.</param>
    /// <param name="stack">A string representing the stack trace of contexts leading to this one.</param>
    public TranslationContext(
        Type type,
        Expression inputExpression,
        TranslationContext? parentContext = null,
        string stack = "translation context")
        : base(type, parentContext, stack)
    {
        Expression = inputExpression;
    }

    /// <summary>
    /// Creates a standardized parameter name for use in expressions.
    /// </summary>
    /// <returns>A string representing a parameter name.</returns>
    public string CreateParameterName()
    {
        return "x";
    }

    /// <summary>
    /// Creates a new parameter expression based on the current context's type.
    /// </summary>
    /// <returns>A new <see cref="ParameterExpression"/>.</returns>
    public ParameterExpression CreateParameterExpression()
    {
        return Expression.Parameter(Type);
    }

    /// <summary>
    /// Creates a child context.
    /// </summary>
    /// <returns>A new <see cref="TranslationContext"/> instance representing the child context.</returns>
    public TranslationContext CreateChildContext()
    {
        return new(Type, Expression, this);
    }

    /// <summary>
    /// Creates a sub-context for translation based on a specified property name, optionally searching parent contexts. <br/>
    /// This method is used to navigate deeper into the object graph of the context's type, creating a new translation context
    /// for a specific property.
    /// </summary>
    /// <param name="propertyName">The property name for which to create a sub-context.</param>
    /// <param name="useParentContexts">If true, the method will search in parent contexts if the property is not found in the current context.</param>
    /// <returns>A new <see cref="TranslationContext"/> instance representing the sub-context for the specified property.</returns>
    /// <exception cref="GeneratorException">Thrown if the specified property is not found within the context hierarchy.</exception>
    /// <exception cref="Exception">Thrown if an unexpected null context is encountered.</exception>
    public TranslationContext CreateChildContext(string propertyName, bool useParentContexts = true)
    {
        PropertyInfo? propertyInfo = null;
        TranslationContext? context = this;

        // Iterate through the context hierarchy to find the property.
        while (context != null)
        {
            propertyInfo = context.Type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo != null)
            {
                break; // Property found.
            }

            context = useParentContexts ? context.ParentContext as TranslationContext : null;
        }

        // Throw an exception if the property is not found.
        if (propertyInfo == null)
        {
            throw new TranslationException($"Property '{propertyName}' not found in the type '{Type.FullName}'. Ensure the property name is correct and exists in the specified type.", this);
        }

        // Create an expression to access the property.
        var subType = propertyInfo.PropertyType;
        var expression = Expression.MakeMemberAccess(context!.Expression, propertyInfo);

        return new TranslationContext(subType, expression, this);
    }

}

/// <summary>
/// A utility class for providing variable names within the context of translations.
/// </summary>
public class VariableNameProvider
{
    private static readonly char[] Alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i' };

    // Additional properties and methods related to variable name generation can be added here.
}
