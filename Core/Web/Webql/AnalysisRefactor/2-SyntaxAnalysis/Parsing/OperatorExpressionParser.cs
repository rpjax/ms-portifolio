using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Syntax;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class OperatorExpressionParser : SyntaxParserBase
{
    private static Dictionary<Symbols.OperatorType, OperatorProduction> ProductionsTable { get; } = CreateProductionsTable();

    private static Dictionary<Symbols.OperatorType, OperatorProduction> CreateProductionsTable()
    {
        var table = new Dictionary<Symbols.OperatorType, OperatorProduction>();

        foreach (var op in OperatorHelper.GetUnaryOperators())
        {
            table[op] = new OperatorProduction(op, new[]
            {
                OperandType.StringLiteral,
                OperandType.Expression,
            });
        }

        foreach (var op in OperatorHelper.GetBinaryOperators())
        {
            table[op] = new OperatorProduction(op, new[]
            {
                OperandType.StringLiteral,
                OperandType.Expression,
                OperandType.Expression
            });
        }

        return table;
    }

    public OperatorExpressionSymbol ParseOperatorExpression(ParsingContext context, ObjectPropertyToken property)
    {
        if (!OperatorTable.TryGetValue(property.Key, out var op))
        {
            throw new ParsingException("", context);
        }
        if (!ProductionsTable.TryGetValue(op, out var production))
        {
            throw new ParsingException("", context);
        }

        var paramsArray = CastToken<ArrayToken>(context, property.Value);
        var parser = new ArrayParser(paramsArray);
        var operands = new List<ExpressionSymbol>();

        foreach (var operandType in production.OperandTypes)
        {
            var operand = null as ExpressionSymbol;

            switch (operandType)
            {
                case OperandType.Expression:
                    operand = parser.ParseNextExpression(context);
                    break;

                case OperandType.StringLiteral:
                    operand = parser.ParseNextStringLiteral(context);
                    break;

                case OperandType.NumberLiteral:
                    operand = parser.ParseNextNumberLiteral(context);
                    break;

                case OperandType.LambdaExpression:
                    operand = parser.ParseNextLambda(context);
                    break;
            }
        }

        var exp = new OpExpressionSymbol(op, operands.ToArray());
        Console.WriteLine();

        switch (op)
        {
            //* 
            //* 
            //* arithmetic expressions parsing.
            case Symbols.OperatorType.Add:
                return ParseAddExpr(context, paramsArray);

            case Symbols.OperatorType.Subtract:
                return ParseSubtractExpr(context, paramsArray);

            case Symbols.OperatorType.Divide:
                break;

            case Symbols.OperatorType.Multiply:
                break;

            case Symbols.OperatorType.Modulo:
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

            case Symbols.OperatorType.Select:
                break;

            case Symbols.OperatorType.Type:
                return ParseTypeExpr(context, paramsArray);

            //* 
            //* 
            //* query expressions parsing.
            case Symbols.OperatorType.Filter:
                return ParseFilterExpr(context, paramsArray);

            case Symbols.OperatorType.Project:
                return ParseProjectionExpr(context, paramsArray);

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

    enum OperandType
    {
        Expression,
        StringLiteral,
        NumberLiteral,
        LambdaExpression
    }

    class OperatorProduction
    {
        public Symbols.OperatorType Operator { get; }
        public OperandType[] OperandTypes { get; }

        public OperatorProduction(Symbols.OperatorType @operator, OperandType[] operandTypes)
        {
            Operator = @operator;
            OperandTypes = operandTypes;
        }
    }

}
