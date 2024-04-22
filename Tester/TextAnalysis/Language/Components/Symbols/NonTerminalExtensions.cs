namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class NonTerminalExtensions
{
    public static NonTerminal Copy(this NonTerminal nonTerminal)
    {
        return new NonTerminal(nonTerminal.Name);
    }
}