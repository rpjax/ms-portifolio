using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ExpressionParser : SyntaxParserBase
{
    public ExpressionSymbol ParseExpression(ParsingContext context, Token token)
    {
        switch (token.TokenType)
        {
            case TokenType.Value:
                return ParseValueTokenToExpression(context, (ValueToken)token);

            case TokenType.ObjectProperty:
                return SyntaxParser.ParseOperatorExpression(context, (ObjectPropertyToken) token);

            case TokenType.Array:
            case TokenType.Object:
            default:
                break;
        }

        throw new ParsingException("", context);
    }

    private ExpressionSymbol ParseValueTokenToExpression(ParsingContext context, ValueToken token)
    {
        switch (token.ValueType)
        {
            case Tokens.ValueType.Null:
                return new NullSymbol();

            case Tokens.ValueType.String:
                var value = ((StringToken)token).Value;

                if (value.StartsWith('$'))
                {
                    return new ReferenceExpressionSymbol(value);
                }

                return new StringSymbol(value);

            case Tokens.ValueType.Bool:
                return new BoolSymbol(((BoolToken)token).Value);

            case Tokens.ValueType.Number:
                return new NumberSymbol(((NumberToken)token).Value);

            default:
                break;
        }

        throw new ParsingException("", context);
    }

}

public class OperatorExpressionParser : SyntaxParserBase
{
    private static Dictionary<string, ExpressionOperator> OpTable { get; } = CreateOpTable();

    private static Dictionary<string, ExpressionOperator> CreateOpTable()
    {
        return Enum.GetValues(typeof(ExpressionOperator))
            .Cast<ExpressionOperator>()
            .ToDictionary(
                keySelector: op => $"${op.ToString().ToLower()}",
                elementSelector: op => op
            )
        ;
    }

    public OperatorExpressionSymbol ParseOperatorExpression(ParsingContext context, ObjectPropertyToken property)
    {
        if (!OpTable.TryGetValue(property.Key, out var op))
        {
            throw new ParsingException("", context);
        }

        var token = CastToken<ArrayToken>(context, property.Value);

        switch (op)
        {
            //* 
            //* 
            //* arithmetic expressions parsing.
            case ExpressionOperator.Add:
                return ParseAddExpr(context, token);

            case ExpressionOperator.Subtract:
                return ParseSubtractExpr(context, token);

            case ExpressionOperator.Divide:
                break;

            case ExpressionOperator.Multiply:
                break;

            case ExpressionOperator.Modulo:
                break;

            //* 
            //* 
            //* relational expressions parsing.
            case ExpressionOperator.Equals:
                break;

            case ExpressionOperator.NotEquals:
                break;

            case ExpressionOperator.Less:
                break;

            case ExpressionOperator.LessEquals:
                break;

            case ExpressionOperator.Greater:
                break;

            case ExpressionOperator.GreaterEquals:
                break;

            //* 
            //* 
            //* pattern match expressions parsing.
            case ExpressionOperator.Like:
                break;

            case ExpressionOperator.RegexMatch:
                break;

            //* 
            //* 
            //* logical expressions parsing.
            case ExpressionOperator.Or:
                break;

            case ExpressionOperator.And:
                break;

            case ExpressionOperator.Not:
                break;

            //* 
            //* 
            //* semantic expressions parsing.
            case ExpressionOperator.Expr:
                break;

            case ExpressionOperator.Parse:
                break;

            case ExpressionOperator.Select:
                break;

            case ExpressionOperator.Type:
                return ParseTypeExpr(context, token);

            //* 
            //* 
            //* query expressions parsing.
            case ExpressionOperator.Filter:
                return ParseFilterExpr(context, token);

            case ExpressionOperator.Project:
                return ParseProjectionExpr(context, token);

            case ExpressionOperator.Transform:
                break;

            case ExpressionOperator.SelectMany:
                break;

            case ExpressionOperator.Limit:
                break;

            case ExpressionOperator.Skip:
                break;

            //* 
            //* 
            //* aggregation expressions parsing
            case ExpressionOperator.Count:
                break;

            case ExpressionOperator.Index:
                break;

            case ExpressionOperator.Any:
                break;

            case ExpressionOperator.All:
                break;
            case ExpressionOperator.Min:
                break;

            case ExpressionOperator.Max:
                break;

            case ExpressionOperator.Sum:
                break;

            case ExpressionOperator.Average:
                break;

            default:
                break;
        }

        throw new ParsingException("", context);
    }

    //*
    //*
    //* arithmetic expressions parsing.
    //*

    private AddExprSymbol ParseAddExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new AddExprSymbol(
            destination: parser.ParseNextDestination(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    private SubtractExprSymbol ParseSubtractExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SubtractExprSymbol(
           destination: parser.ParseNextDestination(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    //*
    //*
    //* relational expressions parsing.
    //*

    //*
    //*
    //* pattern match expressions parsing.
    //*

    //*
    //*
    //* logical expressions parsing.
    //*

    public AndExprSymbol ParseAndExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var args = parser.ParseNextExpressionArray(context);

        return new AndExprSymbol(destination, args);
    }

    public OrExprSymbol ParseOrExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var args = parser.ParseNextExpressionArray(context);

        return new OrExprSymbol(destination, args);
    }

    public NotExprSymbol ParseNotExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var arg = parser.ParseNextExpression(context);

        return new NotExprSymbol(destination, arg);
    }

    //*
    //*
    //* semantic expressions parsing.
    //*

    private TypeExprSymbol ParseTypeExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);
        var lambda = parser.ParseNextProjectionObject(context);

        return new TypeExprSymbol(lambda);
    }

    //*
    //*
    //* query expressions parsing.
    //*

    private FilterExprSymbol ParseFilterExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var source = parser.ParseNextExpression(context);
        var lambda = parser.ParseNextLambda(context);

        return new FilterExprSymbol(destination, source, lambda);
    }

    private ProjectionExprSymbol ParseProjectionExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var source = parser.ParseNextExpression(context);
        var lambda = parser.ParseNextLambda(context);

        return new ProjectionExprSymbol(destination, source, lambda);
    }

    //*
    //*
    //* aggregation expressios parsing.
    //*

}
