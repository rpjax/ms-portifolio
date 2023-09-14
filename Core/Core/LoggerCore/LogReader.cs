using ModularSystem.Core.Helpers;
using ModularSystem.Core.TextAnalysis;
using System.Collections;
using System.Reflection;
using System.Text;

namespace ModularSystem.Core.Logging;

//*
// READER
//*
public class LogReader
{
    LogReaderEnumerable enumerable;
    int sizeOfChar => enumerable.Encoding.GetByteCount(new char[] { 'C' });

    public LogReader(FileInfo fileInfo, Encoding encoding)
    {
        enumerable = new LogReaderEnumerable(fileInfo, encoding, MemorySize.KiloByte(64).Value());
    }

    public HeaderSection GetHeader()
    {
        var grammar = new LogSyntaxGrammar();
        var lexer = new GenericLexer(grammar);
        Dictionary<string, string> pairs = new();

        //*
        // header syntax: header_key: "value";
        //*
        lock (this)
        {
            bool foundHeader = false;
            long startIndex = -1;
            long endIndex = -1;
            TerminalToken? headerKey = null;
            TerminalToken? colon = null;
            TerminalToken? headerValue = null;
            TerminalToken? semicolon = null;

            foreach (var token in lexer.Tokenize(enumerable))
            {
                var isHeader = token.Type == LogSyntaxGrammar.SECTION_STATE_ID && token.Value.ToLower() == HeaderSection.SECTION_NAME.ToLower();

                if (isHeader)
                {
                    foundHeader = true;
                    continue;
                }
                if (!foundHeader)
                {
                    continue;
                }

                //*
                // End of headers section reached.
                //*
                if (token.Type == LogSyntaxGrammar.SECTION_STATE_ID)
                {
                    break;
                }

                if (headerKey == null)
                {
                    if (token.Type != LogSyntaxGrammar.IDENTIFIER_STATE_ID)
                    {
                        throw new Exception("Log syntax error. A log header was not properly set.");
                    }

                    headerKey = token;
                    continue;
                }

                if (colon == null)
                {
                    if (token.Type != LogSyntaxGrammar.COLON_STATE_ID)
                    {
                        throw new Exception("Log syntax error. A log header was not properly set.");
                    }

                    colon = token;
                    continue;
                }

                if (headerValue == null)
                {
                    if (token.Type != LogSyntaxGrammar.STRING_LITERAL_STATE_ID)
                    {
                        throw new Exception("Log syntax error. A log header was not properly set.");
                    }

                    headerValue = token;
                    continue;
                }

                if (semicolon == null)
                {
                    if (token.Type != LogSyntaxGrammar.SEMICOLON_STATE_ID)
                    {
                        throw new Exception("Log syntax error. A log header was not properly set.");
                    }

                    semicolon = token;
                }

                if (startIndex == -1)
                {
                    startIndex = headerKey.StartPosition;
                }
                endIndex = semicolon.EndPosition;

                pairs.Add(headerKey.Value, headerValue.Value);

                headerKey = null;
                colon = null;
                headerValue = null;
                semicolon = null;
            }

            if (headerKey != null)
            {
                throw new Exception("Log syntax error. A log header was not properly set.");
            }

            return new HeaderSection(pairs);
        }
    }

    public LogSection[] GetSections(bool stopOnEntries = true)
    {
        var grammar = new LogSyntaxGrammar();
        var lexer = new GenericLexer(grammar);
        var sections = new List<LogSection>();

        throw new NotImplementedException();
    }

