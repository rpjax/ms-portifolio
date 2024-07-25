﻿using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class StatementTranslator
{
    public Expression? TranslateStatement(TranslationContext context, StatementSymbol symbol)
    {
        if(symbol is OperatorExpressionSymbol exprSymbol)
        {
            return new OperatorExpressionTranslator()
                .TranslateOperatorExpression(context, exprSymbol);
        }

        throw new Exception();
    }
}
