using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Represents a terminal symbol in a context-free grammar.
/// </summary>
public class Terminal : ProductionSymbol
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public override bool IsTerminal => true;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public override bool IsNonTerminal => false;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public override bool IsEpsilon => false;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public override bool IsMacro => false;

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
        return Equals(obj as ProductionSymbol);
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

    public override bool Equals(ProductionSymbol? other)
    {
        return other is Terminal terminal
            && terminal.TokenType == TokenType
            && terminal.Value == Value;
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

