using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents a builder for constructing a Concrete Syntax Tree (CST).
/// </summary>
public class CstBuilder
{
    private List<CstNode> Accumulator { get; }
    private bool UseEpsilons { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstBuilder"/> class.
    /// </summary>
    /// <param name="useEpsilons">Indicates whether to use epsilon nodes in the CST.</param>
    public CstBuilder(bool useEpsilons = false)
    {
        Accumulator = new List<CstNode>();
        UseEpsilons = useEpsilons;
    }

    /// <summary>
    /// Gets the number of nodes in the accumulator.
    /// </summary>
    public int AccumulatorCount => Accumulator.Count;

    /// <summary>
    /// Adds a terminal node to the accumulator.
    /// </summary>
    /// <param name="terminal">The terminal node to add.</param>
    public void AddTerminal(Token token)
    {
        Accumulator.Add(new CstLeaf(token));
    }

    /// <summary>
    /// Reduces an epsilon node in the accumulator.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal associated with the epsilon node.</param>
    public void ReduceEpsilon(NonTerminal nonTerminal)
    {
        Accumulator.Add(new CstInternal(
            name: nonTerminal.Name,
            children: Array.Empty<CstNode>(),
            isEpsilon: true
        ));
    }

    /// <summary>
    /// Reduces a non-terminal node in the accumulator.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal associated with the node.</param>
    /// <param name="length">The number of nodes to reduce.</param>
    public void Reduce(NonTerminal nonTerminal, int length, bool isRoot)
    {
        var offset = Accumulator.Count - length;
        var children = Accumulator
            .Skip(offset)
            .ToList();

        Accumulator.RemoveRange(offset, length);

        if (UseEpsilons)
        {
            children = children
                .ToList();
        }
        else
        {
            children = children
                .Where(x => x is CstInternal node ? !node.IsEpsilon : true)
                .ToList();
        }

        CstNode node = isRoot
            ? new CstRoot(nonTerminal.Name, children.ToArray())
            : new CstInternal(nonTerminal.Name, children.ToArray());

        Accumulator.Add(node);
    }

    /// <summary>
    /// Builds the Concrete Syntax Tree (CST) from the accumulator.
    /// </summary>
    /// <returns>The root node of the CST.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the CST is empty or not complete.</exception>
    public CstRoot Build()
    {
        if (Accumulator.Count == 0)
        {
            throw new InvalidOperationException("CST is empty.");
        }
        if (Accumulator.Count != 1)
        {
            throw new InvalidOperationException("CST is not complete.");
        }

        if(Accumulator.Single() is not CstRoot root)
        {
            throw new InvalidOperationException("CST is not complete.");
        }

        return root;
    }
}

