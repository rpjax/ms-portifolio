using ModularSystem.Core;
using ModularSystem.Core.Helpers;
using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;

namespace ModularSystem.TextAnalysis;

public delegate LexerState LexerStateTransition(LexerState state, LexerInput input);

public class LexerState
{
    /// <summary>
    /// The current state of the lexer engine.
    /// </summary>
    public string Identifier { get; set; } = GenericLexer.INITIAL_STATE;

    /// <summary>
    /// Defines the INITIAL state used by the lexer.
    /// </summary>
    public string InitialState { get; set; } = GenericLexer.INITIAL_STATE;

    /// <summary>
    /// Defines the FINAL state used by the lexer.
    /// </summary>
    public string FinalState { get; set; } = GenericLexer.FINAL_STATE;


    /// <summary>
    /// The current char position within the lexer.
    /// </summary>
    public long Position { get; set; } = 0;

    /// <summary>
    /// Instructs the lexer to consume the the current char and advance the char stream by 1.
    /// </summary>
    public bool Advance { get; set; } = true;

    /// <summary>
    /// Instructs the lexer to go to the set char position.
    /// </summary>
    public long? GotoPosition { get; set; } = null;


    /// <summary>
    /// Instructs the lexer to NOT add the current char to the token and remove it from the <see cref="LexerInput.Value"/>.
    /// </summary>
    public bool SkipChar { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public Range? ExperimentalValueAppend { get; set; } = null;


    /// <summary>
    /// Instructs the lexer to produce a token with the values set to <see cref="LexerInput.Token"/> and <see cref="TokenType"/>.
    /// </summary>
    public bool Accept { get; set; } = false;

    /// <summary>
    /// Does not do anything yet...
    /// </summary>
    public bool TokenIsTerminal { get; set; }

    /// <summary>
    /// Defines the token type when <see cref="Accept"/> is set to true.
    /// </summary>
    public string? TokenType { get; set; } = null;


    /// <summary>
    ///  Instructs the lexer to throw the default end of char exception.
    /// </summary>
    public bool UnexpectedEndOfChars { get; set; } = false;

    /// <summary>
    ///  Instructs the lexer to throw the default end of token exception.
    /// </summary>
    public bool UnexpectedEndOfToken { get; set; } = false;

    /// <summary>
    /// If set instructs the lexer to throw this exception.
    /// </summary>
    public Exception? Exception { get; set; } = null;

    public LexerState(string identifier = GenericLexer.INITIAL_STATE)
    {
        if (identifier != null)
        {
            Identifier = identifier;
        }
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        var cast = obj?.TryTypeCast<LexerState>();

        if (cast == null)
        {
            return false;
        }

        return cast.Identifier == Identifier;
    }

    public LexerState DeepCopy()
    {
        return new LexerState()
        {
            Identifier = Identifier,
            InitialState = InitialState,
            FinalState = FinalState,

            Position = Position,
            Advance = Advance,
            GotoPosition = GotoPosition,

            SkipChar = SkipChar,
            ExperimentalValueAppend = ExperimentalValueAppend,

            Accept = Accept,
            TokenIsTerminal = TokenIsTerminal,
            TokenType = TokenType,

            Exception = Exception,
            UnexpectedEndOfChars = UnexpectedEndOfChars,
            UnexpectedEndOfToken = UnexpectedEndOfToken,
        };
    }

    public void Reset(string initialState, string finalState, long position = 0)
    {
        InitialState = initialState;
        FinalState = finalState;

        Position = position;
        Advance = true;
        GotoPosition = null;

        SkipChar = false;
        ExperimentalValueAppend = null;

        Accept = false;
        TokenIsTerminal = false;
        TokenType = null;

        Exception = null;
        UnexpectedEndOfChars = false;
        UnexpectedEndOfToken = false;
    }

    public void Reset(string state, string initialState, string finalState, long position = 0)
    {
        Identifier = state;
        InitialState = initialState;
        FinalState = finalState;

        Position = position;
        Advance = true;
        GotoPosition = null;

        SkipChar = false;
        ExperimentalValueAppend = null;

        Accept = false;
        TokenIsTerminal = false;
        TokenType = null;

        Exception = null;
        UnexpectedEndOfChars = false;
        UnexpectedEndOfToken = false;
    }
}

public class LexerInput
{
    /// <summary>
    /// Tells if the final char of the stream has been reached.
    /// </summary>
    public bool IsLastChar { get; set; } = false;

    /// <summary>
    /// The position relative to the original input that locates the first char from the token.
    /// </summary>
    public long TokenStartIndex { get; set; }

    /// <summary>
    /// The position relative to the original input that locates the last char from the token.
    /// </summary>
    public long TokenEndIndex { get; set; }

    /// <summary>
    /// Formed token;
    /// </summary>
    public string? Token => GetToken();

    /// <summary>
    /// The formed token + current char.
    /// </summary>
    public string? Value => GetValue();

    /// <summary>
    /// The char at Value[0].
    /// </summary>
    public string? FirstChar => GetFirstChar();

    /// <summary>
    /// The last char read from the stream.
    /// </summary>
    public string? LastChar => GetLastChar();

    /// <summary>
    /// True if Value is null or empty.
    /// </summary>
    public bool ValueIsEmpty => valueBuilder.Length == 0;

    /// <summary>
    /// True if Token is null or empty.
    /// </summary>
    public bool TokenIsEmpty => tokenBuilder.Length == 0;

    public int ValueLength => valueBuilder.Length;
    public int TokenLength => tokenBuilder.Length;

