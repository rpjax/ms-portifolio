using ModularSystem.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public class ExpressionJsonConverter : JsonConverter
{
    public override bool CanConvert(Type type)
    {
        return type.IsAssignableFrom(typeof(ExpressionNode));
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        JObject jo = JObject.Load(reader);

        if (jo.TryGetValue("Operator", out var token))
        {
            var nodeOperator = token.ToObject<ExpressionType>();
            var nodeType = ExpressionNode.GetNodeType(nodeOperator);
            var node = serializer.Deserialize(jo.CreateReader(), nodeType);
            return node;
        }

        throw new AppException("The JSON was not a valid expression node.", ExceptionCode.InvalidInput);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // so far this method was never called...
        throw new NotImplementedException();
    }
}