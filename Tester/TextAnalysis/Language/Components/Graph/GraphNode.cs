using System.Collections;
using System.Numerics;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class GraphNode
{
    public ProductionSymbol Symbol { get; }
    public ProductionRule? Production { get; }
    public bool IsRecursive { get; set; }

    private GraphNode? Parent { get; set; }
    private List<GraphNode> Children { get; } = new();

    public GraphNode(ProductionSymbol symbol, ProductionRule? production = null)
    {
        Symbol = symbol;
        Production = production;
    }

    public bool IsRoot => Parent is null;

    public override string ToString()
    {
        return Symbol.ToString();
    }

    public GraphNode GetParent()
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        return Parent;
    }

    public IEnumerable<GraphNode> GetChildren()
    {
        return Children;
    }

    public IEnumerable<GraphNode> GetSiblings()
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        return Parent.Children;
    }

    public IEnumerable<GraphNode> GetAncestors()
    {
        var node = this;
        while (node.Parent is not null)
        {
            node = node.Parent;
            yield return node;
        }
    }

    public IEnumerable<GraphNode> GetDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetDescendants())
            {
                yield return descendant;
            }
        }
    }

    public void SetParent(GraphNode parent)
    {
        Parent = parent;
    }

    public void AddChild(GraphNode node)
    {
        Children.Add(node);
        node.Parent = this;
    }

    public void RemoveChild(GraphNode node)
    {
        Children.Remove(node);
        node.Parent = null;
    }

    public void RemoveFromTree()
    {
        Parent?.RemoveChild(this);
    }

    public void RemoveAllChildren()
    {
        foreach (var child in Children)
        {
            child.Parent = null;
        }

        Children.Clear();
    }

    public void ReplaceChild(GraphNode oldNode, GraphNode newNode)
    {
        var index = Children.IndexOf(oldNode);
        if (index == -1)
        {
            throw new InvalidOperationException("The old node is not a child of this node.");
        }

        Children[index] = newNode;
        newNode.Parent = this;
        oldNode.Parent = null;
    }

    public void ReplaceWith(GraphNode newNode)
    {
        Parent?.ReplaceChild(this, newNode);
    }

    public void ReplaceWith(IEnumerable<GraphNode> newNodes)
    {
        Parent?.ReplaceChild(this, newNodes.First());

        var index = Parent!.Children.IndexOf(newNodes.First());
        foreach (var node in newNodes.Skip(1))
        {
            Parent.Children.Insert(++index, node);
        }
    }

    public void ReplaceWith(params GraphNode[] newNodes)
    {
        ReplaceWith(newNodes.AsEnumerable());
    }

    public void ReplaceWith(IEnumerable<GraphNode> newNodes, bool removeOld)
    {
        if (removeOld)
        {
            ReplaceWith(newNodes);
        }
        else
        {
            Parent?.ReplaceChild(this, newNodes.First());

            var index = Parent!.Children.IndexOf(newNodes.First());
            foreach (var node in newNodes.Skip(1))
            {
                Parent.Children.Insert(++index, node);
            }
        }
    }
}

public class CharVec2
{
    public int X { get; }
    public int Y { get; }
    public char Char { get; }

    public CharVec2(int x, int y, char c)
    {
        X = x;
        Y = y;
        Char = c;
    }

    override public string ToString()
    {
        return $"({X}, {Y}) = {Char}";
    }
}

public class CharMatrix : IEnumerable<CharVec2>
{
    private List<CharVec2> Chars { get; }
    private bool BoxAdded { get; set; }

    public CharMatrix()
    {
        Chars = new();
    }

    public static CharMatrix operator +(CharMatrix left, CharMatrix right)
    {
        var matrix = new CharMatrix();

        foreach (var cvec in left)
        {
            matrix.AddChar(cvec);
        }

        foreach (var cvec in right)
        {
            matrix.AddChar(cvec);
        }

        return matrix;
    }

    public IEnumerator<CharVec2> GetEnumerator()
    {
        return Chars.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        var matrix = this
            .OrderBy(c => c.Y)
            .ThenBy(c => c.X);

        var builder = new StringBuilder();

        foreach (var @char in matrix)
        {
            builder.Append(@char.Char);
        }

        return builder.ToString();
    }

    public int GetWidth()
    {
        if (Chars.Count == 0)
        {
            return 0;
        }

        return Chars.Max(c => c.X) + 1;
    }

    public int GetHeight()
    {
        if (Chars.Count == 0)
        {
            return 0;
        }

        return Chars.Max(c => c.Y) + 1;
    }

    public CharMatrix AddChar(CharVec2 cvec)
    {
        UnsetChar(cvec.X, cvec.Y);
        Chars.Add(cvec);
        return this;
    }

    public CharMatrix SetChar(int x, int y, char @c)
    {
        UnsetChar(x, y);
        Chars.Add(new CharVec2(x, y, c));
        return this;
    }

