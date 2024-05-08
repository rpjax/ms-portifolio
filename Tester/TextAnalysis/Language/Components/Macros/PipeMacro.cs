using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class PipeMacro : ProductionMacro
{
    public override MacroType MacroType => MacroType.Pipe;

    public PipeMacro()
    {

    }

    public override string ToString()
    {
        return "|";
    }

    public override bool Equals(object? obj)
    {
        return obj is PipeMacro;
    }

    public override bool Equals(Symbol? other)
    {
        return other is PipeMacro;
    }

    public override bool Equals(Symbol? x, Symbol? y)
    {
        return x is PipeMacro && y is PipeMacro;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ MacroType.GetHashCode();

            return hash;
        }
    }

    public override int GetHashCode([DisallowNull] Symbol obj)
    {
        return obj.GetHashCode();
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        throw new InvalidOperationException();
    }

    public override string ToNotation(NotationType notation)
    {
        throw new InvalidOperationException();
    }
}