    private StringBuilder tokenBuilder;
    private StringBuilder valueBuilder;

    public LexerInput()
    {
        tokenBuilder = new StringBuilder(MemorySize.KiloByte(4).Value());
        valueBuilder = new StringBuilder(MemorySize.KiloByte(4).Value());
    }

    /// <summary>
    /// Adds a char to the Token.<br></br>
    /// TOREFACTOR: this method is called every lexer iteration, the usage of new operator could be impacting performance.
    /// </summary>
    /// <param name="char"></param>
    public void AppendToToken(char @char)
    {
        tokenBuilder.Append(@char);
    }

    public void AppendToToken(string str)
    {
        tokenBuilder.Append(str);
    }

    public void AppendToValue(char @char)
    {
        valueBuilder.Append(@char);
    }

    public void AppendToValue(char[] @chars)
    {
        valueBuilder.Append(@chars);
    }

    public void AppendToValue(string str)
    {
        valueBuilder.Append(str);
    }

    public void Reset()
    {
        ResetToken();
        ResetValue();
    }

    public void ResetToken()
    {
        tokenBuilder.Clear();
    }

    public void ResetValue()
    {
        valueBuilder.Clear();
    }

    public void RemoveTokenLastChar()
    {
        tokenBuilder.Length--;
    }

    public void RemoveValueLastChar()
    {
        valueBuilder.Length--;
    }

    public LexerInput DeepCopy()
    {
        var copy = new LexerInput()
        {
            IsLastChar = IsLastChar,
            TokenStartIndex = TokenStartIndex,
            TokenEndIndex = TokenEndIndex,
        };

        copy.AppendToValue(valueBuilder.ToString());
        copy.AppendToToken(tokenBuilder.ToString());

        return copy;
    }

    string? GetToken()
    {
        if (tokenBuilder.Length == 0)
        {
            return null;
        }

        return tokenBuilder.ToString();
    }

    string? GetValue()
    {
        if (valueBuilder.Length == 0)
        {
            return null;
        }

        return valueBuilder.ToString();
    }

    string? GetFirstChar()
    {
        if (string.IsNullOrEmpty(Value))
        {
            return null;
        }

        return new string(new char[] { Value[0] });
    }

    string? GetLastChar()
    {
        return Value != null ? new string(new char[] { Value[0] }) : null;
    }
}

public abstract class LexerToken
{
    public string? Type { get; set; } = null;
    public bool IsTerminal => this is TerminalToken;
    public TerminalToken AsTerminal => (TerminalToken)this;

    protected LexerToken(string? type = null)
    {
        Type = type;
    }
}

public class TerminalToken : LexerToken
{
    public string Value { get; set; }

    public long StartPosition { get; set; }
    public long EndPosition { get; set; }

    public TerminalToken(string value)
    {
        Value = value;
    }

    public TerminalToken(string? type, string value) : base(type)
    {
        Type = type;
        Value = value;
    }
}

public class NonTerminalToken : LexerToken
{
    public long Position { get; set; }
    public long Length { get; set; }

    public NonTerminalToken(string? type) : base(type)
    {

    }
}

/// <summary>
/// Defines a set of syntax production rules.<br></br>
/// Under development!
/// </summary>
public class LexerGrammar
{
    //*
    // ASCII RELEVANT CHARS.
    //*
    public const char ASCII_NULL = '\x00';
    public const char ASCII_TAB = '\x09';
    public const char ASCII_NEW_LINE = '\x0a';
    public const char ASCII_WHITESPACE = '\x20';

    //*
    // DEFAULT DFA STATES.
    //*
    public const string INITIAL_STATE = "initial state";
    public const string FINAL_STATE = "final state";

    //*
    // DEFAULT REGEXES.
    //*
    public static readonly Regex DefaultIgnoreRegex = new Regex(@$"[{ASCII_NULL}]", RegexOptions.Compiled);
    public static readonly Regex DefaultSeparatorRegex = new Regex(@$"[{ASCII_WHITESPACE}]", RegexOptions.Compiled);

    /// <summary>
    /// The state that the lexer will start in and go back to every time a new token is produced.
    /// </summary>
    public string InitialState { get; set; } = INITIAL_STATE;

    /// <summary>
    /// The state that the lexer should be in when the end of the char stream is reached.
    /// </summary>
    public string FinalState { get; set; } = FINAL_STATE;

    /// <summary>
    /// Key value pairs of state and transition functions of the lexer.
    /// </summary>
    public Dictionary<string, LexerStateTransition> StateTransitions { get; set; } = new();

    /// <summary>
    /// Defines a regex that, if matched, will instruct the lexer to ignore the iterated char.<br></br>
    /// Beaware that the ignored chars will not be transmited to the state transitions.
    /// </summary>
    public Regex? IgnoreRegex { get; set; } = DefaultIgnoreRegex;

    /// <summary>
    /// Defines regex that instructs the initial state transition to skip over a char, but only if it is at the start of the token.<br></br>
    /// This behavior enables, for example, a whitespace to end a token and be skipped when after the token is accepted. <br></br>
    /// This also means that when separator is found by the initial state there can only be one possible state transition, if this condition is not matched <br></br>
    /// it means that the syntax is ambiguous or that there is an unexpected end of chars.
    /// </summary>
    public Regex? SeparatorRegex { get; set; } = DefaultSeparatorRegex;

