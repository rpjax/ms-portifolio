using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Components;

public class FirstSemanticPass : AstSemanticTraverser
{
    public FirstSemanticPass(SemanticContext context) : base(context)
    {
    }

    public void Execute(Symbol symbol)
    {
        TraverseTree(symbol);
    }

    protected override void OnVisit(Symbol symbol)
    {
        /* 
         * Adds declarations to the symbol table. 
         */
        if (symbol is IDeclarationSymbol declarationSymbol)
        {
            var semantic = SemanticAnalyser
                .AnalyseDeclaration(Context, declarationSymbol);

            var identifier = declarationSymbol.Identifier;

            if (identifier is null)
            {
                throw new InvalidOperationException();
            }

            Context.DeclareSymbol(
                identifier: declarationSymbol.Identifier,
                symbol: symbol,
                type: semantic.Type,
                false
            );
        }

        /*
         * Creates a new scope context if the symbol is a scope symbol.
         * The idea is to provide a way to seamlessly navigate the AST, entering and exiting from scopes automaticly.
         */
        if (symbol is IScopeSymbol scopeSymbol)
        {
            var identifier = null as string;

            if (scopeSymbol is IIdentifierSymbol identifierSymbol)
            {
                identifier = identifierSymbol.Identifier;
            }
            else
            {
                identifier = scopeSymbol.Hash;
            }

            if (identifier is null)
            {
                throw new InvalidOperationException();
            }

            Context.DeclareSymbol(
                identifier: identifier,
                symbol: symbol,
                type: null,
                true
            );
            Context.EnterScope(identifier);
        }
    }

    protected override void AfterVisitChildren(Symbol symbol)
    {
        if (symbol is IScopeSymbol)
        {
            Context.ExitScope();    
        }
    }

}
