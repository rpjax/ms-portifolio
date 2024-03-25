﻿using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class AxiomTranslator
{
    public Expression TranslateAxiom(TranslationContext context, AxiomSymbol symbol)
    {
        return new LambdaTranslator()
            .TranslateLambda(context, symbol.Lambda);
    }
}
