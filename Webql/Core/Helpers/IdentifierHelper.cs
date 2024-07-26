using ModularSystem.Core.Extensions;

namespace Webql.Core.Helpers;

public static class IdentifierHelper
{
    public static string NormalizeIdentifier(string identifier)
    {
        return identifier.ToCamelCase();
    }
}
