namespace ModularSystem.Core.TextAnalysis.Language.Graph.Renderization;

public class RenderContext
{
    public GraphNode Node { get; }

    public RenderContext(GraphNode node)
    {
        Node = node;
    }

    public GraphNode? Parent => Node.Parent;

    public RenderContext CreateChildContext(GraphNode node)
    {
        if(Node.GetChildren().All(n => n != node))
        {
            throw new InvalidOperationException("The node is not a child of the current node.");
        }

        return new RenderContext(node);
    }
}
