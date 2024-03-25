using ModularSystem.Core;
using ModularSystem.Web.Webql.Synthesis.Symbols;
using ModularSystem.Webql.Analysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Translation helper that operates on an array. <br/>
/// It translates and consumes symbols sequentially. <br/>
/// Ex: [symbol1, symbol2, symbol3]
/// </summary>
public class ArrayTranslator : TranslatorBase
{
    private Queue<Node> Nodes { get; }

    public ArrayTranslator(TranslationOptions options, ArrayNode arrayNode) : base(options)
    {
        Nodes = new Queue<Node>(arrayNode);
    }

    //*
    // consumption helpers.
    //*

    public T ConsumeNextNode<T>(TranslationContextOld context) where T : Node
    {
        if (Nodes.IsEmpty())
        {
            throw new Exception();
        }

        return TypeCastNode<T>(context, Nodes.Dequeue());
    }

    public Node ConsumeNextNode(TranslationContextOld context)
    {
        return ConsumeNextNode<Node>(context);
    }

    public LiteralNode ConsumeNextLiteral(TranslationContextOld context)
    {
        return ConsumeNextNode<LiteralNode>(context);
    }

    public string ConsumeNextString(TranslationContextOld context)
    {
        return ConsumeNextNode<LiteralNode>(context).GetNormalizedValue()
            ?? throw new TranslationException("", context);
    }

    public ArrayNode ConsumeNextArray(TranslationContextOld context)
    {
        return ConsumeNextNode<ArrayNode>(context); 
    }

    public ObjectNode ConsumeNextObject(TranslationContextOld context)
    {
        return ConsumeNextNode<ObjectNode>(context);
    }

    public ArrayNode ConsumeNextLambda(TranslationContextOld context)
    {
        return ConsumeNextArray(context);
    }

    //*
    // translation helpers.
    //*

    /// <summary>
    /// Production: <br/>
    /// &lt;destination&gt; ::= &lt;string&gt; | &lt;null&gt; <br/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public string? TranslateNextDestination(TranslationContextOld context)
    {
        return ConsumeNextNode<LiteralNode>(context).GetNormalizedValue();
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;reference&gt; ::= &lt;string-literal&gt; <br/><br/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNextReference(TranslationContextOld context)
    {
        return TranslateReference(context, ConsumeNextNode<LiteralNode>(context));
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;int32&gt; ::= &lt;digit&gt; { &lt;digit&gt; }
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNextInt32(TranslationContextOld context)
    {
        return new LiteralTranslator(Options)
            .TranslateInt32(context, ConsumeNextLiteral(context));
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;arg&gt; ::= &lt;reference&gt; | &lt;object&gt; <br/><br/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNextArgument(TranslationContextOld context)
    {
        return TranslateArgument(context, ConsumeNextNode<Node>(context));
    }

    /// <summary>
    /// Translates the next &lt;query-arg&gt; and envolopes it in a helper object. <br/>
    /// A queryable argument is a normal argument that implements IEnumerable or IQueryable. <br/>
    /// Production: <br/>
    /// &lt;query-arg&gt; ::= &lt;arg&gt; 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public QueryArgumentExpression TranslateNextQueryArgument(TranslationContextOld context)
    {
        var arg = new QueryArgumentExpression(TranslateNextArgument(context));

        if (arg.IsNotQueryable(context))
        {
            throw new TranslationException("", context);
        }

        return arg;
    }

    /// <summary>
    /// EBNF Production: <br/>
    /// object = "{" , [ expr , { "," , expr } ] , "}";
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNextObject(TranslationContextOld context)
    {
        return TranslateObject(context, ConsumeNextObject(context));
    }

    /// <summary>
    /// EBNF Production: <br/>
    /// projection_object = "{" , [ projection_expr , { "," , projection_expr } ]  , "}";
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public ProjectionExpression TranslateNextProjectionObject(TranslationContextOld context)
    {
        return TranslateProjectionObject(context, ConsumeNextObject(context));
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;type&gt; ::= &lt;string-literal&gt; <br/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Type TranslateNextType(TranslationContextOld context)
    {
        return TranslateType(context, ConsumeNextLiteral(context));
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;arg_array&gt; ::= '[' [ &lt;arg&gt; , { ',' &lt;arg&gt; } ] ']'
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression[] TranslateNextArgumentArray(TranslationContextOld context)
    {
        return new ArgumentArrayTranslator(Options)
            .TranslateArgumentArray(context, ConsumeNextArray(context));
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;lambda_args&gt; ::= '[' [ &lt;string&gt; , { ',' &lt;string&gt; } ]  ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public ParameterExpression[] TranslateNextLambdaArgs(TranslationContextOld context, IEnumerable<Type> types)
    {
        return new LambdaArgsTranslator(Options)
            .TranslateLambdaArgs(context, ConsumeNextArray(context), types);
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;unary_lambda_args&gt; ::= '[' &lt;string&gt; ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public ParameterExpression[] TranslateNextUnaryLambdaArgs(TranslationContextOld context, Type paramType)
    {
        return new LambdaArgsTranslator(Options)
            .TranslateUnaryLambdaArgs(context, ConsumeNextArray(context), paramType);
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;lambda&gt; ::= '[' &lt;lambda_args&gt; ',' &lt;object&gt; ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNextLambda(TranslationContextOld context, IEnumerable<Type> @params)
    {
        return new LambdaTranslator(Options)
            .TranslateLambda(context, ConsumeNextArray(context), @params);
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;unary_lambda&gt; ::= [ [ &lt;string&gt; ] , &lt;object&gt; ] <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateNextUnaryLambda(TranslationContextOld context, Type paramType)
    {
        return new LambdaTranslator(Options)
            .TranslateUnaryLambda(context, ConsumeNextArray(context), paramType);
    }

}
