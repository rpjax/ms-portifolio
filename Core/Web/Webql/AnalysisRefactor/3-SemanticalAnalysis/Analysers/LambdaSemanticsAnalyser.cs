using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaSemanticsAnalyser : SemanticAnalyserBase
{
    public LambdaSemantic AnalyseLambda(SemanticContext context, LambdaSymbol symbol)
    {
        var paramsTypes = SemanticAnalyser.AnalyseLambdaArguments(context, symbol.Arguments)
            .Select(x => x.Type)
            .ToArray();

        var bodySemantics = SemanticAnalyser.AnalyseStatementBlock(context, symbol.Body);

        return new LambdaSemantic(
            parameterTypes: paramsTypes,
            returnType: bodySemantics.ResolvedType
        );
    }
}
