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
        return ReferenceEquals(obj, this);
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

    public override bool Equals(ProductionSymbol? other)
    {
        return ReferenceEquals(other, this);
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

