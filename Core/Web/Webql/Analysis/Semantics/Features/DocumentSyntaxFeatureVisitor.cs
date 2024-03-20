﻿using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.SyntaxFeatures;

public class DocumentSyntaxFeatureVisitor : SemanticsVisitor
{
    private List<string> MemberAccessPath { get; } = new();

    [return: NotNullIfNotNull("node")]
    protected override Node Visit(SemanticContext context, ObjectNode node)
    {
        return base.Visit(context, node);
    }
}
