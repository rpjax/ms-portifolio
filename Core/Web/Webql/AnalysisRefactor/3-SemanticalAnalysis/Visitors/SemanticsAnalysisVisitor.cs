﻿using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

/// <summary>
/// The SemanticsAnalysisVisitor traverses the AST, generating semantic information for each node.
/// This visitor pattern ensures that semantic analysis occurs in a post-order traversal manner,
/// meaning that it first visits all descendant nodes before the node itself. This order is crucial
/// because it allows for the semantic information to bubble up from the leaves to the root of the AST.
/// Each node's semantic information is then stored in the SemanticTable for subsequent phases.
/// </summary>
public class SemanticsAnalysisVisitor : AstSemanticVisitor
{
    public SemanticContext Run(AxiomSymbol symbol, Type[] rootArguments)
    {
        var context = new SemanticContext();

        if (symbol.Lambda is null)
        {
            return context;
        }


        new RootLambdasArgumentTypeFixer(rootArguments)
            .Execute(symbol);

        new LambdaArgumentTypeFixer()
            .Execute(symbol.Lambda);

        VisitAxiom(context, symbol);

        return context;
    }

    protected override DeclarationStatementSymbol VisitDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        symbol.AddDeclaration(context, symbol.Identifier);
        symbol.AddSemantic(context, SemanticAnalyser.AnalyseDeclaration(context, symbol));
        return base.VisitDeclaration(context, symbol);
    }

    protected override LambdaExpressionSymbol VisitLambdaExpression(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        return base.VisitLambdaExpression(context, symbol);
    }

    protected override ExpressionSymbol VisitExpression(SemanticContext context, ExpressionSymbol symbol)
    {
        var semantic = SemanticAnalyser.AnalyseExpression(context, symbol);
        symbol.AddSemantic(context, semantic);
        return base.VisitExpression(context, symbol);
    }
}
