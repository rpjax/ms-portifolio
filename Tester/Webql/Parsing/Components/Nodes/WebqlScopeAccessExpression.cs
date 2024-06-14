﻿using Webql.Parsing.Tools;
using Webql.Semantics.Components;

namespace Webql.Parsing.Components;

public class WebqlScopeAccessExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public WebqlExpression Expression { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlScopeAccessExpression(
        string identifier, 
        WebqlExpression expression, 
        WebqlSyntaxNodeMetadata metadata, 
        Dictionary<string, object>? attributes = null)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.ScopeAccess;
        Identifier = identifier;
        Expression = expression;
        Attributes = attributes ?? new Dictionary<string, object>();

        Attributes.TryAdd(SemanticContextAttributes.ScopeSourceAttribute, new object());
    }

    public override WebqlSyntaxNode Accept(SyntaxNodeVisitor visitor)
    {
        return visitor.VisitScopeAccessExpression(this);
    }
}

