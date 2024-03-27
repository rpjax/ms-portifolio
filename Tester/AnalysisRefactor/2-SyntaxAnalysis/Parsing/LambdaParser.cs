using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LambdaParser
{
    public LambdaSymbol ParseLambda(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var @params = parser.ParseNextDeclarationArray(context);
        var body = parser.ParseStatementBlock(context);

        return new LambdaSymbol(@params, body);
    }

    public ProjectionLambdaSymbol ParseProjectionLambda(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var @params = parser.ParseNextDeclarationArray(context);
        var body = parser.ParseNextProjectionObject(context);

        return new ProjectionLambdaSymbol(@params, body);
    }

}