using System.Text;

namespace ModularSystem.Web.Expressions;

public class ParsingException : Exception
{
    public override string Message => _message;
    protected ParsingContext ParseContext { get; }

    private readonly string _message;

    public ParsingException(string message, ParsingContext context, Exception? innerException, object? additionalData = null) : base(message, innerException)
    {
        ParseContext = context;
        _message = CreateMessage(message);
        Data.Add("Context Additional Data", additionalData);
    }

    private string CreateMessage(string message)
    {
        var strBuilder = new StringBuilder(100);

        message = message.Trim();

        strBuilder.Append("A error has ocoured trying to parse the provided expression tree, this means that the provided serialized expression was malformed and could not be translated into its corresponding expression runtime type. The error message states: ");

        strBuilder.Append(message);

        if (!message.EndsWith(".")) 
            strBuilder.Append('.');

        strBuilder.Append(' ');
        strBuilder.Append($"Error occured at the stack position: [{ParseContext.ToString()}].");

        return strBuilder.ToString();
    }
}