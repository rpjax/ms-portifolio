using ModularSystem.TextAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ModularSystem.Core.Cli;

public class PromptContext
{
    public string? Instruction { get; set; } = null;
    public string[] Flags { get; set; } = new string[0];
    public Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();
    public PromptContext? Next { get; set; }

    public static PromptContext? From(string input)
    {
        return new PromptContextLexerFactory(input).Create();
    }

    public string GetArgument(string name, string? defaultValue = null)
    {
        if (!Arguments.ContainsKey(name))
        {
            if (defaultValue != null)
            {
                return defaultValue;
            }
            throw new Exception($"Missing argument '{name}'. Fix this by calling the command again with '-{name}= \"{{ value }}\"'.");
        }

        return Arguments[name];
    }

    public int GetIntArgument(string name, int? defaultValue = null)
    {
        if (!Arguments.ContainsKey(name))
        {
            if (defaultValue != null)
            {
                return defaultValue.Value;
            }

            throw new Exception($"Missing argument '{name}'. Fix this by calling the command again with '-{name}= \"{{ value }}\"'.");
        }

        return int.Parse(Arguments[name]);
    }

    public long GetLongArgument(string name, long? defaultValue = null)
    {
        if (!Arguments.ContainsKey(name))
        {
            if (defaultValue != null)
            {
                return defaultValue.Value;
            }

            throw new Exception($"Missing argument '{name}'. Fix this by calling the command again with '-{name}= \"{{ value }}\"'.");
        }

        return long.Parse(Arguments[name]);
    }

    public DateTime GetDateTimeArgument(string name, DateTime? defaultValue = null)
    {
        if (!Arguments.ContainsKey(name))
        {
            if (defaultValue != null)
            {
                return defaultValue.Value;
            }

            throw new Exception($"Missing argument '{name}'. Fix this by calling the command again with '-{name}= \"{{ value }}\"'.");
        }

        return DateTime.Parse(Arguments[name]);
    }

    public string? GetOptionalArgument(string name)
    {
        if (!Arguments.ContainsKey(name))
        {
            return null;
        }

        return Arguments[name];
    }

    public bool GetOptionalArgument(string name, [MaybeNullWhen(false)] out string value)
    {
        if (!Arguments.ContainsKey(name))
        {
            value = null;
            return false;
        }

        value = Arguments[name];
        return true;
    }

    public bool GetFlag(string name)
    {
        return Flags.Contains(name);
    }
}

public class PromptContextLexerFactory
{
    readonly string inputString;
    TerminalToken[] tokens;

    public PromptContextLexerFactory(string inputString)
    {
        this.inputString = inputString;
        tokens = new TerminalToken[0];
    }

    public static PromptContext From(string input)
    {
        return new PromptContextLexerFactory(input).Create();
    }

    public PromptContext Create()
    {
        var lexer = new GenericLexer2(GetLexerGrammar());
        var strStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString));
        var stream = new LexerStream(strStream, Encoding.UTF8);
        var context = lexer.CreateContext(stream);
        var engine = lexer.GetEngine(stream, context);

        tokens = engine.GetTerminals().ToArray();

        return new PromptContext()
        {
            Instruction = GetInstruction(),
            Flags = GetFlags(),
            Arguments = GetArgs(),
            Next = GetNextPromt()
        };
    }

    string? GetInstruction()
    {
        var instructions = GetInstructions();

        if (instructions.IsEmpty())
        {
            return null;
        }

        return instructions[0];
    }

    string[] GetInstructions()
    {
        return tokens
            .Where(x => x.Type == CliSyntaxGrammar.COMMAND_STATE_ID)
            .Select(x => x.Value)
            .ToArray();
    }

    string[] GetFlags()
    {
        uint cmdsFound = 0;
        List<string> flags = new();

        foreach (var token in tokens)
        {
            if (token.Type == CliSyntaxGrammar.COMMAND_STATE_ID)
            {
                cmdsFound++;
            }
            if (cmdsFound > 1)
            {
                break;
            }

            if (token.Type == CliSyntaxGrammar.FLAG_STATE_ID)
            {
                flags.Add(token.Value);
            }
        }

        return flags.ToArray();
    }

    Dictionary<string, string> GetArgs()
    {
        var args = new Dictionary<string, string>();

        uint cmdsFound = 0;

        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];

            if (token.Type == CliSyntaxGrammar.COMMAND_STATE_ID)
            {
                cmdsFound++;
            }
            if (cmdsFound > 1)
            {
                break;
            }

            if (token.Type == CliSyntaxGrammar.ARGUMENT_STATE_ID)
            {
                if ((i + 2) >= tokens.Length)
                {
                    throw new Exception("Argument was not properly initialized.");
                }

                var argToken = token;
                var assignmentToken = tokens[i + 1];
                var stringLiteralToken = tokens[i + 2];

                if (assignmentToken.Type != CliSyntaxGrammar.ASSIGNMENT_STATE_ID)
                {
                    throw new Exception($"The argument '{assignmentToken.Value}' was not assigned any value.");
                }
                if (stringLiteralToken.Type != CliSyntaxGrammar.STRING_LITERAL_STATE_ID)
                {
                    throw new Exception($"The argument '{assignmentToken.Value}' was not assigned any value.");
                }

                args.Add(argToken.Value, stringLiteralToken.Value);
            }
        }

        return args;
    }

    PromptContext? GetNextPromt()
    {
        var commandTokens = tokens.Where(x => x.Type == CliSyntaxGrammar.COMMAND_STATE_ID).ToArray();

        if (commandTokens.Length <= 1)
        {
            return null;
        }

        int startIndex;

        if (commandTokens[0].Value == commandTokens[1].Value)
        {
            var _start = inputString.IndexOf(commandTokens[1].Value) + commandTokens[0].Value.Length;
            var _length = inputString.Length - commandTokens[1].Value.Length;
            var _substring = inputString.Substring(_start, _length);

            startIndex = _substring.IndexOf(commandTokens[1].Value) + (inputString.Length - _substring.Length);
        }
        else
        {
            startIndex = Convert.ToInt32(commandTokens[1].StartPosition);
        }

        var substring = inputString.Substring(startIndex, inputString.Length - startIndex);

        return PromptContext.From(substring);
    }

    LexerGrammar GetLexerGrammar()
    {
        return new CliSyntaxGrammar();
    }
}