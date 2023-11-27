using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis;

public class StackFinderVisitor : SemanticsVisitor
{
    private Node StackNode { get; }
    private string? StackString { get; set; }

    public StackFinderVisitor(Node stackNode)
    {
        StackNode = stackNode;
    }

    public string? Run(SemanticContext context)
    {
        Visit(context, StackNode);
        return StackString;
    }

    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, ScopeDefinitionNode node)
    {
        if (node == StackNode)
        {
            StackString = context.Stack;
        }

        return base.Visit(context, node);
    }

    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, ArrayNode node)
    {
        if (node == StackNode)
        {
            StackString = context.Stack;
        }

        return base.Visit(context, node);
    }

    [return: NotNullIfNotNull("node")]
    protected override ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        if (node == StackNode)
        {
            StackString = context.Stack;
        }

        return base.Visit(context, node);
    }

    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, LhsNode node)
    {
        if (node == StackNode)
        {
            StackString = context.Stack;
        }

        return base.Visit(context, node);
    }

    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, RhsNode node)
    {
        if (node == StackNode)
        {
            StackString = context.Stack;
        }

        return base.Visit(context, node);
    }

    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, LiteralNode node)
    {
        if (node == StackNode)
        {
            StackString = context.Stack;
        }

        return base.Visit(context, node);
    }
}