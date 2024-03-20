using System.Linq.Expressions;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Represents the semantic context within the analysis phase of a WebQL query. <br/>
/// This class holds the contextual information required to understand and interpret each part of the syntax tree.
/// </summary>
public class SemanticContext
{
    /// <summary>
    /// Gets the parent context of the current context, if any.
    /// </summary>
    public SemanticContext? ParentSemanticContext { get; }

    public SymbolTable SymbolTable { get; init; } = new();

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
    /// <param name="parentContext">The parent semantic context, if any.</param>
    public SemanticContext(SemanticContext? parentContext = null)
    {
        ParentSemanticContext = parentContext;
        SymbolTable = parentContext?.SymbolTable.Copy() ?? new SymbolTable();
        EnableNavigation = parentContext?.EnableNavigation ?? true;
        EnableImplicitAndSyntax = parentContext?.EnableImplicitAndSyntax ?? true;
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

    public SemanticContext CreateSemanticContext()
    {
        return new SemanticContext(this);
    }

    public bool ContainsSymbol(string identifier)
    {
        return SymbolTable.ContainsSymbol(identifier);
    }

    public SymbolOld? TryGetSymbol(string identifier)
    {
        return SymbolTable.TryGetSymbol(identifier);
    }

    public SymbolOld GetSymbol(string identifier)
    {
        return TryGetSymbol(identifier) ?? throw new SemanticException("", this);
    }

    public SymbolOld SetSymbol(string identifier, Expression expression, bool canWrite = true)
    {
        if(!SymbolTable.CanWriteSymbol(identifier))
        {
            throw new SemanticException("", this);
        }

        return SymbolTable.SetSymbol(identifier, expression, canWrite);
    }

}
