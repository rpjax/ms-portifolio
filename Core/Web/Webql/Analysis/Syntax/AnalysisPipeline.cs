using ModularSystem.Webql.Analysis.SyntaxFeatures;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Represents the pipeline for analyzing WebQL nodes. This static class orchestrates the different phases <br/>
/// of analysis to process and transform WebQL nodes based on semantic context and syntax features.
/// </summary>
public static class AnalysisPipeline
{
    /// <summary>
    /// Runs the analysis pipeline on a WebQL query represented in JSON format.
    /// </summary>
    /// <param name="json">The JSON representation of the WebQL query.</param>
    /// <param name="type">The type context within which the query is analyzed.</param>
    /// <returns>The transformed WebQL node after the analysis.</returns>
    public static Node Run(string json, Type type)
    {
        var node = SyntaxAnalyser.Parse(json) as Node;
        var context = new SemanticContext(type);

        //*
        // Runs the phases of analysis that are based on the syntax tree.
        //*
        node = RunSemanticFeaturesPhase(context, node);
        node = RunSemanticValidationPhase(context, node);

        return node;
    }

    /// <summary>
    /// Executes the semantic features phase of the analysis pipeline.
    /// This phase processes the node using semantic features like relational operators,
    /// adapting the structure to conform to WebQL syntax rules.
    /// </summary>
    /// <param name="context">The semantic context for analysis.</param>
    /// <param name="node">The node to be processed with semantic features.</param>
    /// <returns>The node transformed by the semantic features phase.</returns>
    private static Node RunSemanticFeaturesPhase(SemanticContext context, Node node)
    {
        node = new ImplicitAndSyntaxFeature()
           .Visit(context, node)
           .As<ObjectNode>();

        return node;
    }

    private static Node RunSemanticValidationPhase(SemanticContext context, Node node)
    {
        return node;
    }
}
