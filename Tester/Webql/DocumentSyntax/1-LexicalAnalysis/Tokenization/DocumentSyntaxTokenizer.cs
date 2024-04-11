using ModularSystem.Webql.Analysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;

public class DocumentSyntaxTokenizer
{
    public IEnumerable<Token> Tokenize(IEnumerable<char> source)
    {
        var analyser = new Tokenizer();

        foreach (var token in analyser.Tokenize(source))
        {
            if (token.TokenType == TokenType.String)
            {
                var unquotedValue = token.Value[1..^1];

                if (unquotedValue.StartsWith("$"))
                {
                    yield return new Token(TokenType.Operator, unquotedValue, token.Metadata);
                    continue;
                }
            }
            
            yield return token;
        }
    }
}

