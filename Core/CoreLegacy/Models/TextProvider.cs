//*
// Requires refactoring
//*

namespace ModularSystem.Core;

public class TextProvider
{
    public TextProvider(string text = "")
    {
        Text = text;
    }

    public string Text { get; }

    public TextProvider(string english, string ptBr)
    {
        Text = english;
    }

    public override string ToString()
    {
        return Text;
    }

}
