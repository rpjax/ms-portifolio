using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LambdaParser
{
    public LambdaExpressionSymbol ParseLambda(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var @params = parser.ParseNextDeclarationArray(context);
        var body = parser.ParseNextStatementBlock(context);

        return new LambdaExpressionSymbol(@params, body);
    }

}