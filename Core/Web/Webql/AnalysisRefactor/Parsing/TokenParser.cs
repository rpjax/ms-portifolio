using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public static class TokenParser
{
    public static AxiomSymbol ParseAxiom(ParsingContext context, ArrayToken token)
    {
        return new AxiomParser()
            .ParseAxiom(context, token);
    }

    public static LambdaSymbol ParseLambda(ParsingContext context, ArrayToken token)
    {
        return new LambdaParser()
            .ParseLambda(context, token);
    }

    public static LambdaSymbol ParseUnaryLambda(ParsingContext context, ArrayToken token)
    {
        return new LambdaParser()
            .ParseUnaryLambda(context, token);
    }

    public static LambdaArgumentsSymbol ParseLambdaArguments(ParsingContext context, ArrayToken token)
    {
        return new LambdaArgumentsParser()
            .ParseLambdaArguments(context, token);
    }

    public static LambdaArgumentsSymbol ParseUnaryLambdaArguments(ParsingContext context, ArrayToken token)
    {
        return new LambdaArgumentsParser()
           .ParseUnaryLambdaArguments(context, token);
    }

    public static ObjectSymbol ParseObject(ParsingContext context, ObjectToken token)
    {
        return new ObjectParser()
            .ParseObject(context, token);
    }

    public static ProjectionObjectSymbol ParseProjectionObject(ParsingContext context, ObjectToken token)
    {
        return new ProjectionObjectParser()
            .ParseProjectionObject(context, token);
    }

    public static ExprSymbol ParseExpr(ParsingContext context, ObjectProperty property)
    {
        return new ExprParser()
            .ParseExpr(context, property);
    }

    public static ReferenceSymbol ParseReference(ParsingContext context, StringToken token)
    {
        return new ReferenceParser()
            .ParseReference(context, token);    
    }

    public static DestinationSymbol ParseDestination(ParsingContext context, ValueToken token)
    {
        return new DestinationParser()
            .ParseDestination(context, token);
    }

    public static ArgumentSymbol ParseArgument(ParsingContext context, Token token)
    {
        return new ArgumentParser()
            .ParseArgument(context, token);
    }

    public static BinaryArgumentsSymbol ParseBinaryArguments(ParsingContext context, ArrayToken token)
    {
        return new BinaryArgumentsParser()
            .ParseBinaryArguments(context, token);
    }

}