    /// <summary>
    /// A function that sanitizes the produced token just before the lexer yield returns.
    /// </summary>
    public Action<TerminalToken>? SanitizeFunction { get; set; } = null;
}

//*
// TODO:
// - Create the ability for the lexer to jump back and forth through the stream of chars.
// NOTES:
// - The new proposed log syntax will make use of the non sequential analysis by having "char pointers" as a prefix in the log file.
//   For example: 
//      string declaration: *6"foobar"
//      managed sentence declaration: @20{time:*19"25/07/2023 03:34:10"}      
//*

/// <summary>
/// Wraps a lexical computation within a span of the original <see cref="Stream"/> of char. <br></br>
/// It also enables parallelism by holding a self contained copy of the resources used by the lexer,<br></br> 
/// like <see cref="LexerState"/> and <see cref="LexerInput"/>, so every <see cref="LexerContext"/> can have their own lexer engine<br></br>
/// to analyse diferent parts of the source <see cref="Stream"/> at the same time.<br></br> 
/// If <see cref="GenericLexer2.Strategy.SingleThread"/> is set then the the engine will jump around the stream instead of yield return a non terminal.
/// </summary>
public class LexerContext
{
    public string StateIdentifier { get; set; }
    /// <summary>
    /// The char position relative to the <see cref="LexerStream"/> that is the start of this lexing context.<br></br>
    /// When the context is set to the lexical engine, it will jump to this char position and work from there.
    /// </summary>
    public long InitialPosition { get; set; }
    public long Length { get; set; } = -1;
    public long EndPosition => InitialPosition + Length - 1;

    public LexerGrammar Grammar { get; }
    public LexerState State { get; set; }
    public LexerInput Input { get; set; }

    public LexerContext(string stateIdentifier, long position, long length, LexerGrammar grammar, LexerState? state = null, LexerInput? input = null)
    {
        StateIdentifier = stateIdentifier;
        InitialPosition = position;
        Length = length;
        Grammar = grammar;
        State = state ?? new();
        Input = input ?? new();
    }

    public LexerContext DeepCopy(long? length = null)
    {
        return new LexerContext(StateIdentifier, InitialPosition, length ?? Length, Grammar, State.DeepCopy(), Input.DeepCopy());
    }

    public void CopyTo(LexerContext context)
    {
        context.StateIdentifier = StateIdentifier;
        context.InitialPosition = InitialPosition;
        context.Length = Length;
    }

    public void Reset()
    {
        State.Reset(StateIdentifier, Grammar.InitialState, Grammar.FinalState, InitialPosition);
        Input.Reset();
    }

    public void Reset(long position)
    {
        State.Reset(StateIdentifier, Grammar.InitialState, Grammar.FinalState, position);
        Input.Reset();
    }

    public void ResetState()
    {
        State.Reset(StateIdentifier, Grammar.InitialState, Grammar.FinalState, InitialPosition);
    }

    public void ResetState(long position)
    {
        State.Reset(StateIdentifier, Grammar.InitialState, Grammar.FinalState, position);
    }

    public void ResetInput()
    {
        Input.Reset();
    }

    public char[] GetChars(LexerStream stream, LexerStream.CacheStrategy cacheStrategy = LexerStream.CacheStrategy.Split)
    {
        return stream.GetChars(InitialPosition, Length, cacheStrategy);
    }
}

public class Range
{
    public long Position { get; set; }
    public long Length { get; set; }
    public LexerStream.CacheStrategy CacheStrategy { get; set; }

    public Range(long position, long length, LexerStream.CacheStrategy cacheStrategy = LexerStream.CacheStrategy.Split)
    {
        Position = position;
        Length = length;
        CacheStrategy = cacheStrategy;
    }
}

/// <summary>
/// <code>
/// Allows for non sequential lexical analysis of a <see cref="char"/> sequence. It uses a <see cref="Stream"/> as data source.
/// It uses an internal buffer to pre-fetch a chunk of bytes from the original stream, the buffer defaults to 80 kib.
/// Not thread safe.
///
/// Main usage methods: <br></br>
/// -<see cref="GetChar(long, CacheStrategy)"/>.
/// -<see cref="GetChars(long, long, CacheStrategy)"/>.
/// 
/// The <see cref="CacheStrategy"/> tells the <see cref="LexerStream"/> how to fetch data from the source <see cref="Stream"/> <br></br>
/// when a request for <see cref="char"/> at a given position cannot be fullfilled by the data in the internal buffer.
/// 
/// -<see cref="CacheStrategy.Split"/>: splits the internal buffer equally for <see cref="char"/>s that are located
///  ahead and before the requested position.
/// -<see cref="CacheStrategy.Forward"/>: allocates the internal buffer entirely for <see cref="char"/>s 
///  that are located ahead of the requested position. 
/// -<see cref="CacheStrategy.Backward"/>: allocates the internal buffer entirely for <see cref="char"/>s 
///  that are located before of the requested position.
/// </code>
/// </summary>
public class LexerStream : IDisposable
{
    public const int DEFAULT_BUFFER_SIZE = 80 * 1024;

    public enum CacheStrategy
    {
        Split,
        Backward,
        Forward
    }

    /// <summary>
    /// Total length in <see cref="byte"/>.
    /// </summary>
    public long ByteLength => sourceLength;

    /// <summary>
    /// Total length in <see cref="char"/>.
    /// </summary>
    public long CharLength => sourceLength / sizeOfChar;

    /// <summary>
    /// The <see cref="byte"/> position relative to the source stream that is the first element of the internal buffer.
    /// </summary>
    public long CacheStart => cacheStart;

