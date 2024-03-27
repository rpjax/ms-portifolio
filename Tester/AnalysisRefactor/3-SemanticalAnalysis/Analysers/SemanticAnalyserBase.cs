namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public abstract class SemanticAnalyserBase
{
    public Type GetType(SemanticContext context, string identifier)
    {
        return SemanticAnalyser.GetType(context, identifier);
    }
}
