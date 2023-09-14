using System.Text.Json;

namespace ModularSystem.WebQL;

public static class ParserHelpers
{
    public static string GetPath(JsonElement root, JsonElement target)
    {
        return GetPathRecursive(root, target, "root");
    }

    private static string GetPathRecursive(JsonElement current, JsonElement target, string currentPath)
    {
        if (current.Equals(target))
        {
            return currentPath;
        }

        switch (current.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (JsonProperty property in current.EnumerateObject())
                {
                    string newPath = $"{currentPath}.{property.Name}";
                    string result = GetPathRecursive(property.Value, target, newPath);
                    if (result != null)
                    {
                        return result;
                    }
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (JsonElement element in current.EnumerateArray())
                {
                    string newPath = $"{currentPath}[{index}]";
                    string result = GetPathRecursive(element, target, newPath);
                    if (result != null)
                    {
                        return result;
                    }
                    index++;
                }
                break;
        }

        return null;
    }
}

