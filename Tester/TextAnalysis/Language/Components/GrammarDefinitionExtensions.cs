namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class GrammarDefinitionExtensions
{
    public static ProductionRewrite[] ExpandMacros(this GrammarDefinition self)
    {
        return self.Productions.ExpandMacros();
    }

    public static ProductionRewrite[] AutoClean(this GrammarDefinition self)
    {
        return self.Productions.RecursiveAutoClean();
    }

    public static ProductionRewrite[] AutoFix(this GrammarDefinition self)
    {
        return self.Productions.RecursiveAutoFix();
    }
}