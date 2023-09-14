using ModularSystem.Core;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ModularSystem.WebQL;

public static class Operators
{
    public const string Type = "type";
    public const string Equal = "equals";
    public const string Not = "not";
    public const string Greater = "greater";
    public const string GreaterEquals = "greaterEquals";
    public const string Less = "less";
    public const string LessEquals = "lessEquals";
    public const string Like = "like";
    public const string RegexLike = "regexlike";
    public const string Any = "any";
    public const string All = "all";
}

public class QueryDefinition
{
    public string? JsonString { get; }
    public JsonObject? RootNode { get; }

    public QueryDefinition(string? json)
    {
        JsonString = json;

        if (json != null)
        {
            RootNode = JsonNode.Parse(json)?.AsObject();
        }
    }
}

public class ExpressionDefinition
{
    public JsonNode Node { get; }

    public ExpressionDefinition(JsonNode node)
    {
        Node = node;
    }

    public JsonProperty GetJsonProperty()
    {
        return Parser.GetFirstJsonProperty(Node);
    }

    public ParseStackTrace StackTrace()
    {
        return new ParseStackTrace(Node);
    }
}

public class ParseContext
{
    public JsonObject RootNode { get; }
    public Type Type { get; }
    public ParameterExpression Parameter { get; }

    public ParseContext(JsonObject rootNode, Type type, ParameterExpression? parameter = null)
    {
        RootNode = rootNode;
        Type = type;
        Parameter = parameter ?? Expression.Parameter(Type);
    }
}

public abstract partial class Parser
{
    public const string DefaultOperatorIdentifier = "$";

    protected string OperatorIdentifier { get; init; }
    protected ParseContext Context { get; }

    protected Parser(ParseContext context)
    {
        OperatorIdentifier = DefaultOperatorIdentifier;
        Context = context;
    }

    public static JsonDocument GetJsonDocument(JsonNode node)
    {
        return JsonDocument.Parse(node.ToJsonString());
    }

    public static JsonElement GetJsonElement(JsonNode node)
    {
        return JsonDocument.Parse(node.ToJsonString()).RootElement;
    }

    public static JsonProperty[] GetJsonProperties(JsonNode node)
    {
        return GetJsonDocument(node).RootElement.EnumerateObject().ToArray();
    }

    public static JsonProperty GetFirstJsonProperty(JsonNode node)
    {
        var properties = GetJsonProperties(node);

        if (properties.IsEmpty())
        {
            throw new Exception();
        }

        return properties.First();
    }

    public abstract Expression Parse(ExpressionDefinition expressionDefinition);

    public PropertyInfo GetPropertyInfo(JsonNode node, string propertyName)
    {
        var propertiesEnumerable = GetJsonProperties(node)
            .Where(x => x.Name == propertyName);

        if (propertiesEnumerable.IsEmpty())
        {
            throw new ParseException("", new ParseStackTrace(node));
        }

        var propertyElement = propertiesEnumerable.First();
        var entityType = Context.Type;

        var entityProperties = entityType.GetProperties()
            .Where(x => x.Name.ToLower() == propertyName.ToLower())
            .ToArray();

        if (entityProperties.IsEmpty())
        {
            throw new ParseException($"Could not find property '{propertyName}' in type '{entityType.Name}'.", new ParseStackTrace(node));
        }

        return entityProperties[0];
    }

    public Type GetPropertyType(JsonNode node, string propertyName)
    {
        return GetPropertyInfo(node, propertyName).PropertyType;
    }
}

//public class MemberAccessParser : Parser
//{
//    public MemberAccessParser(ParseContext context) : base(context)
//    {
//    }

//    public override Expression Parse(ExpressionDefinition expressionDefinition)
//    {
//        var jsonProperty = expressionDefinition.GetJsonProperty();
//        var name = jsonProperty.Name;

//        if (jsonProperty.Value.ValueKind != JsonValueKind.Object)
//        {
//            throw new ParseException($"Incorrect usage of member access, the value of the referenced property '{name}' must be an expression.", new ParseStackTrace(expressionDefinition.Node));
//        }
//        if (name.StartsWith(OperatorIdentifier))
//        {
//            throw new ParseException("Expected a property name, not an operator.", expressionDefinition.StackTrace());
//        }

//        var propertyType = GetPropertyType(expressionDefinition.Node, name);
//        var member = Expression.Property(Context.Parameter, name);
//        var node = JsonNode.Parse(jsonProperty.Value.GetRawText());
//        var subContext = new ParseContext(node.AsObject(), propertyType);
//        var value = jsonProperty.Value.EnumerateObject();

//        if (value.IsEmpty())
//        {
//            return EmptyExpression(context);
//        }

//        var expression = ParseExpression(subContext, value.Properties().First());

//        return expression;
//    }
//}

