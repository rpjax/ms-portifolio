using Aidan.Core;
using Aidan.Web.Webql.Synthesis.Productions;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Represents a translator for WebQL operators. This class facilitates the translation of various WebQL operators <br/>
/// into their corresponding LINQ expressions.
/// </summary>
public class OperatorsTranslator : TranslatorBase
{
    /// <summary>
    /// A translator for arithmetic operators.
    /// </summary>
    protected ArithmeticOperatorsTranslator ArithmeticOperatorsTranslator { get; }

    /// <summary>
    /// A translator for relational operators.
    /// </summary>
    protected RelationalOperatorsTranslator RelationalOperatorsTranslator { get; }

    /// <summary>
    /// A translator for pattern relational operators.
    /// </summary>
    protected PatternRelationalOperatorsTranslator PatternRelationalOperatorsTranslator { get; }

    /// <summary>
    /// A translator for logical operators.
    /// </summary>
    protected LogicalOperatorsTranslator LogicalOperatorTranslator { get; }

    /// <summary>
    /// A translator for semantic operators.
    /// </summary>
    protected SemanticOperatorsTranslator SemanticOperatorsTranslator { get; }

    /// <summary>
    /// A translator for queryable operators.
    /// </summary>
    protected QueryableOperatorsTranslator QueryableOperatorsTranslator { get; }

    protected AggregationOperatorsTranslator AggregationOperatorsTranslator { get; }

    /// <summary>
    /// Initializes a new instance of the OperatorTranslator class with the specified options and node parser.
    /// </summary>
    /// <param name="options">The translator options.</param>
    public OperatorsTranslator(TranslationOptions options)
    : base(options)
    {
        ArithmeticOperatorsTranslator = new(options);
        RelationalOperatorsTranslator = new(options);
        PatternRelationalOperatorsTranslator = new(options);
        LogicalOperatorTranslator = new(options);
        SemanticOperatorsTranslator = new(options);
        QueryableOperatorsTranslator = new(options);
        AggregationOperatorsTranslator = new(options);
    }

    /// <summary>
    /// Translates a WebQL operator and its associated node into a LINQ expression.
    /// </summary>
    /// <param name="context">The translation context.</param>
    /// <param name="operator">The WebQL operator to be translated.</param>
    /// <param name="node">The node associated with the operator.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if the operator is unknown or unsupported.</exception>
    public Expression Translate(TranslationContextOld context, ArrayNode node, OperatorOld @operator)
    {
        switch (@operator)
        {
            // Arithmetic Operators
            case OperatorOld.Add:
                return ArithmeticOperatorsTranslator.TranslateAdd(context, node);

            case OperatorOld.Subtract:
                return ArithmeticOperatorsTranslator.TranslateSubtract(context, node);

            case OperatorOld.Divide:
                return ArithmeticOperatorsTranslator.TranslateDivide(context, node);

            case OperatorOld.Multiply:
                return ArithmeticOperatorsTranslator.TranslateMultiply(context, node);

            case OperatorOld.Modulo:
                return ArithmeticOperatorsTranslator.TranslateModulo(context, node);

            // Relational Operators
            case OperatorOld.Equals:
                return RelationalOperatorsTranslator.TranslateEquals(context, node);

            case OperatorOld.NotEquals:
                return RelationalOperatorsTranslator.TranslateNotEquals(context, node);

            case OperatorOld.Less:
                return RelationalOperatorsTranslator.TranslateLess(context, node);

            case OperatorOld.LessEquals:
                return RelationalOperatorsTranslator.TranslateLessEquals(context, node);

            case OperatorOld.Greater:
                return RelationalOperatorsTranslator.TranslateGreater(context, node);

            case OperatorOld.GreaterEquals:
                return RelationalOperatorsTranslator.TranslateGreaterEquals(context, node);

            // Pattern Relational Operators
            case OperatorOld.Like:
                return PatternRelationalOperatorsTranslator.TranslateLike(context, node);

            case OperatorOld.RegexMatch:
                return PatternRelationalOperatorsTranslator.TranslateRegexMatch(context, node);

            // Logical Operators
            case OperatorOld.Or:
                return LogicalOperatorTranslator.TranslateOr(context, node);

            case OperatorOld.And:
                return LogicalOperatorTranslator.TranslateAnd(context, node);

            case OperatorOld.Not:
                return LogicalOperatorTranslator.TranslateNot(context, node);

            // Semantic Operators
            case OperatorOld.Expr:
                return SemanticOperatorsTranslator.TranslateExpr(context, node);

            case OperatorOld.Literal:
                return SemanticOperatorsTranslator.TranslateParse(context, node);

            case OperatorOld.Select:
                return SemanticOperatorsTranslator.TranslateSelect(context, node);

            // Queryable Operators
            case OperatorOld.Filter:
                return QueryableOperatorsTranslator.TranslateFilter(context, node);

            case OperatorOld.Project:
                return QueryableOperatorsTranslator.TranslateProject(context, node);

            case OperatorOld.Transform:
                return QueryableOperatorsTranslator.TranslateTransform(context, node);

            case OperatorOld.SelectMany:
                return QueryableOperatorsTranslator.TranslateSelectMany(context, node);

            case OperatorOld.Limit:
                return QueryableOperatorsTranslator.TranslateLimit(context, node);

            case OperatorOld.Skip:
                return QueryableOperatorsTranslator.TranslateSkip(context, node);

            // Aggregation Operators
            case OperatorOld.Any:
                return AggregationOperatorsTranslator.TranslateAny(context, node);

            case OperatorOld.All:
                return AggregationOperatorsTranslator.TranslateAll(context, node);

            case OperatorOld.Count:
                return AggregationOperatorsTranslator.TranslateCount(context, node);

            case OperatorOld.Min:
                return AggregationOperatorsTranslator.TranslateMin(context, node);

            case OperatorOld.Max:
                return AggregationOperatorsTranslator.TranslateMax(context, node);

            // TODO Aggregation Operators:
            case OperatorOld.Sum:
            case OperatorOld.Average:
            case OperatorOld.Index:
                break;
        }

        throw new TranslationException("The operator encountered during translation is either unknown or not supported. Please verify the operator used in the query and ensure it aligns with the supported operators in WebQL.", context);
    }

}

