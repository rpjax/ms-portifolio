using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Syntax.Extensions;
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

    public BoolToken ConsumeNextBoolToken(ParsingContext context)
    {
        return ConsumeNextToken<BoolToken>(context);
    }

    public NullToken ConsumeNextNullToken(ParsingContext context)
    {
        return ConsumeNextToken<NullToken>(context);
    }

    //*
    // parser methods.
    //*

    public StringSymbol ParseNextStringLiteral(ParsingContext context)
    {
        return SyntaxParser.ParseLiteral(context, ConsumeNextStringToken(context))
            .As<StringSymbol>(context);
    }

    public LiteralExpressionSymbol ParseNextNullableStringLiteral(ParsingContext context)
    {
        var token = ConsumeNextToken<ValueToken>(context);

        if (token is NullToken)
        {
            return new NullSymbol();
        }
        if (token is not StringToken stringToken)
        {
            throw new Exception();
        }

        return new StringSymbol(stringToken.Value);
    }

    public NumberSymbol ParseNextNumberLiteral(ParsingContext context)
    {
        return SyntaxParser.ParseLiteral(context, ConsumeNextNumberToken(context))
            .As<NumberSymbol>(context);
    }

    /// <summary>
    /// This parsing has a limitation due to the lexical analysis rely on a JSON parser to produce the tokens. <br/>
    /// Ex: an expression could be: literal_expression | operator_expression | reference_expression | lambda_expression; <br/>
    /// However a JSON array can only contain predefined values such as: 5, "string", true, { ... }, [...], null; <br/>
    /// So it's limited in how those expressions can be represented. <br/>
    /// In this context a statement_block, defined by an object: "{ statements }", <br/>
    /// could be interpreted as an expression_block, adding a semantic meaning that the block itself resolves to a value: "{ expressions }" <br/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ExpressionSymbol ParseNextExpression(ParsingContext context)
    {
        var token = ConsumeNextToken(context);

        if(token is ObjectPropertyToken objectProperty)
        {
            return SyntaxParser.ParseExpression(context, objectProperty);
        }
        if (token is ValueToken valueToken)
        {
            return SyntaxParser.ParseExpression(context, valueToken);
        }

        throw new Exception();
    }

    public DeclarationStatementSymbol[] ParseNextDeclarationArray(ParsingContext context)
    {
        return SyntaxParser.ParseDeclarationArray(context, ConsumeNextArrayToken(context));
    }

    public LambdaExpressionSymbol ParseNextLambda(ParsingContext context)
    {
        return SyntaxParser.ParseLambda(context, ConsumeNextArrayToken(context));
    }

    public ProjectionLambdaSymbol ParseNextProjectionLambda(ParsingContext context)
    {
        return SyntaxParser.ParseProjectionLambda(context, ConsumeNextArrayToken(context));
    }

    public StatementBlockSymbol ParseNextStatementBlock(ParsingContext context)
    {
        return SyntaxParser.ParseStatementBlock(context, ConsumeNextObjectToken(context));
    }

    public AnonymousTypeExpressionSymbol ParseNextTypeProjection(ParsingContext context)
    {
        return SyntaxParser.ParseTypeProjection(context, ConsumeNextObjectToken(context));
    }

    public ExpressionSymbol[] ParseNextExpressionArray(ParsingContext context)
    {
        return SyntaxParser.ParseExpressionArray(context, ConsumeNextArrayToken(context));
    }

}
