using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class GrammarDefinitionExtensions
{
    public static bool IsLeftRecursive(this Grammar grammar)
    {
        return grammar.Productions
            .Any(x => x.IsLeftRecursive());
    }

    public static bool IsRightRecursive(this Grammar grammar)
    {
        return grammar.Productions
            .Any(x => x.IsRightRecursive());
    }

    public static bool IsNonDeterministic(this Grammar grammar)
    {
        return grammar.Productions
            .Where(x => x.GetTerminalPrefix() is not null)
            .GroupBy(x => x.Head.Name)
            .Any(x => x.GroupBy(y => y.GetTerminalPrefix()).Count() > 1);
    }

    public static Grammar ExpandMacros(this Grammar self)
    {
        self.Productions.ExpandMacros();
        return self;
    }

    public static Grammar AutoClean(this Grammar self)
    {
        self.Productions.RecursiveAutoClean();
        return self;
    }

    public static Grammar AutoFix(this Grammar self)
    {
        self.Productions.RecursiveAutoFix();
        return self;
    }

    public static Grammar AutoTransformLL1(this Grammar self)
    {
        self.Productions.AutoTransformLL1();
        return self;
    }
}
