using ModularSystem.Core.TextAnalysis.Language.Transformations;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class GrammarDefinitionExtensions
{
    public static GrammarDefinition ExpandMacros(this GrammarDefinition self)
    {
        self.Productions.ExpandMacros();    
        return self;
    }

    public static GrammarDefinition AutoClean(this GrammarDefinition self)
    {
        self.Productions.RecursiveAutoClean();
        return self;
    }

    public static GrammarDefinition AutoFix(this GrammarDefinition self)
    {
        self.Productions.RecursiveAutoFix();
        return self;
    }

    public static GrammarDefinition AutoTransformLL1(this GrammarDefinition self)
    {
        self.Productions.AutoTransform();
        return self;
    }
}