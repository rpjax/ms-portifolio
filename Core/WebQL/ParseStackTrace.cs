using System.Text.Json.Nodes;

namespace ModularSystem.WebQL;

public class ParseStackTrace
{
    public JsonNode Node { get; }

    public ParseStackTrace(JsonNode node)
    {
        Node = node;
    }

    public override string ToString()
    {
        return Node.GetPath();
    }
}