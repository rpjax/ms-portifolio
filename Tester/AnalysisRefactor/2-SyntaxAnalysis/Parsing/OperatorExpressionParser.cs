using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class OperatorExpressionParser : SyntaxParserBase
{
    public OperatorExpressionSymbol ParseOperatorExpression(ParsingContext context, ObjectPropertyToken property)
    {
        if (!OperatorTable.TryGetValue(property.Key, out var op))
        {
            throw new ParsingException("", context);
        }

        var paramsArray = CastToken<ArrayToken>(context, property.Value);

        switch (op)
        {
            //* 
            //* 
            //* arithmetic expressions parsing.
            case OperatorType.Add:
                return ParseAddExpression(context, paramsArray);

            case OperatorType.Subtract:
                return ParseSubtractExpression(context, paramsArray);

            case OperatorType.Divide:
                break;

            case OperatorType.Multiply:
                break;

            case OperatorType.Modulo:
                break;

            //* 
            //* 
            //* relational expressions parsing.
            case Symbols.OperatorType.Equals:
                return ParseEqualsExpression(context, paramsArray);

            case Symbols.OperatorType.NotEquals:
                break;

            case Symbols.OperatorType.Less:
                break;

            case Symbols.OperatorType.LessEquals:
                break;

            case Symbols.OperatorType.Greater:
                break;

            case Symbols.OperatorType.GreaterEquals:
                break;

            //* 
            //* 
            //* pattern match expressions parsing.
            case Symbols.OperatorType.Like:
                break;

            case Symbols.OperatorType.RegexMatch:
                break;

            //* 
            //* 
            //* logical expressions parsing.
            case Symbols.OperatorType.Or:
                break;

            case Symbols.OperatorType.And:
                break;

            case Symbols.OperatorType.Not:
                break;

            //* 
            //* 
            //* semantic expressions parsing.
            case Symbols.OperatorType.Expr:
                break;

            case Symbols.OperatorType.Parse:
                break;

            case Symbols.OperatorType.SelectOld:
                break;

            case Symbols.OperatorType.Type:
                return ParseTypeExpression(context, paramsArray);

            //* 
            //* 
            //* query expressions parsing.
            case Symbols.OperatorType.Filter:
                return ParseFilterExpression(context, paramsArray);

            case Symbols.OperatorType.Select:
                return ParseProjectionExpression(context, paramsArray);

            case Symbols.OperatorType.Transform:
                break;

            case Symbols.OperatorType.SelectMany:
                break;

            case Symbols.OperatorType.Limit:
                break;

            case Symbols.OperatorType.Skip:
                break;

            //* 
            //* 
            //* aggregation expressions parsing
            case Symbols.OperatorType.Count:
                break;

            case Symbols.OperatorType.Index:
                break;

            case Symbols.OperatorType.Any:
                break;

            case Symbols.OperatorType.All:
                break;
            case Symbols.OperatorType.Min:
                break;

            case Symbols.OperatorType.Max:
                break;

            case Symbols.OperatorType.Sum:
                break;

            case Symbols.OperatorType.Average:
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

    private AddOperatorExpressionSymbol ParseAddExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new AddOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    private SubtractOperatorExpressionSymbol ParseSubtractExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SubtractOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    //*
    //*
    //* relational expressions parsing.
    //*
    public EqualsOperatorExpressionSymbol ParseEqualsExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new EqualsOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    //*
    //*
    //* string relational expressions parsing.
    //*

    //*
    //*
    //* logical expressions parsing.
    //*

    public AndOperatorExpressionSymbol ParseAndExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new AndOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            expressions: parser.ParseNextExpressionArray(context)
        );
    }

    public OrOperatorExpressionSymbol ParseOrExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new OrOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context), 
            expressions: parser.ParseNextExpressionArray(context)
        );
    }

    public NotOperatorExpressionSymbol ParseNotExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new NotOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context), 
            expression: parser.ParseNextExpression(context)
        );
    }

    //*
    //*
    //* semantic expressions parsing.
    //*

    private SelectOperatorExpressionSymbol ParseTypeExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SelectOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            typeProjection: parser.ParseNextTypeProjection(context)
        );
    }

    //*
    //*
    //* query expressions parsing.
    //*

    private FilterOperatorExpressionSymbol ParseFilterExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new FilterOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            source: parser.ParseNextExpression(context), 
            lambda: parser.ParseNextLambda(context)
        );
    }

    private ProjectionOperatorExpressionSymbol ParseProjectionExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new ProjectionOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            source: parser.ParseNextExpression(context),
            lambda: parser.ParseNextLambda(context)
        );
    }

    //*
    //*
    //* aggregation expressios parsing.
    //*
}
