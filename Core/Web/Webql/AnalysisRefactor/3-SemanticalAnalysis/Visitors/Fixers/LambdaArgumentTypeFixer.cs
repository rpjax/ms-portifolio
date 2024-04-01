using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Syntax;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

//*
// The second semantic fix:
//
// The visitor job: Recursively analyses statement blocks looking for query or aggregation expressions,
// where a lambda is accepted as argument.
// EBNF Ex: filter_expr = destination, query_arg, lambda;
// If the lambda passed as argument to the expression contains an argument (lambda_arg) with no Type value set,
// it resolves the query argument Type as an IEnumerable<T> or IQueryable<T>, and it retrives it's element type T and assignes
// its FullName property to the Type property of the lambda argument symbol (lambda_arg.Type). 
//*

public class LambdaArgumentTypeFixer : BasicSemanticVisitor
{
    private bool UseRecursiveVisitor { get; set; }

    public void Execute(LambdaExpressionSymbol symbol)
    {
        VisitLambdaExpression(new SemanticContext(), symbol);
    }

    protected override StatementBlockSymbol VisitStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        if (UseRecursiveVisitor)
        {
            return new LambdaArgumentTypeFixer()
                .VisitStatementBlock(context, symbol);
        }

        return base.VisitStatementBlock(context, symbol);
    }

    protected override ExpressionSymbol VisitOperatorExpression(SemanticContext context, OperatorExpressionSymbol symbol)
    {
        //var isQueryOperator = OperatorHelper
        //    .GetQueryOperators()
        //    .Contains(symbol.Operator);

        if (symbol is PredicateExpressionSymbol queryExpression)
        {
            var source = queryExpression.Source;
            var lambda = queryExpression.Lambda;

            ApplyFix(context, source, lambda);
        }

        return base.VisitOperatorExpression(context, symbol);
    }

    protected void ApplyFix(SemanticContext context, ExpressionSymbol source, LambdaExpressionSymbol lambdaSymbol)
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

        var sourceSemantic = SemanticAnalyser.AnalyseExpression(context, source);

        if(sourceSemantic.IsNotQueryable(context))
        {
            throw new Exception();
        }

        var elementType = sourceSemantic.GetElementType(context);
        var arg = args[0];

        arg.SetType(elementType.AssemblyQualifiedName!);
    }

}
