using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace Webql.DocumentSyntax.Parsing.Components;

/// <summary>
/// Represents the metadata associated with a syntax node.
/// </summary>
public class SyntaxNodeMetadata
{
    public SyntaxElementPosition StartPosition { get; }
    public SyntaxElementPosition EndPosition { get; }

    public SyntaxNodeMetadata(SyntaxElementPosition startPosition, SyntaxElementPosition endPosition)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
    }

    /// <summary>
    /// Returns a string representation of the metadata.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Starts At: ({StartPosition}), Ends At: ({EndPosition})";
    }

}
