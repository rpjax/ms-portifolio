using ModularSystem.TextAnalysis.Parsing.Components;

namespace Webql.Parsing.Ast;

/// <summary>
/// Represents the metadata associated with a syntax node.
/// </summary>
public class WebqlSyntaxNodeMetadata
{
    /// <summary>
    /// Gets the position of the syntax element.
    /// </summary>
    public SyntaxElementPosition Position { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlSyntaxNodeMetadata"/> class.
    /// </summary>
    /// <param name="position">The position of the syntax element.</param>
    public WebqlSyntaxNodeMetadata(SyntaxElementPosition position)
    {
        Position = position;
    }

    /// <summary>
    /// Returns a string representation of the metadata.
    /// </summary>
    /// <returns>A string representation of the metadata.</returns>
    public override string ToString()
    {
        return $"Position: {Position}";
    }
}
