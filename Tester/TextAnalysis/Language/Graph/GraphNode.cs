using System.Collections;
using System.Text;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class GraphNode : IEnumerable<GraphNode>
{
    public Symbol Symbol { get; }
    public ProductionRule? Production { get; }
    public GraphNode? Parent { get; internal set; }
    public RecursionType RecursionType { get; internal set; }

    internal List<GraphNode> Children { get; set; } 

    public GraphNode(
        Symbol symbol, 
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
    public int ChildrenCount => Children.Count;

    public override string ToString()
    {
        var builder = new StringBuilder();

        if(Production is not null)
        {
            builder.Append($"{Production} ");
        }
        else
        {
            builder.Append("Axiom ");
        }

        builder.Append($"({Symbol})");

        return builder.ToString();
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
