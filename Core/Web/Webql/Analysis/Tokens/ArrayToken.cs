using System.Collections;

namespace ModularSystem.Webql.Analysis.Tokens;

public class ArrayToken : Token, IEnumerable<Token>
{
    public Token[] Values { get; }

    public ArrayToken(Token[] values)
    {
        Values = values;
    }

    public IEnumerator<Token> GetEnumerator()
    {
        return Values.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.GetEnumerator();
    }
}
