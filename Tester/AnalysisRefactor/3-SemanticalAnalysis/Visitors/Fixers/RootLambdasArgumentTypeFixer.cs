using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

//*
// The first semantic fix:
//
// The visitor job: accepts a Type and adds it FullName to the root lambda's argument symbol.
//*

public class RootLambdasArgumentTypeFixer : AstSemanticVisitor
{
    private Type[] ArgumentsTypes { get; }
    private bool ArgumentsFoud { get; set; }

    public RootLambdasArgumentTypeFixer(Type[] argumentsTypes)
    {
        ArgumentsTypes = argumentsTypes;
    }

    public void Execute(AxiomSymbol symbol)
    {
        if (symbol.Lambda is null)
        {
            return;
        }

        VisitLambda(new SemanticContext(), symbol.Lambda);

        if (!ArgumentsFoud)
        {
            throw new Exception();
        }
    }

    protected override LambdaSymbol VisitLambda(SemanticContext context, LambdaSymbol symbol)
    {
        if (ArgumentsTypes.Length != symbol.Parameters.Length)
        {
            throw new Exception();
        }

        for (int i = 0; i < symbol.Parameters.Length; i++)
        {
            var arg = symbol.Parameters[i];
            var type = ArgumentsTypes[i];

            if (type.AssemblyQualifiedName is null)
            {
                throw new Exception();
            }

            arg.SetType(type.AssemblyQualifiedName);
        }

        ArgumentsFoud = true;
        return symbol;
    }
}
