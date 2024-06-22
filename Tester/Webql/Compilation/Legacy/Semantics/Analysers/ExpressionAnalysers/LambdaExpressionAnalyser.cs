using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class LambdaExpressionAnalyzer
{
    public static LambdaExpressionSemantic AnalyzeLambdaExpression(
        SemanticContextOld context,
        LambdaExpressionSymbol symbol)
    {
        var paramsSemantics = SemanticAnalyzer.AnalyzeDeclarations(context, symbol.Parameters)
            .ToArray();

        var paramsTypes = paramsSemantics
            .Select(x => x.Type)
            .ToArray();

        //for (int i = 0; i < symbol.Parameters.Length; i++)
        //{
        //    var param = symbol.Parameters[i];
        //    var semantic = paramsSemantics[i];

        //    param.AddDeclaration(context, param.Identifier, semantic.ExpressionType);
        //}

        var bodySemantic = SemanticAnalyzer.AnalyzeStatementBlock(
            context: context.GetSymbolContext(symbol.Body),
            symbol: symbol.Body
        );

        return new LambdaExpressionSemantic(
            parameterTypes: paramsTypes,
            returnType: bodySemantic.ReturnType
        );
    }

}
