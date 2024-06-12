using Webql.Components;
using Webql.DocumentSyntax.Parsing.Components;
using Webql.Parsing.Components;

namespace Webql.Semantics.Components;

public static class SemanticAnalyzer
{
    public static void ExecuteAnalysisPipeline(
        WebqlCompilationSettings compilationSettings, 
        WebqlSyntaxNode tree, 
        Type entityType)
    {
        AnnotateTree(tree);
        DeclareSymbols(compilationSettings, tree, entityType);
    }

    public static void AnnotateTree(WebqlSyntaxNode node)
    {
        new ContextBinderVisitor()
            .Visit(node);
    }

    public static void DeclareSymbols(
        WebqlCompilationSettings compilationSettings,
        WebqlSyntaxNode tree,
        Type entityType)
    {
        new SymbolDeclaratorVisitor(entityType)
            .Visit(tree);
    }

    /*
     * Main entry point for the semantic analysis.
     */

    public static ISemantics CreateSemantics(WebqlSyntaxNode node)
    {
        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                return CreateQuerySemantics((WebqlQuery)node);

            case WebqlNodeType.Expression:
                return CreateExpressionSemantics((WebqlExpression)node);

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * QUERY SEMANTICS
     */

    public static IQuerySemantics CreateQuerySemantics(WebqlQuery query)
    {
        if(query.Expression is null)
        {
            return new QuerySemantics(typeof(void));
        }

        return new QuerySemantics(
            type: query.Expression.GetSemantics<IExpressionSemantics>().Type
        );
    }

    /*
     * EXPRESSION SEMANTICS
     */
    public static IExpressionSemantics CreateExpressionSemantics(WebqlExpression expression)
    {
        switch (expression.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return CreateLiteralExpressionSymbol((WebqlLiteralExpression)expression);

            case WebqlExpressionType.Reference:
                return CreateReferenceExpressionSymbol((WebqlReferenceExpression)expression);

            case WebqlExpressionType.ScopeAccess:
                throw new NotImplementedException();

            case WebqlExpressionType.TemporaryDeclaration:
                throw new NotImplementedException();

            case WebqlExpressionType.Block:
                throw new NotImplementedException();

            case WebqlExpressionType.Operation:
                throw new NotImplementedException();

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * LITERAL EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateLiteralExpressionSymbol(WebqlLiteralExpression expression)
    {
        switch (expression.LiteralType)
        {
            case WebqlLiteralType.Bool:
                return CreateBoolSemantics(expression);

            case WebqlLiteralType.Null:
                return CreateNullSemantics(expression);

            case WebqlLiteralType.Int:
                return CreateIntLiteralSemantics(expression);

            case WebqlLiteralType.Float:
                return CreateFloatLiteralSemantics(expression);

            case WebqlLiteralType.Hex:
                throw new NotImplementedException();

            case WebqlLiteralType.String:
                return CreateStringLiteralSemantics(expression);

            default:
                throw new InvalidOperationException();
        }
    }

    public static IExpressionSemantics CreateBoolSemantics(WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    public static IExpressionSemantics CreateNullSemantics(WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(Nullable)
        );
    }

    public static IExpressionSemantics CreateIntLiteralSemantics(WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(int)
        );
    }

    public static IExpressionSemantics CreateFloatLiteralSemantics(WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(float)
        );
    }

    public static IExpressionSemantics CreateStringLiteralSemantics(WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(string)
        );
    }

    /*
     * REFERENCE EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateReferenceExpressionSymbol(WebqlReferenceExpression expression)
    {
        throw new NotImplementedException(); 
    }

}

