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

    public static LambdaExpressionSymbol ParseLambda(ParsingContext context, ArrayToken token)
    {
        return new LambdaParser()
            .ParseLambda(context, token);
    }

    public static ProjectionLambdaSymbol ParseProjectionLambda(ParsingContext context, ArrayToken token)
    {
        return new LambdaParser()
            .ParseProjectionLambda(context, token);
    }

    public static DeclarationStatementSymbol[] ParseDeclarationArray(ParsingContext context, ArrayToken token)
    {
        return new DeclarationArrayParser()
            .ParseDeclarationArray(context, token);
    }

    public static StatementBlockSymbol ParseStatementBlock(ParsingContext context, ObjectToken token)
    {
        return new StatementBlockParser()
            .ParseStatementBlock(context, token);
    }

    public static StatementSymbol ParseStatement(ParsingContext context, ObjectPropertyToken token)
    {
        return new StatementParser()
            .ParseStatement(context, token);
    }

    public static AnonymousTypeExpressionSymbol ParseTypeProjection(ParsingContext context, ObjectToken token)
    {
        return new TypeExpressionParser()
            .ParseTypeExpression(context, token);
    }

    public static ExpressionSymbol ParseExpression(ParsingContext context, ObjectPropertyToken token)
    {
        return new ExpressionParser()
            .ParseExpression(context, token);
    }

    public static ExpressionSymbol ParseExpression(ParsingContext context, ValueToken token)
    {
        return new ExpressionParser()
            .ParseExpression(context, token);
    }

    public static ExpressionSymbol[] ParseExpressionArray(ParsingContext context, ArrayToken token)
    {
        return new ExpressionArrayParser()
            .ParseExpressionArray(context, token);
    }

    public static ExpressionSymbol ParseLiteral(ParsingContext context, ValueToken token)
    {
        return new LiteralExpressionParser()
            .ParseLiteral(context, token);
    }

    public static OperatorExpressionSymbol ParseOperatorExpression(ParsingContext context, ObjectPropertyToken property)
    {
        return new OperatorExpressionParser()
            .ParseOperatorExpression(context, property);
    }

    public static ExpressionSymbol ParseReference(ParsingContext context, StringToken token)
    {
        return new ReferenceParser()
            .ParseReference(context, token);    
    }

    public static DestinationSymbol ParseDestination(ParsingContext context, ValueToken token)
    {
        return new DestinationParser()
            .ParseDestination(context, token);
    }

}
