using System.Text;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the lexical context of the lexer.
/// </summary>
public class LexicalContext : IDisposable
{
    public int Position { get; private set; }
    public int Line { get; private set; }
    public int Column { get; private set; }

    private IEnumerable<char> Source { get; }
    private IEnumerator<char> Enumerator { get; }

    private bool IsInit { get; set; }
    private bool EndReached { get; set; }
    private StringBuilder AccumulatorBuilder { get; }

    public LexicalContext(IEnumerable<char> source)
    {
        Position = 0;
        Line = 1;
        Column = 1;

        Source = source;
        Enumerator = source.GetEnumerator();

        IsInit = false;
        EndReached = false;
        AccumulatorBuilder = new StringBuilder(100);
    }

    public char? CurrentChar => GetCurrentChar();
    public string AccumulatorValue => AccumulatorBuilder.ToString();

    public void Dispose()
    {
        Enumerator.Dispose();
    }

    public LexicalContext Init()
    {
        if (IsInit)
        {
            throw new InvalidOperationException("The lexer context is already initialized.");
        }

        IsInit = true;
        Advance();
        return this;
    }

    /// <summary>
    /// Gets the next character in the source text without advancing the enumerator.
    /// </summary>
    /// <returns></returns>
    public char? GetCurrentChar()
    {
        if (EndReached)
        {
            return null;
        }

        return Enumerator.Current;
    }

    /// <summary>
    /// Adds the current character to the accumulator and advances the enumerator by one character.
    /// </summary>
    /// <returns><c>true</c> if a character is read successfully; otherwise, <c>false</c>.</returns>
    public bool Consume()
    {
        if(EndReached)
        {
            return false;
        }

        LoadCurrentCharToAccumulator();
        UpdateMetadata();
        Advance();
        return true;
    }

    /// <summary>
    /// Advances the enumerator by one character without adding it to the accumulator.
    /// </summary>
    /// <returns></returns>
    public bool Skip()
    {
        if(AccumulatorBuilder.Length != 0)
        {
            throw new InvalidOperationException("Invalid skip operation. The lexer character accumulator is not empty.");
        }
        if (EndReached)
        {
            return false;
        }

        UpdateMetadata();
        Advance();
        return true;
    }

    /// <summary>
    /// Resets the character accumulator.
    /// </summary>
    public void ResetAccumulator()
    {
        AccumulatorBuilder.Clear();
    }

    /*
     * private methods.
     */

    private bool Advance()
    {
        EndReached = !Enumerator.MoveNext();
        return !EndReached;
    }

    private void UpdateMetadata()
    {
        Position++;
        Column++;

        if (Enumerator.Current == '\n')
        {
            Line++;
            Column = 1;
        }
    }

    private void LoadCurrentCharToAccumulator()
    {
        AccumulatorBuilder.Append(Enumerator.Current);
    }

    internal TokenMetadata GetMetadata()
    {
        return new TokenMetadata(
            startPosition: Position - AccumulatorBuilder.Length,
            endPosition: Position,
            line: Line,
            column: Column
        );
    }

}
