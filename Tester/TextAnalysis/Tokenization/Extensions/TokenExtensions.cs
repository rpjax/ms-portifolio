namespace ModularSystem.Core.TextAnalysis.Tokenization.Extensions;

public static class TokenExtensions
{
    public static string GetNormalizedStringValue(this Token token)
    {
        if(token.Type != TokenType.String)
        {
            throw new ArgumentException("Token is not a string token.");
        }

        return token.Value[1..^1];
    }
}
