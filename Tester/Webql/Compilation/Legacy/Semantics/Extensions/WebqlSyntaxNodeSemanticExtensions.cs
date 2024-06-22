using Webql.Parsing.Ast;
using Webql.Semantics.Attributes;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;

namespace Webql.Semantics.Extensions;

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeSemanticExtensions
{
    public static T As<T>(this WebqlSyntaxNode node) where T : WebqlSyntaxNode
    {
        if(node is T t)
        {
            return t;
        }

        throw new InvalidOperationException("Attempted to cast a node to an incompatible type during semantic analysis.");
    }

    public static string GetSemanticIdentifier(this WebqlSyntaxNode node)
    {
        return "not implemented yet";
    }

    public static bool HasSemanticContext(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(SemanticContextAttributes.ContextAttribute);
    }

    public static bool HasSemantics(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(SemanticContextAttributes.SemanticsAttribute);
    }

    public static bool IsScopeSource(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(SemanticContextAttributes.ScopeSourceAttribute);
    }

    public static bool IsRoot(this WebqlSyntaxNode node)
    {
        return node.NodeType == WebqlNodeType.Query;
    }

    public static SemanticContext GetSemanticContext(this WebqlSyntaxNode node)
    {
        var attribute = node.TryGetAttribute<SemanticContext>(SemanticContextAttributes.ContextAttribute);

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

        var attribute = node.TryGetAttribute<ISemantics>(SemanticContextAttributes.SemanticsAttribute);

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

        var attribute = node.TryGetAttribute<TSemantics>(SemanticContextAttributes.SemanticsAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static void AddSemanticContext(this WebqlSyntaxNode node, SemanticContext context)
    {
        node.AddAttribute(SemanticContextAttributes.ContextAttribute, context);
    }

    public static void AddSemantics(this WebqlSyntaxNode node, ISemantics semantics)
    {
        node.AddAttribute(SemanticContextAttributes.SemanticsAttribute, semantics);
    }
}
