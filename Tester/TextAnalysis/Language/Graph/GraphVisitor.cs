namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class GraphVisitor
{
    protected virtual GraphNode Visit(GraphNode node)
    {
        if (node.IsRecursive)
        {
            return node;
        }
        
        for (int i = 0; i < node.ChildrenCount; i++)
        {
            node[i] = Visit(node[i]);
        }

        return node;
    }

}
