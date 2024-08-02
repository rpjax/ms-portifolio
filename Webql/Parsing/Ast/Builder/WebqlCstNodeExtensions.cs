using Aidan.TextAnalysis.Parsing.Components;

namespace Webql.Parsing.Ast.Builder.Extensions;

public static class WebqlCstNodeExtensions
{
    const string BuildContextKey = WebqlCstPropertyKeys.BuildContextKey;

    /*
     * build context methods.
     */

    public static bool HasBuildContextAttached(this CstNode node)
    {
        return node.HasProperty(BuildContextKey);
    }

    public static WebqlAstBuildContext GetBuildContext(this CstNode node)
    {
        var current = node;

        while (current is not null)
        {
            if (current.HasBuildContextAttached())
            {
                var context = current.TryGetProperty<WebqlAstBuildContext>(BuildContextKey);

                if (context is null)
                {
                    throw new InvalidOperationException("The build context attached to the node is of an invalid type.");
                }

                return context;
            }

            current = current.Parent;
        }

        throw new Exception("BuildContext not found.");
    }

    public static void BindBuildContext(this CstNode node, WebqlAstBuildContext buildContext)
    {
        if(node.HasBuildContextAttached())
        {
            throw new InvalidOperationException("BuildContext already attached to the node.");
        }

        node.Properties[BuildContextKey] = buildContext;
    }

    /*
     * scope type methods.
     */

    public static WebqlScopeType GetCstScopeType(this CstNode node)
    {
        return node.GetBuildContext().ScopeType;
    }

    public static void SetCstScopeType(this CstNode node, WebqlScopeType scopeType)
    {
        EnsureContextAttached(node);
        node.GetBuildContext().SetScopeType(scopeType);
    }

    /*
     * LHS expression methods.
     */

    public static WebqlExpression GetLhsExpression(this CstNode node)
    {
        return node.GetBuildContext().GetLhsExpression(node);
    }

    public static void SetLhsExpression(this CstNode node, WebqlExpression lhsExpression)
    {
        EnsureContextAttached(node);
        node.GetBuildContext().SetLhsExpression(lhsExpression);
    }

    /*
     * member access stack methods.
     */

    public static void SetLhsExpressionToMemberAccess(this CstNode node, string memberName)
    {
        node.EnsureContextAttached();

        var buildContext = node.GetBuildContext();
        var lhsExpression = node.GetLhsExpression();

        var memberAccessExpression = new WebqlMemberAccessExpression(
            metadata: WebqlAstBuilder.CreateNodeMetadata(node),
            attributes: null,
            expression: lhsExpression,
            memberName: memberName
        );

        buildContext.SetLhsExpression(memberAccessExpression);
    }

    public static void SetLhsExpressionToSourceReference(this CstNode node)
    {
        node.EnsureContextAttached();

        var buildContext = node.GetBuildContext();
        var sourceReferenceExpression = WebqlAstBuilder.CreateSourceReferenceExpression(node);

        buildContext.SetLhsExpression(sourceReferenceExpression);
    }

    public static void SetLhsExpressionToElementReference(this CstNode node)
    {
        node.EnsureContextAttached();

        var buildContext = node.GetBuildContext();
        var elementReferenceExpression = WebqlAstBuilder.CreateElementReferenceExpression(node);

        buildContext.SetLhsExpression(elementReferenceExpression);
    }

    private static void EnsureContextAttached(this CstNode node)
    {
        if (node.HasBuildContextAttached())
        {
            return;
        }

        var parentContext = node.GetBuildContext();
        var localContext = parentContext.CreateChildContext();

        node.BindBuildContext(localContext);
    }

    /*
     * block simplification methods.
     */

    public static bool IsBlockSimplificationDisabled(this CstNode node)
    {
        return node.GetCstScopeType() != WebqlScopeType.LogicalFiltering;
    }

}
