using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Semantics.Components;
using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Semantics.Components;

//*
// The second semantic fix:
//
// The visitor job: Recursively analyses statement blocks looking for query or aggregation expressions,
// where a lambda is accepted as argument.
// EBNF Ex: filter_expr = destination, query_arg, lambda;
// If the lambda passed as argument to the expression contains an argument (lambda_arg) with no ExpressionType value set,
// it resolves the query argument ExpressionType as an IEnumerable<T> or IQueryable<T>, and it retrives it's element type T and assignes
// its FullName property to the ExpressionType property of the lambda argument symbol (lambda_arg.ExpressionType). 
//*

public class LambdaArgumentTypeFixer : FirstSemanticPass
{
    public LambdaArgumentTypeFixer() : base(new SemanticContext())
    {
    }

    public void Execute(LambdaExpressionSymbol symbol)
    {
        TraverseTree(symbol);
    }

    protected override void OnVisit(Symbol symbol)
    {
        base.OnVisit(symbol);

        if (symbol is PredicateOperatorExpressionSymbol queryExpression)
        {
            var source = queryExpression.Source;
            var lambda = queryExpression.Lambda;

            ApplyFix(source, lambda);
        }
    }

    protected void ApplyFix(ExpressionSymbol source, LambdaExpressionSymbol lambdaSymbol)
    {
        var args = lambdaSymbol.Parameters;

        if (!args.Any(x => string.IsNullOrEmpty(x.Type)))
        {
            return;
        }
        if (args.Length != 1)
        {
            throw new Exception();
        }

        var sourceSemantic = SemanticAnalyser.AnalyseExpression(Context, source);

        if(sourceSemantic.IsNotQueryable(Context))
        {
            throw new Exception();
        }

        var elementType = sourceSemantic.GetElementType(Context);
        var arg = args[0];

        if(elementType.AssemblyQualifiedName is null)
        {
            throw new InvalidOperationException();
        }

        arg.SetType(elementType.AssemblyQualifiedName!);
    }

}
