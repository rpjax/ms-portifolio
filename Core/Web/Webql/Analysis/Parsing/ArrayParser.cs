using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ArrayParser
{
    public int TokensCount => Tokens.Count;

    private Queue<Token> Tokens { get; }

    public ArrayParser(ArrayToken arrayToken)
    {
        Tokens = new Queue<Token>(arrayToken);
    }

    //*
    // consumer methods.
    //*

    public T ConsumeNextToken<T>(ParsingContext context) where T : Token
    {
        if (TokensCount == 0)
        {
            throw new ParsingException("", context);
        }

        var token = Tokens.Dequeue();

        if(token is not T result)
        {
            throw new ParsingException($"Expected a token of type: '{typeof(T).Name}', but encountered a token of type: '{token.GetType().Name}'.", context);
        }

        return result;
    }

    public Token ConsumeNextToken(ParsingContext context)
    {
        return ConsumeNextToken<Token>(context);
    }

    public ArrayToken ConsumeNextArrayToken(ParsingContext context)
    {
        return ConsumeNextToken<ArrayToken>(context);
    }

    public ObjectToken ConsumeNextObjectToken(ParsingContext context)
    {
        return ConsumeNextToken<ObjectToken>(context);
    }

    public ValueToken ConsumeNextValueToken(ParsingContext context)
    {
        return ConsumeNextToken<ValueToken>(context);
    }

    public StringToken ConsumeNextStringToken(ParsingContext context)
    {
        return ConsumeNextToken<StringToken>(context);
    }

    public NumberToken ConsumeNextNumberToken(ParsingContext context)
    {
        return ConsumeNextToken<NumberToken>(context);
    }

    //*
    // parser methods.
    //*

    public ReferenceSymbol ParseNextReference(ParsingContext context)
    {
        return TokenParser.ParseReference(context, ConsumeNextStringToken(context));
    }

    public DestinationSymbol ParseNextDestination(ParsingContext context)
    {
        return TokenParser.ParseDestination(context, ConsumeNextStringToken(context));
    }

    public ArgumentSymbol ParseNextArgument(ParsingContext context)
    {
        return TokenParser.ParseArgument(context, ConsumeNextToken(context));
    }

    public LambdaArgumentsSymbol ParseNextLambdaArguments(ParsingContext context)
    {
        return TokenParser.ParseLambdaArguments(context, ConsumeNextArrayToken(context));
    }

    public LambdaArgumentsSymbol ParseNextUnaryLambdaArguments(ParsingContext context)
    {
        return TokenParser.ParseUnaryLambdaArguments(context, ConsumeNextArrayToken(context));
    }

    public LambdaSymbol ParseNextLambda(ParsingContext context)
    {
        return TokenParser.ParseLambda(context, ConsumeNextArrayToken(context));
    }

    public LambdaSymbol ParseNextUnaryLambda(ParsingContext context)
    {
        return TokenParser.ParseUnaryLambda(context, ConsumeNextArrayToken(context));
    }

    public ObjectSymbol ParseNextObject(ParsingContext context)
    {
        return TokenParser.ParseObject(context, ConsumeNextObjectToken(context));
    }

}
