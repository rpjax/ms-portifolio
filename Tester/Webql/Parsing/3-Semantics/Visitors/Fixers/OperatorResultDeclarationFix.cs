using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Visitors.Fixers;

public class OperatorResultDeclarationFix : AstSemanticVisitorOld
{
    protected override ExpressionSymbol VisitExpression(SemanticContextOld context, ExpressionSymbol symbol)
    {
        if(symbol is IResultProducerOperatorExpressionSymbol resultProducer)
        {
            var destination = resultProducer.Destination;

            if(destination is not StringSymbol stringSymbol)
            {
                return base.VisitExpression(context, symbol);
            } 

            var semantic = SemanticAnalyser.AnalyseExpression(context, symbol);

            if (stringSymbol.Value is null)
            {
                throw new Exception();
            }

            
        }   

        return base.VisitExpression(context, symbol);
    }

}
