namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class GraphVisitor
{
    protected virtual GraphNode Visit(GraphNode node)
    {
        if (node.IsRecursive)
        {
            return node;
        }
        
        for (int i = 0; i < node.ChildCount; i++)
        {
            node[i] = Visit(node[i]);
        }

        return node;
    }

}

public class LeftRecursionDetector : GraphVisitor
{
    private List<GraphBranch> RecursiveBranches { get; } = new();

    public List<GraphBranch> Execute(GraphNode node)
    {
        Visit(node);
        return RecursiveBranches;
    }

    protected override GraphNode Visit(GraphNode node)
    {
        if (node.RecursionType == RecursionType.Left || node.RecursionType == RecursionType.IndirectLeft)
        {
            RecursiveBranches.Add(node.GetRecursiveBranch(node.Symbol));
        }

        return base.Visit(node);
    }
}
