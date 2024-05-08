using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Represents the end-of-input symbol.
/// </summary>
public sealed class Eoi : Symbol
{
    /// <inheritdoc/>
    public override bool IsTerminal => false;

    /// <inheritdoc/>
    public override bool IsNonTerminal => false;

    /// <inheritdoc/>
    public override bool IsEpsilon => false;

    /// <inheritdoc/>
    public override bool IsMacro => false;

    /// <inheritdoc/>
    public override bool IsEoi => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Eoi"/> class.
    /// </summary>
    public Eoi()
    {
    }

    public static Eoi Instance { get; } = new Eoi();

    /// <summary>
    /// Returns a string representation of the end-of-input symbol.
    /// </summary>
    /// <returns>A string representation of the end-of-input symbol.</returns>
    public override string ToString()
    {
        return "$";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Symbol);
    }

    public override bool Equals(Symbol? other)
    {
        return other is Eoi;
    }

    public override bool Equals(Symbol? x, Symbol? y)
    {
        return x is Eoi && y is Eoi;
    }

    public override int GetHashCode([DisallowNull] Symbol obj)
    {
        return obj.GetHashCode();
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

    public override string ToNotation(NotationType notation)
    {
        return "$";
    }

}