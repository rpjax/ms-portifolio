using System.Globalization;

namespace Aidan.Core;

public static class CultureProvider
{
    public static readonly CultureInfo Brazil = new CultureInfo("pt-BR");

    public static CultureInfo CultureInfo { get; set; } = Brazil;
}