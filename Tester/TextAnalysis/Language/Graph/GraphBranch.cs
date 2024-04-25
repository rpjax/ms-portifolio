using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class GraphBranch
{
    public IReadOnlyList<GraphNode> Nodes { get; }

    public GraphBranch(IEnumerable<GraphNode> nodes)
    {
        Nodes = nodes.ToArray();

        if(Nodes.Count == 0)
        {
            throw new InvalidOperationException("The list of nodes is empty.");
        }
    }

    public GraphNode Root => Nodes.First();

    public ProductionSet ToProductionSet()
    {
        var set = new ProductionSet();

        foreach (var node in Nodes)
        {
            if (node.Production is not null)
            {
                set.Add(node.Production);
            }
        }

        return set;
    }
}
