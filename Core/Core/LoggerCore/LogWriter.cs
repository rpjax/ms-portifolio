using ModularSystem.Core.Helpers;
using ModularSystem.Core.TextAnalysis;
using System.Text;

namespace ModularSystem.Core.Logging;

public class LogWriter
{
    FileInfo fileInfo;
    Encoding encoding;
    HeaderSection headerPrototype;

    public LogWriter(FileInfo fileInfo, Encoding encoding, HeaderSection? header = null)
    {
        this.fileInfo = fileInfo;
        this.encoding = encoding;
        this.headerPrototype = header ?? new();
    }

    public bool Exists()
    {
        return File.Exists(fileInfo.FullName);
    }

    public void WriteHeader(HeaderSection header, bool @override = false)
    {
        if (Exists())
        {
            throw new Exception("Cannot override a log header.");
        }

        var map = header.Serialize().ToDictionary();
        var strBuilder = new StringBuilder();

        strBuilder.Append($"#{HeaderSection.SECTION_NAME}\n");
        strBuilder.Append(StringifyHeader(map));
        strBuilder.Append($"#{EntriesSection.SECTION_NAME}\n");
        strBuilder.Append("\n");

        var str = strBuilder.ToString();
        var bytes = encoding.GetBytes(str);
        using var fs = fileInfo.OpenWrite();

        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();
        fs.Dispose();
    }

    public void WriteEntry(Entry entry)
    {
        if (!Exists())
        {
            WriteHeader(headerPrototype);
        }

        var reader = new LogReader(fileInfo, encoding);
        var header = reader.GetHeader();
        var insertionCounter = header.InsertionCounter ?? 0;

        entry.InsertionId = insertionCounter + 1;

        var map = entry.Serialize().ToDictionary();
        var str = StringifyEntry(map);
        var bytes = encoding.GetBytes(str);
        using var fs = fileInfo.OpenWrite();

        fs.Position = fs.Length;
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();
        fs.Dispose();

        UpdateHeaderValue(nameof(header.InsertionCounter), HeaderSection.StringifyLong(insertionCounter + 1));
        UpdateHeaderValue(nameof(header.LastInsertionAt), HeaderSection.StringifyDateTime(TimeProvider.Now()));
    }

    public void WriteEntries(Entry[] entries)
    {
        if (!Exists())
        {
            WriteHeader(headerPrototype);
        }

        var reader = new LogReader(fileInfo, encoding);
        var header = reader.GetHeader();
        var insertionCounter = header.InsertionCounter ?? 0;
        List<byte> bytes = new();

        foreach (var entry in entries)
        {
            insertionCounter++;
            entry.InsertionId = insertionCounter;

            var map = entry.Serialize().ToDictionary();
            var str = StringifyEntry(map);
            bytes.AddRange(encoding.GetBytes(str));
        }

        using var fs = fileInfo.OpenWrite();

        fs.Position = fs.Length;
        fs.Write(bytes.ToArray(), 0, bytes.Count);
        fs.Flush();
        fs.Dispose();

        UpdateHeaderValue(nameof(header.InsertionCounter), HeaderSection.StringifyLong(insertionCounter));
        UpdateHeaderValue(nameof(header.LastInsertionAt), HeaderSection.StringifyDateTime(TimeProvider.Now()));
    }

    public void ClearEntries()
    {

    }

    /// <summary>
    /// To update a header value, its new length must match its current length.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="addPadding"></param>
    /// <exception cref="Exception"></exception>
    public void UpdateHeaderValue(string key, string value, bool addPadding = false)
    {
        var grammar = new LogSyntaxGrammar();
        var lexer = new GenericLexer(grammar);
        var enumerable = new LogReaderEnumerable(fileInfo, encoding, MemorySize.KiloByte(2).Value());

        TerminalToken? headerToken = null;
        TerminalToken? keyToken = null;
        TerminalToken? keyValue = null;

        foreach (var token in lexer.Tokenize(enumerable))
        {
            var isHeader = token.Type == LogSyntaxGrammar.SECTION_STATE_ID && token.Value.ToLower() == HeaderSection.SECTION_NAME.ToLower();

            if (isHeader)
            {
                headerToken = token;
                continue;
            }
            if (headerToken != null)
            {
                if (Pairs.FormatKey(token.Value) == Pairs.FormatKey(key) && token.Type == LogSyntaxGrammar.IDENTIFIER_STATE_ID)
                {
                    keyToken = token;
                    continue;
                }
            }
            if (keyToken != null)
            {
                if (token.Type == LogSyntaxGrammar.STRING_LITERAL_STATE_ID)
                {
                    keyValue = token;
                    break;
                }
            }
        }

        if (headerToken == null)
        {
            throw new Exception("Could not find the header section.");
        }
        if (keyToken == null)
        {
            throw new Exception($"Could not find the header key '{key}'.");
        }
        if (keyValue == null)
        {
            throw new Exception($"Could not find the header value for key '{key}'.");
        }

        if (keyValue.Value.Length != value.Length)
        {
            throw new Exception($"Cannot update header '{key}' because the new value length does not match the one in the log file.");
        }

        using var fs = fileInfo.OpenWrite();
        var bytes = encoding.GetBytes($"\"{value}\"");

        fs.Position = keyValue.StartPosition;
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();
        fs.Dispose();
    }

    string StringifyHeader(KeyValuePair<string, string> pair)
    {
        return $"{pair.Key}: \"{pair.Value}\";\n";
    }

    string StringifyHeader(Dictionary<string, string> pairs)
    {
        var strBuilder = new StringBuilder();

        foreach (var pair in pairs)
        {
            strBuilder.Append(StringifyHeader(pair));
        }

        return strBuilder.ToString();
    }

    string StringifyEntry(KeyValuePair<string, string> pair)
    {
        return $"{pair.Key}= \"{pair.Value}\"";
    }

    string StringifyEntry(Dictionary<string, string> pairs)
    {
        var strBuilder = new StringBuilder();

        foreach (var pair in pairs)
        {
            strBuilder.Append(StringifyEntry(pair));
            strBuilder.Append("\n");
        }

        strBuilder.Remove(strBuilder.Length - 1, 1);
        strBuilder.Append(";\n\n");

        return strBuilder.ToString();
    }
}

class WriterHelper
{
    FileInfo fileInfo;
    Encoding encoding;

    public WriterHelper(FileInfo fileInfo, Encoding encoding)
    {
        this.fileInfo = fileInfo;
        this.encoding = encoding;
    }

    public long FindEntriesBytePosition()
    {
        using var fs = fileInfo.OpenRead();

        var reader = new LogReader(fileInfo, encoding);
        var grammar = new LogSyntaxGrammar();
        var lexer = new GenericLexer(grammar);

        throw new NotImplementedException();
    }
}