using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

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
                return ParseAddOperatorExpression(context, paramsArray);

            case OperatorType.Subtract:
                return ParseSubtractOperatorExpression(context, paramsArray);

            case OperatorType.Divide:
                break;

            case OperatorType.Multiply:
                break;

            case OperatorType.Modulo:
                break;

            //* 
            //* 
            //* relational expressions parsing.
            case OperatorType.Equals:
                return ParseEqualsOperatorExpression(context, paramsArray);

            case OperatorType.NotEquals:
                break;

            case OperatorType.Less:
                break;

            case OperatorType.LessEquals:
                break;

            case OperatorType.Greater:
                break;

            case OperatorType.GreaterEquals:
                break;

            //* 
            //* 
            //* pattern match expressions parsing.
            case OperatorType.Like:
                break;

            case OperatorType.RegexMatch:
                break;

            //* 
            //* 
            //* logical expressions parsing.
            case OperatorType.Or:
                break;

            case OperatorType.And:
                break;

            case OperatorType.Not:
                break;

            //* 
            //* 
            //* semantic expressions parsing.
            //case OperatorType.Expr:
            //    break;

            //case OperatorType.Parse:
            //    break;

            case OperatorType.Type:
                return ParseTypeOperatorExpression(context, paramsArray);

            //* 
            //* 
            //* query expressions parsing.
            case OperatorType.Filter:
                return ParseFilterOperatorExpression(context, paramsArray);

            case OperatorType.Select:
                return ParseSelectOperatorExpression(context, paramsArray);

            //case OperatorType.Transform:
            //    break;

            case OperatorType.SelectMany:
                break;

            case OperatorType.Limit:
                break;

            case OperatorType.Skip:
                break;

            //* 
            //* 
            //* aggregation expressions parsing
            case OperatorType.Count:
                break;

            case OperatorType.Index:
                break;

            case OperatorType.Any:
                break;

            case OperatorType.All:
                break;
            case OperatorType.Min:
                break;

            case OperatorType.Max:
                break;

            case OperatorType.Sum:
                break;

            case OperatorType.Average:
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

    private AddOperatorExpressionSymbol ParseAddOperatorExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new AddOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    private SubtractOperatorExpressionSymbol ParseSubtractOperatorExpression(ParsingContext context, ArrayToken token)
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
    public EqualsOperatorExpressionSymbol ParseEqualsOperatorExpression(ParsingContext context, ArrayToken token)
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

    private SelectOperatorExpressionSymbol ParseTypeOperatorExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SelectOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            source: parser.ParseNextExpression(context),
            lambda: parser.ParseNextLambda(context)
        );
    }

    //*
    //*
    //* query expressions parsing.
    //*

    private FilterOperatorExpressionSymbol ParseFilterOperatorExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new FilterOperatorExpressionSymbol(
            destination: parser.ParseNextNullableStringLiteral(context),
            source: parser.ParseNextExpression(context), 
            lambda: parser.ParseNextLambda(context)
        );
    }

    private SelectOperatorExpressionSymbol ParseSelectOperatorExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SelectOperatorExpressionSymbol(
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
