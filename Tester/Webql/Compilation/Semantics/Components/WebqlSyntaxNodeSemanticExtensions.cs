using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Semantics.Components;

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeSemanticExtensions
{
    public static string GetSemanticIdentifier(this WebqlSyntaxNode node)
    {
        return "not implemented yet";
    }

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

    public static bool IsRoot(this WebqlSyntaxNode node)
    {
        return node.NodeType == WebqlNodeType.Query;
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
        if (!node.HasSemantics())
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
        if (!node.HasSemantics())
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
