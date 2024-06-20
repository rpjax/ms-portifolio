﻿using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace Webql.Components;

public class WebqlCompilationException : Exception
{
    protected SyntaxElementPosition? Position { get; }

    public WebqlCompilationException(string message, SyntaxElementPosition? position) : base(message)
    {
        Position = position;
    }

    public override string ToString()
    {
        var positionStr = Position?.ToString() ?? "Unknown position";
        var messageStr = Message;
        var errorStr = $"{messageStr} -> At: {positionStr}";

        return errorStr;
    }

}
