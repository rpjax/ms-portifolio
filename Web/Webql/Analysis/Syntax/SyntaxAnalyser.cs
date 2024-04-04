using System.Text.Json.Nodes;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Provides methods for parsing JSON into specific WebQL syntax nodes.
/// </summary>
public static class SyntaxAnalyser
{
    /// <summary>
    /// Parses a JSON string to its corresponding WebQL syntax tree.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The parsed syntax tree.</returns>
    /// <exception cref="SyntaxException">Thrown when there's an error in the syntax of the provided JSON.</exception>
    public static ObjectNode Parse(string json)
    {
        var node = JsonNode.Parse(json);
        var context = new SyntaxContext();

        if (node == null)
        {
            throw new SyntaxException("Failed to parse the JSON string. Ensure the input is a valid JSON format. Check for syntax errors, missing or misplaced brackets, and correct any inconsistencies in the JSON structure.", context);
        }
        if (node is JsonObject jsonObject)
        {
            return ParseObject(context, jsonObject);
        }

        throw new SyntaxException("Invalid query structure: The root element of the query must be a JSON object. Found: " + node.GetType().Name, context);
    }

    private static RhsType GetRhsType(SyntaxContext context, JsonNode? jsonNode)
    {
        if (jsonNode == null)
        {
            return RhsType.Literal;
        }
        if (jsonNode is JsonObject)
        {
            return RhsType.Object;
        }
        if (jsonNode is JsonArray)
        {
            return RhsType.Array;
        }
        if (jsonNode is JsonValue)
        {
            return RhsType.Literal;
        }

        throw new SyntaxException("The right-hand side (RHS) of the expression is invalid. Ensure that the RHS is a proper literal, object, or array as per the WebQL syntax.", context);
    }

    private static ObjectNode ParseObject(SyntaxContext context, JsonObject jsonObject)
    {
        var children = new List<ExpressionNode>();

        foreach (var item in jsonObject)
        {
            children.Add(ParseExpression(context, item));
        }

        return new ObjectNode(children);
    }

    private static ExpressionNode ParseExpression(SyntaxContext context, KeyValuePair<string, JsonNode?> item)
    {
        var lhs = ParseLeftHandSide(context, item.Key);
        var subContext = context.CreateSubContext(lhs.Value);
        var rhs = ParseRightHandSide(subContext, item.Value);

        return new ExpressionNode(lhs, rhs);
    }

    private static LhsNode ParseLeftHandSide(SyntaxContext context, string key)
    {
        return new LhsNode(key);
    }

    private static RhsNode ParseRightHandSide(SyntaxContext context, JsonNode? jsonNode)
    {
        if (jsonNode == null)
        {
            return new RhsNode(ParseLiteral(context, null));
        }

        switch (GetRhsType(context, jsonNode))
        {
            case RhsType.Literal:
                return new RhsNode(ParseLiteral(context, jsonNode.AsValue()));

            case RhsType.Object:
                return new RhsNode(ParseObject(context, jsonNode.AsObject()));

            case RhsType.Array:
                return new RhsNode(ParseArray(context, jsonNode.AsArray()));

            default:
                throw new Exception();
        }
    }

    private static ArrayNode ParseArray(SyntaxContext context, JsonArray jsonArray)
    {
        var nodes = new List<Node>();

        for (int i = 0; i < jsonArray.Count; i++)
        {
            var item = jsonArray[i];
            var subContext = context.CreateSubContext($"[{i}]");

            if (item is null)
            {
                nodes.Add(new NullNode());
                continue;
            }

            if (item is JsonObject)
            {
                nodes.Add(ParseObject(subContext, item.AsObject()));
                continue;
            }

            if (item is JsonValue)
            {
                nodes.Add(ParseLiteral(subContext, item.AsValue()));
                continue;
            }

            if(item is JsonArray)
            {
                nodes.Add(ParseArray(context, item.AsArray()));
                continue;
            }

            throw new SyntaxException("Invalid item type in array on the right-hand side. Each item must be either a literal or an object.", context);
        }

        return new ArrayNode(nodes);
    }

    private static LiteralNode ParseLiteral(SyntaxContext context, JsonValue? jsonNode)
    {
        if(jsonNode == null)
        {
            return new LiteralNode(null);
        }

        return new LiteralNode(jsonNode.ToJsonString());
    }

}
