using System.Text;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Components;

public class LexicalContext : IDisposable
{
    public int Position { get; private set; }
    public int Line { get; private set; }
    public int Column { get; private set; }
    public string CurrentState { get; private set; }
    public char? InputChar { get => GetInputChar(); }
    public string? InputString { get => GetInput(); }
    public string Accumulator { get => AccumulatorBuilder.ToString(); }
    public string AccumulatorInput { get => $"{Accumulator}{InputString}"; }
    public string CurrentStateInput { get => $"{CurrentState}{InputString}"; }

    private IEnumerable<char> Source { get; }
    private IEnumerator<char> Enumerator { get; }
    private bool IsInit { get; set; }
    private bool EndReached { get; set; }
    private StringBuilder AccumulatorBuilder { get; }

    public LexicalContext(IEnumerable<char> source)
    {
        Source = source;
        Enumerator = source.GetEnumerator();
        AccumulatorBuilder = new StringBuilder(100);
        CurrentState = "";
    }

    public void Dispose()
    {
        Enumerator.Dispose();
    }

    public LexicalContext Init()
    {
        if (IsInit)
        {
            return this;
        }

        IsInit = true;
        EndReached = !Enumerator.MoveNext();
        return this;
    }

    /// <summary>
    /// Reads the next character from the source and adds it to the accumulator.
    /// </summary>
    /// <returns><c>true</c> if a character is read successfully; otherwise, <c>false</c>.</returns>
    public bool Read()
    {
        if(!EndReached)
        {
            AccumulatorBuilder.Append(Enumerator.Current);
        }

        EndReached = !Enumerator.MoveNext();

        if (EndReached)
        {
            return false;
        }
        else
        {
            Position++;
            Column++;

            if (Enumerator.Current == '\n')
            {
                Line++;
                Column = 0;
            }

            return true;
        }        
    }

    public bool Skip()
    {
        EndReached = !Enumerator.MoveNext();

        if (EndReached)
        {
            return false;
        }
        else
        {
            Position++;
            Column++;

            if (Enumerator.Current == '\n')
            {
                Line++;
                Column = 0;
            }

            return true;
        }
    }

    public void SetState(string state)
    {
        CurrentState = state;
    }

    public void ResetAccumulator()
    {
        AccumulatorBuilder.Clear();
    }

    /*
     * private methods.
     */

    private char? GetInputChar()
    {
        if (EndReached)
        {
            return null;
        }

        return Enumerator.Current;
    }

    private string? GetInput()
    {
        if (EndReached)
        {
            return null;
        }

        return Enumerator.Current.ToString();
    }

    internal TokenInfo GetMetadata()
    {
        return new TokenInfo(Position, Line, Column);
    }

    internal TokenInfo GetEoiMetadata()
    {
        return new TokenInfo(Position + 1, Line, Column);
    }
}
