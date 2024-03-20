using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Translates the &lt;reference&gt; symbol to an Expression.
/// </summary>
public class ReferenceTranslator
{
    /// <summary>
    /// Provides access to translation options and configurations.
    /// </summary>
    private TranslationOptions Options { get; }

    public ReferenceTranslator(TranslationOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;reference&gt; ::= &lt;string-literal&gt;
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateReference(TranslationContext context, LiteralNode node)
    {
        var identifier = node.GetNormalizedValue();

        if (string.IsNullOrEmpty(identifier))
        {
            throw new TranslationException("", context);
        }

        var propPath = identifier;

        //propPath = propPath[1..];

        var pathSplit = propPath.Split('.');
        var rootPropertyName = propPath.Contains('.')
            ? pathSplit.First()
            : propPath;

        var symbol = context.GetSymbol(rootPropertyName);
        var symbolType = symbol.Type;
        var symbolValue = symbol.Expression;

        for (int i = 1; i < pathSplit.Length; i++)
        {
            var propertyName = pathSplit[i];
            var property = symbolType.GetProperty(propertyName);

            if (property == null)
            {
                throw new TranslationException("", context);
            }

            symbolValue = Expression.MakeMemberAccess(symbolValue, property);
            symbolType = symbolValue.Type;
        }

        return symbolValue;
    }
}