    /// <summary>
    /// The <see cref="byte"/> position relative to the source stream that is the last element of the internal buffer.
    /// </summary>
    public long CacheEnd => cacheEnd;

    /// <summary>
    /// The <see cref="char"/> position relative to the source stream that is the first element of the internal buffer.
    /// </summary>
    public long CacheStartPosition => cacheStart / sizeOfChar;

    /// <summary>
    /// The <see cref="char"/> position relative to the source stream that is the last element of the internal buffer.
    /// </summary>
    public long CacheEndPosition => cacheEnd / sizeOfChar;

    Stream sourceStream;
    Encoding encoding;

    // *
    // Caaching of source stream, most likely a FileStream.
    byte[] buffer;

    long cacheStart;
    long cacheEnd;
    int sizeOfChar;

    byte[] singleCharBuffer;

    int bufferLength => buffer.Length;
    long sourceLength => sourceStream.Length;

    public LexerStream(Stream stream, Encoding encoding, int bufferSize = DEFAULT_BUFFER_SIZE)
    {
        sourceStream = stream;
        this.encoding = encoding;
        buffer = new byte[bufferSize];
        cacheStart = -1;
        cacheEnd = -1;
        sizeOfChar = encoding.GetByteCount("C");
        singleCharBuffer = new byte[sizeOfChar];
    }

    public void Init()
    {
        SetCache(0, 1, CacheStrategy.Forward);
    }

    public void Dispose()
    {
        sourceStream.Dispose();
    }

    public bool IsInRange(long position, long length)
    {
        EnsureInitialization();
        long byteLength = length * sizeOfChar;
        long bytePosition = position * sizeOfChar;
        long byteEndPosition = bytePosition + byteLength;
        return (bytePosition >= 0) && (byteEndPosition < sourceLength);
    }

    public bool IsInRange(long position)
    {
        return IsInRange(position, 1);
    }

    public char GetChar(long position, CacheStrategy strategy = CacheStrategy.Forward)
    {
        EnsureInitialization();
        SetCache(position, 1, strategy);

        long bytePosition = position * sizeOfChar;
        long index = bytePosition - cacheStart;

        Array.Copy(buffer, index, singleCharBuffer, 0, sizeOfChar);
        return encoding.GetChars(singleCharBuffer).First();
    }

    public char[] GetChars(long position, long length, CacheStrategy strategy = CacheStrategy.Forward)
    {
        EnsureInitialization();
        SetCache(position, length, strategy);

        long bytePosition = position * sizeOfChar;
        long index = bytePosition - cacheStart;
        var chars = new char[length];
        var span = new ReadOnlySpan<byte>(buffer, Convert.ToInt32(index), Convert.ToInt32(length * sizeOfChar));

        encoding.GetChars(span, new Span<char>(chars));
        return chars;
    }

    void EnsureInitialization()
    {
        if (cacheStart == -1 || cacheEnd == -1)
        {
            SetCache(0, 1, CacheStrategy.Split);
        }
    }

    bool IsCacheHit(long position, long length)
    {
        long byteLength = length * sizeOfChar;
        long bytePosition = position * sizeOfChar;
        long byteEndPosition = bytePosition + byteLength;

        bool isCacheHit = bytePosition >= cacheStart && bytePosition < cacheEnd;

        if (length != 1)
        {
            isCacheHit = isCacheHit && byteEndPosition >= cacheStart && byteEndPosition < cacheEnd;
        }

        return isCacheHit;
    }

    void SetCache(long position, long length, CacheStrategy strategy)
    {
        long byteLength = length * sizeOfChar;
        long bytePosition = position * sizeOfChar;
        long byteEndPosition = bytePosition + byteLength;

        if (byteLength > bufferLength)
        {
            throw new InvalidOperationException();
        }

        if (byteEndPosition > sourceLength)
        {
            throw new ArgumentOutOfRangeException(nameof(byteEndPosition));
        }

        bool isCacheHit = IsCacheHit(position, length);

        if (isCacheHit)
        {
            return;
        }

        long streamPosition = 0;

        switch (strategy)
        {
            case CacheStrategy.Split:
                streamPosition = bytePosition - (bufferLength / 2);
                break;
            case CacheStrategy.Backward:
                streamPosition = (bytePosition - bufferLength > 0) ? bytePosition - bufferLength : 0;
                break;
            case CacheStrategy.Forward:
                streamPosition = bytePosition;
                break;
            default:
                throw new ArgumentException("Invalid cache strategy value.");
        }

        if (streamPosition < 0)
        {
            streamPosition = 0;
        }
        if (streamPosition + bufferLength < byteEndPosition)
        {
            streamPosition += byteEndPosition - (streamPosition + bufferLength);
        }

        sourceStream.Position = streamPosition;
        int bytesRead = sourceStream.Read(buffer, 0, bufferLength);

        cacheStart = streamPosition;
        cacheEnd = streamPosition + bytesRead;
    }
}

/// <summary>
/// A generic DFA lexer engine for parsing large streams of text. The lexer is statefull and NOT thread safe!
/// </summary>
public class GenericLexer
{
    public const string INITIAL_STATE = "initial state";
    public const string FINAL_STATE = "final state";

    string initialState { get; }
    string finalState { get; }
    LexerGrammar grammar { get; }
    Dictionary<string, LexerStateTransition> stateTransitions { get; }

    LexerState state { get; set; }
    LexerInput input { get; set; }
    long counter { get; set; }

