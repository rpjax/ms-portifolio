using Webql.Core.Linq;
using Webql.Parsing.Analysis;
using Webql.Translation.Linq.Providers;

namespace Webql.Core;

/// <summary>
/// Represents the settings for the WebqlCompiler.
/// </summary>
public class WebqlCompilerSettings
{
    /// <summary>
    /// Gets or sets the WebqlLinqProvider used for compiling WebQL queries.
    /// </summary>
    public IWebqlLinqProvider LinqProvider { get; }

    /// <summary>
    /// Gets or sets the array of pre-validation syntax tree visitors.
    /// </summary>
    public ISyntaxTreeVisitor[] PreValidationVisitors { get; }

    /// <summary>
    /// Gets or sets the array of post-validation syntax tree visitors.
    /// </summary>
    public ISyntaxTreeVisitor[] PostValidationVisitors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlCompilerSettings"/> class.
    /// </summary>
    /// <param name="linqProvider">The WebqlLinqProvider to use for compiling WebQL queries.</param>
    /// <param name="preValidationVisitors">The array of pre-validation syntax tree visitors.</param>
    /// <param name="postValidationVisitors">The array of post-validation syntax tree visitors.</param>
    public WebqlCompilerSettings(
        IWebqlLinqProvider? linqProvider = null,
        ISyntaxTreeVisitor[]? preValidationVisitors = null,
        ISyntaxTreeVisitor[]? postValidationVisitors = null)
    {
        LinqProvider = linqProvider ?? new WebqlLinqProvider();
        PreValidationVisitors = preValidationVisitors ?? Array.Empty<ISyntaxTreeVisitor>();
        PostValidationVisitors = postValidationVisitors ?? Array.Empty<ISyntaxTreeVisitor>();
    }
}