    public IEnumerable<T> GetEntries<T>() where T : Entry, new()
    {
        var grammar = new LogSyntaxGrammar();
        var lexer = new GenericLexer(grammar);
        Pairs pairs = new(new());

        //*
        // entry syntax: entry_key= "value";
        //*

        bool foundEntriesSection = false;
        long yieldCounter = 0L;

        TerminalToken? entryKey = null;
        TerminalToken? assignment = null;
        TerminalToken? entryValue = null;

        Type type = typeof(T);
        Type[] constructorParams = new Type[] { typeof(Pairs) };
        ConstructorInfo? constructorInfo = type.GetConstructor(constructorParams);

        if (constructorInfo == null)
        {
            constructorParams = new Type[0];
            constructorInfo = type.GetConstructor(constructorParams);
        }

        if (constructorInfo == null)
        {
            throw new InvalidOperationException();
        }

        long length;

        using (var stream = enumerable.FileInfo.OpenRead())
        {
            length = stream.Length / sizeOfChar;
            stream.Dispose();
        }

        foreach (var token in lexer.Tokenize(enumerable, length))
        {
            var isEntriesSection = token.Type == LogSyntaxGrammar.SECTION_STATE_ID && token.Value == EntriesSection.SECTION_NAME;

            if (isEntriesSection)
            {
                foundEntriesSection = true;
                continue;
            }
            if (!foundEntriesSection)
            {
                continue;
            }

            //*
            // End of entries section reached.
            //*
            if (token.Type == LogSyntaxGrammar.SECTION_STATE_ID)
            {
                break;
            }

            if (token.Type == LogSyntaxGrammar.SEMICOLON_STATE_ID)
            {
                if (entryKey != null)
                {
                    throw new Exception("Log syntax error.");
                }

                object[] _constructorParams = new[] { pairs };
                T instance;
                instance = (T)constructorInfo.Invoke(_constructorParams);
                yieldCounter++;
                pairs.Clear();
                yield return instance;
                continue;
            }

            if (entryKey == null)
            {
                if (token.Type != LogSyntaxGrammar.IDENTIFIER_STATE_ID)
                {
                    throw new Exception("Log syntax error.");
                }

                entryKey = token;
                continue;
            }

            if (assignment == null)
            {
                if (token.Type != LogSyntaxGrammar.ASSIGNMENT_STATE_ID)
                {
                    throw new Exception("Log syntax error.");
                }

                assignment = token;
                continue;
            }

            if (entryValue == null)
            {
                if (token.Type != LogSyntaxGrammar.STRING_LITERAL_STATE_ID)
                {
                    throw new Exception("Log syntax error.");
                }
                if (pairs.Contains(entryKey.Value))
                {
                    throw new Exception($"Log syntax error.The log entry of number {yieldCounter} contains a duplicated key: '{entryKey.Value}'.");
                }

                entryValue = token;
                pairs.Add(entryKey.Value, entryValue.Value);
                entryKey = null;
                assignment = null;
                entryValue = null;
                continue;
            }
        }
    }
}

/// <summary>
/// It wraps log file stream and exposes it as an <see cref="IEnumerable{T}"/> of <see cref="char"/>. This class implements the IDisposable interface.<br></br>
/// The file is read according to the set <see cref="Encoding"/>. <br></br>
/// Resource allocation only occur when <see cref="LogReaderEnumerable.GetEnumerator"/> is called, and they are held by the returned enumerator.
/// </summary>
public class LogReaderEnumerable : IEnumerable<char>
{
    public FileInfo FileInfo => fileInfo;
    public Encoding Encoding => encoding;

    FileInfo fileInfo;
    Encoding encoding;
    int bufferSize;

    ///// <summary>
    ///// The mutex lock is required because no write can occur simultaneously with a read, or another write.
    ///// </summary>
    //Mutex mutex;

    public LogReaderEnumerable(FileInfo fileInfo, Encoding encoding, int bufferSize = 1024 * 4)
    {
        this.fileInfo = fileInfo;
        this.encoding = encoding;
        this.bufferSize = bufferSize;
    }

    public IEnumerator<char> GetEnumerator()
    {
        return new LogReaderEnumerator(fileInfo, encoding, bufferSize).Init();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new LogReaderEnumerator(fileInfo, encoding, bufferSize).Init();
    }
}

