using Webql.Parsing.Ast.Extensions;

namespace Webql.Parsing.Ast;

/*
 * There's no support for block expressions in most LINQ providers, so any translation for the block would be confusing and implicit.
 * The main purpose for initially supporting block expressions was to allow variable declarations and assignments. This has been achieved through the use of temporary declarations.
 */

//public class WebqlBlockExpression : WebqlExpression
//{
//    public override WebqlSyntaxNodeMetadata Metadata { get; }
//    public override Dictionary<string, object> Attributes { get; }
//    public override WebqlExpressionType ExpressionType { get; }
//    public WebqlExpression[] Expressions { get; }

//    public WebqlBlockExpression(
//        WebqlSyntaxNodeMetadata metadata,
//        Dictionary<string, object>? attributes,
//        IEnumerable<WebqlExpression> expressions)
//    {
//        Metadata = metadata;
//        Attributes = attributes ?? new Dictionary<string, object>();
//        ExpressionType = WebqlExpressionType.Block;
//        Expressions = expressions.ToArray();

//        foreach (var expression in Expressions)
//        {
//            expression.Parent = this;
//        }
//    }

//    public override IEnumerable<WebqlSyntaxNode> GetChildren()
//    {
//        return Expressions;
//    }

//    public override string ToString()
//    {
//        return $"{{ {string.Join(", ", Expressions.Select(x => x.ToString()))} }}";
//    }

//}
