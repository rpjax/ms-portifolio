namespace ModularSystem.Core.TextAnalysis.Language.Components;

public abstract class ProductionMacro : Symbol
{
    /// <inheritdoc/>
    public override bool IsTerminal => false;

    /// <inheritdoc/>
    public override bool IsNonTerminal => false;

    /// <inheritdoc/>
    public override bool IsEpsilon => false;

    /// <inheritdoc/>
    public override bool IsMacro => true;

    /// <inheritdoc/>
    public override bool IsEoi => false;

    public abstract MacroType MacroType { get; }

    public abstract IEnumerable<Sentence> Expand(NonTerminal nonTerminal);
}

