namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Represents an epsilon symbol in a context-free grammar.
/// </summary>
public class Epsilon : Symbol
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
    public override bool IsEpsilon => true;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public override bool IsMacro => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Epsilon"/> class.
    /// </summary>
    public Epsilon()
    {
    }

    /// <summary>
    /// Returns a string representation of the epsilon symbol.
    /// </summary>
    /// <returns>A string representation of the epsilon symbol.</returns>
    public override string ToString()
    {
        return GreekLetters.Epsilon.ToString();
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

            hash = (hash * 16777619) ^ ToString().GetHashCode();
            return hash;
        }
    }

    public override bool Equals(Symbol? other)
    {
        return other is Epsilon;
    }

    public override string ToNotation(NotationType notation)
    {
        return GreekLetters.Epsilon.ToString();
    }
}

