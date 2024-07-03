using Webql.Semantics.Attributes;

namespace Webql.Parsing.Ast;

//public class WebqlScopeAccessExpression : WebqlExpression
//{
//    public override WebqlSyntaxNodeMetadata Metadata { get; }
//    public override Dictionary<string, object> Attributes { get; }
//    public override WebqlExpressionType ExpressionType { get; }
//    public string Identifier { get; }
//    public WebqlExpression Expression { get; }

//    public WebqlScopeAccessExpression(
//        WebqlSyntaxNodeMetadata metadata, 
//        Dictionary<string, object>? attributes,
//        string identifier, 
//        WebqlExpression expression)
//    {
//        Metadata = metadata;
//        Attributes = attributes ?? new Dictionary<string, object>();
//        ExpressionType = WebqlExpressionType.ScopeAccess;
//        Identifier = identifier;
//        Expression = expression;

//        Attributes.TryAdd(AstSemanticAttributes.ScopeSourceAttribute, new object());
//        Expression.Parent = this;
//    }

//    public override string ToString()
//    {
//        return $"{Identifier}: {Expression}";
//    }

//}