    public GenericLexer(LexerGrammar lexer_grammar)
    {
        initialState = lexer_grammar.InitialState ?? INITIAL_STATE;
        finalState = lexer_grammar.FinalState ?? FINAL_STATE;
        grammar = lexer_grammar;
        stateTransitions = new();

        state = new LexerState(initialState);
        input = new LexerInput();
        counter = 0;

        foreach (var item in lexer_grammar.StateTransitions)
        {
            stateTransitions.Add(item.Key, item.Value);
        }
    }

    /// <summary>
    /// Advances the char stream until a token is produced and returned.
    /// </summary>
    /// <param name="inputCharacters"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public IEnumerable<TerminalToken> Tokenize(IEnumerable<char> inputCharacters, long? length = null)
    {
        long inputLength = length ?? inputCharacters.LongCount();

        if (inputLength == 0)
        {
            yield break;
        }

        using IEnumerator<char> enumerator = inputCharacters.GetEnumerator();

        InitInternalState();

        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException("Could not initialize the stream of chars.");
        }

        while (true)
        {
            bool isLast = inputLength == counter + 1;
            char @char = enumerator.Current;
            bool ignoreChar = Ignore(@char);

            if (ignoreChar)
            {
                if (!enumerator.MoveNext())
                {
                    break;
                }

                counter++;
                continue;
            }

            input.IsLastChar = isLast;
            input.AppendToValue(@char);

            state = GetNextState();
            CheckForExceptions();

            if (state.SkipChar)
            {
                input.RemoveValueLastChar();
            }
            else
            {
                if (input.TokenIsEmpty)
                {
                    input.TokenStartIndex = counter;
                }

                input.TokenEndIndex = counter + 1;
                input.AppendToToken(@char);
            }

            if (state.Accept)
            {
                var token = Sanitize(Accept());
                yield return token;
                ResetInput();
            }
            if (state.Advance)
            {
                if (!enumerator.MoveNext())
                {
                    break;
                }

                counter++;
            }

            ResetState();
        }

        if (state.Identifier != finalState)
        {
            throw UnexpectedEndOfCharsException();
        }
    }

    void InitInternalState()
    {
        state = new LexerState(initialState);
        input = new LexerInput();
        counter = 0;
    }

    bool Ignore(char @char)
    {
        string str = new string(new char[] { @char });
        bool ignoreChar =
                grammar.IgnoreRegex != null &&
                grammar.IgnoreRegex.IsMatch(str);
        return ignoreChar;
    }

    Exception UnexpectedEndOfCharsException()
    {
        return new InvalidOperationException($"Unexpected end of chars at char position {counter}.");
    }

    Exception UnexpectedEndOfTokenException()
    {
        return new InvalidOperationException($"Unexpected end of token at char position {counter}.");
    }

    LexerStateTransition? GetTransition()
    {
        if (stateTransitions.ContainsKey(state.Identifier))
        {
            return stateTransitions[state.Identifier];
        }

        return null;
    }

    LexerState GetNextState()
    {
        var transition = GetTransition();

        if (transition == null)
        {
            throw new Exception("There is no transition for the current state at char position {counter}.");
        }

        return transition.Invoke(state, input);
    }

    TerminalToken Sanitize(TerminalToken token)
    {
        grammar.SanitizeFunction?.Invoke(token);
        return token;
    }

    TerminalToken Accept()
    {
        if (input.TokenIsEmpty)
        {
            throw new InvalidOperationException("invalid lexer internal state.");
        }

        TerminalToken token = new TerminalToken(state.TokenType, input.Token!)
        {
            StartPosition = input.TokenStartIndex,
            EndPosition = input.TokenEndIndex,
        };

        return token;
    }

    void CheckForExceptions()
    {
        if (state.Exception != null)
        {
            throw state.Exception;
        }
        if (state.UnexpectedEndOfChars)
        {
            throw UnexpectedEndOfCharsException();
        }
        if (state.UnexpectedEndOfToken)
        {
            throw UnexpectedEndOfTokenException();
        }
    }

    void ResetState()
    {
        state.Reset(initialState, finalState);
    }

    void ResetInput()
    {
        input.Reset();
    }
}

/// <summary>
/// Defines token production rules. <br></br>
/// Another aproach to the problem where a given input could lead to multiple states, and the lexer must advance the char stream <br/>
/// until there is only one transition possible.
/// </summary>
public abstract class TokenProducer
{
    protected Regex? productionCache;

    public abstract string StateIdentifier();
    public abstract string ProductionPattern();

    /// <summary>
    /// Compiles the production regex.
    /// </summary>
    public void Init()
    {
        productionCache ??= new Regex(ProductionPattern(), RegexOptions.Compiled);
    }

    public virtual char? RequiredCloseChar()
    {
        return null;
    }

    public virtual bool IsMatch(string input)
    {
        var match = ProductionRegex().Match(input);
        return match.Success && match.Value.Length == input.Length;
    }

    public virtual Dictionary<string, LexerStateTransition> Transitions()
    {
        return new Dictionary<string, LexerStateTransition>()
        {
            { StateIdentifier(), GetState }
        };
    }

