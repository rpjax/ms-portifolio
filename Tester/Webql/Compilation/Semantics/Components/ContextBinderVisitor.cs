using System.Diagnostics.CodeAnalysis;
using Webql.DocumentSyntax.Parsing.Components;
using Webql.DocumentSyntax.Parsing.Tools;

namespace Webql.DocumentSyntax.Semantics.Components;

public class ContextBinderVisitor : SyntaxNodeVisitor
{
    public ContextBinderVisitor()
    {

    }


    [return: NotNullIfNotNull("node")]
    public override WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return null;
        }

        var context = null as SemanticContext;

        if (node.HasSemanticContext())
        {
            context = node.GetSemanticContext();
        }
        else
        {
            context = SemanticContext.CreateRootContext();
        }

        if (node.IsScopeSource())
        {
            context = context.CreateSubContext();
        }

        node?.AddSemanticContext(context);

        return base.Visit(node);
    }

}

public class SymbolDeclaratorVisitor : SyntaxNodeVisitor
{
    private Type SourceType { get; }

    public SymbolDeclaratorVisitor(Type entityType)
    {
        SourceType = entityType;
    }

    [return: NotNullIfNotNull("node")]
    public override WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return null;
        }

        var context = node.GetSemanticContext();

        if (node.IsRoot())
        {
            // declare the existance of symbols, and their types. The left hand side is an implicit declaration.

            context.SetLeftHandSide(new LhsSymbol(SourceType));
        }

        if(node is WebqlScopeAccessExpression scopeAccessExpression)
        {
            var properties = SourceType.GetProperties();
            var accessedProperty = properties.FirstOrDefault(x => x.Name == scopeAccessExpression.Identifier);

            if (accessedProperty is null)
            {
                throw new InvalidOperationException();
            }

            var subContext = scopeAccessExpression.Expression.GetSemanticContext();

            foreach (var property in properties)
            {
                subContext.AddSymbol(
                    identifier: property.Name, 
                    symbol: new ScopePropertySymbol(property.PropertyType)
                );
            }
        }

        if(node is WebqlOperationExpression operationExpression)
        {
            var operatorCategory = operationExpression.GetOperatorCategory();
        }

        return base.Visit(node);
    }

}
