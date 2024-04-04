using ModularSystem.Webql.Analysis.Tokens;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ModularSystem.Webql.Analysis.Tokenization;

public class LexicalAnalyser
{
    public Token Tokenize(string json)
    {
        return Tokenize(JsonNode.Parse(json));
    }

    private Token Tokenize(JsonNode? node)
    {
        if(node is null)
        {
            return new NullToken();
        }
        if(node is JsonObject jsonObject)
        {
            return Tokenize(jsonObject);
        }
        if (node is JsonArray jsonArray)
        {
            return Tokenize(jsonArray);
        }
        if (node is JsonValue jsonValue)
        {
            return Tokenize(jsonValue);
        }

        throw new Exception();
    }

    private ObjectToken Tokenize(JsonObject jsonObject)
    {
        var props = new List<ObjectPropertyToken>();

        foreach (var item in jsonObject)
        {
            props.Add(new ObjectPropertyToken(item.Key, Tokenize(item.Value)));
        }

        return new ObjectToken(props.ToArray());
    }

    private ArrayToken Tokenize(JsonArray jsonArray)
    {
        var values = new List<Token>();

        foreach (var item in jsonArray)
        {
            values.Add(Tokenize(item));
        }

        return new ArrayToken(values.ToArray());
    }

    private Token Tokenize(JsonValue jsonValue)
    {
        var jsonElement = jsonValue.GetValue<JsonElement>();
        var type = jsonElement.ValueKind;jsonValue.GetValue<JsonElement>();

        switch (type)
        {
            case JsonValueKind.Object:
                return Tokenize(jsonValue.AsObject());

            case JsonValueKind.Array:
                return Tokenize(jsonValue.AsArray());

            case JsonValueKind.String:
                return new StringToken(jsonElement.GetString());

            case JsonValueKind.Number:
                return new NumberToken(jsonElement.GetRawText());

            case JsonValueKind.True:
            case JsonValueKind.False:
                return new BoolToken(jsonElement.GetBoolean());

            case JsonValueKind.Null:
                return new NullToken();

            case JsonValueKind.Undefined:
            default:
                
                throw new Exception();
        }
    }

}
