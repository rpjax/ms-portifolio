using System.Globalization;

namespace ModularSystem.Webql.Analysis.Symbols;

public interface ISymbol
{
    string Hash { get; }
}

public abstract class Symbol : ISymbol
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

    protected string Stringify(Symbols.OperatorType exprType)
    {
        return $"${ToCamelCase(exprType.ToString())}";
    }
}

