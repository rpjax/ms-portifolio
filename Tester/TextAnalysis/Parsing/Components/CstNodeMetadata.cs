namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents the position of a syntatical element in the source code.
/// </summary>
public struct SyntaxElementPosition
{
    /// <summary>
    /// Gets the character index position of the element in the source code.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the line number of the element in the source code.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number of the element in the source code.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="SyntaxElementPosition"/> class.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="line"></param>
    /// <param name="column"></param>
    public SyntaxElementPosition(int index, int line, int column)
    {
        Index = index;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Returns a string representation of the position.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Line: {Line}, Char: {Column}, Index: {Index}";
    }
}

/// <summary>
/// Represents the metadata associated with a node in the concrete syntax tree (CST).
/// </summary>
public class CstNodeMetadata
{
    /// <summary>
    /// Gets the start position of the node in the source text.
    /// </summary>
    public SyntaxElementPosition StartPosition { get; }

    /// <summary>
    /// Gets the end position of the node in the source text.
    /// </summary>
    public SyntaxElementPosition EndPosition { get; }

    ///// <summary>
    ///// Gets the collection of tokens that represents the node in the source text.
    ///// </summary>
    //public Token[] Tokens { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstNodeMetadata"/> class.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <param name="tokens"></param>
    public CstNodeMetadata(SyntaxElementPosition startPosition, SyntaxElementPosition endPosition/*, Token[] tokens*/)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        //Tokens = tokens;
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