    public virtual LexerState GetState(LexerState state, LexerInput input)
    {
        if (input.ValueIsEmpty)
        {
            throw new InvalidOperationException();
        }

        if (IsMatch(input.Value!))
        {
            if (input.IsLastChar && RequiredCloseChar() != null && input.Value?.Last() != RequiredCloseChar())
            {
                return new LexerState()
                {
                    UnexpectedEndOfChars = true,
                };
            }

            return new LexerState()
            {
                Identifier = input.IsLastChar ? state.FinalState : StateIdentifier(),
                Accept = input.IsLastChar,
                TokenType = StateIdentifier(),
                TokenIsTerminal = true,
            };
        }

        if (input.TokenIsEmpty)
        {
            throw new InvalidOperationException();
        }

        if (input.IsLastChar && RequiredCloseChar() != null && input.Token!.Last() != RequiredCloseChar())
        {
            return new LexerState()
            {
                UnexpectedEndOfChars = true,
            };
        }

        return new LexerState()
        {
            Identifier = state.InitialState,
            Accept = true,
            Advance = false,
            SkipChar = true,
            TokenType = StateIdentifier(),
            TokenIsTerminal = true,
        };
    }

    protected virtual Regex ProductionRegex()
    {
        if (productionCache == null)
        {
            productionCache = new Regex(ProductionPattern(), RegexOptions.Compiled);
        }

        return productionCache;
    }
}

public abstract class TokenProducerRouter : TokenProducer
{
    public abstract TokenProducer[] Producers();

    public override string ProductionPattern()
    {
        var strBuilder = new StringBuilder();
        var producers = Producers();

        for (int i = 0; i < producers.Length; i++)
        {
            var producer = producers[i];
            var isFirst = i == 0;

            if (isFirst)
            {
                strBuilder.Append($"{producer.ProductionPattern()}");
            }
            else
            {
                strBuilder.Append($"|{producer.ProductionPattern()}");
            }
        }

        return strBuilder.ToString();
    }

    public override Dictionary<string, LexerStateTransition> Transitions()
    {
        var transitions = new Dictionary<string, LexerStateTransition>
        {
            { StateIdentifier(), GetState }
        };

        for (int i = 0; i < Producers().Length; i++)
        {
            var producer = Producers()[i];

            foreach (var transition in producer.Transitions())
            {
                transitions.Add(transition.Key, transition.Value);
            }
        }

        return transitions;
    }

    public override LexerState GetState(LexerState state, LexerInput input)
    {
        if (input.ValueIsEmpty)
        {
            throw new InvalidOperationException();
        }

        var producers = Producers().Where(producer => producer.IsMatch(input.Value!)).ToArray();

        if (producers.IsEmpty() && input.Value.Length > 1)
        {
            return new LexerState()
            {
                UnexpectedEndOfToken = !input.IsLastChar,
                UnexpectedEndOfChars = input.IsLastChar
            };
        }

        if (producers.IsEmpty())
        {
            return new LexerState()
            {
                Exception = new Exception("No production rule was found to handle the input.")
            };
        }
        else if (producers.Length > 1)
        {
            return new LexerState()
            {
                Identifier = StateIdentifier(),
            };
        }
        else
        {
            return producers.First().GetState(state, input);
        }
    }
}

public class SyntaxGrammar : LexerGrammar
{
    public List<TokenProducer> TokenProducers { get; set; } = new();

    public void CreateTransitions()
    {
        if (!StateTransitions.ContainsKey(INITIAL_STATE))
        {
            StateTransitions.Add(INITIAL_STATE, InitialStateTransition);
        }

        foreach (var producer in TokenProducers)
        {
            producer.Init();

            foreach (var transition in producer.Transitions())
            {
                if (!StateTransitions.ContainsKey(transition.Key))
                {
                    StateTransitions.Add(transition.Key, transition.Value);
                }
            }
        }
    }

    public virtual LexerState InitialStateTransition(LexerState state, LexerInput input)
    {
        if (input.ValueIsEmpty)
        {
            throw new InvalidOperationException();
        }

        var producers = TokenProducers.Where(producer => producer.IsMatch(input.Value!)).ToArray();

        //*
        // Skips separator chars, such as whitespaces. If the last char is ignored then the lexer will transition to its final state.
        //*
        if (input.ValueLength == 1 && SeparatorRegex != null && SeparatorRegex.IsMatch(input.Value!))
        {
            return new LexerState()
            {
                Identifier = input.IsLastChar ? state.FinalState : state.InitialState,
                SkipChar = true
            };
        }

        if (producers.IsEmpty())
        {
            return new LexerState()
            {
                Exception = new Exception("No production rule was found to handle the input.")
            };
        }
        else if (producers.Length > 1)
        {
            return new LexerState()
            {
                Exception = new Exception("Ambiguous syntax.")
            };
        }
        else
        {
            return producers.First().GetState(state, input);
        }
    }
}

/// <summary>
/// This version of the lexer offers non sequential navigation of the char stream.
/// </summary>
public class GenericLexer2
{
    public enum Strategy
    {
        SingleThread,
        MultiThread
    }

    LexerGrammar grammar { get; }
    Strategy threadingStrategy { get; }

    public GenericLexer2(LexerGrammar lexer_grammar, Strategy strategy = Strategy.SingleThread)
    {
        grammar = lexer_grammar;
        threadingStrategy = strategy;
    }

