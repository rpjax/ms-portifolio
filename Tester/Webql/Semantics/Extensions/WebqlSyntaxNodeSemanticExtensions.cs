using Webql.Parsing.Ast;
using Webql.Semantics.Attributes;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;
using Webql.Semantics.Exceptions;

namespace Webql.Semantics.Extensions;

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeSemanticExtensions
{
    public static string GetSemanticIdentifier(this WebqlSyntaxNode node)
    {
        return "not implemented yet";
    }

    public static bool HasSemanticContext(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstSemanticAttributes.ContextAttribute);
    }

    public static bool HasSemantics(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstSemanticAttributes.SemanticsAttribute);
    }

    public static bool IsScopeSource(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstSemanticAttributes.ScopeSourceAttribute);
    }

    public static bool IsRoot(this WebqlSyntaxNode node)
    {
        return node.NodeType == WebqlNodeType.Query;
    }

    public static SemanticContext GetSemanticContext(this WebqlSyntaxNode node)
    {
        var attribute = node.TryGetAttribute<SemanticContext>(AstSemanticAttributes.ContextAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static ISemantics GetSemantics(this WebqlSyntaxNode node)
    {
        if (!node.HasSemantics())
        {
            return node.GetSemanticContext().GetSemantics(node);
        }

        var attribute = node.TryGetAttribute<ISemantics>(AstSemanticAttributes.SemanticsAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static TSemantics GetSemantics<TSemantics>(this WebqlSyntaxNode node) where TSemantics : ISemantics
    {
        if (!node.HasSemantics())
        {
            return node.GetSemanticContext().GetSemantics<TSemantics>(node);
        }

        var attribute = node.TryGetAttribute<TSemantics>(AstSemanticAttributes.SemanticsAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static void AddSemanticContext(this WebqlSyntaxNode node, SemanticContext context)
    {
        node.AddAttribute(AstSemanticAttributes.ContextAttribute, context);
    }

    public static void AddSemantics(this WebqlSyntaxNode node, ISemantics semantics)
    {
        node.AddAttribute(AstSemanticAttributes.SemanticsAttribute, semantics);
    }

    /*
     * Exceptions extensions
     */

    public static SemanticException CreateOperatorIncompatibleTypeException(
    this WebqlSyntaxNode node,
    Type leftType,
    Type rightType)
    {
        return new SemanticException($@"
Error: Operator Incompatible Type Exception

Description:
The operation you are attempting to perform involves incompatible types.
This error occurs when the types of the operands do not match the expected types
for the operation being executed. Specifically, the operator cannot be applied to 
operands of the given types.

For example, attempting to add a string to an integer or comparing a boolean 
with a string will result in this exception.

Details:
- Left Operand Type: {leftType.FullName}
- Right Operand Type: {rightType.FullName}

Resolution:
Ensure that the operands used in the operation are of compatible types. 
Refer to the documentation or type specifications for the expected operand types 
for the operator being used.
", node);
    }


}
