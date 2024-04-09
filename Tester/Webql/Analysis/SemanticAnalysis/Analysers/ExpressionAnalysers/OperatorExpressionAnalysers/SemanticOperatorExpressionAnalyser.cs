using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;
using System.Reflection;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class SemanticOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseSemanticOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetSemanticOperatorType(symbol.Operator))
        {
            //case SemanticOperatorType.Expr:
            //    throw new NotImplementedException();

            //case SemanticOperatorType.Parse:
            //    throw new NotImplementedException();

            case SemanticOperatorType.Type:
                return AnalyseTypeOperatorExpression(context, (TypeOperatorExpressionSymbol)symbol);

            case SemanticOperatorType.MemberAccess:
                return AnalyseMemberAccessOperatorExpression(context, (MemberAccessOperatorExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static OperatorExpressionSemantic AnalyseTypeOperatorExpression(
        SemanticContext context,
        TypeOperatorExpressionSymbol symbol)
    {
        var semantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(symbol.TypeExpression),
            symbol: symbol.TypeExpression
        );

        return new OperatorExpressionSemantic(
            type: semantic.Type
        );
    }

    public static OperatorExpressionSemantic AnalyseMemberAccessOperatorExpression(
        SemanticContext context,
        MemberAccessOperatorExpressionSymbol symbol)
    {
        var operandSemantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(symbol.Operand),
            symbol: symbol.Operand
        );

        var operandType = operandSemantic.Type;
        var memberName = symbol.MemberName;

        var members = operandType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name.ToLower() == memberName)
            .ToArray();

        if (members.Length == 0)
        {
            throw new Exception();
        }
        if (members.Length > 1)
        {
            throw new Exception();
        }

        var member = members[0];

        if (member.PropertyType is null)
        {
            throw new Exception();
        }
     
        return new OperatorExpressionSemantic(
            type: member.PropertyType
        );
    }

}
