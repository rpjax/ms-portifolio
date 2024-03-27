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

    public ReferenceExpressionSymbol ParseNextReference(ParsingContext context)
    {
        return SyntaxParser.ParseReference(context, ConsumeNextStringToken(context));
    }

    public DestinationSymbol ParseNextDestination(ParsingContext context)
    {
        return SyntaxParser.ParseDestination(context, ConsumeNextStringToken(context));
    }

    public ExpressionSymbol ParseNextExpression(ParsingContext context)
    {
        return SyntaxParser.ParseExpression(context, ConsumeNextToken(context));
    }

    public DeclarationStatementSymbol[] ParseNextDeclarationArray(ParsingContext context)
    {
        return SyntaxParser.ParseDeclarationArray(context, ConsumeNextArrayToken(context));
    }

    public LambdaSymbol ParseNextLambda(ParsingContext context)
    {
        return SyntaxParser.ParseLambda(context, ConsumeNextArrayToken(context));
    }

    public ProjectionLambdaSymbol ParseNextProjectionLambda(ParsingContext context)
    {
        return SyntaxParser.ParseProjectionLambda(context, ConsumeNextArrayToken(context));
    }

    public StatementBlockSymbol ParseStatementBlock(ParsingContext context)
    {
        return SyntaxParser.ParseStatementBlock(context, ConsumeNextObjectToken(context));
    }

    public ProjectionObjectSymbol ParseNextProjectionObject(ParsingContext context)
    {
        return SyntaxParser.ParseProjectionObject(context, ConsumeNextObjectToken(context));
    }

    public ExpressionSymbol[] ParseNextExpressionArray(ParsingContext context)
    {
        return SyntaxParser.ParseArgumentArray(context, ConsumeNextArrayToken(context));
    }

}
