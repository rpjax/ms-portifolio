using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public static class GraphNodeExtensions
{
    public static GraphNode GetParent(this GraphNode node)
    {
        if (node.Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        return node.Parent;
    }

    public static IEnumerable<GraphNode> GetChildren(this GraphNode node)
    {
        return node.Children;
    }

    public static IEnumerable<GraphNode> GetSiblings(this GraphNode node)
    {
        if (node.Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        return node.Parent.Children;
    }

    public static IEnumerable<GraphNode> GetAncestors(this GraphNode node)
    {
        var currentNode = node;

        while (currentNode.Parent is not null)
        {
            currentNode = currentNode.Parent;
            yield return currentNode;
        }
    }

    public static IEnumerable<GraphNode> GetDescendants(this GraphNode node)
    {
        foreach (var child in node.Children)
        {
            yield return child;

            if (child.IsRecursive)
            {
                continue;
            }

            foreach (var descendant in child.GetDescendants())
            {
                yield return descendant;
            }
        }
    }

    public static IEnumerable<GraphNode> GetLeafs(this GraphNode node)
    {
        foreach (var child in node.Children)
        {
            if(child.IsRecursive || child.ChildrenCount == 0)
            {
                yield return child;
            }

            foreach (var leaf in child.GetLeafs())
            {
                yield return leaf;
            }
        }
    }

    public static void SetParent(this GraphNode self, GraphNode parent)
    {
        self.Parent = parent;
    }

    public static void AddChild(this GraphNode self, GraphNode child)
    {
        self.Children.Add(child);
        child.Parent = self;
    }

    public static void RemoveChild(this GraphNode self, GraphNode child)
    {
        self.Children.Remove(child);
        child.Parent = null;
    }

    public static void RemoveFromTree(this GraphNode self)
    {
        self.Parent?.RemoveChild(self);
    }

    public static void RemoveAllChildren(this GraphNode self)
    {
        foreach (var child in self)
        {
            child.Parent = null;
        }

        self.Children.Clear();
    }

    public static void ReplaceChild(this GraphNode self, GraphNode oldNode, GraphNode newNode)
    {
        var index = self.Children.IndexOf(oldNode);

        if (index == -1)
        {
            throw new InvalidOperationException("The old node is not a child of this node.");
        }

        self[index] = newNode;
        newNode.Parent = self;
        oldNode.Parent = null;
    }

    public static void ReplaceWith(this GraphNode self, GraphNode newNode)
    {
        self.Parent?.ReplaceChild(self, newNode);
    }

    public static ProductionSet ToProductionSet(this GraphNode node)
    {
        var set = new ProductionSet();
        var productions = node.GetDescendants()
            .Select(x => x.Production)
            .Concat(new[] { node.Production })
            .ToArray();

        foreach (var production in productions)
        {
            if (production is not null)
            {
                set.Add(production);
            }
        }

        return set;
    }

    public static GraphBranch GetRecursiveBranch(this GraphNode self, Symbol root)
    {
        if (!self.IsRecursive)
        {
            throw new InvalidOperationException("The node is not recursive.");
        }
        if(self.Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        var currentNode = self.Parent;
        var rootFound = false;
        var nodes = new List<GraphNode>();

        nodes.Add(self);

        while (true)
        {
            nodes.Add(currentNode);

            if (currentNode.Symbol == root)
            {
                rootFound = true;
                break;
            }

            if (currentNode.Parent is null)
            {
                break;
            }

            currentNode = currentNode.Parent;
        }

        if (!rootFound)
        {
            throw new InvalidOperationException("The stop symbol was not found in the branch.");
        }

        nodes.Reverse();

        return new GraphBranch(nodes);
    }

}
