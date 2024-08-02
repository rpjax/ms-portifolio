using System.Diagnostics.CodeAnalysis;

namespace Aidan.Webql.Analysis.SyntaxFeatures;

public class DocumentSyntaxFeatureVisitor : SemanticsVisitor
{
    private List<string> MemberAccessPath { get; } = new();

    [return: NotNullIfNotNull("node")]
    protected override Node Visit(SemanticContextOld context, ObjectNode node)
    {
        return base.Visit(context, node);
    }
}
