using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public static class SyntaxParser
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

    public static ProjectionLambdaSymbol ParseProjectionLambda(ParsingContext context, ArrayToken token)
    {
        return new LambdaParser()
            .ParseProjectionLambda(context, token);
    }

    public static LambdaSymbol ParseUnaryLambda(ParsingContext context, ArrayToken token)
    {
        return new LambdaParser()
            .ParseUnaryLambda(context, token);
    }

    public static LambdaArgumentSymbol[] ParseLambdaArguments(ParsingContext context, ArrayToken token)
    {
        return new LambdaArgumentsParser()
            .ParseLambdaArguments(context, token);
    }

    public static LambdaArgumentSymbol[] ParseUnaryLambdaArguments(ParsingContext context, ArrayToken token)
    {
        return new LambdaArgumentsParser()
           .ParseUnaryLambdaArguments(context, token);
    }

    public static StatementBlockSymbol ParseStatementBlock(ParsingContext context, ObjectToken token)
    {
        return new StatementBlockParser()
            .ParseStatementBlock(context, token);
    }

    public static ProjectionObjectSymbol ParseProjectionObject(ParsingContext context, ObjectToken token)
    {
        return new ProjectionObjectParser()
            .ParseProjectionObject(context, token);
    }

    public static StatementSymbol ParseStatement(ParsingContext context, ObjectPropertyToken property)
    {
        return new StatementParser()
            .ParseStatement(context, property);
    }

    public static ExpressionSymbol ParseExpression(ParsingContext context, ObjectPropertyToken property)
    {
        return new ExpressionParser()
            .ParseExpression(context, property);
    }

    public static OperatorExpressionSymbol ParseOperatorExpression(ParsingContext context, ObjectPropertyToken property)
    {
        return new OperatorExpressionParser()
            .ParseOperatorExpression(context, property);
    }

    public static ReferenceExpressionSymbol ParseReference(ParsingContext context, StringToken token)
    {
        return new ReferenceParser()
            .ParseReference(context, token);    
    }

    public static DestinationSymbol ParseDestination(ParsingContext context, ValueToken token)
    {
        return new DestinationParser()
            .ParseDestination(context, token);
    }

    public static ExpressionSymbol ParseExpression(ParsingContext context, Token token)
    {
        return new ExpressionParser()
            .ParseExpression(context, token);
    }

    public static BinaryArgumentsSymbol ParseBinaryArguments(ParsingContext context, ArrayToken token)
    {
        return new BinaryArgumentsParser()
            .ParseBinaryArguments(context, token);
    }

    public static ExpressionSymbol[] ParseArgumentArray(ParsingContext context, ArrayToken token)
    {
        return new ExpressionArrayParser()
            .ParseExpressionArray(context, token);
    }

}
