using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularSystem.Webql.Analysis.SyntaxFeatures;

/// <summary>
/// Enalbes the filter syntax for "query documents" where the documents properties are infered "$equals" operators, 
/// combined by logical "$and" operators.
/// </summary>
internal class RelationalOperatorsSyntaxFeature : SemanticsVisitor
{
    [return: NotNullIfNotNull("node")]
    protected override ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        var lhs = node.Lhs.Value;
        var rhs = node.Rhs.Value;

        if(node.Rhs.Value is LiteralNode)
        {

        }

        return base.Visit(context, node);
    }

    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, ObjectNode node)
    {
        var andExpressions = new List<ExpressionNode>();

        foreach (var item in node)
        {
            var lhs = item.Lhs.Value;
            var rhs = item.Rhs.Value;
            var op = HelperTools.ParseOperatorString(lhs);
            var returnType = HelperTools.
        }

        var andLhs = new LhsNode(HelperTools.StringifyOperator(OperatorV2.And));
        var andRhs = new RhsNode(new ArrayNode(andExpressions));
        var andNode = new ExpressionNode(andLhs, andRhs);

        return andNode;
        return base.Visit(context, node);
    }
}
