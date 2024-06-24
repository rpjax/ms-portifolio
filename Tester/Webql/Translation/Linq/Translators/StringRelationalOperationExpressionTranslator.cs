﻿using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq;

public static class StringRelationalOperationExpressionTranslator
{
    public static Expression TranslateStringRelationalOperationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Like:
                return TranslateLikeExpression(context, node);

            case WebqlOperatorType.RegexMatch:
                return TranslateRegexMatchExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateLikeExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateRegexMatchExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