    public CharMatrix UnsetChar(int x, int y)
    {
        var index = Chars.FindIndex(c => c.X == x && c.Y == y);

        if (index != -1)
        {
            Chars.RemoveAt(index);
        }

        return this;

    }

    public void Offset(int x, int y)
    {
        for (int i = 0; i < Chars.Count; i++)
        {
            var c = Chars[i];
            Chars[i] = new CharVec2(c.X + x, c.Y + y, c.Char);
        }
    }

    public CharMatrix AddBox()
    {
        /*
            Full example:
               ┌─────┐
               │ABCD │
               └──┬──┘
              ┌───┴───┐
            ┌─┴─┐   ┌─┴─┐
            │ABC│   │ABC│
            └───┘   └───┘
        */

        // creates an ascii art style box around the body of the node

        if (BoxAdded)
        {
            return this;
        }

        var width = GetWidth();
        var height = GetHeight();

        Offset(1, 1);

        var a = new CharVec2(0, 0, '┌');
        var b = new CharVec2(width + 1, 0, '┐');
        var c = new CharVec2(0, height + 1, '└');
        var d = new CharVec2(width + 1, height + 1, '┘');

        AddChar(a);
        AddChar(b);
        AddChar(c);
        AddChar(d);

        for (int i = 1; i <= width; i++)
        {
            AddChar(new CharVec2(i, 0, '─'));
            AddChar(new CharVec2(i, height + 1, '─'));
        }

        for (int i = 1; i <= height; i++)
        {
            AddChar(new CharVec2(0, i, '│'));
            AddChar(new CharVec2(width + 1, i, '│'));
        }

        /*
            Corners:
               a    b
               ┌────┐
               │    │
               └────┘
               c    d
        */

        BoxAdded = true;
        return this;
    }

    public (int x, int y) AddDownConnection()
    {
        AddBox();

        var width = GetWidth();
        var height = GetHeight();
        var x = (width - 1) / 2;
        var y = height - 1;

        SetChar(x, y, '┬');

        return (x, y);
    }

    public (int x, int y) AddUpConnection()
    {
        AddBox();

        var width = GetWidth();
        var x = (width - 1) / 2;

        SetChar(x, 0, '┴');

        return (x, 0);
    }

    public CharMatrix FillGaps()
    {
        var maxX = Chars.Max(c => c.X);
        var maxY = Chars.Max(c => c.Y);

        for (int yi = 0; yi <= maxY; yi++)
        {
            for (int xi = 0; xi <= maxX; xi++)
            {
                var c = Chars.FirstOrDefault(c => c.X == xi && c.Y == yi);

                if (c == null)
                {
                    SetChar(xi, yi, ' ');
                }
            }

            SetChar(maxX + 1, yi, '\n');
        }

        return this;
    }

}

public static class GraphNodeRenderExtensions
{
    public static CharMatrix Render(this GraphNode node)
    {
        var body = node.RenderBody()
            .AddBox();

        var children = node.GetChildren()
            .Select(c => c.Render())
            .ToArray();

        if (children.Length == 0)
        {
            return body.FillGaps();
        }

        // double check that the body is not empty. It should not happen, since the body render method ensures that there is at least one character, even if it's a placeholder.
        if (body.Count() == 0)
        {
            throw new InvalidOperationException("The body of the node is empty.");
        }

        var bodyWidth = body.Max(c => c.X);

        var matrix = new CharMatrix();
        var xOffset = 0;
        var xSpacing = 2;

        foreach (var child in children)
        {
            var width = child.GetWidth();

            child.AddUpConnection();

            child.Offset(xOffset + xSpacing, 0);
            xOffset += width;
            matrix += child;
        }

        return matrix.FillGaps();
    }

    private static CharMatrix RenderBody(this GraphNode node)
    {
        var builder = new StringBuilder();

        // builds the content of the body.
        if (node.IsRecursive)
        {
            // denotes a recursive node. 
            // this notaion has to reviewed to make it more clear, i don't like the star, it's not expressive enough.
            builder.Append("*");
        }

        builder.Append(node.Symbol.ToString());

        // this ensures that the body is never empty.
        // if the body is empty, enumeration of CharMatrix will throw an exception. 
        // there has to be at least one character. The tree dots are used as a placeholder.
        if (builder.Length == 0)
        {
            builder.Append("...");
        }

        // copies the string to a char matrix.
        var str = builder.ToString();
        var matrix = new CharMatrix();

        var x = 0;
        var y = 0;

        foreach (var @char in str)
        {
            matrix.SetChar(x++, y, @char);

            if (@char == '\n')
            {
                x = 0;
                y++;
            }
        }

        /*
            Ensures that the body is always impar width, this is important for drawing the connections with proper alignment. This comes at the cost of adding a trailing space to the body, making the node content disaligned, but it preserves the connections alignment.
        */

        var width = matrix.GetWidth();

        if (width % 2 == 0)
        {
            matrix.SetChar(width, 0, ' ');
        }

        return matrix;
    }

}
