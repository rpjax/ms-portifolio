using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;
using System.Reflection;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class SemanticOperatorExpressionAnalyzer
{
    public static OperatorExpressionSemantic AnalyzeSemanticOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetSemanticOperatorType(symbol.Operator))
        {
            //case SemanticOperatorType.Expr:
            //    throw new NotImplementedException();

            //case SemanticOperatorType.Parse:
            //    throw new NotImplementedException();

            case SemanticOperatorType.Type:
                return AnalyzeTypeOperatorExpression(context, (TypeOperatorExpressionSymbol)symbol);

            case SemanticOperatorType.MemberAccess:
                return AnalyzeMemberAccessOperatorExpression(context, (MemberAccessOperatorExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static OperatorExpressionSemantic AnalyzeTypeOperatorExpression(
        SemanticContextOld context,
        TypeOperatorExpressionSymbol symbol)
    {
        var semantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(symbol.TypeExpression),
            symbol: symbol.TypeExpression
        );

        return new OperatorExpressionSemantic(
            type: semantic.Type
        );
    }

    public static OperatorExpressionSemantic AnalyzeMemberAccessOperatorExpression(
        SemanticContextOld context,
        MemberAccessOperatorExpressionSymbol symbol)
    {
        var operandSemantic = SemanticAnalyzer.AnalyzeExpression(
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
