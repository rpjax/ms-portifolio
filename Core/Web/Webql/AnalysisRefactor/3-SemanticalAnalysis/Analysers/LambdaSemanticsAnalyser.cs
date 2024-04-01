using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaSemanticsAnalyser : SemanticAnalyserBase
{
    public LambdaExpressionSemantic AnalyseLambda(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        var paramsTypes = SemanticAnalyser.AnalyseDeclarations(context, symbol.Parameters)
            .Select(x => x.Type)
            .ToArray();

        var bodySemantic = SemanticAnalyser.AnalyseStatementBlock(context, symbol.Body);

        return new LambdaExpressionSemantic(
            parameterTypes: paramsTypes,
            returnType: bodySemantic.ResolvedType
        );
    }
}
