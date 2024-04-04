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
    /// <param name="query">The JSON representation of the WebQL query.</param>
    /// <param name="type">The type context within which the query is analyzed.</param>
    /// <returns>The transformed WebQL node after the analysis.</returns>
    public static Node Run(string query)
    {
        var node = SyntaxAnalyser.Parse(query) as Node;

        //*
        // Runs the phases of analysis that are based on the syntax tree.
        //*
        node = RunSemanticFeaturesPhase(node);
        node = RunSemanticValidationPhase(node);

        return node;
    }

    private static Node RunSemanticFeaturesPhase(Node axiom)
    {
        axiom = new ImplicitEqualsSyntaxFeature()
            .Visit(new SemanticContextOld(), axiom);

        axiom = new ImplicitAndSyntaxFeature()
           .Visit(new SemanticContextOld(), axiom);

        return axiom;
    }

    private static Node RunSemanticValidationPhase(Node axiom)
    {
        return axiom;
    }
}
