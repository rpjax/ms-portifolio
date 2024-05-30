using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Semantics.Components;
using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Semantics.Components;

/*
 * The visitor job: accepts a ExpressionType and adds it FullName to the root lambda's argument symbol.
 */
public class RootLambdasArgumentTypeFixer : FirstSemanticPass
{
    private Type[] ArgumentsTypes { get; }
    private bool ArgumentsFoud { get; set; }

    public RootLambdasArgumentTypeFixer(
        Type[] argumentsTypes
    ) 
    : base(new SemanticContext())
    {
        ArgumentsTypes = argumentsTypes;
    }

    public void Execute(AxiomSymbol symbol)
    {
        if (symbol.Lambda is null)
        {
            return;
        }

        TraverseTree(symbol);

        if (!ArgumentsFoud)
        {
            throw new Exception();
        }
    }

    protected override void OnVisit(Symbol symbol)
    {
        base.OnVisit(symbol);

        if(symbol is not LambdaExpressionSymbol lambda)
        {
            return;
        }

        if (ArgumentsTypes.Length != lambda.Parameters.Length)
        {
            throw new Exception();
        }

        for (int i = 0; i < lambda.Parameters.Length; i++)
        {
            var arg = lambda.Parameters[i];
            var type = ArgumentsTypes[i];

            if (type.AssemblyQualifiedName is null)
            {
                throw new Exception();
            }

            arg.SetType(type.AssemblyQualifiedName);
        }

        ArgumentsFoud = true;
        Stop = true;
    }

}
