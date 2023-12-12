namespace Core.TextAnalysis;

public class BnfGrammar
{
    public char[] IgnoredChars { get; set; } = Array.Empty<char>();
    public string[] IgnoredTokens { get; set; } = Array.Empty<string>();
    public TokenProduction[] Productions { get => GetProductions(); set => SetProductions(value); }
    public Dictionary<string, TokenProduction> ProductionsMap { get; private set; } = new();

    public BnfGrammar()
    {

    }

    public BnfGrammar Copy()
    {
        return new BnfGrammar()
        {
            Productions = Productions.ToArray(),
            IgnoredTokens = IgnoredTokens.ToArray(),
        };
    }

    public TokenProduction[] GetProductions()
    {
        return ProductionsMap.Values.ToArray();
    }

    public void SetProductions(TokenProduction[] productions)
    {
        var dic = new Dictionary<string, TokenProduction>();

        foreach (var production in productions)
        {
            foreach (var formationStr in production.GetFormationStrings())
            {
                dic.Add(formationStr, production);
            }
        }

        ProductionsMap = dic;
    }
}

// <digit> ::= ...
// <letter> ::= ...
// <identifier> ::= ...
// <number> ::=
// <operator> ::= ...
// <term> ::= <identifier> | <number>
// <expression> ::= <term> | <expression> "+" <term>
// "5 + 5" => "<digit><ignore><operator><ignore><digit>"
// 4 * sizeof(char) | (4 * sizeof(char)) + (4 * sizeof(object))
// "1024" => "<digit><digit><digit><digit>" => "<number>"
