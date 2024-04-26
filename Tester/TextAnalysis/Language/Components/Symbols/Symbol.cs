namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Abstract base class for production symbols in a context-free grammar.
/// </summary>
public abstract class Symbol : IEquatable<Symbol>
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public abstract bool IsTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public abstract bool IsNonTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public abstract bool IsEpsilon { get; }

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public abstract bool IsMacro { get; }

    public static bool operator ==(Symbol left, Symbol right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Symbol left, Symbol right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Returns a string representation of the production symbol.
    /// </summary>
    /// <returns>A string representation of the production symbol.</returns>
    public abstract override string ToString();

    public abstract string ToNotation(NotationType notation);

    public abstract override bool Equals(object? obj);

    public abstract override int GetHashCode();

    public abstract bool Equals(Symbol? other);

    public Terminal AsTerminal()
    {
        if (this is not Terminal terminal)
        {
            throw new InvalidCastException("The production symbol is not a terminal symbol.");
        }

        return terminal;
    }

    public NonTerminal AsNonTerminal()
    {
        if (this is not NonTerminal nonTerminal)
        {
            throw new InvalidCastException("The production symbol is not a non-terminal symbol.");
        }

        return nonTerminal;
    }

    public ProductionMacro AsMacro()
    {
        if (this is not ProductionMacro productionMacro)
        {
            throw new InvalidCastException("The production symbol is not a macro.");
        }

        return productionMacro;
    }

}

