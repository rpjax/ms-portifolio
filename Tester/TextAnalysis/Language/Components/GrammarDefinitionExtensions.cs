namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class GrammarDefinitionExtensions
{
    public static TransformationRecordCollection ExpandMacros(this GrammarDefinition self)
    {
        var transformations = self.Productions.ExpandMacros();
        self.Transformations.Add(transformations);
        return transformations;
    }

    public static TransformationRecordCollection AutoClean(this GrammarDefinition self)
    {
        var transformations = self.Productions.RecursiveAutoClean();
        self.Transformations.Add(transformations);
        return transformations;
    }

    public static TransformationRecordCollection AutoFix(this GrammarDefinition self)
    {
        var transformations = self.Productions.RecursiveAutoFix();
        self.Transformations.Add(transformations);
        return transformations;
    }

    public static TransformationRecordCollection AutoTransform(this GrammarDefinition self)
    {
        var transformations = self.Productions.AutoTransform();
        self.Transformations.Add(transformations);
        return transformations;
    }
}