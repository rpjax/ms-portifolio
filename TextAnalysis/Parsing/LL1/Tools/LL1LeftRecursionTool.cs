using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Graph;

namespace ModularSystem.Core.TextAnalysis.Parsing.LL1.Tools;

public class LL1LeftRecursionTool : GraphVisitor
{
    private List<GraphBranch> RecursiveBranches { get; } = new();

    public GraphBranch[] Execute(LL1GraphNode node)
    {
        Visit(node);
        return RecursiveBranches.ToArray();
    }

    public GraphBranch[] Execute(ProductionSet set)
    {
        return Execute(LL1GraphBuilder.CreateGraphTree(set));
    }

    protected override LL1GraphNode Visit(LL1GraphNode node)
    {
        if (node.RecursionType == RecursionType.Left || node.RecursionType == RecursionType.IndirectLeft)
        {
            RecursiveBranches.Add(node.GetRecursiveBranch(node.Symbol));
        }

        return base.Visit(node);
    }
}
