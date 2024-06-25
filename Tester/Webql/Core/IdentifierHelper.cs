using ModularSystem.Core;

namespace Webql.Core;

public static class IdentifierHelper
{
    public static string NormalizeIdentifier(string identifier)
    {
        return identifier.ToCamelCase();
    }
}
