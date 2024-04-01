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
                break;

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

            case Symbols.OperatorType.AnonymousType:
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

    private AddExpressionSymbol ParseAddExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new AddExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context),
            left: parser.ParseNextExpression(context),
            right: parser.ParseNextExpression(context)
        );
    }

    private SubtractExpressionSymbol ParseSubtractExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SubtractExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context),
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

    public AndExpressionSymbol ParseAndExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new AndExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context),
            expressions: parser.ParseNextExpressionArray(context)
        );
    }

    public OrExpressionSymbol ParseOrExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new OrExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context), 
            expressions: parser.ParseNextExpressionArray(context)
        );
    }

    public NotExpressionSymbol ParseNotExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new NotExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context), 
            expression: parser.ParseNextExpression(context)
        );
    }

    //*
    //*
    //* semantic expressions parsing.
    //*

    private SelectExpressionSymbol ParseTypeExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new SelectExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context),
            typeProjection: parser.ParseNextTypeProjection(context)
        );
    }

    //*
    //*
    //* query expressions parsing.
    //*

    private FilterExpressionSymbol ParseFilterExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new FilterExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context),
            source: parser.ParseNextExpression(context), 
            lambda: parser.ParseNextLambda(context)
        );
    }

    private ProjectionExpressionSymbol ParseProjectionExpression(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        return new ProjectionExpressionSymbol(
            destination: parser.ParseNextStringLiteral(context),
            source: parser.ParseNextExpression(context),
            lambda: parser.ParseNextLambda(context)
        );
    }

    //*
    //*
    //* aggregation expressios parsing.
    //*

    enum OperandType
    {
        Expression,
        StringLiteral,
        NumberLiteral,
        LambdaExpression
    }

    class OperatorProduction
    {
        public OperatorType Operator { get; }
        public OperandType[] OperandTypes { get; }

        public OperatorProduction(OperatorType @operator, OperandType[] operandTypes)
        {
            Operator = @operator;
            OperandTypes = operandTypes;
        }
    }

}
