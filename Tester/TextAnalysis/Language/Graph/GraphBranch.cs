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
        var builder = new ProductionSetBuilder();

        foreach (var node in Nodes)
        {
            if (node.Production is not null)
            {
                builder.Add(node.Production);
            }
        }

        return builder.Build();
    }
}