public class ArithmeticOperatorsTranslator : TranslatorBase
{
    public ArithmeticOperatorsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;add-expr&gt; ::= $add: [ &lt;destination&gt;, &lt;arg&gt;, &lt;arg&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateAdd(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("add-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads the 'destination' symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        var addExpression = Expression.Add(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, addExpression, true);
        }

        return addExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;subtract-expr&gt; ::= $subtract: [ &lt;destination&gt;, &lt;arg&gt;, &lt;arg&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateSubtract(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("subtract-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads the 'destination' symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        var subtractExpression = Expression.Subtract(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, subtractExpression, true);
        }

        return subtractExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;divide-expr&gt; ::= $divide: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateDivide(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("divide-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var divideExpression = Expression.Divide(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, divideExpression, true);
        }

        return divideExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;multiply-expr&gt; ::= $multiply: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateMultiply(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("multiply-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var multiplyExpression = Expression.Multiply(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, multiplyExpression, true);
        }

        return multiplyExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;modulo-expr&gt; ::= $modulo: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateModulo(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("modulo-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var moduloExpression = Expression.Modulo(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, moduloExpression, true);
        }

        return moduloExpression;
    }

}

public class RelationalOperatorsTranslator : TranslatorBase
{
    public RelationalOperatorsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;equal-expr&gt; ::= $equal: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateEquals(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("equal-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var equalExpression = Expression.Equal(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, equalExpression, true);
        }

        return equalExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;not-equal-expr&gt; ::= $notEqual: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNotEquals(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("not-equal-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var notEqualExpression = Expression.NotEqual(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, notEqualExpression, true);
        }

        return notEqualExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;less-expr&gt; ::= $less: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateLess(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("less-than-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var lessThanExpression = Expression.LessThan(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, lessThanExpression, true);
        }

        return lessThanExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;less-equal-expr&gt; ::= $lessEqual: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateLessEquals(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("less-equal-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var lessThanOrEqualExpression = Expression.LessThanOrEqual(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, lessThanOrEqualExpression, true);
        }

        return lessThanOrEqualExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;greater-expr&gt; ::= $greater: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateGreater(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("greater-than-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var greaterThanExpression = Expression.GreaterThan(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, greaterThanExpression, true);
        }

        return greaterThanExpression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;greater-equal-expr&gt; ::= $greaterEqual: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateGreaterEquals(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("greater-equal-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var left = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var right = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(left, right);

        if (destination is not null)
        {
            context.SetSymbol(destination, greaterThanOrEqualExpression, true);
        }

        return greaterThanOrEqualExpression;
    }

}

public class PatternRelationalOperatorsTranslator : TranslatorBase
{
    public PatternRelationalOperatorsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;like-expr&gt; ::= $like: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateLike(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the first 'arg' symbol.
        var arg1 = translator.TranslateNextArgument(context);

        //* reads and translates the second 'arg' symbol.
        var arg2 = translator.TranslateNextArgument(context);

        //* reads the 'destination' symbol.
        var destination = translator.ConsumeNextString(context);

        var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { })!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var lhsTolowerExpression = Expression.Call(arg1, toLowerMethod);
        var rhsToLowerExpression = Expression.Call(arg2, toLowerMethod);

        var containsArgs = new[] { rhsToLowerExpression };

        var likeExpression = Expression.Call(lhsTolowerExpression, containsMethod!, containsArgs);

        if (destination != null)
        {
            context.SetSymbol(destination, likeExpression);
        }

        return likeExpression;
    }

    /// <summary>
    /// NOT USABLE! UNDER DEVLOPMENT! <br/>
    /// Production: <br/>
    /// &lt;regex-match-expr&gt; ::= $regexMatch: [&lt;arg&gt;, &lt;arg&gt;, &lt;destination&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateRegexMatch(TranslationContextOld context, ArrayNode node)
    {
        throw new NotImplementedException();
    }

}

public class LogicalOperatorsTranslator : TranslatorBase
{
    public LogicalOperatorsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;or-expr&gt; ::= '$or:' '[' &lt;destination&gt; ',' &lt;arg-array&gt; ']' <br/>
    /// &lt;arg-array&gt; ::= '[' [ &lt;arg&gt; ] { ',' &lt;arg&gt; } ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateOr(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("or-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads the '<destination>' symbol.
        var destination = translator.ConsumeNextString(context);

        //* reads and translates the '<args-array>' symbol.
        var args = translator.TranslateNextArgumentArray(context);

        var expression = null as Expression;

        foreach (var item in args)
        {
            if (expression == null)
            {
                expression = item;
                continue;
            }

            expression = Expression.OrElse(expression, item);
        }

        if (expression is null)
        {
            return Expression.Constant(true, typeof(bool));
        }

        if (destination is not null)
        {
            context.SetSymbol(destination, expression, true);
        }

        return expression;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;and-expr&gt; ::= '$and:' '[' &lt;destination&gt; ',' &lt;arg-array&gt; ']' <br/>
    /// &lt;arg-array&gt; ::= '[' [ &lt;arg&gt; ] { ',' &lt;arg&gt; } ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateAnd(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new SymbolProduction("and-expression", new()));

        var translator = new ArrayTranslator(Options, node);

        //* reads the '<destination>' symbol.
        var destination = translator.ConsumeNextString(context);

        //* reads and translates the '<args-array>' symbol.
        var args = translator.TranslateNextArgumentArray(context);

        var expression = null as Expression;

        foreach (var item in args)
        {
            if (expression == null)
            {
                expression = item;
                continue;
            }

            expression = Expression.AndAlso(expression, item);
        }

        if (expression is null)
        {
            return Expression.Constant(true, typeof(bool));
        }

        if (destination is not null)
        {
            context.SetSymbol(destination, expression, true);
        }

        return expression;
    }

    /// <summary>
    /// Production:
    /// &lt;not-expr&gt; ::= '$not:' '[' &lt;arg&gt; ']' <br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNot(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);
        var expression = translator.TranslateNextArgument(context);

        return Expression.Not(expression);
    }

}

public class SemanticOperatorsTranslator : TranslatorBase
{
    public SemanticOperatorsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production:
    /// &lt;parse_expr&gt; ::= $parse: [&lt;type&gt;, &lt;string&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateParse(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the '<type>' symbol.
        var type = translator.TranslateNextType(context);

        //* reads the '<string>' symbol.
        var value = translator.ConsumeNextString(context);

        var deserializedValue = Options.Deserialize(value, type);

        return Expression.Constant(deserializedValue, type);
    }

    /// <summary>
    /// Production:
    /// &lt;select-expr&gt; ::= $select: [&lt;arg&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateSelect(TranslationContextOld context, ArrayNode node)
    {
        return new ArrayTranslator(Options, node)
            .TranslateArgument(context, node);
    }

    /// <summary>
    /// Production:
    /// &lt;scope-expr&gt; ::= $scope: [&lt;arg&gt;]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateExpr(TranslationContextOld context, ArrayNode node)
    {
        return new ArrayTranslator(Options, node)
            .TranslateArgument(context, node);
    }

}

public class QueryableOperatorsTranslator : TranslatorBase
{
    public QueryableOperatorsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;filter_expr&gt; ::= $filter: [&lt;destination&gt;, &lt;query_arg&gt;, &lt;predicate_lambda&gt; ]<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateFilter(TranslationContextOld context, ArrayNode arguments)
    {
        context = context.CreateTranslationContext(new FilterProduction());

        var translator = new ArrayTranslator(Options, arguments);

        //* reads and translates 'arg' symbol
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads 'destination' symbol
        var destination = translator.ConsumeNextString(context);

        //* reads 'lambda' symbol
        var lambdaSymbol = translator.ConsumeNextNode<ArrayNode>(context);

        //* translates the lambda
        var elementType = arg.GetElementType(context);
        var lambdaExpression = new LambdaTranslator(Options)
            .TranslatePredicateLambda(context, lambdaSymbol, elementType);

        var methodInfo = Options.LinqProvider
            .GetWhereMethodInfo(context, arg);

        var callExpression = Expression.Call(methodInfo, arg.Expression, lambdaExpression);

        if (destination is not null)
        {
            context.SetSymbol(destination, lambdaExpression);
        }

        return callExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;project_expr&gt; ::= $project: [ &lt;destination&gt;, &lt;query_arg&gt;, &lt;projection_lambda&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateProject(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new ProjectionProduction());

        var translator = new ArrayTranslator(Options, node);

        //* reads and translates the '<arg>' symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the '<destination>' symbol.
        var destination = translator.ConsumeNextString(context);

        //* reads the '<lambda>' symbol.
        var projectionLambda = translator.ConsumeNextArray(context);

        var elementType = arg.GetElementType(context);

        var lambdaExpression = new LambdaTranslator(Options)
            .TranslateProjectionLambda(context, projectionLambda, elementType);

        var resultType = lambdaExpression.ReturnType;

        var selectMethod = Options.LinqProvider
            .GetSelectMethodInfo(context, arg, resultType);

        var callExpression = Expression.Call(selectMethod, arg.Expression, lambdaExpression);

        if (destination is not null)
        {
            context.SetSymbol(destination, callExpression);
        }

        return callExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;transform_expr&gt; ::= $transform: [ &lt;destination&gt;, &lt;query_arg&gt;, &lt;lambda&gt; ] <br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateTransform(TranslationContextOld context, ArrayNode node)
    {
        context = context.CreateTranslationContext(new ProjectionProduction());

        var translator = new ArrayTranslator(Options, node);

        //* reads the '<destination>' symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads and translates the '<arg>' symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the '<lambda>' symbol.
        var lambda = translator.ConsumeNextArray(context);

        var elementType = arg.GetElementType(context);
        var lambdaExpression = new LambdaTranslator(Options)
            .TranslateUnaryLambda(context, lambda, elementType);

        var resultType = lambdaExpression.ReturnType;

        var selectMethod = Options.LinqProvider
           .GetSelectMethodInfo(context, arg, resultType);

        var callExpression = Expression.Call(selectMethod, arg.Expression, lambdaExpression);

        if (destination != null)
        {
            context.SetSymbol(destination, callExpression);
        }

        return callExpression;
    }

    [Obsolete("NOT IMPLEMENTED, DO NOT USE IT.")]
    public Expression TranslateSelectMany(TranslationContextOld context, ArrayNode node)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;limit_expr&gt; ::= &lt;limit_op&gt; '[' &lt;destination&gt; ',' &lt;query_arg&gt; ',' &lt;int32&gt; ']' <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateLimit(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the <destination> symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads the <query-arg> symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the <int32> symbol.
        var int32 = translator.TranslateNextInt32(context);

        var methodInfo = Options.LinqProvider
            .GetTakeMethodInfo(context, arg);

        var limitExpression = Expression.Call(methodInfo, arg.Expression, int32);

        if (destination is not null)
        {
            context.SetSymbol(destination, limitExpression);
        }

        return limitExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;skip_expr&gt; ::= &lt;skip_op&gt; '[' &lt;destination&gt; ',' &lt;query_arg&gt; ',' &lt;int32&gt; ']' <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateSkip(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the <destination> symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads the <query-arg> symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the <int32> symbol.
        var int32 = translator.TranslateNextInt32(context);

        var methodInfo = Options.LinqProvider
            .GetSkipMethodInfo(context, arg);

        var skipExpression = Expression.Call(methodInfo, arg.Expression, int32);

        if (destination is not null)
        {
            context.SetSymbol(destination, skipExpression);
        }

        return skipExpression;
    }

}

public class AggregationOperatorsTranslator
{
    private TranslationOptions Options { get; }

    public AggregationOperatorsTranslator(TranslationOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;any_expr&gt; ::= $any: [ &lt;destination&gt;, &lt;query_arg&gt;, &lt;predicate_lambda&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateAny(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the <destination> symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads the <query_arg> symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the <lambda> symbol.
        var lambda = translator.ConsumeNextArray(context);

        var methodInfo = Options.LinqProvider
            .GetAnyMethodInfo(context, arg);

        var elementType = arg.GetElementType(context);

        var lambdaExpression = new LambdaTranslator(Options)
            .TranslatePredicateLambda(context, lambda, elementType);

        var callExpression = Expression.Call(methodInfo, arg.Expression, lambdaExpression);

        if (destination is not null)
        {
            context.SetSymbol(destination, callExpression, true);
        }

        return callExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/>
    /// &lt;all_expr&gt; ::= $all: [ &lt;destination&gt;, &lt;query_arg&gt;, &lt;predicate_lambda&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateAll(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the <destination> symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads the <query_arg> symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the <lambda> symbol.
        var lambda = translator.ConsumeNextArray(context);

        var methodInfo = Options.LinqProvider
            .GetAllMethodInfo(context, arg);

        var elementType = arg.GetElementType(context);

        var lambdaExpression = new LambdaTranslator(Options)
            .TranslatePredicateLambda(context, lambda, elementType);

        var callExpression = Expression.Call(methodInfo, arg.Expression, lambdaExpression);

        if (destination is not null)
        {
            context.SetSymbol(destination, callExpression, true);
        }

        return callExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/> 
    /// &lt;count_expr&gt; ::= $count: [ &lt;destination&gt; , &lt;query_arg&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateCount(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the <destination> symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads the <query_arg> symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        var methodInfo = Options.LinqProvider
            .GetCountMethodInfo(context, arg);

        var countExpression = Expression.Call(null, methodInfo, arg.Expression);

        if(destination is not null)
        {
            context.SetSymbol(destination, countExpression, true);
        }

        return countExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/> 
    /// &lt;min_expr&gt; ::= $min: [ &lt;destination&gt; , &lt;query_arg&gt; , &lt;selection_lambda&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateMin(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the '<destination>' symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads and translates the '<arg>' symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the '<lambda>' symbol.
        var lambda = translator.ConsumeNextArray(context);
        
        var elementType = arg.GetElementType(context);
        var lambdaExpression = new LambdaTranslator(Options)
            .TranslateUnaryLambda(context, lambda, elementType);

        var resultType = lambdaExpression.ReturnType;

        var methodInfo = Options.LinqProvider
            .GetMinMethodInfo(context, arg, resultType);

        var callExpression = Expression.Call(methodInfo, arg.Expression, lambdaExpression);

        if (destination is not null)
        {
            context.SetSymbol(destination, callExpression, true);
        }

        return callExpression;
    }

    /// <summary>
    /// Pseudo-BNF Production: <br/> 
    /// &lt;max_expr&gt; ::= $max: [ &lt;destination&gt; , &lt;query_arg&gt; , &lt;selection_lambda&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    public Expression TranslateMax(TranslationContextOld context, ArrayNode node)
    {
        var translator = new ArrayTranslator(Options, node);

        //* reads the '<destination>' symbol.
        var destination = translator.TranslateNextDestination(context);

        //* reads and translates the '<arg>' symbol.
        var arg = translator.TranslateNextQueryArgument(context);

        //* reads the '<lambda>' symbol.
        var lambda = translator.ConsumeNextArray(context);

        var elementType = arg.GetElementType(context);
        var lambdaExpression = new LambdaTranslator(Options)
            .TranslateUnaryLambda(context, lambda, elementType);

        var resultType = lambdaExpression.ReturnType;

        var methodInfo = Options.LinqProvider
            .GetMaxMethodInfo(context, arg, resultType);

        var callExpression = Expression.Call(methodInfo, arg.Expression, lambdaExpression);

        if (destination is not null)
        {
            context.SetSymbol(destination, callExpression, true);
        }

        return callExpression;
    }

}
