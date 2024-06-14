using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Components;

/// <summary>
/// The SemanticsAnalysisVisitor traverses the AST, generating semantic information for each node.
/// This visitor pattern ensures that semantic analysis occurs in a post-order traversal manner,
/// meaning that it first visits all descendant nodes before the node itself. This order is crucial
/// because it allows for the semantic information to bubble up from the leaves to the root of the AST.
/// Each node's semantic information is then stored in the SemanticTable for subsequent phases.
/// </summary>
public class AstSemanticAnalysis : AstSemanticTraverser
{
    public AstSemanticAnalysis() : base(new SemanticContextOld())
    {
    }

    public SemanticContextOld Execute(AxiomSymbol symbol)
    {
        if (symbol.Lambda is null)
        {
            return Context;
        }

        new FirstSemanticPass(Context)
            .Execute(symbol);

        TraverseTree(symbol);

        return Context;
    }

    protected override void OnSemanticVisit(Symbol symbol)
    {
        if (symbol is ExpressionSymbol expressionSymbol)
        {
            var semantic = SemanticAnalyser
                .AnalyseExpression(Context, expressionSymbol);

            Context.DeclareSymbol(
                identifier: expressionSymbol.Hash,
                symbol: expressionSymbol,
                type: semantic.Type,
                false
            );
        }
    }

}
