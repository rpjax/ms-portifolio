﻿using ModularSystem.Core.TextAnalysis.Parsing.Components;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Parsing.Extensions;

public static class CstNodeExtensions
{
    public static string ToHtmlTreeView(this CstNode node)
    {
        return new CstNodeHtmlBuilder(node)
            .Build();
    }
}

public class CstNodeHtmlBuilder
{
    private CstNode Root { get; }
    private StringBuilder Builder { get; }
    private int Depth { get; set; }

    public CstNodeHtmlBuilder(CstNode root)
    {
        Root = root;
        Builder = new StringBuilder();
        Depth = 0;
    }

    public string Build()
    {
        Builder.Append("<html>");
        BuildHead();
        BuildBody();
        Builder.Append("</html>");

        return Builder.ToString();
    }

    private void BuildHead()
    {
        Builder.Append("<head>");
        Builder.Append("<style>");

        Builder.Append(".tree{\r\n  --spacing : 1.5rem;\r\n  --radius  : 10px;\r\n}\r\n\r\n.tree li{\r\n  display      : block;\r\n  position     : relative;\r\n  padding-left : calc(2 * var(--spacing) - var(--radius) - 2px);\r\n}\r\n\r\n.tree ul{\r\n  margin-left  : calc(var(--radius) - var(--spacing));\r\n  padding-left : 0;\r\n}\r\n\r\n.tree ul li{\r\n  border-left : 2px solid #ddd;\r\n}\r\n\r\n.tree ul li:last-child{\r\n  border-color : transparent;\r\n}\r\n\r\n.tree ul li::before{\r\n  content      : '';\r\n  display      : block;\r\n  position     : absolute;\r\n  top          : calc(var(--spacing) / -2);\r\n  left         : -2px;\r\n  width        : calc(var(--spacing) + 2px);\r\n  height       : calc(var(--spacing) + 1px);\r\n  border       : solid #ddd;\r\n  border-width : 0 0 2px 2px;\r\n}\r\n\r\n.tree summary{\r\n  display : block;\r\n  cursor  : pointer;\r\n}\r\n\r\n.tree summary::marker,\r\n.tree summary::-webkit-details-marker{\r\n  display : none;\r\n}\r\n\r\n.tree summary:focus{\r\n  outline : none;\r\n}\r\n\r\n.tree summary:focus-visible{\r\n  outline : 1px dotted #000;\r\n}\r\n\r\n.tree li::after,\r\n.tree summary::before{\r\n  content       : '';\r\n  display       : block;\r\n  position      : absolute;\r\n  top           : calc(var(--spacing) / 2 - var(--radius));\r\n  left          : calc(var(--spacing) - var(--radius) - 1px);\r\n  width         : calc(2 * var(--radius));\r\n  height        : calc(2 * var(--radius));\r\n  border-radius : 50%;\r\n  background    : #ddd;\r\n}\r\n\r\n.tree summary::before{\r\n  z-index    : 1;\r\n  background : #696 url('expand-collapse.svg') 0 0;\r\n}\r\n\r\n.tree details[open] > summary::before{\r\n  background-position : calc(-2 * var(--radius)) 0;\r\n}");
        
        Builder.Append("</style>");
        Builder.Append("</head>");
    }

    private void BuildBody()
    {
        Builder.Append("<body>");
        Builder.Append("<ul class=\"tree\">");
        BuildNode(Root);
        Builder.Append("</ul>");
        Builder.Append("</body>");
    }

    private void BuildNode(CstNode node)
    {
        switch (node.Type)
        {
            case CstNodeType.Root:
                BuildRoot((CstRoot)node);
                break;

            case CstNodeType.Internal:
                BuildInternal((CstInternal)node);
                break;

            case CstNodeType.Leaf:
                BuildLeaf((CstLeaf)node);
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    private void BuildRoot(CstRoot node)
    {
        Builder.Append("<li>");
        Builder.Append("<details>");
        Builder.Append($"<summary>{node.Symbol.ToString()}</summary>");
        Builder.Append("<ul>");

        foreach (var child in node.Children)
        {
            BuildNode(child);
        }

        Builder.Append("</ul>");
        Builder.Append("</details>");
        Builder.Append("</li>");
    }

    private void BuildInternal(CstInternal node)
    {
        Builder.Append("<li>");
        Builder.Append("<details>");
        Builder.Append($"<summary>{node.Symbol.ToString()}</summary>");
        Builder.Append("<ul>");

        foreach (var child in node.Children)
        {
            BuildNode(child);
        }

        Builder.Append("</ul>");
        Builder.Append("</details>");
        Builder.Append("</li>");
    }


    private void BuildLeaf(CstLeaf node)
    {
        var text = node.IsEpsilon
            ? $"<li> ε ({node.Symbol.ToString()})</li>"
            : $"<li>{node.Symbol.ToString()}</li>"
            ;

        Builder.Append(text);
    }

    private string Indent(string text)
    {
        var builder = new StringBuilder();
        var tab = new string(' ', 4 * Depth);

        foreach (var line in text.Split(Environment.NewLine))
        {
            builder.AppendLine($"{tab}{line}");
        }

        return builder.ToString();
    }

}