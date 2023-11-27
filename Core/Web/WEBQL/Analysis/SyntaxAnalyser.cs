using System.Text.Json.Nodes;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Provides methods for parsing JSON nodes into specific WebQL syntax nodes.
/// </summary>
public static class SyntaxAnalyser
{
    /// <summary>
    /// Parses a JsonNode to its corresponding WebQL Node.
    /// </summary>
    /// <param name="node">The JsonNode to parse.</param>
    /// <returns>The parsed Node.</returns>
    /// <exception cref="Exception">Thrown when jsonNode is not a JsonObject.</exception>
    public static Node Parse(JsonNode node)
    {
        if (node is JsonObject jsonObject)
        {
            return ParseScopeDefinition(jsonObject);
        }

        throw new SyntaxException("Invalid query structure: The root element of the query must be a JSON object. Found: " + node.GetType().Name, node);
    }

    /// <summary>
    /// Parses a JsonObject to a ScopeDefinitionNode.
    /// </summary>
    /// <param name="jsonObject">The JsonObject to parse.</param>
    /// <returns>The parsed ScopeDefinitionNode.</returns>
    public static ScopeDefinitionNode ParseScopeDefinition(JsonObject jsonObject)
    {
        var children = new List<ExpressionNode>();

        foreach (var item in jsonObject)
        {
            children.Add(ParseExpression(item));
        }

        return new ScopeDefinitionNode(children);
    }

    /// <summary>
    /// Parses the left-hand side of an expression.
    /// </summary>
    /// <remarks>
    /// The left-hand side of an expression in WebQL is always a string.
    /// </remarks>
    /// <param name="key">The key representing the left-hand side.</param>
    /// <returns>The parsed LhsNode.</returns>
    public static LhsNode ParseLeftHandSide(string key)
    {
        return new LhsNode(key);
    }

    /// <summary>
    /// Parses the right-hand side of an expression.
    /// </summary>
    /// <param name="jsonNode">The JsonNode representing the right-hand side.</param>
    /// <returns>The parsed RhsNode.</returns>
    /// <exception cref="Exception">Thrown when the right-hand side type is invalid or null.</exception>
    public static RhsNode ParseRightHandSide(JsonNode? jsonNode)
    {
        if (jsonNode == null)
        {
            return new RhsNode(new LiteralNode("null"));
        }

        var type = RhsNode.GetRhsType(jsonNode);

        if (type == RhsType.Invalid)
        {
            throw new SyntaxException("The right-hand side (RHS) of the expression is invalid. Ensure that the RHS is a proper literal, object, or array as per the WebQL syntax.", jsonNode);
        }
        if (type == RhsType.Literal)
        {
            return new RhsNode(ParseLiteral(jsonNode.AsValue()));
        }

        if (type == RhsType.Object)
        {
            return new RhsNode(ParseScopeDefinition(jsonNode.AsObject()));
        }

        if (type == RhsType.Array)
        {
            var nodes = new List<Node>();
            var array = jsonNode.AsArray();

            foreach (var item in array)
            {
                if (item is JsonObject)
                {
                    nodes.Add(ParseScopeDefinition(item.AsObject()));
                    continue;
                }

                if (item is JsonValue)
                {
                    nodes.Add(ParseLiteral(item.AsValue()));
                    continue;
                }

                throw new SyntaxException("Invalid item type in array on the right-hand side. Each item must be either a literal or an object.", item ?? array);
            }

            return new RhsNode(new ArrayNode(nodes));
        }

        throw new Exception();
    }

    /// <summary>
    /// Parses a KeyValuePair to an ExpressionNode.
    /// </summary>
    /// <param name="item">The KeyValuePair to parse.</param>
    /// <returns>The parsed ExpressionNode.</returns>
    public static ExpressionNode ParseExpression(KeyValuePair<string, JsonNode?> item)
    {
        var lhs = ParseLeftHandSide(item.Key);
        var rhs = ParseRightHandSide(item.Value);

        return new ExpressionNode(lhs, rhs);
    }

    /// <summary>
    /// Parses a JsonValue to a LiteralNode.
    /// </summary>
    /// <param name="jsonNode">The JsonValue to parse.</param>
    /// <returns>The parsed LiteralNode.</returns>
    public static LiteralNode ParseLiteral(JsonValue jsonNode)
    {
        return new LiteralNode(jsonNode.ToJsonString());
    }

}
