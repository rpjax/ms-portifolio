using System.Text.Json;

namespace ModularSystem.Core;

public static class JsonSerializerExtensions
{

}

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
        name.ToLower();
}
