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
        var operatorType = OperatorHelper.GetSemanticOperatorType(symbol.Operator);

        switch (operatorType)
        {
            case SemanticOperatorType.Expr:
                break;
            case SemanticOperatorType.Parse:
                break;
            case SemanticOperatorType.SelectOld:
                break;
            case SemanticOperatorType.AnonymousType:
                break;
            case SemanticOperatorType.MemberAccess:
                return AnalyseMemberAccessOperatorExpression(context, (MemberAccessExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static 

    public static OperatorExpressionSemantic AnalyseMemberAccessOperatorExpression(
        SemanticContext context,
        MemberAccessExpressionSymbol symbol)
    {
        var operandSemantic = SemanticAnalyser.AnalyseExpression(context, symbol.Operand);
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
