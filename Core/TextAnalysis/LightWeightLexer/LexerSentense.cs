using ModularSystem.Core;
using System.Text;

namespace Core.TextAnalysis;

/// <summary>
/// Represents a sequence of one or more tokens.
/// </summary>
public class LexerSentense
{
    public int Length => Tokens.Count;
    public string FormationString => GetProductionString();

    private StringBuilder StringBuilder { get; } = new();
    private List<LexerToken> Tokens = new(128);

    public LexerSentense(params LexerToken[] tokens)
    {
        foreach (var token in tokens)
        {
            Add(token);
        }
    }

    public LexerSentense ShallowCopy()
    {
        return new LexerSentense(GetTokens());
    }

    public string GetProductionString(string? value = null)
    {
        StringBuilder.Clear();

        foreach (var str in Tokens)
        {
            StringBuilder.Append(str.GetProductionString());
        }

        StringBuilder.Append(value);

        return StringBuilder.ToString();
    }

    public string GetFormationString(LexerToken? value)
    {
        return GetProductionString(value?.GetProductionString());
    }

    public LexerToken[] GetTokens()
    {
        return Tokens.ToArray();
    }

    public T[] GetTokens<T>() where T : LexerToken
    {
        return Tokens.Transform(x => (T)x).ToArray();
    }

    public LexerToken TokenAt(int index)
    {
        return Tokens.ElementAt(index);
    }

    public T TokenAt<T>(int index) where T : LexerToken
    {
        return Tokens.ElementAt(index).TypeCast<T>();
    }

    public LexerToken FirstToken()
    {
        return TokenAt(0);
    }

    public T FirstToken<T>() where T : LexerToken
    {
        return TokenAt<T>(0);
    }

    public LexerToken LastToken()
    {
        return TokenAt(Length - 1);
    }

    public T LastToken<T>() where T : LexerToken
    {
        return TokenAt<T>(Length - 1);
    }

    public override string ToString()
    {
        var strBuilder = new StringBuilder();

        foreach (var token in Tokens)
        {
            strBuilder.Append(token.ToString());
        }

        return strBuilder.ToString();
    }

    public string ToString(string? appendedValue)
    {
        var strBuilder = new StringBuilder();

        foreach (var token in Tokens)
        {
            strBuilder.Append(token.ToString());
        }

        strBuilder.Append(appendedValue);

        return strBuilder.ToString();
    }

    public string ToString(char? appendedValue)
    {
        return ToString($"{appendedValue}");
    }

    public string ToString(LexerToken? appendedValue)
    {
        return ToString($"{appendedValue?.ToString()}");
    }

    public void Clear()
    {
        Tokens.Clear();
    }

    public void RemoveLast()
    {
        if (Length == 0)
        {
            throw new InvalidOperationException();
        }

        Tokens.RemoveAt(Length - 1);
    }

    public void Add(LexerToken token)
    {
        Tokens.Add(token);
    }

    public void Add(char symbol)
    {
        Add(new LiteralToken(symbol));
    }
}
