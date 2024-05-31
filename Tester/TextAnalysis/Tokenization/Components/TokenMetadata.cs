namespace ModularSystem.Core.TextAnalysis.Tokenization;

/// <summary>
/// Represents the set of information associated with a token.
/// </summary>
public struct TokenMetadata
{
    /// <summary>
    /// Gets the character startPosition in the source text where the token starts. (0-based)
    /// </summary>
    public int StartPosition { get; }

    /// <summary>
    /// Gets the character startPosition in the source text where the token ends. (0-based)
    /// </summary>
    public int EndPosition { get; }

    /// <summary>
    /// Gets the line number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="TokenMetadata"/> struct.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPosition"></param>
    /// <param name="line"></param>
    /// <param name="column"></param>
    public TokenMetadata(int startPosition, int endPosition, int line, int column)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Returns a string representation of the metadata.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"line: {Line + 1}, column: {Column + 1}";
    }
}