/// <summary>
/// Exposes the log file stream as an IEnumerator of char. It uses an internal buffer to read the file in chunks. <br></br>
/// File system I/O read operations are done in cicles according to the size of the internal buffer, <br></br>
/// and the file stream remains open until this object is disposed of.<br></br>
/// The internal buffer defaults to 4 kib.
/// </summary>
public class LogReaderEnumerator : IEnumerator<char>, IDisposable
{
    public static readonly int DefaultBufferSize = MemorySize.KiloByte(4).Value();

    public bool IsInit => fileStream != null;
    public long Length => GetLength();
    public char Current => GetCurrentChar();
    object IEnumerator.Current => GetCurrentChar();

    FileInfo fileInfo;
    Encoding encoding;
    Stream? fileStream;

    MemoryBuffer buffer;

    long cursor;
    int sizeOfChar => encoding.GetByteCount(new char[] { 'C' });

    public LogReaderEnumerator(FileInfo fileInfo, Encoding encoding, int bufferSize = 1024 * 4)
    {
        this.fileInfo = fileInfo;
        this.encoding = encoding;

        buffer = new MemoryBuffer(bufferSize);
        _buffer = new byte[bufferSize];
        cursor = -1;
    }

    public LogReaderEnumerator Init()
    {
        fileStream ??= File.OpenRead(fileInfo.FullName);
        return this;
    }

    public void Dispose()
    {
        fileStream?.Dispose();
    }

    public bool MoveNext()
    {
        EnsureInitialization();
        cursor++;
        return cursor < fileStream.Length;
    }

    public void Reset()
    {
        EnsureInitialization();
        fileStream.Position = 0L;
        cursor = -1;
        buffer.Reset();
        storedBytes = 0;
    }

    void EnsureInitialization()
    {
        if (fileStream == null)
        {
            throw new InvalidOperationException("The enumerator has not been initialized.");
        }
    }

    long GetLength()
    {
        EnsureInitialization();
        return fileStream.Length;
    }
    byte[] _buffer;
    int storedBytes = 0;
    char GetCurrentChar()
    {
        EnsureInitialization();

        if (cursor > fileStream.Length)
        {
            throw new InvalidOperationException("The log reader reached the end of the file.");
        }

        int streamLength = Convert.ToInt32(fileStream.Length);
        //long cacheStart = fileStream.Position - buffer.StoredBytes;
        //long cacheEnd = fileStream.Position;        
        long cacheStart = fileStream.Position - storedBytes;
        long cacheEnd = fileStream.Position;
        bool isCacheHit = cursor >= cacheStart && cursor < cacheEnd && cacheStart != cacheEnd;

        if (!isCacheHit)
        {
            fileStream.Position = cursor;
            bool bufferLengthIsGreaterThanAvailableBytes = (fileStream.Position + buffer.Length) >= streamLength;
            int readSize = bufferLengthIsGreaterThanAvailableBytes ? streamLength - Convert.ToInt32(fileStream.Position) : buffer.Length;
            byte[]? _bytes = new byte[readSize];

            //fileStream.Read(_bytes, 0, readSize);
            //buffer.Reset();
            //buffer.Write(_bytes);

            fileStream.Read(_buffer, 0, readSize);
            storedBytes = readSize;
            //cacheStart = fileStream.Position - buffer.StoredBytes;
            cacheStart = fileStream.Position - storedBytes;
            cacheEnd = fileStream.Position;
        }

        int offset = Convert.ToInt32(cursor - cacheStart);
        //byte[]? bytes = buffer.PeekRead(sizeOfChar, offset);
        byte[] bytes = new byte[sizeOfChar];
        Array.Copy(_buffer, offset, bytes, 0, sizeOfChar);

        if (bytes == null)
        {
            throw new InvalidOperationException("Could not read bytes from the buffer.");
        }

        return encoding.GetChars(bytes).Last();
    }
}