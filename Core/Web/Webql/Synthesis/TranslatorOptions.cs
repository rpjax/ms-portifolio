namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Contains options for translating WebQL nodes into LINQ expressions.
/// </summary>
public class TranslatorOptions
{
    /// <summary>
    /// Gets or sets the LINQ provider used for translating nodes.
    /// </summary>
    public LinqProvider LinqProvider { get; set; } = new LinqProvider();

    /// <summary>
    /// Indicates whether the 'Take' operation supports Int64.
    /// </summary>
    public bool TakeSupportsInt64 { get; set; } = false;

    /// <summary>
    /// Indicates whether the 'Skip' operation supports Int64.
    /// </summary>
    public bool SkipSupportsInt64 { get; set; } = false;

    /// <summary>
    /// Retrieves the queryable type used by the LINQ provider.
    /// </summary>
    public Type QueryableType => LinqProvider.GetQueryableType();

    /// <summary>
    /// Initializes a new instance of the TranslatorOptions class.
    /// </summary>
    public TranslatorOptions()
    {
    }
}
