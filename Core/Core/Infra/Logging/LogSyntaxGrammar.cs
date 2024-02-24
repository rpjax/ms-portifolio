using ModularSystem.Core.TextAnalysis;
using System.Text.RegularExpressions;

namespace ModularSystem.Core.Logging;

/// <summary>
/// Defines the production rules for the ".log" files produced by the modular system library.
/// </summary>
public class LogSyntaxGrammar : SyntaxGrammar
{
    public const string FILE_EXTENSION = "log";

    public const string SECTION_STATE_ID = "section";
    public const string IDENTIFIER_STATE_ID = "identifier";
    public const string ASSIGNMENT_STATE_ID = "assignment";
    public const string COLON_STATE_ID = "colon";
    public const string SEMICOLON_STATE_ID = "semicolon";
    public const string STAR_STATE_ID = "star";
    public const string AT_STATE_ID = "at symbol";
    public const string LEFT_BRACE_STATE_ID = "open brace";
    public const string RIGHT_BRACE_STATE_ID = "close brace";
    public const string STRING_LITERAL_STATE_ID = "string literal";

    public static Regex StaticSeparatorRegex = new Regex(@"\s|\n", RegexOptions.Compiled);

    public LogSyntaxGrammar()
    {
        TokenProducers = new()
        {
            new SectionProducer(),
            new IdentifierProducer(),
            new AssignmentProducer(),
            new ColonProducer(),
            new SemicolonProducer(),
            new StarProducer(),
            new AtSymbolProducer(),
            new LeftBraceProducer(),
            new RightBraceProducer(),
            new StringLiteralProducer(),
            new NumberProducer()
        };
        SeparatorRegex = StaticSeparatorRegex;
        SanitizeFunction = Sanitize;

        CreateTransitions();
    }

    static void Sanitize(TerminalToken token)
    {
        if (token.Type == STRING_LITERAL_STATE_ID)
        {
            if (token.Value.Length >= 2 && token.Value[0] == '"' && token.Value.Last() == '"')
            {
                token.Value = token.Value.Substring(1, token.Value.Length - 2);
            }
        }
        if (token.Type == SECTION_STATE_ID)
        {
            token.Value = token.Value.Replace("#", "");
        }
    }
}

class SectionProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^#[A-z_]*$|^#";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.SECTION_STATE_ID;
    }
}

class IdentifierProducer : TokenProducer
{
    public override string ProductionPattern()
    {
        return "^[A-z][A-z0-9_]*$";
    }

    public override string StateIdentifier()
    {
        return LogSyntaxGrammar.IDENTIFIER_STATE_ID;
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
        return LogSyntaxGrammar.ASSIGNMENT_STATE_ID;
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
        return LogSyntaxGrammar.STRING_LITERAL_STATE_ID;
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
        return "number";
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