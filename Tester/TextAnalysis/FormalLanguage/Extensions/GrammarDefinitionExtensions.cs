namespace ModularSystem.Core.TextAnalysis.Language;

public static class GrammarDefinitionExtensions
{
    public static bool IsLeftRecursive(this GrammarDefinition grammar)
    {
        return grammar.Productions
            .Any(x => x.IsLeftRecursive());
    }

    public static bool IsRightRecursive(this GrammarDefinition grammar)
    {
        return grammar.Productions
            .Any(x => x.IsRightRecursive());
    }

    public static bool IsNonDeterministic(this GrammarDefinition grammar)
    {
        return grammar.Productions
            .Where(x => x.GetTerminalPrefix() is not null)
            .GroupBy(x => x.Head.Name)
            .Any(x => x.GroupBy(y => y.GetTerminalPrefix()).Count() > 1);
    }
}
