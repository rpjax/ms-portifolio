﻿using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ReferenceParser : SyntaxParserBase
{
    public ExpressionSymbol ParseReference(ParsingContext context, StringToken token)
    {
        var value = token.Value;

        if (!value.Contains("."))
        {
            return new ReferenceExpressionSymbol(value);
        }

        // $item.nickname.value
        var split = value.Split('.').ToList();
        var root = split.First();
        split.RemoveAt(0);

        var reference = new ReferenceExpressionSymbol(root);
        var expression = null as ExpressionSymbol;

        foreach (var item in split)
        {
            expression = new MemberAccessOperatorExpressionSymbol(item, reference);
        }

        if (expression is null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }
}
