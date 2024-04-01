using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

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

    public ProjectionLambdaSymbol ParseProjectionLambda(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var @params = parser.ParseNextDeclarationArray(context);
        var body = parser.ParseNextTypeProjection(context);

        return new ProjectionLambdaSymbol(@params, body);
    }

}