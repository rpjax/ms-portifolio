using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;
using System.Numerics;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class ArithmeticOperatorExpressionAnalyzer
{
    public static OperatorExpressionSemantic AnalyzeArithmeticOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        if (symbol is not ArithmeticOperatorExpressionSymbol arithmeticExpression)
        {
            throw new Exception();
        }

        var leftSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(arithmeticExpression.LeftOperand), 
            symbol: arithmeticExpression.LeftOperand
        );

        var rightSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(arithmeticExpression.RightOperand),
            symbol: arithmeticExpression.RightOperand
        );

        var leftType = leftSemantic.Type;
        var rightType = rightSemantic.Type;

        if (SemanticsHelper.TypeIsNullable(leftType))
        {
            leftType = SemanticsHelper.GetNullableUnderlyingType(leftType);
        }
        if (SemanticsHelper.TypeIsNullable(rightType))
        {
            rightType = SemanticsHelper.GetNullableUnderlyingType(rightType);
        }

        if (!SemanticsHelper.TypeIsNumber(leftType))
        {
            throw new Exception();
        }
        if (!SemanticsHelper.TypeIsNumber(rightType))
        {
            throw new Exception();
        }

        var type = ResolveType(leftType, rightType);

        return new OperatorExpressionSemantic(
            type: type
        );
    }

    /// <summary>
    /// Resolves the type for arithmetic operations between two operands.
    /// </summary>
    /// <param name="leftType">The type of the left operand.</param>
    /// <param name="rightType">The type of the right operand.</param>
    /// <returns>The resolved type for the arithmetic operation.</returns>
    private static Type ResolveType(Type leftType, Type rightType)
    {
        var anyIsFloat = SemanticsHelper.TypeIsFloatNumber(leftType) || SemanticsHelper.TypeIsFloatNumber(rightType);
        var anyIsInt = SemanticsHelper.TypeIsIntNumber(leftType) || SemanticsHelper.TypeIsIntNumber(rightType);

        if (anyIsFloat)
        {
            return ResolveFloatType(leftType, rightType);
        }
        if (anyIsInt)
        {
            return ResolveIntType(leftType, rightType);
        }

        throw new Exception();
    }

    private static Type ResolveFloatType(Type leftType, Type rightType)
    {
        var anyIsDecimal = leftType == typeof(decimal) || rightType == typeof(decimal);
        var anyIsDouble = leftType == typeof(double) || rightType == typeof(double);
        var anyIsFloat = leftType == typeof(float) || rightType == typeof(float);

        if (anyIsDecimal)
        {
            return typeof(decimal);
        }
        if (anyIsDouble)
        {
            return typeof(double);
        }
        if (anyIsFloat)
        {
            return typeof(float);
        }

        throw new Exception();
    }

    private static Type ResolveIntType(Type leftType, Type rightType)
    {
        //* 
        //* 
        //* normal cases (does not require type casting).
        //* 

        var is_ulong = leftType == typeof(ulong) || rightType == typeof(ulong);
        var is_long = leftType == typeof(long) || rightType == typeof(long);
        var is_uint = leftType == typeof(uint) || rightType == typeof(uint);
        var is_int = leftType == typeof(int) || rightType == typeof(int);

        if (is_ulong)
        {
            return typeof(ulong);
        }
        if (is_long)
        {
            return typeof(long);
        }
        if (is_uint)
        {
            return typeof(uint);
        }
        if (is_int)
        {
            return typeof(int);
        }

        //* 
        //* 
        //* casting cases.
        //* 

        var is_sbyte = leftType == typeof(sbyte) || rightType == typeof(sbyte);
        var is_byte = leftType == typeof(byte) || rightType == typeof(byte);
        var is_ushort = leftType == typeof(ushort) || rightType == typeof(ushort);
        var is_short = leftType == typeof(short) || rightType == typeof(short);

        if(is_sbyte)
        {
            return typeof(sbyte);
        }
        if(is_byte)
        {
            return typeof(byte);
        }

        if (is_ushort)
        {
            return typeof(ushort);
        }
        if(is_short)
        {
            return typeof(short);
        }

        var is_bigInteger = leftType == typeof(BigInteger) || rightType == typeof(BigInteger);

        if(is_bigInteger)
        {
            return typeof(BigInteger);
        }

        throw new Exception();
    }
}
