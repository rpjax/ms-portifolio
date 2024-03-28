using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

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

public class LambdaArgumentTypeFixer : AstSemanticVisitor
{
    private bool UseRecursiveVisitor { get; set; }

    public void Execute(LambdaExpressionSymbol symbol)
    {
        var context = new SemanticContext();
        Visit(context, symbol);
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
        if (symbol is QueryExpressionSymbol queryExpression)
        {
            var source = queryExpression.Source;
            var lambda = queryExpression.Lambda;

            ApplyFix(context, source, lambda);
        }

        return base.VisitOperatorExpression(context, symbol);
    }

    protected override DeclarationStatementSymbol VisitDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        //* creates the semantics object.
        var semantic = SemanticAnalyser.AnalyseDeclaration(context, symbol); ;

        //* binds the semantics object to the symbol.
        symbol.AddSemantic(context, semantic);

        //* declares the symbol.
        symbol.AddDeclaration(context, symbol.Identifier);

        return base.VisitDeclaration(context, symbol);
    }

    protected override ReferenceExpressionSymbol VisitReference(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        //* creates the semantics object.
        var semantics = SemanticAnalyser.AnalyseReference(context, symbol);

        //* binds the semantics object to the symbol.
        symbol.AddSemantic(context, semantics);

        return base.VisitReference(context, symbol);
    }

    protected void ApplyFix(SemanticContext context, ExpressionSymbol source, LambdaExpressionSymbol lambdaSymbol)
    {
        var args = lambdaSymbol.Parameters;

        if (!args.Any(x => string.IsNullOrEmpty(x.Type)))
        {
            return;
        }

        if (source is not ReferenceExpressionSymbol reference)
        {
            throw new Exception();
        }
        if (args.Length != 1)
        {
            throw new Exception();
        }

        var arg = args[0];

        if(reference.IsNotQueryable(context))
        {
            throw new Exception();
        }

        var elementType = reference.GetElementType(context);

        arg.SetType(elementType.AssemblyQualifiedName!);

        Console.WriteLine();
    }

}
