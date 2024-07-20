using Webql.Parsing.Ast;

namespace Webql.Core.Helpers;

public static class OperatorHelper
{
    public static string ToPascalCase(WebqlOperatorType operatorType)
    {
        var str = operatorType.ToString();
        var pascalCase = "";

        if (!string.IsNullOrEmpty(str))
        {
            var words = str.Split('_');
            foreach (var word in words)
            {
                pascalCase += char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
        }

        return pascalCase;
    }

    public static string ToCamelCase(WebqlOperatorType operatorType)
    {
        var str = operatorType.ToString();
        var camelCase = "";

        if (!string.IsNullOrEmpty(str))
        {
            var words = str.Split('_');
            foreach (var word in words)
            {
                camelCase += char.ToLower(word[0]) + word.Substring(1).ToLower();
            }
        }

        return camelCase;
    }
}
