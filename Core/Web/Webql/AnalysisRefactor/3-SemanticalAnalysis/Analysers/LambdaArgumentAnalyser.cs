using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaArgumentAnalyser : SemanticAnalyserBase
{
    public LambdaArgumentSemantic AnalyseLambdaArgument(SemanticContext context, LambdaArgumentSymbol symbol)
    {
        return new LambdaArgumentSemantic(
            type: SemanticAnalyser.GetType(context, symbol.Type)
        );
    }
}
