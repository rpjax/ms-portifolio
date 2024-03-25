using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

public abstract class TranslatorBase
{
    protected TranslationOptions Options { get; }

    protected TranslatorBase(TranslationOptions options)
    {
        Options = options;  
    }

    //*
    // Common translation methods.
    //*

    /// <summary>
    /// Production: <br/>
    /// &lt;S&gt; ::= &lt;object&gt;<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateAxiom(TranslationContextOld context, ObjectNode node)
    {
        return new AxiomTranslator(Options)
            .TranslateAxiom(context, node);
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;reference&gt; ::= &lt;string-literal&gt;
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateReference(TranslationContextOld context, LiteralNode node)
    {
        return new ReferenceTranslator(Options)
            .TranslateReference(context, node);
    }

    /// <summary>
    /// EBNF Production: <br/>
    /// object = "{" , [ expr , { "," , expr } ] , "}";
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateObject(TranslationContextOld context, ObjectNode node)
    {
        return new ObjectTranslator(Options)
            .TranslateObject(context, node);
    }

    /// <summary>
    /// EBNF Production: <br/>
    /// projection_object = "{" , [ projection_expr , { "," , projection_expr } ]  , "}";
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public ProjectionExpression TranslateProjectionObject(TranslationContextOld context, ObjectNode node)
    {
        return new ProjectionObjectTranslator(Options)
            .TranslateProjectionObject(context, node);
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;arg&gt; ::= &lt;reference&gt; | &lt;object&gt; <br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateArgument(TranslationContextOld context, Node node)
    {
        return new ArgumentTranslator(Options)
            .TranslateArgument(context, node); 
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;type&gt; ::= &lt;string-literal&gt; <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Type TranslateType(TranslationContextOld context, LiteralNode node)
    {
        return new TypeTranslator(Options)
            .TranslateType(context, node);
    }

    //*
    // Commom errors.
    //*

    public Error NullArgumentError()
    {
        return new Error();
    }

    //*
    // Translation helpers.
    //*

    protected T TypeCastNode<T>(TranslationContextOld context, Node node) where T : Node
    {
        if (node is not T result)
        {
            throw new TranslationException("", context);
        }

        return result;
    }

}
