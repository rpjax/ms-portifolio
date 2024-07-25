using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

public class ArgumentTranslator
{
    private TranslationOptions Options { get; }

    public ArgumentTranslator(TranslationOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;arg&gt; ::= &lt;reference&gt; | &lt;object&gt; <br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateArgument(TranslationContextOld context, Node node)
    {
        if (node is LiteralNode literalNode)
        {
            return TranslateArgument(context, literalNode);
        }
        if (node is ObjectNode objectNode)
        {
            return TranslateArgument(context, objectNode);
        }

        throw new TranslationException("", context);
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;arg&gt; ::= &lt;reference&gt; | &lt;object&gt; <br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateArgument(TranslationContextOld context, LiteralNode node)
    {          
        return new ReferenceTranslator(Options)
            .TranslateReference(context, node);
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;arg&gt; ::= &lt;reference&gt; | &lt;object&gt; <br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateArgument(TranslationContextOld context, ObjectNode node)
    {
        return new ObjectTranslator(Options)
                 .TranslateObject(context, node);
    }

}
