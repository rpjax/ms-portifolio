using Aidan.Webql.Analysis;
using Aidan.Webql.Synthesis.Compilation.LINQ;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Represents the context for a translation process within the WebQL framework.
/// This class encapsulates information about the current state of translation, including
/// the type being translated, the current expression, and the hierarchy of translation contexts.
/// </summary>
[Obsolete("Use TranslationContext instead.")]
public class TranslationContextOld : SemanticContextOld
{
    public TranslationContextOld? ParentTranslationContext { get; }

    /// <summary>
    /// Gets the current production being translated.
    /// </summary>
    public SymbolProduction Production { get; }

    public TranslationTable TranslationTable { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the current context is for a projection operation.
    /// </summary>
    /// <value>
    /// <c>true</c> if this context is used for a projection operation; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// This property is used to distinguish contexts that are specifically handling projection operations
    /// (e.g., translating a '$project' operator in WebQL to a LINQ 'Select' expression). It affects how expressions
    /// are generated and interpreted within the framework, ensuring that operations appropriate for projections
    /// are applied.
    /// </remarks>
    public bool IsProjectionContext { get; set; }

    public TranslationContextOld(
        SymbolProduction production,
        TranslationContextOld? parentContext = null
    )
    : base(parentContext)
    {
        ParentTranslationContext = parentContext;
        Production = production;
        IsProjectionContext = parentContext?.IsProjectionContext ?? false;      
    }

    /// <summary>
    /// Creates a child context.
    /// </summary>
    /// <returns>A new <see cref="TranslationContextOld"/> instance representing the child context.</returns>
    public TranslationContextOld CreateTranslationContext(SymbolProduction production)
    {
        return new TranslationContextOld(production, this);
    }

}

/// <summary>
/// Represents a specialized translation context used for projection operations within the WebQL framework.
/// </summary>
/// <remarks>
/// This context extends <see cref="TranslationContextOld"/> by setting <see cref="TranslationContextOld.IsProjectionContext"/>
/// to <c>true</c>, indicating that it is specifically handling a projection operation. This distinction is important
/// for the translation process, as it enables the application of logic and expressions unique to projection scenarios.
/// </remarks>
public class ProjectionTranslationContext : TranslationContextOld
{
    public ProjectionTranslationContext(TranslationContextOld? parentContext = null)
        : base(new SymbolProduction("projection-expression", new List<SymbolTypeCollection>()))
    {
        IsProjectionContext = true;
    }
}
