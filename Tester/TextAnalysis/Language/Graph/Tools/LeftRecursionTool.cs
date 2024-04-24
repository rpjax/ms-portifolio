using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class LeftRecursionTool : GraphVisitor
{
    private List<GraphBranch> RecursiveBranches { get; } = new();

    public GraphBranch[] Execute(GraphNode node)
    {
        Visit(node);
        return RecursiveBranches.ToArray();
    }

    public GraphBranch[] Execute(ProductionSet set)
    {
        return Execute(GraphBuilder.CreateDerivationTree(set));
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