    /// <summary>
    /// Creates a lexical engine that consumes the <paramref name="stream"/> based on the <paramref name="context"/>.
    /// </summary>
    /// <param name="stream">The source stream of <see cref="char"/>.</param>
    /// <param name="context">The context under which the parsing should occur. If null, a default context is used.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TerminalToken"/>.</returns>
    public IEnumerable<TerminalToken> GetEnumerable(LexerStream stream, LexerContext context)
    {
        InitInternalState(context);

        while (true)
        {
            bool isLast = context.State.Position + 1 == stream.CharLength;
            char @char = stream.GetChar(context.State.Position);
            bool ignoreChar = Ignore(@char);

            if (ignoreChar)
            {
                if (!Goto(stream, context, context.State.Position + 1))
                {
                    break;
                }

                continue;
            }

            context.Input.IsLastChar = isLast;
            context.Input.AppendToValue(@char);

            context.State = GetNextState(context.State, context.Input);
            CheckForExceptions(context.State);

            if (context.State.SkipChar)
            {
                context.Input.RemoveValueLastChar();
            }
            else
            {
                if (context.Input.TokenIsEmpty)
                {
                    context.Input.TokenStartIndex = context.State.Position;
                }

                context.Input.TokenEndIndex = context.State.Position + 1;
                context.Input.AppendToToken(@char);
            }

            if (context.State.ExperimentalValueAppend != null)
            {
                var range = context.State.ExperimentalValueAppend;
                var chars = stream.GetChars(range.Position, range.Length, range.CacheStrategy);
                context.Input.AppendToValue(chars);
            }

            if (context.State.Accept)
            {
                var token = Sanitize(Accept(context.State, context.Input));
                yield return token;
                ResetInput(context.Input);
            }

            if (context.State.GotoPosition != null)
            {
                long pos = context.State.GotoPosition.Value;
                long posLimit = context.InitialPosition + context.Length;

                if (!Goto(stream, context, pos, posLimit))
                {
                    break;
                }
            }
            else if (context.State.Advance)
            {
                if (!Goto(stream, context, context.State.Position + 1))
                {
                    break;
                }
            }

            ResetState(context.State);
        }

        if (context.State.Position == stream.CharLength && context.State.Identifier != grammar.FinalState)
        {
            throw UnexpectedEndOfCharsException(context.State);
        }
    }

    public LexerContext CreateContext(LexerStream stream)
    {
        return new LexerContext(grammar.InitialState, 0, stream.CharLength, grammar);
    }

    /// <summary>
    /// Under development new lexer API.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public LexerEngine GetEngine(LexerStream stream, LexerContext context)
    {
        return new LexerEngine(stream, context);
    }

    void InitInternalState(LexerContext? context = null)
    {
        context?.Reset();
    }

    bool Ignore(char @char)
    {
        string str = new string(new char[] { @char });
        bool ignoreChar =
                grammar.IgnoreRegex != null &&
                grammar.IgnoreRegex.IsMatch(str);
        return ignoreChar;
    }

    bool Goto(LexerStream stream, LexerContext context, long position, long? positionLimit = null)
    {
        bool isInRange = stream.IsInRange(context.State.Position) && (positionLimit == null ? true : context.State.Position <= positionLimit);

        if (!isInRange)
        {
            return false;
        }

        context.State.Position = position;
        return true;
    }

    LexerStateTransition? GetTransition(LexerState state)
    {
        if (grammar.StateTransitions.ContainsKey(state.Identifier))
        {
            return grammar.StateTransitions[state.Identifier];
        }

        return null;
    }

    LexerState GetNextState(LexerState state, LexerInput input)
    {
        var transition = GetTransition(state);

        if (transition == null)
        {
            throw new Exception("There is no transition for the current state at char position {counter}.");
        }

        var position = state.Position;
        var nextState = transition.Invoke(state, input);

        nextState.Position = position;
        return nextState;
    }

    TerminalToken Sanitize(TerminalToken token)
    {
        grammar.SanitizeFunction?.Invoke(token);
        return token;
    }

    TerminalToken Accept(LexerState state, LexerInput input)
    {
        if (input.TokenIsEmpty)
        {
            throw new InvalidOperationException("invalid lexer internal state.");
        }

        TerminalToken token = new TerminalToken(state.TokenType, input.Token!)
        {
            StartPosition = input.TokenStartIndex,
            EndPosition = input.TokenEndIndex,
        };

        return token;
    }

    void CheckForExceptions(LexerState state)
    {
        if (state.Exception != null)
        {
            throw state.Exception;
        }
        if (state.UnexpectedEndOfChars)
        {
            throw UnexpectedEndOfCharsException(state);
        }
        if (state.UnexpectedEndOfToken)
        {
            throw UnexpectedEndOfTokenException(state);
        }
    }

    void ResetState(LexerState state)
    {
        state.Reset(grammar.InitialState, grammar.FinalState, state.Position);
    }

    void ResetInput(LexerInput input)
    {
        input.Reset();
    }

    Exception UnexpectedEndOfCharsException(LexerState state)
    {
        return new InvalidOperationException($"Unexpected end of chars at char position {state.Position}.");
    }

    Exception UnexpectedEndOfTokenException(LexerState state)
    {
        return new InvalidOperationException($"Unexpected end of token at char position {state.Position}.");
    }
}

public class LexerEngine
{
    public LexerState state => context.State;
    public LexerInput input => context.Input;
    public LexerGrammar grammar => context.Grammar;
    public long Position => state.Position;

    LexerStream stream { get; }
    LexerContext context { get; }

    IEnumerator<LexerToken> enumerator;

    public LexerEngine(LexerStream stream, LexerContext context)
    {
        this.stream = stream;
        this.context = context;
        enumerator = GetEnumerator();
    }

    public void Dispose()
    {
        enumerator.Dispose();
    }

    public void Reset()
    {
        context.Reset();
        enumerator = GetEnumerator();
    }

    public bool MoveNext()
    {
        return enumerator.MoveNext();
    }

