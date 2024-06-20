using System.Runtime.CompilerServices;
using ModularSystem.Core.TextAnalysis.Gdef;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Tools;
using Webql.Parsing.Components;
using Webql.Parsing.Tools;

namespace Webql.Parsing;

/// <summary>
/// Parser for the WebQL document syntax (V3).
/// </summary>
public static class WebqlParser
{
    const string RawGrammarText = @"
/*
	Document Syntax for WebQL Version 3.0.0, created by Rodrigo Jacques.
*/

start
	: document 
	;

document
	: expression
	;

expression 
	: literal_expression
	| reference_expression
	| scope_access_expression
	| block_expression
	| operation_expression
	;

literal_expression
	: bool
	| null
	| $int
	| $float
	| $hex
	| $string
	;

bool
	: 'true'
	| 'false'
	;

null
	: 'null'
	;

reference_expression
	: $id
	| '$' $string
	;

scope_access_expression
	: reference_expression ':' expression
	;

block_expression
	: '{' [ expression ] { ',' expression } '}' 
	;

operation_expression
	: operator ':' expression
	;

operator 
	: arithmetic_operator
	| relational_operator
	| string_relational_operator
	| logical_operator
	| collection_manipulation_operator
	| collection_aggregation_operator
	;

arithmetic_operator
	: '$add'
	| '$subtract'
	| '$multiply'
	| '$divide'
	;

relational_operator
	: '$equals'
	| '$notEquals'
	| '$greater'
	| '$less'
	| '$greaterEquals'
	| '$lessEquals'
	;

string_relational_operator
	: '$like'
	;

logical_operator
	: '$and'
	| '$or'
	| '$not'
	;

collection_manipulation_operator
	: '$filter'
	| '$select'
	| '$group'
	| '$order'
	| '$limit'
	| '$skip'
	;

collection_aggregation_operator
	: '$count'
	| '$any'
	| '$all'
	| '$sum'
	| '$average'
	| '$min'
	| '$max'
	;
";

    private static string[] ReduceWhitelist { get; } = new string[]
    {
		// main constructs
		"document",
        "operator",

		// expressions
        "literal_expression",
        "reference_expression",
        "scope_access_expression",
        "block_expression",
        "operation_expression",
    };

    private static LR1Parser? ParserInstance { get; set; }

    static WebqlParser()
    {
		// initialize the parser instance
        GetParser();
    }

	/// <summary>
	/// Get the LR1 parser instance for the WebQL syntax grammar.
	/// </summary>
	/// <returns></returns>
    public static LR1Parser GetParser()
    {
        if (ParserInstance is null)
        {
            ParserInstance = new LR1Parser(
                grammar: GdefParser.ParseGrammar(RawGrammarText)
            );
        }

        return ParserInstance;
    }

    /// <summary>
    /// Parse a text into a CST and reduce it into to a more manageable CST.
    /// </summary>
    /// <remarks>
    /// Note that the reduction process does not create an AST, but a CST with a reduced set of nodes. 
    /// <br/>
    /// To create the AST use the <see cref="ParseToAst"/>.
    /// </remarks>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CstRoot Parse(string text)
    {
        var cst = GetParser().Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);

        return reducer.Execute();
    }

	/// <summary>
	/// Parse a text into an AST according to the WebQL syntax grammar.
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WebqlQuery ParseToAst(string text)
    {
        return WebqlAstBuilder.TranslateQuery(Parse(text));
    }

}

