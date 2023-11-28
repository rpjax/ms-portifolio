namespace ModularSystem.Webql.Analysis;

public class OrderSemanticsAnalyser
{
    /// <summary>
    /// Parses the given syntax tree node, applying ordering semantics.
    /// </summary>
    /// <param name="node">The root node of the syntax tree to parse.</param>
    /// <returns>The parsed node with ordering semantics applied.</returns>
    public static Node Parse(Type type, Node node)
    {
        var semanticContext = new SemanticContext(type, null, "$order");

        node = new OrderSemanticsValidator().Visit(semanticContext, node);

        return node;
    }
}