    public LexerToken GetToken()
    {
        return enumerator.Current;
    }

    public IEnumerable<LexerToken> GetTokens()
    {
        while (MoveNext())
        {
            yield return GetToken();
        }
    }

    public IEnumerable<TerminalToken> GetTerminals()
    {
        while (MoveNext())
        {
            yield return GetToken().AsTerminal;
        }
    }

    public void SetContext(LexerContext context)
    {
        context.CopyTo(this.context);
        Reset();
    }

    /// <summary>
    /// Allows for navigation within the source of chars.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="positionLimit"></param>
    /// <returns></returns>
    public bool Goto(long position, long? positionLimit = null)
    {
        bool isInRange = stream.IsInRange(context.State.Position) && (positionLimit == null ? true : context.State.Position <= positionLimit);
        context.State.Position = position;
        return isInRange;
    }

    public bool Goto(LexerContext context)
    {
        return Goto(context.InitialPosition, context.InitialPosition + context.Length);
    }

    public LexerContext SnapshotContext(long? length = null)
    {
        return new LexerContext(state.Identifier, state.Position, length ?? stream.CharLength - state.Position, context.Grammar);
    }

    public IEnumerator<LexerToken> GetEnumerator()
    {
        while (Position <= context.EndPosition)
        {
            bool isLast = context.State.Position == context.EndPosition;
            char @char = stream.GetChar(context.State.Position);
            bool ignoreChar = Ignore(@char);

            if (ignoreChar)
            {
                if (!Goto(context.State.Position + 1))
                {
                    break;
                }

                continue;
            }

            context.Input.IsLastChar = isLast;
            context.Input.AppendToValue(@char);

            context.State = GetNextState();
            CheckForExceptions();

            if (context.State.SkipChar)
            {
                context.Input.RemoveValueLastChar();
            }
            else
            {
                if (context.Input.TokenIsEmpty)
                {
                    context.Input.TokenStartIndex = context.State.Position;
                }

                context.Input.TokenEndIndex = context.State.Position + 1;
                context.Input.AppendToToken(@char);
            }

            if (context.State.ExperimentalValueAppend != null)
            {
                var range = context.State.ExperimentalValueAppend;
                var chars = stream.GetChars(range.Position, range.Length, range.CacheStrategy);
                context.Input.AppendToValue(chars);
            }

            if (context.State.Accept)
            {
                yield return Sanitize(Accept());
                ResetInput();
            }

            if (context.State.GotoPosition != null)
            {
                long pos = context.State.GotoPosition.Value;
                long posLimit = context.InitialPosition + context.Length;

                if (!Goto(pos, posLimit))
                {
                    break;
                }
            }
            else if (context.State.Advance)
            {
                if (!Goto(context.State.Position + 1))
                {
                    break;
                }
            }

            ResetState();
        }

        if (context.State.Position == stream.CharLength && context.State.Identifier != grammar.FinalState)
        {
            throw UnexpectedEndOfCharsException(context.State);
        }
    }

    bool Ignore(char @char)
    {
        bool ignoreChar =
                input.ValueIsEmpty &&
                context.Grammar.IgnoreRegex != null &&
                context.Grammar.IgnoreRegex.IsMatch(new string(new char[] { @char }));
        return ignoreChar;
    }

    LexerStateTransition? GetTransition(LexerState state)
    {
        if (context.Grammar.StateTransitions.ContainsKey(state.Identifier))
        {
            return context.Grammar.StateTransitions[state.Identifier];
        }

        return null;
    }

    LexerState GetNextState()
    {
        var transition = GetTransition(state);

        if (transition == null)
        {
            throw new Exception("There is no transition for the current state at char position {counter}.");
        }

        var position = state.Position;
        var nextState = transition.Invoke(state, input);

        nextState.Position = position;
        return nextState;
    }

    LexerToken Sanitize(LexerToken token)
    {
        if (token.IsTerminal)
        {
            context.Grammar.SanitizeFunction?.Invoke(token.AsTerminal);
        }

        return token;
    }

    LexerToken Accept()
    {
        if (input.TokenIsEmpty)
        {
            throw new InvalidOperationException("invalid lexer internal state.");
        }

        LexerToken token;

        if (state.TokenIsTerminal)
        {
            token = new TerminalToken(state.TokenType, input.Token!)
            {
                StartPosition = input.TokenStartIndex,
                EndPosition = input.TokenEndIndex,
            };
        }
        else
        {
            token = new NonTerminalToken(state.TokenType)
            {
                Position = input.TokenStartIndex,
                Length = state.Position - input.TokenStartIndex,
            };
        }

        return token;
    }

    void CheckForExceptions()
    {
        if (state.Exception != null)
        {
            throw state.Exception;
        }
        if (state.UnexpectedEndOfChars)
        {
            throw UnexpectedEndOfCharsException(state);
        }
        if (state.UnexpectedEndOfToken)
        {
            throw UnexpectedEndOfTokenException(state);
        }
    }

    void ResetState()
    {
        state.Reset(grammar.InitialState, grammar.FinalState, state.Position);
    }

    void ResetInput()
    {
        context.ResetInput();
    }

    Exception UnexpectedEndOfCharsException(LexerState state)
    {
        return new InvalidOperationException($"Unexpected end of chars at char position {state.Position}.");
    }

    Exception UnexpectedEndOfTokenException(LexerState state)
    {
        return new InvalidOperationException($"Unexpected end of token at char position {state.Position}.");
    }
}