using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ExprParser : SyntaxParser
{
    private static Dictionary<string, ExprOp> OpTable { get; } = CreateOpTable();

    private static Dictionary<string, ExprOp> CreateOpTable()
    {
        return Enum.GetValues(typeof(ExprOp))
            .Cast<ExprOp>()
            .ToDictionary(
                keySelector: op => $"${op.ToString().ToLower()}",
                elementSelector: op => op
            )
        ;
    }

    public ExprSymbol ParseExpr(ParsingContext context, ObjectProperty property)
    {
        if (!OpTable.TryGetValue(property.Key, out var op))
        {
            throw new ParsingException("", context);
        }

        var token = CastToken<ArrayToken>(context, property.Value);

        switch (op)
        {
            case ExprOp.Add:
                return ParseAddExpr(context, token);

            case ExprOp.Subtract:
                return ParseSubtractExpr(context, token);   

            case ExprOp.Divide:
                break;
            case ExprOp.Multiply:
                break;
            case ExprOp.Modulo:
                break;
            case ExprOp.Equals:
                break;
            case ExprOp.NotEquals:
                break;
            case ExprOp.Less:
                break;
            case ExprOp.LessEquals:
                break;
            case ExprOp.Greater:
                break;
            case ExprOp.GreaterEquals:
                break;
            case ExprOp.Like:
                break;
            case ExprOp.RegexMatch:
                break;
            case ExprOp.Or:
                break;
            case ExprOp.And:
                break;
            case ExprOp.Not:
                break;
            case ExprOp.Expr:
                break;
            case ExprOp.Literal:
                break;
            //* 
            //* 
            //* query expressions
            case ExprOp.Select:
                break;

            case ExprOp.Filter:
                return ParseFilterExpr(context, token);

            case ExprOp.Project:
                return ParseProjectionExpr(context, token);

            case ExprOp.Transform:
                break;
            case ExprOp.SelectMany:
                break;
            case ExprOp.Limit:
                break;
            case ExprOp.Skip:
                break;
            case ExprOp.Count:
                break;
            case ExprOp.Index:
                break;
            case ExprOp.Any:
                break;
            case ExprOp.All:
                break;
            case ExprOp.Min:
                break;
            case ExprOp.Max:
                break;
            case ExprOp.Sum:
                break;
            case ExprOp.Average:
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
        var args = TokenParser.ParseBinaryArguments(context, token);

        return new AddExprSymbol(
            arguments: args
        );
    }

    private SubtractExprSymbol ParseSubtractExpr(ParsingContext context, ArrayToken token)
    {
        var args = TokenParser.ParseBinaryArguments(context, token);

        return new SubtractExprSymbol(
            arguments: args
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
        var args = parser.ParseNextArgumentArray(context);

        return new AndExprSymbol(destination, args);
    }

    public OrExprSymbol ParseOrExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var args = parser.ParseNextArgumentArray(context);

        return new OrExprSymbol(destination, args);
    }

    public NotExprSymbol ParseNotExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var arg = parser.ParseNextArgument(context);

        return new NotExprSymbol(destination, arg);
    }

    //*
    //*
    //* semantic expressions parsing.
    //*

    //*
    //*
    //* query expressions parsing.
    //*

    private FilterExprSymbol ParseFilterExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var source = parser.ParseNextArgument(context);
        var lambda = parser.ParseNextUnaryLambda(context);

        return new FilterExprSymbol(destination, source, lambda);
    }

    private ProjectionExprSymbol ParseProjectionExpr(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var source = parser.ParseNextArgument(context);
        var lambda = parser.ParseNextProjectionLambda(context);

        return new ProjectionExprSymbol(destination, source, lambda);
    }

    //*
    //*
    //* aggregation expressios parsing.
    //*

}
