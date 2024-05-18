﻿using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents a stream of <see cref="Token"/> with one token lookahead. It is used by LL(1) and LR(1) parsers.
/// </summary>
public class InputStream : IDisposable
{
    private IEnumerator<Token> TokenStream { get; }
    private bool IsEndReached { get; set; }
    private TokenType[] IgnoreSet { get; }

    public InputStream(
        string input, 
        Tokenizer tokenizer,
        TokenType[]? ignoreSet = null)
    {
        TokenStream = tokenizer.Tokenize(input).GetEnumerator();
        IsEndReached = false;
        IgnoreSet = ignoreSet ?? Array.Empty<TokenType>();

        Init();
    }

    /// <summary>
    /// Gets the lookahead token.
    /// </summary>
    public Terminal? Lookahead => Peek();

    public void Dispose()
    {
        TokenStream.Dispose();
    }

    /// <summary>
    /// Gets the current token.
    /// </summary>
    /// <returns></returns>
    public Terminal? Peek()
    {
        if (IsEndReached)
        {
            return null;
        }

        return new Terminal(TokenStream.Current.Type, TokenStream.Current.Value);
    }

    /// <summary>
    /// Consumes the current token and moves to the next one. 
    /// </summary>
    /// <remarks> 
    /// It skips the tokens in the ignore set. 
    /// </remarks>
    /// <exception cref="InvalidOperationException"></exception>
    public void Consume()
    {
        if (IsEndReached)
        {
            throw new InvalidOperationException("The end of the input stream has been reached.");
        }

        IsEndReached = !TokenStream.MoveNext();

        while (!IsEndReached && IgnoreSet.Contains(TokenStream.Current.Type))
        {
            IsEndReached = !TokenStream.MoveNext();
        }
    }

    private void Init()
    {
        IsEndReached = !TokenStream.MoveNext();

        var ignoreToken = IgnoreSet.Any(x => x == TokenStream.Current.Type);

        if (!ignoreToken)
        {
            return;
        }

        while (!IsEndReached && IgnoreSet.Contains(TokenStream.Current.Type))
        {
            IsEndReached = !TokenStream.MoveNext();
        }
    }
}