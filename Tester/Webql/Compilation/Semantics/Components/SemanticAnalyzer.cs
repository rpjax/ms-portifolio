using System.Diagnostics.CodeAnalysis;
using Webql.DocumentSyntax.Parsing.Components;
using Webql.DocumentSyntax.Parsing.Tools;

namespace Webql.DocumentSyntax.Semantics.Components;

public static class SemanticAnalyzer
{
    public static void ExecuteFirstPass(WebqlSyntaxNode node)
    {
        new InitialAnalysisVisitor()
            .Visit(node);
    }

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

    public static IQuerySemantics CreateQuerySemantics(WebqlQuery query)
    {
        return new QuerySemantics(
            type: query.Expression?.GetSemantics<IExpressionSemantics>()?.Type ?? new VoidTypeSymbol()
        );
    }

    public static IExpressionSemantics CreateExpressionSemantics(WebqlExpression expression)
    {
        switch (expression.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return CreateLiteralExpressionSymbol((WebqlLiteralExpression)expression);

            case WebqlExpressionType.Reference:
                throw new NotImplementedException();

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

    public static IExpressionSemantics CreateLiteralExpressionSymbol(WebqlLiteralExpression expression)
    {
        switch (expression.LiteralType)
        {
            case WebqlLiteralType.Bool:
                throw new NotImplementedException();

            case WebqlLiteralType.Null:
                throw new NotImplementedException();

            case WebqlLiteralType.Int:
                return CreateIntLiteralSemantics(expression);

            case WebqlLiteralType.Float:
                throw new NotImplementedException();

            case WebqlLiteralType.Hex:
                throw new NotImplementedException();

            case WebqlLiteralType.String:
                throw new NotImplementedException();

            default:
                throw new InvalidOperationException();
        }
    }

    public static IExpressionSemantics CreateIntLiteralSemantics(WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: new IntTypeSymbol()
        );
    }

}

public static class SyntaxNodeSemanticExtensions
{
    public static string GetSemanticIdentifier(this WebqlSyntaxNode node)
    {
        return "not implemented yet";
    }
}

public class InitialAnalysisVisitor : SyntaxNodeVisitor
{
    public InitialAnalysisVisitor()
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

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeSemanticExtensions
{
    public static bool HasSemanticContext(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(SemanticsHelper.ContextAttribute);
    }

    public static bool HasSemantics(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(SemanticsHelper.SemanticsAttribute);
    }

    public static bool IsScopeSource(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(SemanticsHelper.ScopeSourceAttribute);
    }

    public static SemanticContext GetSemanticContext(this WebqlSyntaxNode node)
    {
        var attribute = node.TryGetAttribute<SemanticContext>(SemanticsHelper.ContextAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static ISemantics GetSemantics(this WebqlSyntaxNode node)
    {
        if(!node.HasSemantics())
        {
            return node.GetSemanticContext().GetSemantics(node);
        }

        var attribute = node.TryGetAttribute<ISemantics>(SemanticsHelper.SemanticsAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static TSemantics GetSemantics<TSemantics>(this WebqlSyntaxNode node) where TSemantics : ISemantics
    {
        if(!node.HasSemantics())
        {
            return node.GetSemanticContext().GetSemantics<TSemantics>(node);
        }

        var attribute = node.TryGetAttribute<TSemantics>(SemanticsHelper.SemanticsAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static void AddSemanticContext(this WebqlSyntaxNode node, SemanticContext context)
    {
        node.AddAttribute(SemanticsHelper.ContextAttribute, context);
    }

    public static void AddSemantics(this WebqlSyntaxNode node, ISemantics semantics)
    {
        node.AddAttribute(SemanticsHelper.SemanticsAttribute, semantics);
    }
}
