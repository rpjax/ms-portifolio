namespace ModularSystem.Core.TextAnalysis.Language.Components;

public abstract class SentenceMacro : ProductionMacro
{
    public Sentence Sentence { get; }

    public SentenceMacro(Sentence sentence)
    {
        Sentence = sentence;

        if (Sentence.Length == 0)
        {
            throw new ArgumentException("The production macro must contain at least one symbol.");
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is SentenceMacro macro
            && macro.Sentence.SequenceEqual(Sentence);
    }

    public override bool Equals(Symbol? other)
    {
        return other is SentenceMacro macro
            && macro.Sentence.SequenceEqual(Sentence);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var symbol in Sentence)
            {
                hash = (hash * 16777619) ^ symbol.GetHashCode();
            }

            return hash;
        }
    }

}

