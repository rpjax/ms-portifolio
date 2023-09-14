using ModularSystem.Core.Cli;
using ModularSystem.Core.Logging;

namespace ModularSystem.Core.TextAnalysis.CommomGrammar;

public class CommonSyntaxGrammar : SyntaxGrammar
{
    public const string STRING_LITERAL_STATE_ID = "string_literal";
    public const string NUMBER_STATE_ID = "number";
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
        return CommonSyntaxGrammar.STRING_LITERAL_STATE_ID;
    }
}

class OptimezedStringLiteralProducer : TokenProducer
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
        return CommonSyntaxGrammar.STRING_LITERAL_STATE_ID;
    }

    public override bool IsMatch(string input)
    {
        if (input[0] != '"')
        {
            return false;
        }

        if (input.Length <= 2)
        {
            return true;
        }

        if (input[input.Length - 2] == '"')
        {
            if (input[input.Length - 3] == '\\')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}

public class NumberProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^[0-9]*[.][0-9]*|^[0-9]*";
    }

    public override string StateIdentifier()
    {
        return CommonSyntaxGrammar.NUMBER_STATE_ID;
    }
}

public class IntProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^[0-9]*";
    }

    public override string StateIdentifier()
    {
        return "int";
    }
}

public class FloatProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^[0-9]*[.][0-9]*|^[0-9]*";
    }

    public override string StateIdentifier()
    {
        return "float";
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

class ColonProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^:";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.COLON_STATE_ID;
    }
}

class SemicolonProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^;";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.SEMICOLON_STATE_ID;
    }
}

class StarProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^[*]";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.STAR_STATE_ID;
    }
}

class AtSymbolProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^@";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.AT_STATE_ID;
    }
}

class LeftBraceProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^{";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.LEFT_BRACE_STATE_ID;
    }
}

class RightBraceProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^}";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.RIGHT_BRACE_STATE_ID;
    }
}