using ModularSystem.Core.TextAnalysis.Tokenization;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Represents a terminal symbol in a context-free grammar.
/// </summary>
public class Terminal : Symbol
{
    /// <inheritdoc/>
    public override bool IsTerminal => true;
    
    /// <inheritdoc/>
    public override bool IsNonTerminal => false;

    /// <inheritdoc/>
    public override bool IsEpsilon => false;

    /// <inheritdoc/>
    public override bool IsMacro => false;

    /// <inheritdoc/>
    public override bool IsEoi => false;

    /// <summary>
    /// Gets the token type associated with the terminal symbol.
    /// </summary>
    public TokenType TokenType { get; }

    /// <summary>
    /// Gets the value associated with the terminal symbol.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified token type and value.
    /// </summary>
    /// <param name="tokenType">The token type associated with the terminal symbol.</param>
    /// <param name="value">The value associated with the terminal symbol.</param>
    public Terminal(TokenType tokenType, string? value = null)
    {
        TokenType = tokenType;
        Value = value;
    }

    /// <summary>
    /// Returns a string representation of the terminal symbol.
    /// </summary>
    /// <returns>A string representation of the terminal symbol.</returns>
    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Symbol);
    }

    public override bool Equals(Symbol? other)
    {
        return other is Terminal terminal
            && terminal.TokenType == TokenType
            && terminal.Value == Value;
    }

    public override bool Equals(Symbol? x, Symbol? y)
    {
        return x is not null 
            && y is not null 
            && x.Equals(y);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ TokenType.GetHashCode();
            hash = (hash * 16777619) ^ (Value?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public override int GetHashCode([DisallowNull] Symbol obj)
    {
        return obj.GetHashCode();
    }

    public override string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return ToSententialNotation();

            case NotationType.Bnf:
                return ToBnfNotation();

            case NotationType.Ebnf:
                return ToEbnfNotation();

            case NotationType.EbnfKleene:
                return ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    private string ToSententialNotation()
    {
        var typeStr = TokenType.ToString();

        if (!string.IsNullOrEmpty(Value))
        {
            return $"\"{Value}\"";
        }

        return typeStr.ToCamelCase();
    }

    private string ToBnfNotation()
    {
        var typeStr = TokenType.ToString();

        if (!string.IsNullOrEmpty(Value))
        {
            return $"\"{Value}\"";
        }

        return typeStr;
    }

    private string ToEbnfNotation()
    {
        var typeStr = TokenType.ToString();

        if (!string.IsNullOrEmpty(Value))
        {
            return $"\"{Value}\"";
        }

        return typeStr;
    }

    private string ToEbnfKleeneNotation()
    {
        var typeStr = TokenType.ToString();

        if (!string.IsNullOrEmpty(Value))
        {
            return $"\"{Value}\"";
        }

        return typeStr;
    }

}

