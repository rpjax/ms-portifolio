namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Provides functionality for applying filter semantics to a syntax tree in the WebQL language. <br/>
/// This class is responsible for processing the syntax tree and applying specific filter-related semantic rules.
/// </summary>
public static class FilterSemanticsAnalyser
{
    /// <summary>
    /// Parses the given syntax tree node, applying filter semantics.
    /// </summary>
    /// <param name="node">The root node of the syntax tree to parse.</param>
    /// <returns>The parsed node with filter semantics applied.</returns>
    public static Node Parse(Type type, Node node)
    {
        var semanticContext = new SemanticContext(type, null, "$where");

        node = new FilterSemanticsValidatorVisitor().Visit(semanticContext, node);
        node = new FilterSemanticsRewriterVisitor().Visit(semanticContext, node);

        return node;
    }
}
