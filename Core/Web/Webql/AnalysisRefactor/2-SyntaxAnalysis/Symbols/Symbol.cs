using ModularSystem.Webql.Analysis.Symbols;
using System.Globalization;

namespace ModularSystem.Webql.Analysis;

public abstract class Symbol 
{
    public string Hash { get; }

    protected Symbol()
    {
        Hash = Guid.NewGuid().ToString();
    }

    public abstract override string ToString();

    protected static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Se o valor já começa com um caractere minúsculo, retorne como está.
        if (char.IsLower(value[0]))
        {
            return value;
        }

        // Converta o primeiro caractere para minúsculo.
        string camelCase = char.ToLower(value[0], CultureInfo.CurrentCulture) + value.Substring(1);

        return camelCase;
    }

    protected string Stringify(ExpressionOperator exprType)
    {
        return $"${ToCamelCase(exprType.ToString())}";
    }
}

public class ProductionSemantics
{

}
