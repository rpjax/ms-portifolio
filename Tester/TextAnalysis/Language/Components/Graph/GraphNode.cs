using System.Collections;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class GraphNode : IEnumerable<GraphNode>
{
    public ProductionSymbol Symbol { get; }
    public ProductionRule? Production { get; }
    public GraphNode? Parent { get; internal set; }
    public RecursionType RecursionType { get; internal set; }

    internal List<GraphNode> Children { get; set; } 

    public GraphNode(
        ProductionSymbol symbol, 
        ProductionRule? production = null, 
        GraphNode? parent = null,
        RecursionType recursionType = RecursionType.None,
        IEnumerable<GraphNode>? children = null)
    {
        Symbol = symbol;
        Production = production;
        Parent = parent;
        RecursionType = recursionType;
        Children = children?.ToList() ?? new();
    }

    public bool IsRecursive => RecursionType != RecursionType.None;

    public GraphNode this[int index]
    {
        get
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return Children[index];
        }
        set
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new IndexOutOfRangeException();
            }

            Children[index] = value;
            value.Parent = this;
        }
    }

    public bool IsRoot => Parent is null;
    public int ChildCount => Children.Count;

    public override string ToString()
    {
        return $"({Symbol}): {string.Join(" ", Children.Select(c => c.Symbol))}";
    }

    public IEnumerator<GraphNode> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
