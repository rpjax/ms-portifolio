using System.Runtime.CompilerServices;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Represents a non-terminal symbol in a context-free grammar.
/// </summary>
public class NonTerminal : Symbol
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public override bool IsTerminal => false;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public override bool IsNonTerminal => true;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public override bool IsEpsilon => false;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public override bool IsMacro => false;

    /// <summary>
    /// Gets the name of the non-terminal symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonTerminal"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the non-terminal symbol.</param>
    public NonTerminal(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot be empty.");
        }
        if (name.Contains(" "))
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot contain spaces.");
        }
        if (name == GreekLetters.Epsilon.ToString())
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot be epsilon.");
        }

        Name = name;
    }

    public static implicit operator NonTerminal(string name)
    {
        return new NonTerminal(name);
    }

    /// <summary>
    /// Returns a string representation of the non-terminal symbol.
    /// </summary>
    /// <returns>A string representation of the non-terminal symbol.</returns>
    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Symbol);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Name.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(Symbol? other)
    {
        return other is NonTerminal nonTerminal
            && nonTerminal.Name == Name;
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
        return Name.ToPascalCase();
    }

    private string ToBnfNotation()
    {
        return $"<{Name}>";
    }

    private string ToEbnfNotation()
    {
        return Name;
    }

    private string ToEbnfKleeneNotation()
    {
        return Name;
    }
}
