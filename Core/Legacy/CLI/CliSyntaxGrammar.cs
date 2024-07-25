using ModularSystem.TextAnalysis;

namespace ModularSystem.Core.Cli;

public class CliSyntaxGrammar : SyntaxGrammar
{
    public const string COMMAND_STATE_ID = "command";
    public const string ARGUMENT_STATE_ID = "argument";
    public const string FLAG_STATE_ID = "flag";
    public const string ASSIGNMENT_STATE_ID = "assignment";
    public const string STRING_LITERAL_STATE_ID = "string-literal";

    public CliSyntaxGrammar()
    {
        TokenProducers = GetTokenProducers();
        SanitizeFunction = Sanitize;

        CreateTransitions();
    }

    static List<TokenProducer> GetTokenProducers()
    {
        return new()
        {
            new CommandProducer(),
            new ArgFlagProducer(),
            new AssignmentProducer(),
            new StringLiteralProducer(),
        };
    }

    static void Sanitize(TerminalToken token)
    {
        if (token.Type == ARGUMENT_STATE_ID)
        {
            token.Value = token.Value.Replace(CLI.ArgumentIdentifier, "");
        }
        if (token.Type == FLAG_STATE_ID)
        {
            token.Value = token.Value.Replace(CLI.FlagIdentifier, "");
        }
        if (token.Type == STRING_LITERAL_STATE_ID)
        {
            if (token.Value.Length >= 2 && token.Value[0] == '"' && token.Value.Last() == '"')
            {
                token.Value = token.Value.Substring(1, token.Value.Length - 2);
            }
        }
    }
}

// SECTION: Token producers (token production rules)
class CommandProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^[A-z][A-z0-9_]*$";
    }

    public override string StateIdentifier()
    {
        return CliSyntaxGrammar.COMMAND_STATE_ID;
    }
}

class ArgProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return $"{CLI.ArgumentIdentifier}(?:[a-zA-Z_]\\w*)|^{CLI.ArgumentIdentifier}";
    }

    public override string StateIdentifier()
    {
        return CliSyntaxGrammar.ARGUMENT_STATE_ID;
    }
}

class FlagProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^--(?:[a-zA-Z_]\\w*)|^--|^-";
    }

    public override string StateIdentifier()
    {
        return CliSyntaxGrammar.FLAG_STATE_ID;
    }
}

class ArgFlagProducer : TokenProducerRouter
{
    public override TokenProducer[] Producers()
    {
        return new TokenProducer[]
        {
            new ArgProducer(),
            new FlagProducer(),
        };
    }

    public override string StateIdentifier()
    {
        return "argument or flag";
    }
}

class AssignmentProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^=";
    }

    public override string StateIdentifier()
    {
        return CliSyntaxGrammar.ASSIGNMENT_STATE_ID;
    }
}

class StringLiteralProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^\"([\\s\\S]*)\"|^\"([\\s\\S]*)|^\"";
    }

    public override char? RequiredCloseChar()
    {
        return '"';
    }

    public override string StateIdentifier()
    {
        return CliSyntaxGrammar.STRING_LITERAL_STATE_ID;
    }
}